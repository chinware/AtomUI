using AtomUI.Localization;
using AtomUI.Registration;
using AtomUI.Theme;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Schema;
using Avalonia.Controls;
using Avalonia.Media;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Registration;

public sealed class AotTrimControlPackageRegistrationBuilderTests
{
    [Fact]
    public void Register_Aggregates_Selected_Fragments_And_Submits_One_Package()
    {
        var themeBuilder = new RecordingThemeManagerBuilder();
        var builder = new TestAtomUIBuilder(themeBuilder);
        var provider = new TestControlThemesProvider("Acme.Controls");
        var firstIdentity = new ControlTokenIdentity("Acme", "Alpha");
        var secondIdentity = new ControlTokenIdentity("Acme", "Beta");
        var firstAsset = CreateAsset("AlphaTheme.axaml", firstIdentity);
        var secondAsset = CreateAsset("BetaTheme.axaml", secondIdentity);
        var loadedResources = new List<string>();
        var registration = new AotTrimControlPackageRegistrationBuilder(
            builder,
            provider,
            includeIdentity: identity => identity != secondIdentity,
            selectAssets: assets => assets.Where(asset => asset.AssetUri == firstAsset.AssetUri).ToArray());

        registration.AddControl(new ControlTokenDescriptor(typeof(Button), secondIdentity));
        registration.AddControl(new ControlTokenDescriptor(typeof(Button), firstIdentity));
        registration.AddThemeAsset(firstAsset, _ => loadedResources.Add("alpha"));
        registration.AddThemeAsset(secondAsset, _ => loadedResources.Add("beta"));
        registration.AddPackageSharedThemeAsset(_ => loadedResources.Add("core"));
        registration.AddUnitThemeResource(_ => loadedResources.Add("unit-resource"));

        registration.Register();

        themeBuilder.Packages.Count.ShouldBe(1);
        var package = themeBuilder.Packages.Single();
        package.Id.ShouldBe(provider.Id);
        package.Controls.Select(control => control.Identity).ShouldBe([firstIdentity]);
        package.ThemeAssets.Select(asset => asset.AssetUri).ShouldBe([firstAsset.AssetUri]);
        loadedResources.ShouldBe(["core", "unit-resource", "alpha"]);
    }

    [Fact]
    public void TryEnterUnit_Returns_True_Once_Per_Builder()
    {
        var registration = new AotTrimControlPackageRegistrationBuilder(
            new TestAtomUIBuilder(new RecordingThemeManagerBuilder()),
            new TestControlThemesProvider("Acme.Controls"));

        registration.TryEnterUnit("Acme.Controls/DatePicker").ShouldBeTrue();
        registration.TryEnterUnit("Acme.Controls/DatePicker").ShouldBeFalse();
    }

    [Fact]
    public void TryEnterUnit_Breaks_Direct_Unit_Cycles()
    {
        var registration = new AotTrimControlPackageRegistrationBuilder(
            new TestAtomUIBuilder(new RecordingThemeManagerBuilder()),
            new TestControlThemesProvider("Acme.Controls"));
        var entered = new List<string>();

        AddAlpha(registration);

        entered.ShouldBe(["Beta", "Alpha"]);
        return;

        void AddAlpha(AotTrimControlPackageRegistrationBuilder builder)
        {
            if (!builder.TryEnterUnit("Acme.Controls/Alpha"))
            {
                return;
            }

            AddBeta(builder);
            entered.Add("Alpha");
        }

        void AddBeta(AotTrimControlPackageRegistrationBuilder builder)
        {
            if (!builder.TryEnterUnit("Acme.Controls/Beta"))
            {
                return;
            }

            AddAlpha(builder);
            entered.Add("Beta");
        }
    }

