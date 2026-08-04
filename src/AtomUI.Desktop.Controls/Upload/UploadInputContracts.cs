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

public enum UploadInputBatchStatus
{
    Completed,
    Cancelled,
    Failed
}

public enum UploadInputFailureReason
{
    DataSnapshotFailed,
    ProcessingFailed
}

public enum UploadRejectionReason
{
    UnsupportedStorageItem,
    DirectoryNotAllowed,
    DirectoryDepthExceeded,
    DirectoryCycleDetected,
    EnumerationLimitExceeded,
    AccessDenied,
    StorageReadFailed,
    FileTypeNotAllowed,
    AdmissionRejected,
    AdmissionPolicyFailed,
    CountLimitExceeded
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

public sealed class UploadAdmissionDecision
{
    public bool IsAccepted { get; }
    public string? RejectionCode { get; }
    public string? Message { get; }

    private UploadAdmissionDecision(bool isAccepted, string? rejectionCode, string? message)
    {
        IsAccepted    = isAccepted;
        RejectionCode = rejectionCode;
        Message       = message;
    }

    public static UploadAdmissionDecision Accept() => new(true, null, null);

    public static UploadAdmissionDecision Reject(
        string? rejectionCode = null,
        string? message = null) =>
        new(false, rejectionCode, message);
}

public sealed class UploadRejectedItem
{
    public string Name { get; }
    public Uri? Path { get; }
    public UploadRejectionReason Reason { get; }
    public string? RejectionCode { get; }
    public string? Message { get; }

    internal UploadRejectedItem(
        string name,
        Uri? path,
        UploadRejectionReason reason,
        string? rejectionCode = null,
        string? message = null)
    {
        Name          = name;
        Path          = path;
        Reason        = reason;
        RejectionCode = rejectionCode;
        Message       = message;
    }
}

public sealed class UploadInputBatchCompletedEventArgs : EventArgs
{
    public Guid BatchId { get; }
    public UploadInputSource Source { get; }
    public UploadInputBatchStatus Status { get; }
    public UploadInputFailureReason? FailureReason { get; }
    public IReadOnlyList<UploadFileInfo> AcceptedFiles { get; }
    public IReadOnlyList<UploadRejectedItem> RejectedItems { get; }

    internal UploadInputBatchCompletedEventArgs(
        Guid batchId,
        UploadInputSource source,
        UploadInputBatchStatus status,
        UploadInputFailureReason? failureReason,
        IReadOnlyList<UploadFileInfo> acceptedFiles,
        IReadOnlyList<UploadRejectedItem> rejectedItems)
    {
        ArgumentNullException.ThrowIfNull(acceptedFiles);
        ArgumentNullException.ThrowIfNull(rejectedItems);
        if ((status == UploadInputBatchStatus.Failed) != failureReason.HasValue)
        {
            throw new ArgumentException("FailureReason must be set if and only if Status is Failed.");
        }

        BatchId       = batchId;
        Source        = source;
        Status        = status;
        FailureReason = failureReason;
        AcceptedFiles = acceptedFiles;
        RejectedItems = rejectedItems;
    }
}
