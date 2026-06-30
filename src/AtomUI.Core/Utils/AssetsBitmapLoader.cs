using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace AtomUI.Utils;

public static class AssetsLoader
{
    public static Bitmap LoadBitmap(string path)
    {
        return new Bitmap(OpenStream(path));
    }

    public static Stream OpenStream(string path)
    {
        if (TryResolveLocalFilePath(path, out var localFilePath))
        {
            return new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        var assets = AvaloniaLocator.Current.GetRequiredService<IAssetLoader>();
        return assets.Open(new Uri(path, UriKind.RelativeOrAbsolute));
    }

    internal static bool TryResolveLocalFilePath(string path, out string localFilePath)
    {
        if (Uri.TryCreate(path, UriKind.Absolute, out var uri))
        {
            if (uri.IsFile)
            {
                localFilePath = uri.LocalPath;
                return true;
            }

            localFilePath = string.Empty;
            return false;
        }

        if (Path.IsPathFullyQualified(path) || Path.IsPathRooted(path))
        {
            localFilePath = Path.GetFullPath(path);
            return true;
        }

        if (!path.StartsWith("avares:", StringComparison.OrdinalIgnoreCase))
        {
            localFilePath = Path.GetFullPath(path);
            return true;
        }

        localFilePath = string.Empty;
        return false;
    }
}
