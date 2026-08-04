using AtomUI.Controls;
using Avalonia.Platform.Storage;

namespace AtomUI.Desktop.Controls;

public partial class Upload
{
    internal Task ProcessStorageItemsAsync(
        UploadInputSource source,
        IReadOnlyList<IStorageItem> storageItems,
        UploadDirectoryDropMode directoryMode,
        int maxDirectoryDepth,
        int maxEnumeratedItems,
        CancellationToken cancellationToken = default)
    {
        return _inputPipeline.ProcessStorageItemsAsync(
            source,
            storageItems,
            directoryMode,
            maxDirectoryDepth,
            maxEnumeratedItems,
            cancellationToken);
    }

    internal async Task<UploadInputPipelineOptions> GetInputPipelineOptionsAsync()
    {
        UploadInputPipelineOptions? options = null;
        await InvokeOnUiThreadAsync(() => options = new UploadInputPipelineOptions(
            AllowedFileTypes,
            AdmissionPolicy,
            CountOverflowBehavior,
            MaxCount,
            EffectiveFiles.Count)).ConfigureAwait(false);
        return options!;
    }

    internal Task CommitInputFilesAsync(
        IReadOnlyList<UploadFileInfo> files,
        Action<UploadFileInfo> transferOwnership)
    {
        return InvokeOnUiThreadAsync(() =>
        {
            foreach (var file in files)
            {
                CommitInputFile(file, transferOwnership);
            }
        });
    }

    internal async Task RemoveAllFilesForReplacementAsync(CancellationToken cancellationToken)
    {
        var fileIds = await GetEffectiveFileIdsAsync().ConfigureAwait(false);
        foreach (var fileId in fileIds)
        {
            await _uploadQueue.CancelAsync(fileId, cancellationToken).ConfigureAwait(false);
        }

        await InvokeOnUiThreadAsync(() =>
        {
            foreach (var item in EffectiveFiles.ToArray())
            {
                RemoveFileCore(item, raiseRemovedEvent: true);
            }
        }).ConfigureAwait(false);
    }

    internal Task RaiseInputBatchCompletedAsync(UploadInputBatchCompletedEventArgs args)
    {
        return InvokeOnUiThreadAsync(() => InputBatchCompleted?.Invoke(this, args));
    }

    private async Task<Guid[]> GetEffectiveFileIdsAsync()
    {
        Guid[]? ids = null;
        await InvokeOnUiThreadAsync(() => ids = EffectiveFiles.Select(file => file.Id).ToArray())
            .ConfigureAwait(false);
        return ids!;
    }

    private Task InvokeOnUiThreadAsync(Action action)
    {
        if (Dispatcher.CheckAccess())
        {
            action();
            return Task.CompletedTask;
        }

        return Dispatcher.InvokeAsync(action).GetTask();
    }
}
