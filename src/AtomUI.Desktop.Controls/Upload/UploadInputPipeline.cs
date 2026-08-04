using System.Diagnostics;
using System.Runtime.ExceptionServices;
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
        var operation = new UploadInputBatchOperation(source);
        return TrackOperation(operation, (batch, options, token) =>
            ProcessFileInfosAsync(batch, files, options, token), cancellationToken);
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

        var operation = new UploadInputBatchOperation(source);
        var seenStorageItems = new HashSet<IStorageItem>(ReferenceEqualityComparer.Instance);
        var snapshot = new List<IStorageItem>(storageItems.Count);
        foreach (var storageItem in storageItems)
        {
            ArgumentNullException.ThrowIfNull(storageItem);
            if (seenStorageItems.Add(storageItem))
            {
                operation.AdoptStorageItem(storageItem);
                snapshot.Add(storageItem);
            }
        }

        return TrackOperation(operation, async (batch, options, token) =>
        {
            var enumeration = await UploadStorageItemEnumerator.EnumerateAsync(
                batch,
                snapshot,
                directoryMode,
                maxDirectoryDepth,
                maxEnumeratedItems,
                token).ConfigureAwait(false);

            foreach (var rejection in enumeration.RejectedItems)
            {
                batch.Reject(rejection);
            }

            var fileInfos = new List<UploadFileInfo>(enumeration.Candidates.Count);
            foreach (var candidate in enumeration.Candidates)
            {
                token.ThrowIfCancellationRequested();
                StorageItemProperties properties;
                long? size;
                try
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(candidate.Name);
                    properties = await candidate.StorageFile.GetBasicPropertiesAsync()
                                                .WaitAsync(token)
                                                .ConfigureAwait(false);
                    size = properties.Size switch
                    {
                        null => null,
                        <= long.MaxValue => (long)properties.Size.Value,
                        _ => throw new OverflowException("The storage item size exceeds Int64.MaxValue.")
                    };
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Upload storage metadata read failed for '{candidate.Name}': {ex.Message}");
                    batch.ReleaseStorageItem(candidate.StorageFile);
                    batch.Reject(new UploadRejectedItem(
                        candidate.Name,
                        candidate.Path,
                        UploadRejectionReason.StorageReadFailed,
                        message: "The storage file could not be read."));
                    continue;
                }

                fileInfos.Add(CreateStorageFileInfo(batch, candidate, properties, size));
            }

            await ProcessFileInfosAsync(batch, fileInfos, options, token)
                .ConfigureAwait(false);
        }, cancellationToken);
    }

    internal Task ProcessFailureAsync(
        UploadInputSource source,
        UploadInputFailureReason failureReason,
        Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        return TrackTerminalFailure(source, failureReason, exception);
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
        UploadInputBatchOperation operation,
        Func<UploadInputBatchOperation, UploadInputPipelineOptions, CancellationToken, Task> process,
        CancellationToken cancellationToken,
        UploadInputFailureReason processingFailureReason = UploadInputFailureReason.ProcessingFailed)
    {
        CancellationTokenSource linkedCancellation;
        Task operationTask;
        lock (_syncRoot)
        {
            linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                _generationCancellation.Token,
                cancellationToken);
            operationTask = ExecuteAsync(operation, process, linkedCancellation, processingFailureReason);
            _activeOperations.Add(operationTask);
        }

        return ObserveOperationAsync(operationTask);
    }

    private async Task ExecuteAsync(
        UploadInputBatchOperation operation,
        Func<UploadInputBatchOperation, UploadInputPipelineOptions, CancellationToken, Task> process,
        CancellationTokenSource linkedCancellation,
        UploadInputFailureReason processingFailureReason)
    {
        var gateEntered = false;
        var wasCancelled = false;
        Exception? terminalException = null;
        try
        {
            await _arrivalGate.WaitAsync(linkedCancellation.Token).ConfigureAwait(false);
            gateEntered = true;
            var options = await _owner.GetInputPipelineOptionsAsync().ConfigureAwait(false);
            await Task.Run(
                () => process(operation, options, linkedCancellation.Token),
                CancellationToken.None).ConfigureAwait(false);
        }
        catch (OperationCanceledException ex) when (linkedCancellation.IsCancellationRequested)
        {
            wasCancelled = true;
            terminalException = ex;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Upload input batch failed: {ex.Message}");
            terminalException = ex;
        }

        Exception? cleanupException = null;
        try
        {
            operation.Dispose();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Upload input batch cleanup failed: {ex.Message}");
            cleanupException = ex;
            terminalException = CombineExceptions(terminalException, ex);
        }

        var status = cleanupException is not null || terminalException is not null && !wasCancelled
            ? UploadInputBatchStatus.Failed
            : wasCancelled
                ? UploadInputBatchStatus.Cancelled
                : UploadInputBatchStatus.Completed;
        UploadInputFailureReason? failureReason = status == UploadInputBatchStatus.Failed
            ? processingFailureReason
            : null;
        operation.SetTerminalState(status, failureReason);
        var eventArgs = operation.CreateCompletedEventArgs();

        try
        {
            await _owner.RaiseInputBatchCompletedAsync(eventArgs).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            terminalException = CombineExceptions(terminalException, ex);
        }
        finally
        {
            if (gateEntered)
            {
                _arrivalGate.Release();
            }
            linkedCancellation.Dispose();
        }

        if (terminalException is not null)
        {
            ExceptionDispatchInfo.Capture(terminalException).Throw();
        }
    }

    private Task TrackTerminalFailure(
        UploadInputSource source,
        UploadInputFailureReason failureReason,
        Exception exception)
    {
        var operation = new UploadInputBatchOperation(source);
        return TrackOperation(
            operation,
            (_, _, _) => Task.FromException(exception),
            CancellationToken.None,
            failureReason);
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
        UploadInputPipelineOptions options,
        CancellationToken cancellationToken)
    {
        var admittedFiles = new List<UploadFileInfo>(files.Count);

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(file);
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

        if (admittedFiles.Count == 0)
        {
            return;
        }

        IReadOnlyList<UploadFileInfo> rejectedFiles;
        if (options.CountOverflowBehavior == UploadCountOverflowBehavior.ReplaceExisting)
        {
            rejectedFiles = await _owner.ReplaceInputFilesAsync(admittedFiles, operation, options)
                                        .ConfigureAwait(false);
        }
        else
        {
            rejectedFiles = await _owner.CommitInputFilesAsync(admittedFiles, operation, options)
                                        .ConfigureAwait(false);
        }

        foreach (var file in rejectedFiles)
        {
            operation.Reject(file, CreateCountRejection(file));
        }
    }

    private static UploadRejectedItem CreateCountRejection(UploadFileInfo file)
    {
        return new UploadRejectedItem(
            file.Name,
            file.Path,
            UploadRejectionReason.CountLimitExceeded,
            message: "The upload count limit has been reached.");
    }

    private static UploadFileInfo CreateStorageFileInfo(
        UploadInputBatchOperation operation,
        UploadInputCandidate candidate,
        StorageItemProperties properties,
        long? size)
    {
        var source = operation.PromoteStorageFile(candidate.StorageFile);
        return new UploadFileInfo(
            candidate.Name,
            source,
            candidate.Path,
            size,
            dateCreated: properties.DateCreated,
            dateModified: properties.DateModified);
    }

    private static Exception CombineExceptions(Exception? first, Exception second)
    {
        return first is null
            ? second
            : new AggregateException(first, second).Flatten();
    }
}

internal sealed record UploadFileTypeRule(
    IReadOnlyList<string> Patterns,
    IReadOnlyList<string> MimeTypes);

internal sealed record UploadInputPipelineOptions(
    IReadOnlyList<UploadFileTypeRule> AllowedFileTypes,
    IUploadAdmissionPolicy? AdmissionPolicy,
    UploadCountOverflowBehavior CountOverflowBehavior,
    int MaxCount);
