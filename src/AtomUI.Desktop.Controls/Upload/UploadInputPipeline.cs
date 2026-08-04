using AtomUI.Controls;
using Avalonia.Platform.Storage;

namespace AtomUI.Desktop.Controls;

internal sealed class UploadInputPipeline
{
    private readonly Upload _owner;
    private readonly SemaphoreSlim _arrivalGate = new(1, 1);
    private readonly object _syncRoot = new();
    private readonly HashSet<Task> _activeOperations = [];
    private CancellationTokenSource _generationCancellation = new();

    internal UploadInputPipeline(Upload owner)
    {
        _owner = owner;
    }

    internal Task ProcessFilesAsync(
        UploadInputSource source,
        IReadOnlyList<UploadFileInfo> files,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(files);
        return TrackOperation(source, (operation, token) =>
            ProcessFileInfosAsync(operation, files, ownsFileSources: false, token), cancellationToken);
    }

    internal Task ProcessStorageItemsAsync(
        UploadInputSource source,
        IReadOnlyList<IStorageItem> storageItems,
        UploadDirectoryDropMode directoryMode,
        int maxDirectoryDepth,
        int maxEnumeratedItems,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storageItems);

        return TrackOperation(source, async (operation, token) =>
        {
            foreach (var storageItem in storageItems)
            {
                operation.Transfer(storageItem);
            }

            var enumeration = await UploadStorageItemEnumerator.EnumerateAsync(
                storageItems,
                directoryMode,
                maxDirectoryDepth,
                maxEnumeratedItems,
                token).ConfigureAwait(false);

            foreach (var rejection in enumeration.RejectedItems)
            {
                operation.Reject(rejection);
            }

            var fileInfos = new List<UploadFileInfo>(enumeration.Candidates.Count);
            foreach (var candidate in enumeration.Candidates)
            {
                using (candidate)
                {
                    token.ThrowIfCancellationRequested();
                    try
                    {
                        var file = await candidate.CreateFileInfoAsync(token).ConfigureAwait(false);
                        operation.OwnFileSource(file);
                        fileInfos.Add(file);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        operation.Reject(new UploadRejectedItem(
                            candidate.Name,
                            candidate.Path,
                            UploadRejectionReason.MetadataReadFailed,
                            Message: ex.Message,
                            Exception: ex));
                    }
                }
            }

            await ProcessFileInfosAsync(operation, fileInfos, ownsFileSources: true, token)
                .ConfigureAwait(false);
        }, cancellationToken, storageItems.Cast<IDisposable>());
    }

    internal async Task CancelAllAsync(CancellationToken cancellationToken = default)
    {
        CancellationTokenSource oldGeneration;
        Task[] activeOperations;
        lock (_syncRoot)
        {
            oldGeneration = _generationCancellation;
            _generationCancellation = new CancellationTokenSource();
            activeOperations = _activeOperations.ToArray();
        }

        oldGeneration.Cancel();
        try
        {
            await Task.WhenAll(activeOperations).WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
        }
        finally
        {
            oldGeneration.Dispose();
        }
    }

    private Task TrackOperation(
        UploadInputSource source,
        Func<UploadInputBatchOperation, CancellationToken, Task> process,
        CancellationToken cancellationToken,
        IEnumerable<IDisposable>? initiallyOwnedResources = null)
    {
        var operation = new UploadInputBatchOperation(source);
        if (initiallyOwnedResources is not null)
        {
            foreach (var resource in initiallyOwnedResources)
            {
                operation.Own(resource);
            }
        }

        CancellationTokenSource linkedCancellation;
        Task operationTask;
        lock (_syncRoot)
        {
            linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                _generationCancellation.Token,
                cancellationToken);
            operationTask = ExecuteAsync(operation, process, linkedCancellation);
            _activeOperations.Add(operationTask);
        }

        return ObserveOperationAsync(operationTask);
    }

    private async Task ExecuteAsync(
        UploadInputBatchOperation operation,
        Func<UploadInputBatchOperation, CancellationToken, Task> process,
        CancellationTokenSource linkedCancellation)
    {
        using (operation)
        {
            var gateEntered = false;
            try
            {
                await _arrivalGate.WaitAsync(linkedCancellation.Token).ConfigureAwait(false);
                gateEntered = true;
                await process(operation, linkedCancellation.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                operation.MarkCancelled();
            }
            catch (Exception ex)
            {
                operation.Reject(new UploadRejectedItem(
                    string.Empty,
                    null,
                    UploadRejectionReason.InputFailed,
                    Message: ex.Message,
                    Exception: ex));
            }
            finally
            {
                if (gateEntered)
                {
                    _arrivalGate.Release();
                }

                try
                {
                    await _owner.RaiseInputBatchCompletedAsync(operation.CreateCompletedEventArgs())
                                .ConfigureAwait(false);
                }
                finally
                {
                    linkedCancellation.Dispose();
                }
            }
        }
    }

    private async Task ObserveOperationAsync(Task operationTask)
    {
        try
        {
            await operationTask.ConfigureAwait(false);
        }
        finally
        {
            lock (_syncRoot)
            {
                _activeOperations.Remove(operationTask);
            }
        }
    }

    private async Task ProcessFileInfosAsync(
        UploadInputBatchOperation operation,
        IReadOnlyList<UploadFileInfo> files,
        bool ownsFileSources,
        CancellationToken cancellationToken)
    {
        var options = await _owner.GetInputPipelineOptionsAsync().ConfigureAwait(false);
        var admittedFiles = new List<UploadFileInfo>(files.Count);

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(file);
            if (ownsFileSources)
            {
                operation.OwnFileSource(file);
            }

            var admission = await UploadFileAdmissionService.EvaluateAsync(
                operation.BatchId,
                operation.Source,
                file,
                options.AllowedFileTypes,
                options.AdmissionPolicy,
                cancellationToken).ConfigureAwait(false);
            if (admission.IsAccepted)
            {
                admittedFiles.Add(file);
            }
            else
            {
                operation.Reject(file, admission.Rejection!);
            }
        }

        var filesToCommit = ApplyCountPolicy(operation, admittedFiles, options);
        if (filesToCommit.Count == 0)
        {
            return;
        }

        if (options.CountOverflowBehavior == UploadCountOverflowBehavior.ReplaceExisting)
        {
            await _owner.RemoveAllFilesForReplacementAsync(cancellationToken).ConfigureAwait(false);
        }

        await _owner.CommitInputFilesAsync(filesToCommit, file =>
        {
            if (file.Source is IUploadFileSourceLease lease)
            {
                operation.Transfer(lease);
            }
            operation.Accept(file);
        }).ConfigureAwait(false);
    }

    private static IReadOnlyList<UploadFileInfo> ApplyCountPolicy(
        UploadInputBatchOperation operation,
        IReadOnlyList<UploadFileInfo> admittedFiles,
        UploadInputPipelineOptions options)
    {
        var maxCount = Math.Max(0, options.MaxCount);
        var availableCount = Math.Max(0, maxCount - options.ExistingCount);
        if (options.CountOverflowBehavior == UploadCountOverflowBehavior.ReplaceExisting)
        {
            availableCount = maxCount;
        }

        if (admittedFiles.Count <= availableCount)
        {
            return admittedFiles;
        }

        if (options.CountOverflowBehavior == UploadCountOverflowBehavior.RejectBatch)
        {
            foreach (var file in admittedFiles)
            {
                operation.Reject(file, CreateCountRejection(file));
            }
            return [];
        }

        var accepted = admittedFiles.Take(availableCount).ToArray();
        foreach (var file in admittedFiles.Skip(availableCount))
        {
            operation.Reject(file, CreateCountRejection(file));
        }
        return accepted;
    }

    private static UploadRejectedItem CreateCountRejection(UploadFileInfo file)
    {
        return new UploadRejectedItem(
            file.Name,
            file.Path,
            UploadRejectionReason.CountLimitExceeded,
            Message: "The upload count limit has been reached.");
    }
}

internal sealed record UploadInputPipelineOptions(
    IReadOnlyList<FilePickerFileType>? AllowedFileTypes,
    IUploadAdmissionPolicy? AdmissionPolicy,
    UploadCountOverflowBehavior CountOverflowBehavior,
    int MaxCount,
    int ExistingCount);
