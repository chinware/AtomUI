using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Logging;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace AtomUI.Controls.Loader;

internal class BaseWebImageLoader : IAsyncImageLoader {
    private readonly ParametrizedLogger? _logger;
    private readonly bool _shouldDisposeHttpClient;

    
    public BaseWebImageLoader() : this(new HttpClient(), true) { }
    
    public BaseWebImageLoader(HttpClient httpClient, bool disposeHttpClient) {
        HttpClient = httpClient;
        _shouldDisposeHttpClient = disposeHttpClient;
        _logger = Logger.TryGet(LogEventLevel.Error, "AsyncImageLoader");
    }

    protected HttpClient HttpClient { get; }
    
    public virtual async Task<Bitmap?> ProvideImageAsync(string url) {
        return await LoadAsync(url).ConfigureAwait(false);
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual async Task<Bitmap?> LoadAsync(string url) {
        var internalOrCachedBitmap =
            await LoadFromLocalAsync(url).ConfigureAwait(false)
         ?? await LoadFromInternalAsync(url).ConfigureAwait(false)
         ?? await LoadFromGlobalCache(url).ConfigureAwait(false);
        if (internalOrCachedBitmap != null) return internalOrCachedBitmap;

        try {
            var externalBytes = await LoadDataFromExternalAsync(url).ConfigureAwait(false);
            if (externalBytes == null) return null;

            using var memoryStream = new MemoryStream(externalBytes);
            var bitmap = new Bitmap(memoryStream);
            await SaveToGlobalCache(url, externalBytes).ConfigureAwait(false);
            return bitmap;
        }
        catch (Exception e)
        {
            _logger?.Log(this, "Failed to resolve image: {RequestUri}\nException: {Exception}", url, e);

            return null;
        }
    }
    private Task<Bitmap?> LoadFromLocalAsync(string url) {
        return Task.FromResult(File.Exists(url) ? new Bitmap(url) : null);
    }
    
    protected virtual Task<Bitmap?> LoadFromInternalAsync(string url) {
        try {
            var uri = url.StartsWith("/")
                ? new Uri(url, UriKind.Relative)
                : new Uri(url, UriKind.RelativeOrAbsolute);

            if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                return Task.FromResult<Bitmap?>(null);

            if (uri is { IsAbsoluteUri: true, IsFile: true })
                return Task.FromResult(new Bitmap(uri.LocalPath))!;

            return Task.FromResult(new Bitmap(AssetLoader.Open(uri)))!;
        }
        catch (Exception e) {
            _logger?.Log(this,
                "Failed to resolve image from request with uri: {RequestUri}\nException: {Exception}", url, e);
            return Task.FromResult<Bitmap?>(null);
        }
    }
    
    protected virtual async Task<byte[]?> LoadDataFromExternalAsync(string url) {
        try {
            return await HttpClient.GetByteArrayAsync(url).ConfigureAwait(false);
        }
        catch (Exception e) {
            _logger?.Log(this,
                "Failed to resolve image from request with uri: {RequestUri}\nException: {Exception}", url, e);
            return null;
        }
    }
    
    protected virtual Task<Bitmap?> LoadFromGlobalCache(string url) {
        return Task.FromResult<Bitmap?>(null);
    }
    
    protected virtual Task SaveToGlobalCache(string url, byte[] imageBytes) {
        return Task.CompletedTask;
    }

    ~BaseWebImageLoader() {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing) {
        if (disposing && _shouldDisposeHttpClient) HttpClient.Dispose();
    }
}