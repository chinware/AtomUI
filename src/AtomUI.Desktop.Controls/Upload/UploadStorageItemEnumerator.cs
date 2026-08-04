using System.Diagnostics;
using Avalonia.Platform.Storage;

namespace AtomUI.Desktop.Controls;

internal static class UploadStorageItemEnumerator
{
    internal static async Task<UploadStorageEnumerationResult> EnumerateAsync(
        UploadInputBatchOperation operation,
        IReadOnlyList<IStorageItem> storageItems,
        UploadDirectoryDropMode directoryMode,
        int maxDirectoryDepth,
        int maxEnumeratedItems,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(storageItems);
        ArgumentOutOfRangeException.ThrowIfNegative(maxDirectoryDepth);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxEnumeratedItems, 1);

        var candidates = new List<UploadInputCandidate>();
        var rejectedItems = new List<UploadRejectedItem>();
        var context = new DirectoryEnumerationContext(
            operation,
            storageItems,
            directoryMode,
            maxDirectoryDepth,
            maxEnumeratedItems,
            candidates,
            rejectedItems);

        foreach (var storageItem in storageItems)
        {
            cancellationToken.ThrowIfCancellationRequested();
            switch (storageItem)
            {
                case IStorageFile storageFile:
                    TryAddCandidate(operation, storageFile, candidates, rejectedItems);
                    break;

                case IStorageFolder storageFolder:
                    if (directoryMode == UploadDirectoryDropMode.Reject)
                    {
                        rejectedItems.Add(CreateRejection(
                            storageFolder,
                            UploadRejectionReason.DirectoryNotAllowed,
                            "Directory input is disabled."));
                        operation.ReleaseStorageItem(storageFolder);
                        break;
                    }

                    context.TryVisit(storageFolder);
                    await EnumerateFolderAsync(storageFolder, 0, context, cancellationToken).ConfigureAwait(false);
                    break;

                default:
                    rejectedItems.Add(CreateRejection(
                        storageItem,
                        UploadRejectionReason.UnsupportedStorageItem,
                        "The storage item is neither a file nor a folder."));
                    operation.ReleaseStorageItem(storageItem);
                    break;
            }
        }

