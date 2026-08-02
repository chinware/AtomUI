using AtomUI.Theme.Schema;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class ControlThemeAssetManifestTests
{
    [Fact]
    public void Registry_Tracks_Theme_Asset_References_With_Resource_Key_Schema_Fingerprint()
    {
        var control = ThemeCompilerTests.CreateCompilerButtonDescriptor();
        var provisional = new ControlThemeAssetDescriptor(
            new Uri("avares://Tests/Themes/SearchButtonTheme.axaml"),
            control.Identity,
            [control.Identity],
            new ControlThemeSemanticPartDescriptor(
                "SearchButtonTheme",
                "global::Avalonia.Controls.Button"),
            1);
        var baseRegistry = TypedThemeSnapshotCacheTests.CreateRegistry([control]);
        var asset = new ControlThemeAssetDescriptor(
            provisional.AssetUri,
            provisional.OwnerIdentity,
            provisional.ReferencedControlIdentities,
            provisional.SemanticPart,
            ThemeSchemaRegistry.ComputeResourceKeySchemaFingerprint(
                provisional,
                baseRegistry.GlobalTokens));

        var registry = TypedThemeSnapshotCacheTests.CreateRegistry(
            [control],
            themeAssets: [asset]);
        var manifest = new ControlThemeAssetManifest(registry, [asset]);

        manifest.Descriptors.ShouldBe([asset]);

        var withoutAsset = TypedThemeSnapshotCacheTests.CreateRegistry([control]);
        registry.Revision.ShouldNotBe(withoutAsset.Revision);
    }

    [Fact]
    public void Manifest_Accepts_Registered_Owner_Dependencies_And_Semantic_Part_Metadata()
    {
        var registry = CreateRegistry();
        var identity = registry.Controls[0].Identity;
        var semanticPart = new ControlThemeSemanticPartDescriptor(
            "SearchButtonTheme",
            "global::Avalonia.Controls.Button");
        var provisional = new ControlThemeAssetDescriptor(
            new Uri("avares://Tests/Themes/Button.axaml"),
            identity,
            [identity],
            semanticPart,
            1);
        var descriptor = new ControlThemeAssetDescriptor(
            provisional.AssetUri,
            provisional.OwnerIdentity,
            provisional.ReferencedControlIdentities,
            provisional.SemanticPart,
            ThemeSchemaRegistry.ComputeResourceKeySchemaFingerprint(
                provisional,
                registry.GlobalTokens));

        var manifest = new ControlThemeAssetManifest(registry, [descriptor]);

        manifest.Descriptors.ShouldBe([descriptor]);
        descriptor.ReferencedControlIdentities.ShouldBe([identity]);
        descriptor.SemanticPart.ShouldBe(semanticPart);
    }

    [Fact]
    public void Manifest_Rejects_Unknown_Owner_Duplicate_Uri_And_Empty_Fingerprint()
    {
        var registry = CreateRegistry();
        var identity = registry.Controls[0].Identity;

        Should.Throw<ThemeSchemaException>(() => new ControlThemeAssetManifest(
            registry,
            [Asset(new ControlTokenIdentity("AtomUI", "Missing"), fingerprint: 1)]));
        Should.Throw<ThemeSchemaException>(() => new ControlThemeAssetManifest(
            registry,
            [
                Asset(identity, fingerprint: Fingerprint(identity)),
                Asset(identity, fingerprint: Fingerprint(identity))
            ]));
        Should.Throw<ThemeSchemaException>(() => new ControlThemeAssetManifest(
            registry,
            [Asset(identity, fingerprint: 0)]));
    }

    [Fact]
    public void Manifest_Rejects_Resource_Key_Schema_Fingerprint_Mismatch()
    {
        var registry = CreateRegistry();
        var identity = registry.Controls[0].Identity;
        var expected = Fingerprint(identity);
        var mismatched = expected == ulong.MaxValue ? expected - 1 : expected + 1;

        Should.Throw<ThemeSchemaException>(() => new ControlThemeAssetManifest(
            registry,
            [Asset(identity, fingerprint: mismatched)]));
    }

    [Fact]
    public void Manifest_Rejects_Unregistered_Referenced_Control_Identity()
    {
        var registry = CreateRegistry();
        var owner = registry.Controls[0].Identity;
        var descriptor = new ControlThemeAssetDescriptor(
            new Uri("avares://Tests/Themes/Button.axaml"),
            owner,
            [new ControlTokenIdentity("AtomUI", "Missing")],
            null,
            1);
        var descriptorWithFingerprint = new ControlThemeAssetDescriptor(
            descriptor.AssetUri,
            descriptor.OwnerIdentity,
            descriptor.ReferencedControlIdentities,
            descriptor.SemanticPart,
            ThemeSchemaRegistry.ComputeResourceKeySchemaFingerprint(
                descriptor,
                registry.GlobalTokens));

        Should.Throw<ThemeSchemaException>(() => new ControlThemeAssetManifest(
            registry,
            [descriptorWithFingerprint]));
    }

    private static ControlThemeAssetDescriptor Asset(
        ControlTokenIdentity identity,
        ulong fingerprint)
    {
        return new ControlThemeAssetDescriptor(
            new Uri("avares://Tests/Themes/Button.axaml"),
            identity,
            [identity],
            null,
            fingerprint);
    }

    private static ulong Fingerprint(ControlTokenIdentity identity)
    {
        var asset = Asset(identity, 1);
        var registry = CreateRegistry();
        return ThemeSchemaRegistry.ComputeResourceKeySchemaFingerprint(asset, registry.GlobalTokens);
    }

    private static ThemeSchemaRegistry CreateRegistry()
    {
        return TypedThemeSnapshotCacheTests.CreateRegistry(
            [ThemeCompilerTests.CreateCompilerButtonDescriptor()]);
    }
}
