using AtomUI.Controls;

namespace AtomUI.Desktop.Controls;

public enum UploadDragState
{
    None,
    Accepting,
    Rejecting
}

public enum UploadDirectoryDropMode
{
    Reject,
    TopLevelFiles,
    RecursiveFiles
}

public enum UploadCountOverflowBehavior
{
    RejectExcess,
    RejectBatch,
    ReplaceExisting
}

public enum UploadInputSource
{
    FilePicker,
    DirectoryPicker,
    DragDrop,
    Programmatic
}

public enum UploadRejectionReason
{
    UnsupportedDataFormat,
    UnsupportedStorageItem,
    DirectoryNotAllowed,
    DirectoryDepthExceeded,
    DirectoryCycleDetected,
    EnumerationLimitExceeded,
    AccessDenied,
    MetadataReadFailed,
    ContentSourceCreationFailed,
    FileTypeNotAllowed,
    AdmissionRejected,
    CountLimitExceeded,
    Cancelled,
    InputFailed
}

public interface IUploadAdmissionPolicy
{
    ValueTask<UploadAdmissionDecision> EvaluateAsync(
        UploadAdmissionContext context,
        CancellationToken cancellationToken = default);
}

public sealed record UploadAdmissionContext(
    Guid BatchId,
    UploadInputSource Source,
    UploadFileInfo File);

public sealed record UploadAdmissionDecision(
    bool IsAccepted,
    string? RejectionCode = null,
    string? Message = null);

public sealed record UploadRejectedItem(
    string Name,
    Uri? Path,
    UploadRejectionReason Reason,
    string? RejectionCode = null,
    string? Message = null,
    Exception? Exception = null);

public sealed class UploadInputBatchCompletedEventArgs : EventArgs
{
    public Guid BatchId { get; }
    public UploadInputSource Source { get; }
    public IReadOnlyList<UploadFileInfo> AcceptedFiles { get; }
    public IReadOnlyList<UploadRejectedItem> RejectedItems { get; }
    public bool IsCancelled { get; }

    public UploadInputBatchCompletedEventArgs(
        Guid batchId,
        UploadInputSource source,
        IReadOnlyList<UploadFileInfo> acceptedFiles,
        IReadOnlyList<UploadRejectedItem> rejectedItems,
        bool isCancelled)
    {
        ArgumentNullException.ThrowIfNull(acceptedFiles);
        ArgumentNullException.ThrowIfNull(rejectedItems);

        BatchId       = batchId;
        Source        = source;
        AcceptedFiles = acceptedFiles;
        RejectedItems = rejectedItems;
        IsCancelled   = isCancelled;
    }
}
