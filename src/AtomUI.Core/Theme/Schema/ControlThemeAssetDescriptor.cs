namespace AtomUI.Theme.Schema;

public sealed record ControlThemeAssetDescriptor(
    Uri AssetUri,
    ControlTokenIdentity Identity,
    ulong ResourceKeySchemaFingerprint);
