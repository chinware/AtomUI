using Avalonia.Platform.Storage;

namespace AtomUI.Controls;

internal sealed class StorageFileImageSourceReader : ImageSourceReader
{
    private readonly ImageLoadingOptions _options;

    internal StorageFileImageSourceReader(ImageLoadingOptions options)
    {
        _options = options;
    }

    internal override ImageLoadSourceKind Kind => ImageLoadSourceKind.StorageFile;

    internal override async Task<ImageSourceReadResult> ReadAsync(
        NormalizedImageRequest request,
        ImageEncodedContent? staleContent,
        IProgress<ImageLoadProgress>? progress,
        CancellationToken cancellationToken)
    {
        if (request.CacheMode != ImageCacheMode.Reload &&
            staleContent is not null && request.Source.Version is not null &&
            staleContent.SourceVersion == request.Source.Version)
        {
            return new ImageSourceReadResult(staleContent.WithCacheSource(ImageCacheSource.EncodedMemory));
        }
        try
        {
            await using var stream = await ((IStorageFile)request.Source.Value).OpenReadAsync().ConfigureAwait(false);
            ImageProgressDispatcher.Report(progress, ImageLoadProgress.Create(ImageLoadStage.Reading));
            var bytes = await ImageSourceReadHelpers.ReadAllBytesAsync(
                stream,
                _options.MaxResponseBytes,
                ImageLoadStage.Reading,
                progress,
                stream.CanSeek ? stream.Length : null,
                cancellationToken).ConfigureAwait(false);
            return new ImageSourceReadResult(new ImageEncodedContent(
                bytes,
                null,
                ImageCacheSource.Local,
                DateTimeOffset.UtcNow,
                FreshUntil: request.Source.Version is null ? null : DateTimeOffset.MaxValue,
                SourceVersion: request.Source.Version));
        }
        catch (UnauthorizedAccessException exception)
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.AccessDenied,
                "Access to the selected image was denied.",
                request.Source.DisplayName,
                exception);
        }
    }
}
