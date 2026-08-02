namespace AtomUI.Theme.Schema;

public sealed class ControlThemeAssetDescriptor
{
    public ControlThemeAssetDescriptor(
        Uri assetUri,
        ControlTokenIdentity ownerIdentity,
        IEnumerable<ControlTokenIdentity> referencedControlIdentities,
        ControlThemeSemanticPartDescriptor? semanticPart,
        ulong resourceKeySchemaFingerprint)
    {
        ArgumentNullException.ThrowIfNull(assetUri);
        ArgumentNullException.ThrowIfNull(referencedControlIdentities);

        AssetUri = assetUri;
        OwnerIdentity = ownerIdentity;
        ReferencedControlIdentities = Array.AsReadOnly(
            referencedControlIdentities.Distinct()
                                       .OrderBy(static identity => identity.Catalog, StringComparer.Ordinal)
                                       .ThenBy(static identity => identity.Id, StringComparer.Ordinal)
                                       .ToArray());
        SemanticPart = semanticPart;
        ResourceKeySchemaFingerprint = resourceKeySchemaFingerprint;
    }

    public Uri AssetUri { get; }
    public ControlTokenIdentity OwnerIdentity { get; }
    public IReadOnlyList<ControlTokenIdentity> ReferencedControlIdentities { get; }
    public ControlThemeSemanticPartDescriptor? SemanticPart { get; }
    public ulong ResourceKeySchemaFingerprint { get; }
}
