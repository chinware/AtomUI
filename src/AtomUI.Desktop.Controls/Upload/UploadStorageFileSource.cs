using AtomUI.Controls;
using Avalonia.Platform.Storage;

namespace AtomUI.Desktop.Controls;

internal sealed class UploadStorageFileSource : IUploadFileSource, IUploadFileSourceLease
{
    private IStorageFile? _storageFile;

    internal UploadStorageFileSource(IStorageFile storageFile)
    {
        ArgumentNullException.ThrowIfNull(storageFile);
        _storageFile = storageFile;
    }

    public async ValueTask<Stream> OpenReadAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var storageFile = Volatile.Read(ref _storageFile) ??
                          throw new ObjectDisposedException(nameof(UploadStorageFileSource));
        return await storageFile.OpenReadAsync().WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    public void Dispose()
    {
        Interlocked.Exchange(ref _storageFile, null)?.Dispose();
    }
}
