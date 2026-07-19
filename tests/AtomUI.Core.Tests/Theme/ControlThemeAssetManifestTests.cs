using AtomUI.Theme.Schema;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class ControlThemeAssetManifestTests
{
    [Fact]
    public void Manifest_Accepts_Registered_Identities_With_Unique_Asset_Uris()
    {
        var registry = CreateRegistry();
        var identity = registry.Controls[0].Identity;
        var descriptor = new ControlThemeAssetDescriptor(
            new Uri("avares://Tests/Themes/Button.axaml"),
            identity,
            1);

        var manifest = new ControlThemeAssetManifest(registry, [descriptor]);

        manifest.Descriptors.ShouldBe([descriptor]);
    }

    [Fact]
    public void Manifest_Rejects_Unknown_Identity_Duplicate_Uri_And_Empty_Fingerprint()
    {
        var registry = CreateRegistry();
        var identity = registry.Controls[0].Identity;

        Should.Throw<ThemeSchemaException>(() => new ControlThemeAssetManifest(
            registry,
            [new ControlThemeAssetDescriptor(
                new Uri("avares://Tests/Themes/Missing.axaml"),
                new ControlTokenIdentity("AtomUI", "Missing"),
                1)]));
        Should.Throw<ThemeSchemaException>(() => new ControlThemeAssetManifest(
            registry,
            [
                new ControlThemeAssetDescriptor(new Uri("avares://Tests/Themes/Button.axaml"), identity, 1),
                new ControlThemeAssetDescriptor(new Uri("avares://Tests/Themes/Button.axaml"), identity, 1)
            ]));
        Should.Throw<ThemeSchemaException>(() => new ControlThemeAssetManifest(
            registry,
            [new ControlThemeAssetDescriptor(
                new Uri("avares://Tests/Themes/Button.axaml"),
                identity,
                0)]));
    }

    private static ThemeSchemaRegistry CreateRegistry()
    {
        return TypedThemeSnapshotCacheTests.CreateRegistry(
            [ThemeCompilerTests.CreateCompilerButtonDescriptor()]);
    }
}
