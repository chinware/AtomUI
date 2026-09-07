namespace AtomUI.Controls;

internal sealed class BytesImageSourceReader : ImageSourceReader
{
    private readonly ImageLoadingOptions _options;

    internal BytesImageSourceReader(ImageLoadingOptions options)
    {
        _options = options;
    }

    internal override ImageSourceKind Kind => ImageSourceKind.Bytes;

    internal override Task<ImageSourceReadResult> ReadAsync(
        NormalizedImageRequest request,
        ImageEncodedContent? staleContent,
        IProgress<ImageLoadProgress>? progress,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var bytes = ((BytesImageSource)request.Source).Content.ToArray();
        if (bytes.LongLength > _options.MaxResponseBytes)
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.ResponseTooLarge,
                "Image content exceeds the configured size limit.",
                request.Source.DisplayName);
        }
        ImageProgressDispatcher.Report(progress, ImageLoadProgress.Create(ImageLoadStage.Reading, bytes.LongLength, bytes.LongLength));
        return Task.FromResult(new ImageSourceReadResult(new ImageEncodedContent(
            bytes,
            null,
            ImageLoadOrigin.Local,
            DateTimeOffset.UtcNow,
            FreshUntil: DateTimeOffset.MaxValue,
            SourceVersion: request.Source.SourceRevision),
            SourceValidation: ImageSourceValidation.NotRequired));
    }
}
