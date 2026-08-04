using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace AtomUI.Desktop.Controls;

public partial class Upload
{
    internal IUploadStorageProviderAdapter? StorageProviderAdapter { get; set; }

    public async Task SelectFilesAsync(CancellationToken cancellationToken = default)
    {
        var storageProvider = ResolveStorageProviderAdapter();
        if (storageProvider is null || !storageProvider.CanOpenFiles)
        {
            return;
        }

        var files = await storageProvider.OpenFilesAsync(new FilePickerOpenOptions
        {
            AllowMultiple = IsMultipleEnabled,
            FileTypeFilter = AllowedFileTypes
        }, cancellationToken).ConfigureAwait(false);

        await ProcessStorageItemsAsync(
            UploadInputSource.FilePicker,
            files.Cast<IStorageItem>().ToArray(),
            UploadDirectoryDropMode.Reject,
            0,
            10_000,
            cancellationToken).ConfigureAwait(false);
    }

    public async Task SelectDirectoriesAsync(CancellationToken cancellationToken = default)
    {
        var storageProvider = ResolveStorageProviderAdapter();
        if (storageProvider is null || !storageProvider.CanOpenFolders)
        {
            return;
        }

        var folders = await storageProvider.OpenFoldersAsync(new FolderPickerOpenOptions
        {
            AllowMultiple = IsMultipleEnabled
        }, cancellationToken).ConfigureAwait(false);

        await ProcessStorageItemsAsync(
            UploadInputSource.DirectoryPicker,
            folders.Cast<IStorageItem>().ToArray(),
            UploadDirectoryDropMode.TopLevelFiles,
            0,
            10_000,
            cancellationToken).ConfigureAwait(false);
    }

    private IUploadStorageProviderAdapter? ResolveStorageProviderAdapter()
    {
        if (StorageProviderAdapter is not null)
        {
            return StorageProviderAdapter;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        return topLevel is null
            ? null
            : new AvaloniaUploadStorageProviderAdapter(topLevel.StorageProvider);
    }
}