    [Fact]
    public void AddThemeAsset_Rejects_An_Asset_When_A_Referenced_Control_Is_Excluded()
    {
        var owner = new ControlTokenIdentity("Acme", "Owner");
        var dependency = new ControlTokenIdentity("Acme", "Dependency");
        var asset = new ControlThemeAssetDescriptor(
            new Uri("avares://Acme.Controls/Themes/OwnerTheme.axaml"),
            owner,
            [owner, dependency],
            null,
            1UL);
        var loaded = false;
        var themeBuilder = new RecordingThemeManagerBuilder();
        var registration = new AotTrimControlPackageRegistrationBuilder(
            new TestAtomUIBuilder(themeBuilder),
            new TestControlThemesProvider("Acme.Controls"),
            identity => identity != dependency);

        registration.AddControl(new ControlTokenDescriptor(typeof(Button), owner));
        registration.AddThemeAsset(asset, _ => loaded = true);
        registration.Register();

        themeBuilder.Packages.Single().ThemeAssets.ShouldBeEmpty();
        loaded.ShouldBeFalse();
    }

    [Fact]
    public void Register_Rejects_A_Null_Asset_Selection()
    {
        var registration = new AotTrimControlPackageRegistrationBuilder(
            new TestAtomUIBuilder(new RecordingThemeManagerBuilder()),
            new TestControlThemesProvider("Acme.Controls"),
            selectAssets: _ => null!);

        Should.Throw<ArgumentNullException>(() => registration.Register());
    }

    [Fact]
    public void Registered_Builder_Rejects_Further_Unit_Or_Resource_Mutations()
    {
        var registration = new AotTrimControlPackageRegistrationBuilder(
            new TestAtomUIBuilder(new RecordingThemeManagerBuilder()),
            new TestControlThemesProvider("Acme.Controls"));

        registration.Register();

        Should.Throw<InvalidOperationException>(() => registration.TryEnterUnit("Acme.Controls/Alpha"));
        Should.Throw<InvalidOperationException>(() =>
            registration.AddPackageSharedThemeAsset(static _ => { }));
        Should.Throw<InvalidOperationException>(() =>
            registration.AddUnitThemeResource(static _ => { }));
        Should.Throw<InvalidOperationException>(() => registration.Register());
    }

    private static ControlThemeAssetDescriptor CreateAsset(
        string fileName,
        ControlTokenIdentity identity)
    {
        return new ControlThemeAssetDescriptor(
            new Uri($"avares://Acme.Controls/Themes/{fileName}"),
            identity,
            [identity],
            null,
            1UL);
    }

    private sealed class TestAtomUIBuilder(IThemeManagerBuilder theme) : IAtomUIBuilder
    {
        public IThemeManagerBuilder Theme { get; } = theme;
        public ILocalizationBuilder Localization { get; } = new TestLocalizationBuilder();
    }

    private sealed class RecordingThemeManagerBuilder : IThemeManagerBuilder
    {
        internal List<ControlPackageRegistration> Packages { get; } = new();

        public void AddThemeDefinitionResolver(IThemeDefinitionResolver resolver) { }
        public void AddControlPackage(ControlPackageRegistration package) => Packages.Add(package);
        public void AddInitializer(Action<IThemeManager> initializer) { }
        public void WithInitialTheme(string themeId, ThemeConfig? config = null) { }
        public void WithFollowSystemThemes(ThemeRequest light, ThemeRequest dark) { }
        public void WithApplicationId(string applicationId) { }
        public void UseUserThemeDirectory() { }
        public void UseUserThemeDirectory(string directory) { }
        public void WithDefaultFontFamily(FontFamily fontFamily) { }
        public void WithDefaultFontFamily(string fontFamily) { }
    }

    private sealed class TestLocalizationBuilder : ILocalizationBuilder
    {
        public void AddCatalog(LanguageCatalogDescriptor descriptor) { }
        public void AddTranslationBundle(TranslationBundleDescriptor descriptor) { }
        public void AddLanguageDefinition(LanguageDefinition definition) { }
        public void ConfigureLanguages(LanguageTag defaultLanguage, IEnumerable<LanguageTag> supportedLanguages) { }
    }

    private sealed class TestControlThemesProvider : ControlThemesProvider
    {
        internal TestControlThemesProvider(string id)
        {
            Id = id;
        }
    }
}
