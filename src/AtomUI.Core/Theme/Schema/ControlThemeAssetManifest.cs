namespace AtomUI.Theme.Schema;

internal sealed class ControlThemeAssetManifest
{
    internal ControlThemeAssetManifest(
        ThemeSchemaRegistry registry,
        IEnumerable<ControlThemeAssetDescriptor> descriptors)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(descriptors);

        var ordered = descriptors.OrderBy(
            static descriptor => descriptor.AssetUri.ToString(),
            StringComparer.Ordinal).ToArray();
        var uris = new HashSet<string>(StringComparer.Ordinal);
        foreach (var descriptor in ordered)
        {
            ArgumentNullException.ThrowIfNull(descriptor);
            if (!descriptor.AssetUri.IsAbsoluteUri)
            {
                throw new ThemeSchemaException(
                    $"Control theme asset URI '{descriptor.AssetUri}' must be absolute.");
            }
            if (!registry.TryGetControl(descriptor.Identity, out _))
            {
                throw new ThemeSchemaException(
                    $"Control theme asset '{descriptor.AssetUri}' uses unregistered identity '{descriptor.Identity}'.");
            }
            if (descriptor.ResourceKeySchemaFingerprint == 0)
            {
                throw new ThemeSchemaException(
                    $"Control theme asset '{descriptor.AssetUri}' has an empty resource-key schema fingerprint.");
            }
            if (!uris.Add(descriptor.AssetUri.ToString()))
            {
                throw new ThemeSchemaException(
                    $"Control theme asset URI '{descriptor.AssetUri}' is registered more than once.");
            }
        }

        Descriptors = Array.AsReadOnly(ordered);
    }

    internal IReadOnlyList<ControlThemeAssetDescriptor> Descriptors { get; }
}
