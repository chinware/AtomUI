namespace AtomUI.Desktop.Controls;

internal sealed class DefaultImageSourceLoader : IImageSourceLoader
{
    private static readonly Lazy<HttpClient> SharedHttpClient = new(static () => new HttpClient());
    private readonly HttpClient _httpClient;

    public DefaultImageSourceLoader()
        : this(SharedHttpClient.Value)
    {
    }

    internal DefaultImageSourceLoader(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoadedImageSource> LoadAsync(IImagePreviewSource source, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var sourceStream = await OpenReadAsync(source, cancellationToken).ConfigureAwait(false);
        return await LoadFromStreamAsync(source, sourceStream, cancellationToken).ConfigureAwait(false);
    }

    private ValueTask<Stream> OpenReadAsync(IImagePreviewSource source, CancellationToken cancellationToken)
    {
        return source is UriImagePreviewSource uriSource
            ? uriSource.OpenReadAsync(_httpClient, cancellationToken)
            : source.OpenReadAsync(cancellationToken);
    }

    private static async Task<LoadedImageSource> LoadFromStreamAsync(IImagePreviewSource source,
                                                                     Stream sourceStream,
                                                                     CancellationToken cancellationToken)
    {
        await using var memoryStream = new MemoryStream();
        await sourceStream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            memoryStream.Position = 0;
            return CreateLoadedImageSource(source, memoryStream);
        }, cancellationToken).ConfigureAwait(false);
    }

    private static LoadedImageSource CreateLoadedImageSource(IImagePreviewSource source, Stream stream)
    {
        return IsSvgSource(source)
            ? LoadedImageSource.CreateSvg(stream)
            : LoadedImageSource.CreateBitmap(stream);
    }

    private static bool IsSvgSource(IImagePreviewSource source)
    {
        if (IsSvgContentType(source.ContentType))
        {
            return true;
        }

        var sourcePath = source is UriImagePreviewSource uriSource
            ? uriSource.SourceUri.LocalPath ?? uriSource.SourceUri.Uri?.AbsolutePath ?? uriSource.SourceUri.OriginalString
            : source.DisplayName;
        return string.Equals(Path.GetExtension(sourcePath), ".svg", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSvgContentType(string? contentType)
    {
        return string.Equals(contentType, "image/svg+xml", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(contentType, "application/svg+xml", StringComparison.OrdinalIgnoreCase);
    }
}
