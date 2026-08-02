using AtomUI.Theme.Schema;

namespace AtomUI.Desktop.Controls;

internal static class DesktopControlThemeAssetSelector
{
    private const string BrowserPathSegment = "/Themes/Browser/";
    private const string CommonPathSegment = "/Themes/";
    private static readonly HashSet<string> s_browserExcludedControlIds = new(StringComparer.Ordinal)
    {
        "AdornerLayer",
        "OtpLineEdit",
        "OtpLineEditCell",
        "SplitView",
        "TreeViewFlyoutPresenter",
        "Window",
        "WindowTitleBar"
    };

    internal static bool IsBrowserControlSupported(ControlTokenIdentity identity)
    {
        return !s_browserExcludedControlIds.Contains(identity.Id);
    }

    internal static IReadOnlyList<ControlThemeAssetDescriptor> SelectNative(
        IReadOnlyList<ControlThemeAssetDescriptor> assets)
    {
        ArgumentNullException.ThrowIfNull(assets);

        return assets.Where(static asset => !IsBrowserAsset(asset)).ToArray();
    }

    internal static IReadOnlyList<ControlThemeAssetDescriptor> SelectBrowser(
        IReadOnlyList<ControlThemeAssetDescriptor> assets)
    {
        ArgumentNullException.ThrowIfNull(assets);

        var browserOverrides = assets.Where(static asset => IsBrowserAsset(asset))
                                     .Select(static asset => GetCommonAssetUri(asset))
                                     .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var selected = new List<ControlThemeAssetDescriptor>(assets.Count);
        foreach (var asset in assets)
        {
            if (IsBrowserAsset(asset) || !browserOverrides.Contains(asset.AssetUri.AbsoluteUri))
            {
                selected.Add(asset);
            }
        }
        return selected;
    }

    private static bool IsBrowserAsset(ControlThemeAssetDescriptor asset)
    {
        return asset.AssetUri.AbsoluteUri.IndexOf(
            BrowserPathSegment,
            StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static string GetCommonAssetUri(ControlThemeAssetDescriptor asset)
    {
        var uri = asset.AssetUri.AbsoluteUri;
        var index = uri.IndexOf(BrowserPathSegment, StringComparison.OrdinalIgnoreCase);
        return uri.Substring(0, index) +
               CommonPathSegment +
               uri.Substring(index + BrowserPathSegment.Length);
    }
}
