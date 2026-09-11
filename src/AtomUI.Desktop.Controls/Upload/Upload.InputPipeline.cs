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

    internal Task ProcessInputFailureAsync(
        UploadInputSource source,
        UploadInputFailureReason failureReason,
        Exception exception)
    {
        return _inputPipeline.ProcessFailureAsync(source, failureReason, exception);
    }

    internal async Task<UploadInputPipelineOptions> GetInputPipelineOptionsAsync()
    {
        UploadInputPipelineOptions? options = null;
        await InvokeOnUiThreadAsync(() =>
        {
            var fileTypes = AllowedFileTypes?.Select(fileType => new UploadFileTypeRule(
                fileType.Patterns?.ToArray() ?? [],
                fileType.MimeTypes?.ToArray() ?? [])).ToArray() ?? [];
            options = new UploadInputPipelineOptions(
                fileTypes,
                AdmissionPolicy,
                CountOverflowBehavior,
                MaxCount,
                IsMultipleEnabled);
        }).ConfigureAwait(false);
        return options!;
    }

    internal async Task<IReadOnlyList<UploadFileInfo>> CommitInputFilesAsync(
        IReadOnlyList<UploadFileInfo> files,
        UploadInputBatchOperation operation,
        UploadInputPipelineOptions options)
    {
        IReadOnlyList<UploadFileInfo>? rejectedFiles = null;
        await InvokeOnUiThreadAsync(() =>
            rejectedFiles = CommitInputFiles(files, operation, options)).ConfigureAwait(false);
        return rejectedFiles!;
    }

    internal async Task<IReadOnlyList<UploadFileInfo>> ReplaceInputFilesAsync(
        IReadOnlyList<UploadFileInfo> files,
        UploadInputBatchOperation operation,
        UploadInputPipelineOptions options)
    {
        IReadOnlyList<UploadFileInfo>? rejectedFiles = null;
        await InvokeOnUiThreadAsync(() =>
            rejectedFiles = ReplaceInputFiles(files, operation, options)).ConfigureAwait(false);
        return rejectedFiles!;
    }

    internal Task RaiseInputBatchCompletedAsync(UploadInputBatchCompletedEventArgs args)
    {
        return InvokeOnUiThreadAsync(() => InputBatchCompleted?.Invoke(this, args));
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
