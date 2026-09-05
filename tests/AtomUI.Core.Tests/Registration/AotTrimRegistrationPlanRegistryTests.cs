using AtomUI.Localization;
using AtomUI.Registration;
using AtomUI.Theme;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Schema;
using Avalonia.Media;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Registration;

public sealed class AotTrimRegistrationPlanRegistryTests : IDisposable
{
    public AotTrimRegistrationPlanRegistryTests()
    {
        AotTrimRegistrationPlanRegistry.ResetForTests();
    }

    [Fact]
    public void ApplyPackage_Rejects_A_Missing_Application_Plan_Without_Full_Fallback()
    {
        var builder = new TestAtomUIBuilder();
        var provider = new TestControlThemesProvider("AtomUI.Desktop.Controls");

        var exception = Should.Throw<InvalidOperationException>(() =>
            AotTrimRegistrationPlanRegistry.ApplyPackage(
                builder,
                provider.Id,
                provider));

        exception.Message.ShouldContain("AtomUI.Desktop.Controls");
        exception.Message.ShouldContain("ProjectReference");
        exception.Message.ShouldContain("precompiled library");
        exception.Message.ShouldContain("AtomUIPackageRoot");
    }

    [Fact]
    public void Install_Rejects_A_Second_Application_Plan()
    {
        AotTrimRegistrationPlanRegistry.Install("First", static (_, _, _, _, _) => true);

        var exception = Should.Throw<InvalidOperationException>(() =>
            AotTrimRegistrationPlanRegistry.Install("Second", static (_, _, _, _, _) => true));

        exception.Message.ShouldContain("First");
        exception.Message.ShouldContain("Second");
    }

    [Fact]
    public void ApplyPackage_Dispatches_The_Same_Immutable_Plan_For_Each_Builder()
    {
        var packageIds = new List<string>();
        AotTrimRegistrationPlanRegistry.Install(
            "Test.Application",
            (_, packageId, _, _, _) =>
            {
                packageIds.Add(packageId);
                return true;
            });

        var provider = new TestControlThemesProvider("AtomUI.Controls");
        var first = new TestAtomUIBuilder();
        var second = new TestAtomUIBuilder();

        AotTrimRegistrationPlanRegistry.ApplyPackage(first, provider.Id, provider)
                                         .ShouldBeSameAs(first);
        AotTrimRegistrationPlanRegistry.ApplyPackage(second, provider.Id, provider)
                                         .ShouldBeSameAs(second);

        packageIds.ShouldBe(["AtomUI.Controls", "AtomUI.Controls"]);
    }

    [Fact]
    public void ApplyPackage_Rejects_A_Package_That_Is_Not_In_The_Installed_Plan()
    {
        AotTrimRegistrationPlanRegistry.Install("Test.Application", static (_, _, _, _, _) => false);
        var builder = new TestAtomUIBuilder();
        var provider = new TestControlThemesProvider("AtomUI.Desktop.Controls.DataGrid");

        var exception = Should.Throw<InvalidOperationException>(() =>
            AotTrimRegistrationPlanRegistry.ApplyPackage(builder, provider.Id, provider));

        exception.Message.ShouldContain(provider.Id);
        exception.Message.ShouldContain("UseXxxControls");
        exception.Message.ShouldContain("ProjectReference");
        exception.Message.ShouldContain("precompiled library");
        exception.Message.ShouldContain("AtomUIPackageRoot");
    }

    public void Dispose()
    {
        AotTrimRegistrationPlanRegistry.ResetForTests();
    }

    private sealed class TestAtomUIBuilder : IAtomUIBuilder
    {
        public IThemeManagerBuilder Theme { get; } = new TestThemeManagerBuilder();
        public ILocalizationBuilder Localization { get; } = new TestLocalizationBuilder();
    }

    private sealed class TestThemeManagerBuilder : IThemeManagerBuilder
    {
        public void AddThemeDefinitionResolver(IThemeDefinitionResolver resolver) { }
        public void AddControlPackage(ControlPackageRegistration package) { }
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
