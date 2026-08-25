namespace AtomUI.Controls;

internal sealed class StreamImageSourceReader : ImageSourceReader
{
    private readonly ImageLoadingOptions _options;

    internal StreamImageSourceReader(ImageLoadingOptions options)
    {
        _options = options;
    }

    internal override ImageLoadSourceKind Kind => ImageLoadSourceKind.Stream;

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
        var factory = (Func<CancellationToken, ValueTask<Stream>>)request.Source.Value;
        await using var stream = await factory(cancellationToken).ConfigureAwait(false);
        if (stream is null)
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.InvalidSource,
                "Image stream factory returned null.",
                request.Source.DisplayName);
        }
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
}
