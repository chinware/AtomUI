namespace AtomUI.Desktop.Controls;

internal interface IImageSourceLoader
{
    Task<LoadedImageSource> LoadAsync(ImageSourceUri sourceUri, CancellationToken cancellationToken);
}
