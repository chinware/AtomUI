using Avalonia.Media;

namespace AtomUI.Controls;

internal sealed class BorrowedImageSourceReader : ImageSourceReader
{
    internal override ImageLoadSourceKind Kind => ImageLoadSourceKind.Image;

    internal override Task<ImageSourceReadResult> ReadAsync(
        NormalizedImageRequest request,
        ImageEncodedContent? staleContent,
        IProgress<ImageLoadProgress>? progress,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ImageProgressDispatcher.Report(progress, ImageLoadProgress.Create(ImageLoadStage.Reading));
        return Task.FromResult(new ImageSourceReadResult(BorrowedImage: (IImage)request.Source.Value));
    }
}
