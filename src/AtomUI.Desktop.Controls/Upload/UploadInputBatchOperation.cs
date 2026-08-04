using AtomUI.Controls;
using Avalonia.Platform.Storage;

namespace AtomUI.Desktop.Controls;

internal sealed class UploadInputBatchOperation : IDisposable
{
    private readonly HashSet<IStorageItem> _ownedStorageItems =
        new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<IUploadFileSourceLease> _ownedSourceLeases =
        new(ReferenceEqualityComparer.Instance);
    private readonly List<UploadFileInfo> _acceptedFiles = [];
    private readonly List<UploadRejectedItem> _rejectedItems = [];
    private bool _isDisposed;

    internal Guid BatchId { get; } = Guid.NewGuid();
    internal UploadInputSource Source { get; }
    internal UploadInputBatchStatus Status { get; private set; } = UploadInputBatchStatus.Completed;
    internal UploadInputFailureReason? FailureReason { get; private set; }

    internal UploadInputBatchOperation(UploadInputSource source)
    {
        Source = source;
    }

    internal void AdoptStorageItem(IStorageItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        ThrowIfDisposed();
        if (!_ownedStorageItems.Add(item))
        {
            throw new InvalidOperationException("The storage item is already owned by this input batch.");
        }
    }

    internal void ReleaseStorageItem(IStorageItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        ThrowIfDisposed();
        if (!_ownedStorageItems.Remove(item))
        {
            throw new InvalidOperationException("The storage item is not owned by this input batch.");
        }
        item.Dispose();
    }

    internal UploadStorageFileSource PromoteStorageFile(IStorageFile file)
    {
        ArgumentNullException.ThrowIfNull(file);
        ThrowIfDisposed();
        if (!_ownedStorageItems.Remove(file))
        {
            throw new InvalidOperationException("The storage file is not owned by this input batch.");
        }

        UploadStorageFileSource source;
        try
        {
            source = new UploadStorageFileSource(file);
        }
        catch
        {
            file.Dispose();
            throw;
        }

        if (!_ownedSourceLeases.Add(source))
        {
            source.Dispose();
            throw new InvalidOperationException("The promoted file source is already owned by this input batch.");
        }
        return source;
    }

    internal bool OwnsFileSource(UploadFileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);
        return !_isDisposed &&
               file.Source is IUploadFileSourceLease lease &&
               _ownedSourceLeases.Contains(lease);
    }

    internal void TransferFileSourceToUpload(UploadFileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);
        ThrowIfDisposed();
        if (file.Source is not IUploadFileSourceLease lease || !_ownedSourceLeases.Remove(lease))
        {
            throw new InvalidOperationException("The file source is not owned by this input batch.");
        }
    }

    internal void ReleaseFileSource(UploadFileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);
        ThrowIfDisposed();
        if (file.Source is not IUploadFileSourceLease lease || !_ownedSourceLeases.Remove(lease))
        {
            throw new InvalidOperationException("The file source is not owned by this input batch.");
        }
        lease.Dispose();
    }

    internal void Accept(UploadFileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);
        ThrowIfDisposed();
        _acceptedFiles.Add(file);
    }

    internal void Reject(UploadRejectedItem rejection)
    {
        ArgumentNullException.ThrowIfNull(rejection);
        ThrowIfDisposed();
        _rejectedItems.Add(rejection);
    }

    internal void Reject(UploadFileInfo file, UploadRejectedItem rejection)
    {
        if (file.Source is IUploadFileSourceLease)
        {
            ReleaseFileSource(file);
        }
        Reject(rejection);
    }

    internal void MarkCancelled()
    {
        ThrowIfDisposed();
        if (Status != UploadInputBatchStatus.Failed)
        {
            Status = UploadInputBatchStatus.Cancelled;
        }
    }

    internal void MarkFailed(UploadInputFailureReason reason)
    {
        ThrowIfDisposed();
        Status        = UploadInputBatchStatus.Failed;
        FailureReason = reason;
    }

    internal UploadInputBatchCompletedEventArgs CreateCompletedEventArgs()
    {
        ThrowIfDisposed();
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
        if (_isDisposed)
        {
            return;
        }
        _isDisposed = true;

        var storageItems = _ownedStorageItems.ToArray();
        var sourceLeases = _ownedSourceLeases.ToArray();
        _ownedStorageItems.Clear();
        _ownedSourceLeases.Clear();

        List<Exception>? exceptions = null;
        foreach (var storageItem in storageItems)
        {
            try
            {
                storageItem.Dispose();
            }
            catch (Exception ex)
            {
                (exceptions ??= []).Add(ex);
            }
        }

        foreach (var sourceLease in sourceLeases)
        {
            try
            {
                sourceLease.Dispose();
            }
            catch (Exception ex)
            {
                (exceptions ??= []).Add(ex);
            }
        }

        if (exceptions is not null)
        {
            throw new AggregateException(exceptions);
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
    }
}
