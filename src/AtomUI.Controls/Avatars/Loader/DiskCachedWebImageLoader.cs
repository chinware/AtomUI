
using System.Security.Cryptography;
using System.Text;
using Avalonia.Media.Imaging;

namespace AtomUI.Controls.Loader; 

internal class DiskCachedWebImageLoader : RamCachedWebImageLoader {
    private readonly string _cacheFolder;

    public DiskCachedWebImageLoader(string cacheFolder = "Cache/Images/") {
        _cacheFolder = cacheFolder;
    }

    public DiskCachedWebImageLoader(HttpClient httpClient, bool disposeHttpClient,
                                    string cacheFolder = "Cache/Images/")
        : base(httpClient, disposeHttpClient) {
        _cacheFolder = cacheFolder;
    }
    
    protected override Task<Bitmap?> LoadFromGlobalCache(string url) {
        var path = Path.Combine(_cacheFolder, CreateMD5(url));

        return File.Exists(path) ? Task.FromResult<Bitmap?>(new Bitmap(path)) : Task.FromResult<Bitmap?>(null);
    }

#if NETSTANDARD2_1
        protected override async Task SaveToGlobalCache(string url, byte[] imageBytes) {
            var path = Path.Combine(_cacheFolder, CreateMD5(url));

            Directory.CreateDirectory(_cacheFolder);
            await File.WriteAllBytesAsync(path, imageBytes).ConfigureAwait(false);
        }
#else
    protected override Task SaveToGlobalCache(string url, byte[] imageBytes)
    {
        var path = Path.Combine(_cacheFolder, CreateMD5(url));
        Directory.CreateDirectory(_cacheFolder);
        File.WriteAllBytes(path, imageBytes);
        return Task.CompletedTask;
    }
#endif

    protected static string CreateMD5(string input) {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "");
    }
}