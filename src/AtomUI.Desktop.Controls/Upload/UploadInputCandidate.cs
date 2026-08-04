using AtomUI.Controls;
using Avalonia.Platform.Storage;

namespace AtomUI.Desktop.Controls;

internal sealed class UploadInputCandidate : IDisposable
{
    private IStorageFile? _storageFile;

    internal string Name { get; }
    internal Uri? Path { get; }

    internal UploadInputCandidate(IStorageFile storageFile)
    {
        ArgumentNullException.ThrowIfNull(storageFile);
        _storageFile = storageFile;
        Name         = storageFile.Name;
        Path         = storageFile.Path;
    }

    internal async ValueTask<UploadFileInfo> CreateFileInfoAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var storageFile = Volatile.Read(ref _storageFile) ??
                          throw new ObjectDisposedException(nameof(UploadInputCandidate));
        var properties = await storageFile.GetBasicPropertiesAsync()
                                          .WaitAsync(cancellationToken)
                                          .ConfigureAwait(false);

        long? size = properties.Size switch
        {
            null => null,
            <= long.MaxValue => (long)properties.Size.Value,
            _ => throw new OverflowException("The storage item size exceeds Int64.MaxValue.")
        };

        var source = new UploadStorageFileSource(
            Interlocked.Exchange(ref _storageFile, null) ??
            throw new ObjectDisposedException(nameof(UploadInputCandidate)));
        try
        {
            return new UploadFileInfo(
                Name,
                source,
                Path,
                size,
                dateCreated: properties.DateCreated,
                dateModified: properties.DateModified);
        }
        catch
        {
            source.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        Interlocked.Exchange(ref _storageFile, null)?.Dispose();
    }
}

internal sealed record UploadStorageEnumerationResult(
    IReadOnlyList<UploadInputCandidate> Candidates,
    IReadOnlyList<UploadRejectedItem> RejectedItems);
