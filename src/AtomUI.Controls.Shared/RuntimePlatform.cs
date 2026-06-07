namespace AtomUI.Controls;

internal interface IPlatformFeatureProvider
{
    bool SupportsNativeWindow { get; }
    bool SupportsWindowModalDialog { get; }
    bool SupportsWindowChrome { get; }
    bool SupportsWindowDeactivation { get; }
    bool SupportsLocalFileSystemEnumeration { get; }
}

internal static class RuntimePlatform
{
    internal static IPlatformFeatureProvider Features { get; } = CreateFeatureProvider();

    private static IPlatformFeatureProvider CreateFeatureProvider()
    {
        return OperatingSystem.IsBrowser()
            ? BrowserPlatformFeatureProvider.Instance
            : DesktopPlatformFeatureProvider.Instance;
    }
}

internal sealed class DesktopPlatformFeatureProvider : IPlatformFeatureProvider
{
    internal static readonly DesktopPlatformFeatureProvider Instance = new();

    private DesktopPlatformFeatureProvider()
    {
    }

    bool IPlatformFeatureProvider.SupportsNativeWindow => true;
    bool IPlatformFeatureProvider.SupportsWindowModalDialog => true;
    bool IPlatformFeatureProvider.SupportsWindowChrome => true;
    bool IPlatformFeatureProvider.SupportsWindowDeactivation => true;
    bool IPlatformFeatureProvider.SupportsLocalFileSystemEnumeration => true;
}

internal sealed class BrowserPlatformFeatureProvider : IPlatformFeatureProvider
{
    internal static readonly BrowserPlatformFeatureProvider Instance = new();

    private BrowserPlatformFeatureProvider()
    {
    }

    bool IPlatformFeatureProvider.SupportsNativeWindow => false;
    bool IPlatformFeatureProvider.SupportsWindowModalDialog => false;
    bool IPlatformFeatureProvider.SupportsWindowChrome => false;
    bool IPlatformFeatureProvider.SupportsWindowDeactivation => false;
    bool IPlatformFeatureProvider.SupportsLocalFileSystemEnumeration => false;
}
