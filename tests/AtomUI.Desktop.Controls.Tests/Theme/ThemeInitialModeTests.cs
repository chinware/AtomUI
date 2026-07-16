using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using AtomUI.Theme;
using AtomUI.Theme.Language;
using AtomUI.Theme.Schema;
using AtomUI.Theme.Styling;
using Avalonia.Media;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class ThemeInitialModeTests
{
    [Fact]
    public void Builder_Default_Theme_With_Dark_Algorithm_Uses_Dark_Variant_As_Initial_Theme()
    {
        var builder = new TestThemeManagerBuilder();

        builder.WithDefaultTheme(IThemeManager.DEFAULT_THEME_ID, ThemeAlgorithm.Dark);

        builder.ThemeId.ShouldBe($"{IThemeManager.DEFAULT_THEME_ID}-Dark");
    }

    [Fact]
    public void Builder_Default_Theme_With_Dark_And_Compact_Algorithms_Uses_Normalized_Variant_As_Initial_Theme()
    {
        var builder = new TestThemeManagerBuilder();

        builder.WithDefaultTheme(IThemeManager.DEFAULT_THEME_ID, ThemeAlgorithm.Compact, ThemeAlgorithm.Dark);

        builder.ThemeId.ShouldBe($"{IThemeManager.DEFAULT_THEME_ID}-Dark-Compact");
    }

    [Fact]
    public void Builder_Default_Theme_With_Duplicate_Algorithms_Uses_Each_Algorithm_Once()
    {
        var builder = new TestThemeManagerBuilder();

        builder.WithDefaultTheme(
            IThemeManager.DEFAULT_THEME_ID,
            ThemeAlgorithm.Dark,
            ThemeAlgorithm.Dark,
            ThemeAlgorithm.Compact);

        builder.ThemeId.ShouldBe($"{IThemeManager.DEFAULT_THEME_ID}-Dark-Compact");
    }

    private sealed class TestThemeManagerBuilder : IThemeManagerBuilder
    {
        public IList<Type> ControlDesignTokens { get; } = new List<Type>();
        public IList<IThemeAssetPathProvider> ThemeAssetPathProviders { get; } = new List<IThemeAssetPathProvider>();
        public IList<IControlThemesProvider> ControlThemesProviders { get; } = new List<IControlThemesProvider>();
        public IList<LanguageProvider> LanguageProviders { get; } = new List<LanguageProvider>();
        public IList<EventHandler> InitializedHandlers { get; } = new List<EventHandler>();
        public LanguageVariant LanguageVariant { get; private set; } = LanguageVariant.en_US;
        public string ThemeId { get; private set; } = IThemeManager.DEFAULT_THEME_ID;

        public void AddControlToken(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                                        DynamicallyAccessedMemberTypes.PublicProperties |
                                        DynamicallyAccessedMemberTypes.NonPublicProperties)]
            Type tokenType)
        {
            ControlDesignTokens.Add(tokenType);
        }

        public void AddControlToken(ControlTokenDescriptor descriptor)
        {
        }

        public void AddControlThemesProvider(IThemeAssetPathProvider themeAssetPathProvider)
        {
            ThemeAssetPathProviders.Add(themeAssetPathProvider);
        }

        public void AddControlThemesProvider(IControlThemesProvider controlThemesProvider)
        {
            ControlThemesProviders.Add(controlThemesProvider);
        }

        public void AddLanguageProviders(LanguageProvider languageProvider)
        {
            LanguageProviders.Add(languageProvider);
        }

        public void WithDefaultTheme(string themeId)
        {
            ThemeId = themeId;
        }

        public void WithDefaultFontFamily(FontFamily fontFamily)
        {
        }

        public void WithDefaultFontFamily(string fontFamily)
        {
        }

        public void WithDefaultCultureInfo(CultureInfo cultureInfo)
        {
            LanguageVariant = LanguageVariant.FromCultureInfo(cultureInfo);
        }

        public void WithDefaultLanguageVariant(LanguageVariant languageVariant)
        {
            LanguageVariant = languageVariant;
        }

        public void WithThemeVariantCalculatorFactory(IThemeVariantCalculatorFactory factory)
        {
        }
    }
}
