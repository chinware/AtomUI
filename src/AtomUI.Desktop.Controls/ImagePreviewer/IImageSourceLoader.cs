namespace AtomUI.Desktop.Controls;

internal interface IImageSourceLoader
{
    Task<LoadedImageSource> LoadAsync(IImagePreviewSource source, CancellationToken cancellationToken);
}
