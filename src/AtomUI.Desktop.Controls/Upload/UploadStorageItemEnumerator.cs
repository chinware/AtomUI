using System.Diagnostics;
using Avalonia.Platform.Storage;

namespace AtomUI.Desktop.Controls;

internal static class UploadStorageItemEnumerator
{
    internal static async Task<UploadStorageEnumerationResult> EnumerateAsync(
        IReadOnlyList<IStorageItem> storageItems,
        UploadDirectoryDropMode directoryMode,
        int maxDirectoryDepth,
        int maxEnumeratedItems,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storageItems);
        ArgumentOutOfRangeException.ThrowIfNegative(maxDirectoryDepth);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxEnumeratedItems, 1);

        var candidates = new List<UploadInputCandidate>();
        var rejectedItems = new List<UploadRejectedItem>();
        var currentIndex = 0;
        var firstUnownedIndex = 0;

        try
        {
            for (; currentIndex < storageItems.Count; currentIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var storageItem = storageItems[currentIndex];
                switch (storageItem)
                {
                    case IStorageFile storageFile:
                        TryAddCandidate(storageFile, candidates, rejectedItems);
                        firstUnownedIndex = currentIndex + 1;
                        break;

                    case IStorageFolder storageFolder:
                        if (directoryMode == UploadDirectoryDropMode.Reject)
                        {
                            rejectedItems.Add(CreateRejection(
                                storageFolder,
                                UploadRejectionReason.DirectoryNotAllowed,
                                "Directory input is disabled."));
                            storageFolder.Dispose();
                            firstUnownedIndex = currentIndex + 1;
                            break;
                        }

                        var context = new DirectoryEnumerationContext(
                            directoryMode,
                            maxDirectoryDepth,
                            maxEnumeratedItems,
                            candidates,
                            rejectedItems);
                        context.TryVisit(storageFolder);
                        firstUnownedIndex = currentIndex + 1;
                        await EnumerateFolderAsync(storageFolder, 0, context, cancellationToken).ConfigureAwait(false);
                        break;

                    default:
                        rejectedItems.Add(CreateRejection(
                            storageItem,
                            UploadRejectionReason.UnsupportedStorageItem,
                            "The storage item is neither a file nor a folder."));
                        storageItem.Dispose();
                        firstUnownedIndex = currentIndex + 1;
                        break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            foreach (var candidate in candidates)
            {
                candidate.Dispose();
            }

            for (var index = firstUnownedIndex; index < storageItems.Count; index++)
            {
                storageItems[index].Dispose();
            }

            throw;
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
                if (!context.TryObserve(child))
                {
                    child.Dispose();
                    break;
                }

                switch (child)
                {
                    case IStorageFile storageFile:
                        TryAddCandidate(storageFile, context.Candidates, context.RejectedItems);
                        break;

                    case IStorageFolder childFolder:
                        if (context.DirectoryMode == UploadDirectoryDropMode.TopLevelFiles)
                        {
                            context.RejectedItems.Add(CreateRejection(
                                childFolder,
                                UploadRejectionReason.DirectoryNotAllowed,
                                "Nested directories are not expanded in TopLevelFiles mode."));
                            childFolder.Dispose();
                        }
                        else if (depth + 1 > context.MaxDirectoryDepth)
                        {
                            context.RejectedItems.Add(CreateRejection(
                                childFolder,
                                UploadRejectionReason.DirectoryDepthExceeded,
                                $"Directory depth exceeds {context.MaxDirectoryDepth}."));
                            childFolder.Dispose();
                        }
                        else if (!context.TryVisit(childFolder))
                        {
                            context.RejectedItems.Add(CreateRejection(
                                childFolder,
                                UploadRejectionReason.DirectoryCycleDetected,
                                "The directory was already visited."));
                            childFolder.Dispose();
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
                        child.Dispose();
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
            folder.Dispose();
        }
    }

    private static void TryAddCandidate(
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
            storageFile.Dispose();
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
        private readonly HashSet<string> _visitedDirectories = new(StringComparer.Ordinal);
        private int _enumeratedItemCount;

        internal UploadDirectoryDropMode DirectoryMode { get; }
        internal int MaxDirectoryDepth { get; }
        internal int MaxEnumeratedItems { get; }
        internal List<UploadInputCandidate> Candidates { get; }
        internal List<UploadRejectedItem> RejectedItems { get; }

        internal DirectoryEnumerationContext(
            UploadDirectoryDropMode directoryMode,
            int maxDirectoryDepth,
            int maxEnumeratedItems,
            List<UploadInputCandidate> candidates,
            List<UploadRejectedItem> rejectedItems)
        {
            DirectoryMode       = directoryMode;
            MaxDirectoryDepth   = maxDirectoryDepth;
            MaxEnumeratedItems  = maxEnumeratedItems;
            Candidates          = candidates;
            RejectedItems       = rejectedItems;
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
