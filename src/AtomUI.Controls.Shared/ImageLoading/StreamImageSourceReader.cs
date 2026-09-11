namespace AtomUI.Controls;

internal sealed class StreamImageSourceReader : ImageSourceReader
{
    private readonly ImageLoadingOptions _options;

    internal StreamImageSourceReader(ImageLoadingOptions options)
    {
        _options = options;
    }

    internal override ImageSourceKind Kind => ImageSourceKind.Stream;

    internal override async Task<ImageSourceReadResult> ReadAsync(
        NormalizedImageRequest request,
        ImageEncodedContent? staleContent,
        IProgress<ImageLoadProgress>? progress,
        CancellationToken cancellationToken)
    {
        if (request.CacheRead != ImageCacheReadPolicy.RefreshSource &&
            staleContent is not null && request.Source.SourceRevision is not null &&
            staleContent.SourceVersion == request.Source.SourceRevision)
        {
            return new ImageSourceReadResult(staleContent.WithOrigin(ImageLoadOrigin.EncodedMemory));
        }
        var factory = ((StreamImageSource)request.Source).OpenStream;
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
            ImageLoadOrigin.Local,
            DateTimeOffset.UtcNow,
            FreshUntil: request.Source.SourceRevision is null ? null : DateTimeOffset.MaxValue,
            SourceVersion: request.Source.SourceRevision));
    }
}
