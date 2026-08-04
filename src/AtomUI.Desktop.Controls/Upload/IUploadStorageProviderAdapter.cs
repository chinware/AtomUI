using Avalonia.Platform.Storage;

namespace AtomUI.Desktop.Controls;

internal interface IUploadStorageProviderAdapter
{
    bool CanOpenFiles { get; }
    bool CanOpenFolders { get; }

    Task<IReadOnlyList<IStorageFile>> OpenFilesAsync(
        FilePickerOpenOptions options,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<IStorageFolder>> OpenFoldersAsync(
        FolderPickerOpenOptions options,
        CancellationToken cancellationToken);
}

internal sealed class AvaloniaUploadStorageProviderAdapter : IUploadStorageProviderAdapter
{
    private readonly IStorageProvider _storageProvider;

    internal AvaloniaUploadStorageProviderAdapter(IStorageProvider storageProvider)
    {
        ArgumentNullException.ThrowIfNull(storageProvider);
        _storageProvider = storageProvider;
    }

    public bool CanOpenFiles => _storageProvider.CanOpen;
    public bool CanOpenFolders => _storageProvider.CanOpen;

    public async Task<IReadOnlyList<IStorageFile>> OpenFilesAsync(
        FilePickerOpenOptions options,
        CancellationToken cancellationToken)
    {
        return await _storageProvider.OpenFilePickerAsync(options)
                                     .WaitAsync(cancellationToken)
                                     .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<IStorageFolder>> OpenFoldersAsync(
        FolderPickerOpenOptions options,
        CancellationToken cancellationToken)
    {
        return await _storageProvider.OpenFolderPickerAsync(options)
                                     .WaitAsync(cancellationToken)
                                     .ConfigureAwait(false);
    }
}
