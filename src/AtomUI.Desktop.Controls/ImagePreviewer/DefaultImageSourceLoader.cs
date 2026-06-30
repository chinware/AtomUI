using AtomUI.Utils;

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

    public Task<LoadedImageSource> LoadAsync(ImageSourceUri sourceUri, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return sourceUri.Kind switch
        {
            ImageSourceUriKind.LocalFile => LoadLocalFileAsync(sourceUri, cancellationToken),
            ImageSourceUriKind.AvaloniaResource => LoadAvaloniaResourceAsync(sourceUri, cancellationToken),
            ImageSourceUriKind.Remote => LoadRemoteAsync(sourceUri, cancellationToken),
            _ => throw new NotSupportedException($"Unsupported image source uri: {sourceUri.OriginalString}")
        };
    }

    private static async Task<LoadedImageSource> LoadLocalFileAsync(ImageSourceUri sourceUri, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (sourceUri.LocalPath is null)
        {
            throw new InvalidOperationException($"Local image source uri has no resolved path: {sourceUri.OriginalString}");
        }

        await using var sourceStream = new FileStream(sourceUri.LocalPath,
                                                      FileMode.Open,
                                                      FileAccess.Read,
                                                      FileShare.Read,
                                                      bufferSize: 81920,
                                                      FileOptions.Asynchronous | FileOptions.SequentialScan);
        return await LoadFromStreamAsync(sourceUri, sourceStream, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<LoadedImageSource> LoadAvaloniaResourceAsync(ImageSourceUri sourceUri, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var sourceStream = AssetsLoader.OpenStream(sourceUri.OriginalString);
        return await LoadFromStreamAsync(sourceUri, sourceStream, cancellationToken).ConfigureAwait(false);
    }

    private async Task<LoadedImageSource> LoadRemoteAsync(ImageSourceUri sourceUri, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(sourceUri.Uri,
                                                        HttpCompletionOption.ResponseHeadersRead,
                                                        cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var contentType = response.Content.Headers.ContentType?.MediaType;
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        return await LoadFromStreamAsync(sourceUri, stream, cancellationToken, contentType).ConfigureAwait(false);
    }

    private static async Task<LoadedImageSource> LoadFromStreamAsync(ImageSourceUri sourceUri,
                                                                     Stream sourceStream,
                                                                     CancellationToken cancellationToken,
                                                                     string? contentType = null)
    {
        await using var memoryStream = new MemoryStream();
        await sourceStream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            memoryStream.Position = 0;
            return CreateLoadedImageSource(sourceUri, memoryStream, contentType);
        }, cancellationToken).ConfigureAwait(false);
    }

    private static LoadedImageSource CreateLoadedImageSource(ImageSourceUri sourceUri, Stream stream, string? contentType = null)
    {
        return IsSvgSource(sourceUri, contentType)
            ? LoadedImageSource.CreateSvg(stream)
            : LoadedImageSource.CreateBitmap(stream);
    }

    private static bool IsSvgSource(ImageSourceUri sourceUri, string? contentType = null)
    {
        if (IsSvgContentType(contentType))
        {
            return true;
        }

        var sourcePath = sourceUri.LocalPath ?? sourceUri.Uri?.AbsolutePath ?? sourceUri.OriginalString;
        return string.Equals(Path.GetExtension(sourcePath), ".svg", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSvgContentType(string? contentType)
    {
        return string.Equals(contentType, "image/svg+xml", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(contentType, "application/svg+xml", StringComparison.OrdinalIgnoreCase);
    }
}
