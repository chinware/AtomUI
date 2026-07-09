using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace AtomUI.Desktop.Controls;

public partial class Upload
{
    public async Task SelectFilesAsync(CancellationToken cancellationToken = default)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return;
        }

        var storageProvider = topLevel.StorageProvider;
        if (!storageProvider.CanOpen)
        {
            if (RuntimePlatform.Features.SupportsNativeWindow)
            {
                throw new InvalidOperationException("Can't open storage provider");
            }

            return;
        }

        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            AllowMultiple     = IsMultipleEnabled,
            SuggestedFileType = new FilePickerFileType("filter")
            {
                MimeTypes                   = Accepts,
                Patterns                    = Accepts,
                AppleUniformTypeIdentifiers = Accepts
            }
        });

        await EnqueueStorageFilesAsync(files, cancellationToken);
    }

    public async Task SelectDirectoriesAsync(CancellationToken cancellationToken = default)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return;
        }

        var storageProvider = topLevel.StorageProvider;
        if (!storageProvider.CanOpen || !RuntimePlatform.Features.SupportsLocalFileSystemEnumeration)
        {
            return;
        }

        var directories = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            AllowMultiple = IsMultipleEnabled
        });

        var files = new List<UploadFileInfo>();
        foreach (var directory in directories)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var filePath in Directory.EnumerateFiles(directory.Path.LocalPath, "*", SearchOption.TopDirectoryOnly))
            {
                var fileInfo = new FileInfo(filePath);
                files.Add(new UploadFileInfo(
                    fileInfo.Name,
                    new Uri(fileInfo.FullName),
                    fileInfo.Length,
                    fileInfo.CreationTime,
                    fileInfo.LastWriteTime));
            }
        }

        await EnqueueFilesAsync(files, cancellationToken);
    }

    internal async Task EnqueueStorageFilesAsync(IReadOnlyList<IStorageFile> files, CancellationToken cancellationToken = default)
    {
        var uploadFiles = new List<UploadFileInfo>(files.Count);
        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            uploadFiles.Add(await CreateUploadFileInfoAsync(file));
        }

        await EnqueueFilesAsync(uploadFiles, cancellationToken);
    }

    internal static async Task<UploadFileInfo> CreateUploadFileInfoAsync(IStorageFile file)
    {
        var properties = await file.GetBasicPropertiesAsync();
        return new UploadFileInfo(
            file.Name,
            file.Path,
            (long)(properties.Size ?? 0),
            properties.DateCreated,
            properties.DateModified);
    }
}
