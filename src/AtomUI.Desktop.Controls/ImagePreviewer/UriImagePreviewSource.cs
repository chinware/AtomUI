using AtomUI.Utils;

namespace AtomUI.Desktop.Controls;

public sealed class UriImagePreviewSource : IImagePreviewSource, IImagePreviewSourceIdentity
{
    private static readonly Lazy<HttpClient> SharedHttpClient = new(static () => new HttpClient());
    private string? _contentType;

    public UriImagePreviewSource(string source)
        : this(ImageSourceUri.Parse(source))
    {
    }

    public UriImagePreviewSource(Uri uri)
        : this(ImageSourceUri.Parse(uri.ToString()))
    {
    }

    public UriImagePreviewSource(ImageSourceUri sourceUri)
    {
        SourceUri = sourceUri;
    }

    public ImageSourceUri SourceUri { get; }

    public object Identity => SourceUri.CacheKey;

    public string? DisplayName => null;

    public string? ContentType => _contentType;

    public ValueTask<Stream> OpenReadAsync(CancellationToken cancellationToken)
    {
        return OpenReadAsync(SharedHttpClient.Value, cancellationToken);
    }

    internal ValueTask<Stream> OpenReadAsync(HttpClient httpClient, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return SourceUri.Kind switch
        {
            ImageSourceUriKind.LocalFile        => OpenLocalFileAsync(cancellationToken),
            ImageSourceUriKind.AvaloniaResource => OpenAvaloniaResourceAsync(cancellationToken),
            ImageSourceUriKind.Remote           => OpenRemoteAsync(httpClient, cancellationToken),
            _ => throw new NotSupportedException($"Unsupported image source uri: {SourceUri.OriginalString}")
        };
    }

    private ValueTask<Stream> OpenLocalFileAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (SourceUri.LocalPath is null)
        {
            throw new InvalidOperationException($"Local image source uri has no resolved path: {SourceUri.OriginalString}");
        }

        Stream stream = new FileStream(SourceUri.LocalPath,
                                       FileMode.Open,
                                       FileAccess.Read,
                                       FileShare.Read,
                                       bufferSize: 81920,
                                       FileOptions.Asynchronous | FileOptions.SequentialScan);
        return new ValueTask<Stream>(stream);
    }

    private ValueTask<Stream> OpenAvaloniaResourceAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return new ValueTask<Stream>(AssetsLoader.OpenStream(SourceUri.OriginalString));
    }

    private async ValueTask<Stream> OpenRemoteAsync(HttpClient httpClient, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(SourceUri.Uri,
                                                       HttpCompletionOption.ResponseHeadersRead,
                                                       cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        _contentType = response.Content.Headers.ContentType?.MediaType;

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var memoryStream = new MemoryStream();
        await responseStream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        memoryStream.Position = 0;
        return memoryStream;
    }
}
