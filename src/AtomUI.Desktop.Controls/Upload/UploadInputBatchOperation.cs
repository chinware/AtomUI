using AtomUI.Controls;

namespace AtomUI.Desktop.Controls;

internal sealed class UploadInputBatchOperation : IDisposable
{
    private readonly HashSet<IDisposable> _ownedResources = new(ReferenceEqualityComparer.Instance);
    private readonly List<UploadFileInfo> _acceptedFiles = [];
    private readonly List<UploadRejectedItem> _rejectedItems = [];

    internal Guid BatchId { get; } = Guid.NewGuid();
    internal UploadInputSource Source { get; }
    internal UploadInputBatchStatus Status { get; private set; } = UploadInputBatchStatus.Completed;
    internal UploadInputFailureReason? FailureReason { get; private set; }

    internal UploadInputBatchOperation(UploadInputSource source)
    {
        Source = source;
    }

    internal void Own(IDisposable resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        _ownedResources.Add(resource);
    }

    internal void Transfer(IDisposable resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        _ownedResources.Remove(resource);
    }

    internal void OwnFileSource(UploadFileInfo file)
    {
        if (file.Source is IUploadFileSourceLease lease)
        {
            Own(lease);
        }
    }

    internal void Accept(UploadFileInfo file)
    {
        _acceptedFiles.Add(file);
    }

    internal void Reject(UploadRejectedItem rejection)
    {
        _rejectedItems.Add(rejection);
    }

    internal void Reject(UploadFileInfo file, UploadRejectedItem rejection)
    {
        if (file.Source is IUploadFileSourceLease lease && _ownedResources.Remove(lease))
        {
            lease.Dispose();
        }

        _rejectedItems.Add(rejection);
    }

    internal void MarkCancelled()
    {
        if (Status != UploadInputBatchStatus.Failed)
        {
            Status = UploadInputBatchStatus.Cancelled;
        }
    }

    internal void MarkFailed(UploadInputFailureReason reason)
    {
        Status        = UploadInputBatchStatus.Failed;
        FailureReason = reason;
    }

    internal UploadInputBatchCompletedEventArgs CreateCompletedEventArgs()
    {
        return new UploadInputBatchCompletedEventArgs(
            BatchId,
            Source,
            Status,
            FailureReason,
            _acceptedFiles.ToArray(),
            _rejectedItems.ToArray());
    }

    public void Dispose()
    {
        foreach (var resource in _ownedResources)
        {
            resource.Dispose();
        }

        _ownedResources.Clear();
    }
}
