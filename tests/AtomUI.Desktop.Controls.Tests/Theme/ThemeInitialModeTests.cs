using System.Globalization;
using AtomUI.Theme;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Language;
using AtomUI.Theme.Schema;
using Avalonia.Media;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class ThemeInitialModeTests
{
    [Fact]
    public void Builder_Initial_Theme_Keeps_Theme_Id_And_Dark_Algorithm_Config()
    {
        var builder = new TestThemeManagerBuilder();
        var config = new ThemeConfigBuilder().WithAlgorithms("Default", "Dark").Build();

        builder.WithInitialTheme(IThemeManager.DEFAULT_THEME_ID, config);

        builder.ThemeId.ShouldBe(IThemeManager.DEFAULT_THEME_ID);
        builder.InitialConfig.ShouldBeSameAs(config);
        builder.InitialConfig!.Algorithms.ShouldBe(["Default", "Dark"]);
    }

    [Fact]
    public void Builder_Initial_Theme_Preserves_Explicit_Algorithm_Order()
    {
        var builder = new TestThemeManagerBuilder();
        var config = new ThemeConfigBuilder().WithAlgorithms("Default", "Compact", "Dark").Build();

        builder.WithInitialTheme(IThemeManager.DEFAULT_THEME_ID, config);

        builder.InitialConfig!.Algorithms.ShouldBe(["Default", "Compact", "Dark"]);
    }

    [Fact]
    public void Builder_Initial_Theme_Allows_Definition_Defaults_When_Config_Is_Null()
    {
        var builder = new TestThemeManagerBuilder();

        builder.WithInitialTheme(IThemeManager.DEFAULT_THEME_ID);

        builder.ThemeId.ShouldBe(IThemeManager.DEFAULT_THEME_ID);
        builder.InitialConfig.ShouldBeNull();
    }

    private sealed class TestThemeManagerBuilder : IThemeManagerBuilder
    {
        public IList<IControlThemesProvider> ControlThemesProviders { get; } = new List<IControlThemesProvider>();
        public IList<LanguageProvider> LanguageProviders { get; } = new List<LanguageProvider>();
        public IList<Action<IThemeManager>> Initializers { get; } = new List<Action<IThemeManager>>();
        public LanguageVariant LanguageVariant { get; private set; } = LanguageVariant.en_US;
        public string ThemeId { get; private set; } = IThemeManager.DEFAULT_THEME_ID;
        public ThemeConfig? InitialConfig { get; private set; }
        public ThemeRequest? FollowSystemLight { get; private set; }
        public ThemeRequest? FollowSystemDark { get; private set; }

        public void AddControlToken(ControlTokenDescriptor descriptor)
        {
        }

        public void AddControlThemesProvider(IControlThemesProvider controlThemesProvider)
        {
            ControlThemesProviders.Add(controlThemesProvider);
        }

        public void AddLanguageProviders(LanguageProvider languageProvider)
        {
            LanguageProviders.Add(languageProvider);
        }

        public void AddInitializer(Action<IThemeManager> initializer)
        {
            Initializers.Add(initializer);
        }

        public void WithInitialTheme(string themeId, ThemeConfig? config = null)
        {
            ThemeId      = themeId;
            InitialConfig = config;
        }

        public void WithFollowSystemThemes(ThemeRequest light, ThemeRequest dark)
        {
            FollowSystemLight = light;
            FollowSystemDark  = dark;
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

    }
}
