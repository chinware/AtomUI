using AtomUI.Theme.Schema;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class ControlThemeAssetManifestTests
{
    [Fact]
    public void Registry_Tracks_Theme_Asset_References_Without_Global_Token_Dependencies()
    {
        var control = ThemeCompilerTests.CreateCompilerButtonDescriptor();
        var asset = new ControlThemeAssetDescriptor(
            new Uri("avares://Tests/Themes/SearchButtonTheme.axaml"),
            control.Identity,
            [control.Identity],
            new ControlThemeSemanticPartDescriptor(
                "SearchButtonTheme",
                "global::Avalonia.Controls.Button"),
            1);

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
        var descriptor = new ControlThemeAssetDescriptor(
            new Uri("avares://Tests/Themes/Button.axaml"),
            identity,
            [identity],
            semanticPart,
            1);

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
                Asset(identity, fingerprint: 1),
                Asset(identity, fingerprint: 1)
            ]));
        Should.Throw<ThemeSchemaException>(() => new ControlThemeAssetManifest(
            registry,
            [Asset(identity, fingerprint: 0)]));
    }

    [Fact]
    public void Manifest_Rejects_Unregistered_Referenced_Control_Identity()
    {
        var registry = CreateRegistry();
        var owner = registry.Controls[0].Identity;

        Should.Throw<ThemeSchemaException>(() => new ControlThemeAssetManifest(
            registry,
            [new ControlThemeAssetDescriptor(
                new Uri("avares://Tests/Themes/Button.axaml"),
                owner,
                [new ControlTokenIdentity("AtomUI", "Missing")],
                null,
                1)]));
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

    private static ThemeSchemaRegistry CreateRegistry()
    {
        return TypedThemeSnapshotCacheTests.CreateRegistry(
            [ThemeCompilerTests.CreateCompilerButtonDescriptor()]);
    }
}
