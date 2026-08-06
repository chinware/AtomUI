using AtomUI.Theme.Resources;
using AtomUI.Theme.Schema;

namespace AtomUI.Theme;

public sealed class ControlPackageRegistration
{
    public ControlPackageRegistration(
        string id,
        IEnumerable<ControlTokenDescriptor> controls,
        IEnumerable<ControlThemeAssetDescriptor> themeAssets,
        IControlThemesProvider controlThemesProvider)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(controls);
        ArgumentNullException.ThrowIfNull(themeAssets);
        ArgumentNullException.ThrowIfNull(controlThemesProvider);
        if (!string.Equals(id, controlThemesProvider.Id, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"Control package id '{id}' must match theme provider id '{controlThemesProvider.Id}'.",
                nameof(controlThemesProvider));
        }

        var controlArray = controls.OrderBy(static control => control.Identity.Catalog, StringComparer.Ordinal)
                                   .ThenBy(static control => control.Identity.Id, StringComparer.Ordinal)
                                   .ToArray();
        EnsureUnique(
            controlArray,
            static control => control.Identity,
            EqualityComparer<ControlTokenIdentity>.Default,
            "Control identity");
        var assetArray = themeAssets.OrderBy(
            static asset => asset.AssetUri.ToString(),
            StringComparer.Ordinal).ToArray();
        EnsureUnique(
            assetArray,
            static asset => asset.AssetUri.ToString(),
            StringComparer.Ordinal,
            "theme asset URI");
        Id = id;
        Controls = Array.AsReadOnly(controlArray);
        ThemeAssets = Array.AsReadOnly(assetArray);
        ControlThemesProvider = controlThemesProvider;
    }

    public string Id { get; }
    public IReadOnlyList<ControlTokenDescriptor> Controls { get; }
    public IReadOnlyList<ControlThemeAssetDescriptor> ThemeAssets { get; }
    public IControlThemesProvider ControlThemesProvider { get; }

    private static void EnsureUnique<TItem, TKey>(
        IEnumerable<TItem> items,
        Func<TItem, TKey> keySelector,
        IEqualityComparer<TKey> comparer,
        string kind)
        where TKey : notnull
    {
        var keys = new HashSet<TKey>(comparer);
        foreach (var item in items)
        {
            var key = keySelector(item);
            if (!keys.Add(key))
            {
                throw new ArgumentException($"Control package contains duplicate {kind} '{key}'.");
            }
        }
    }
}
