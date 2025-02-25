using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace AtomUI.Controls.Loader; 

internal class RamCachedWebImageLoader : BaseWebImageLoader {
    private readonly ConcurrentDictionary<string, Task<Bitmap?>> _memoryCache = new();
    
    public RamCachedWebImageLoader() { }
    
    public RamCachedWebImageLoader(HttpClient httpClient, bool disposeHttpClient) : base(httpClient,
        disposeHttpClient) { }
    
    public override async Task<Bitmap?> ProvideImageAsync(string url) {
        var bitmap = await _memoryCache.GetOrAdd(url, LoadAsync).ConfigureAwait(false);
        if (bitmap == null) _memoryCache.TryRemove(url, out _);
        return bitmap;
    }
}