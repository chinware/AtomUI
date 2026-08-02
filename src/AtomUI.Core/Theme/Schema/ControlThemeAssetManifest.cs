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
            if (!registry.TryGetControl(descriptor.OwnerIdentity, out _))
            {
                throw new ThemeSchemaException(
                    $"Control theme asset '{descriptor.AssetUri}' uses unregistered owner identity " +
                    $"'{descriptor.OwnerIdentity}'.");
            }
            if (descriptor.ResourceKeySchemaFingerprint == 0)
            {
                throw new ThemeSchemaException(
                    $"Control theme asset '{descriptor.AssetUri}' has an empty resource-key schema fingerprint.");
            }
            var expectedFingerprint = ThemeSchemaRegistry.ComputeResourceKeySchemaFingerprint(
                descriptor,
                registry.GlobalTokens);
            if (descriptor.ResourceKeySchemaFingerprint != expectedFingerprint)
            {
                throw new ThemeSchemaException(
                    $"Control theme asset '{descriptor.AssetUri}' was compiled against a different resource-key schema.");
            }
            if (!uris.Add(descriptor.AssetUri.ToString()))
            {
                throw new ThemeSchemaException(
                    $"Control theme asset URI '{descriptor.AssetUri}' is registered more than once.");
            }

            var identities = new HashSet<ControlTokenIdentity>();
            foreach (var identity in descriptor.ReferencedControlIdentities)
            {
                if (!identities.Add(identity))
                {
                    throw new ThemeSchemaException(
                        $"Control theme asset '{descriptor.AssetUri}' declares duplicate Control identity " +
                        $"'{identity}'.");
                }
                if (!registry.TryGetControl(identity, out _))
                {
                    throw new ThemeSchemaException(
                        $"Control theme asset '{descriptor.AssetUri}' references unregistered identity " +
                        $"'{identity}'.");
                }
            }
        }

        Descriptors = Array.AsReadOnly(ordered);
    }

    internal IReadOnlyList<ControlThemeAssetDescriptor> Descriptors { get; }
}
