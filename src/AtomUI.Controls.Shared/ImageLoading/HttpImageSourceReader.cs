namespace AtomUI.Controls;

internal sealed class HttpImageSourceReader : ImageSourceReader
{
    private readonly HttpImageTransport _transport;

    internal HttpImageSourceReader(HttpImageTransport transport)
    {
        _transport = transport;
    }

    internal override ImageLoadSourceKind Kind => ImageLoadSourceKind.Http;

    internal override async Task<ImageSourceReadResult> ReadAsync(
        NormalizedImageRequest request,
        ImageEncodedContent? staleContent,
        IProgress<ImageLoadProgress>? progress,
        CancellationToken cancellationToken)
    {
        var content = await _transport.FetchAsync(
            request,
            staleContent,
            progress,
            cancellationToken).ConfigureAwait(false);
        return new ImageSourceReadResult(content);
    }
}
