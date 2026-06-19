namespace AtomUI.Toolkits.GalleryBase.Configuration;

public static class GalleryBaseConfigurationProvider
{
    private static GalleryBaseConfiguration? s_current;

    public static GalleryBaseConfiguration? Current => s_current;

    public static GalleryBaseConfiguration GetRequired()
    {
        return s_current
               ?? throw new GalleryConfigurationException(
                   "GalleryBase configuration has not been registered. Call UseGalleryBase with product options first.");
    }

    internal static void SetCurrent(GalleryBaseConfiguration configuration)
    {
        s_current = configuration;
    }
}