        return new UploadStorageEnumerationResult(candidates, rejectedItems);
    }

    private static async Task EnumerateFolderAsync(
        IStorageFolder folder,
        int depth,
        DirectoryEnumerationContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var child in folder.GetItemsAsync()
                                              .WithCancellation(cancellationToken)
                                              .ConfigureAwait(false))
            {
                if (!context.TryAdoptChild(child))
                {
                    continue;
                }

                if (!context.TryObserve(child))
                {
                    context.Operation.ReleaseStorageItem(child);
                    break;
                }

                switch (child)
                {
                    case IStorageFile storageFile:
                        TryAddCandidate(
                            context.Operation,
                            storageFile,
                            context.Candidates,
                            context.RejectedItems);
                        break;

                    case IStorageFolder childFolder:
                        if (context.DirectoryMode == UploadDirectoryDropMode.TopLevelFiles)
                        {
                            context.RejectedItems.Add(CreateRejection(
                                childFolder,
                                UploadRejectionReason.DirectoryNotAllowed,
                                "Nested directories are not expanded in TopLevelFiles mode."));
                            context.Operation.ReleaseStorageItem(childFolder);
                        }
                        else if (depth + 1 > context.MaxDirectoryDepth)
                        {
                            context.RejectedItems.Add(CreateRejection(
                                childFolder,
                                UploadRejectionReason.DirectoryDepthExceeded,
                                $"Directory depth exceeds {context.MaxDirectoryDepth}."));
                            context.Operation.ReleaseStorageItem(childFolder);
                        }
                        else if (!context.TryVisit(childFolder))
                        {
                            context.RejectedItems.Add(CreateRejection(
                                childFolder,
                                UploadRejectionReason.DirectoryCycleDetected,
                                "The directory was already visited."));
                            context.Operation.ReleaseStorageItem(childFolder);
                        }
                        else
                        {
                            await EnumerateFolderAsync(childFolder, depth + 1, context, cancellationToken)
                                .ConfigureAwait(false);
                        }
                        break;

                    default:
                        context.RejectedItems.Add(CreateRejection(
                            child,
                            UploadRejectionReason.UnsupportedStorageItem,
                            "The storage item is neither a file nor a folder."));
                        context.Operation.ReleaseStorageItem(child);
                        break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            Debug.WriteLine($"Upload directory access failed: {ex.Message}");
            context.RejectedItems.Add(CreateRejection(
                folder,
                UploadRejectionReason.AccessDenied,
                "Directory access was denied."));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Upload directory enumeration failed: {ex.Message}");
            context.RejectedItems.Add(CreateRejection(
                folder,
                UploadRejectionReason.StorageReadFailed,
                "Directory enumeration failed."));
        }
        finally
        {
            context.Operation.ReleaseStorageItem(folder);
        }
    }

    private static void TryAddCandidate(
        UploadInputBatchOperation operation,
        IStorageFile storageFile,
        ICollection<UploadInputCandidate> candidates,
        ICollection<UploadRejectedItem> rejectedItems)
    {
        try
        {
            candidates.Add(new UploadInputCandidate(storageFile));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Upload storage item snapshot failed: {ex.Message}");
            rejectedItems.Add(CreateRejection(
                storageFile,
                UploadRejectionReason.StorageReadFailed,
                "The storage file could not be read."));
            operation.ReleaseStorageItem(storageFile);
        }
    }

    private static UploadRejectedItem CreateRejection(
        IStorageItem storageItem,
        UploadRejectionReason reason,
        string message)
    {
        string name;
        Uri? path;
        try
        {
            name = storageItem.Name;
        }
        catch
        {
            name = string.Empty;
        }

        try
        {
            path = storageItem.Path;
        }
        catch
        {
            path = null;
        }

        return new UploadRejectedItem(name, path, reason, message: message);
    }

    private sealed class DirectoryEnumerationContext
    {
        private readonly HashSet<IStorageItem> _observedItems;
        private readonly HashSet<string> _visitedDirectories = new(StringComparer.Ordinal);
        private int _enumeratedItemCount;

        internal UploadInputBatchOperation Operation { get; }
        internal UploadDirectoryDropMode DirectoryMode { get; }
        internal int MaxDirectoryDepth { get; }
        internal int MaxEnumeratedItems { get; }
        internal List<UploadInputCandidate> Candidates { get; }
        internal List<UploadRejectedItem> RejectedItems { get; }

        internal DirectoryEnumerationContext(
            UploadInputBatchOperation operation,
            IEnumerable<IStorageItem> storageItems,
            UploadDirectoryDropMode directoryMode,
            int maxDirectoryDepth,
            int maxEnumeratedItems,
            List<UploadInputCandidate> candidates,
            List<UploadRejectedItem> rejectedItems)
        {
            Operation          = operation;
            DirectoryMode      = directoryMode;
            MaxDirectoryDepth  = maxDirectoryDepth;
            MaxEnumeratedItems = maxEnumeratedItems;
            Candidates         = candidates;
            RejectedItems      = rejectedItems;
            _observedItems     = new HashSet<IStorageItem>(storageItems, ReferenceEqualityComparer.Instance);
        }

        internal bool TryAdoptChild(IStorageItem storageItem)
        {
            if (!_observedItems.Add(storageItem))
            {
                return false;
            }

            Operation.AdoptStorageItem(storageItem);
            return true;
        }

        internal bool TryObserve(IStorageItem storageItem)
        {
            _enumeratedItemCount++;
            if (_enumeratedItemCount <= MaxEnumeratedItems)
            {
                return true;
            }

            RejectedItems.Add(CreateRejection(
                storageItem,
                UploadRejectionReason.EnumerationLimitExceeded,
                $"Directory enumeration exceeds {MaxEnumeratedItems} items."));
            return false;
        }

        internal bool TryVisit(IStorageFolder folder)
        {
            string? identity;
            try
            {
                identity = folder.Path.IsAbsoluteUri
                    ? folder.Path.GetComponents(UriComponents.AbsoluteUri, UriFormat.SafeUnescaped)
                    : folder.Path.OriginalString;
            }
            catch
            {
                identity = null;
            }

            return string.IsNullOrWhiteSpace(identity) || _visitedDirectories.Add(identity);
        }
    }
}
