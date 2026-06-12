using System.Reflection;

namespace AtomUIGallery;

public static class GalleryVersionInfo
{
    private const string AtomUIVersionMetadataKey = "AtomUIVersion";

    public static string DisplayVersion => $"v{Version}";

    public static string Version
    {
        get
        {
            var metadataVersion = typeof(GalleryVersionInfo).Assembly
                                                            .GetCustomAttributes<AssemblyMetadataAttribute>()
                                                            .FirstOrDefault(attribute =>
                                                                attribute.Key == AtomUIVersionMetadataKey)
                                                            ?.Value;
            if (!string.IsNullOrWhiteSpace(metadataVersion))
            {
                return metadataVersion;
            }

            return typeof(GalleryVersionInfo).Assembly.GetName().Version?.ToString(3) ?? string.Empty;
        }
    }
}
