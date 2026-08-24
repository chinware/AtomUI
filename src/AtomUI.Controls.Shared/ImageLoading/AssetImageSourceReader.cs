using Avalonia.Platform;

namespace AtomUI.Controls;

internal sealed class AssetImageSourceReader : ImageSourceReader
{
    private readonly ImageLoadingOptions _options;

    internal AssetImageSourceReader(ImageLoadingOptions options)
    {
        _options = options;
    }

    internal override ImageLoadSourceKind Kind => ImageLoadSourceKind.Asset;

    internal override async Task<ImageSourceReadResult> ReadAsync(
        NormalizedImageRequest request,
        ImageEncodedContent? staleContent,
        IProgress<ImageLoadProgress>? progress,
        CancellationToken cancellationToken)
    {
        if (request.CacheMode != ImageCacheMode.Reload && staleContent is not null)
        {
            return new ImageSourceReadResult(staleContent.WithCacheSource(ImageCacheSource.EncodedMemory));
        }
        try
        {
            await using var stream = AssetLoader.Open((Uri)request.Source.Value);
            progress?.Report(ImageLoadProgress.Create(ImageLoadStage.Reading));
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
                FreshUntil: DateTimeOffset.MaxValue,
                IsTrustedAsset: true,
                SourceVersion: "application-resource-v1"));
        }
        catch (FileNotFoundException exception)
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.NotFound,
                "Image asset was not found.",
                request.Source.DisplayName,
                exception);
        }
    }
}
