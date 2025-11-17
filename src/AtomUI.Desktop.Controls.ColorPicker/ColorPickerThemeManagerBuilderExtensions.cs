using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUI.Controls;

public static class ColorPickerThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseColorPicker(this IThemeManagerBuilder themeManagerBuilder)
    {
        var controlTokenTypes = ControlTokenTypePool.GetTokenTypes();
        foreach (var controlType in controlTokenTypes)
        {
            themeManagerBuilder.AddControlToken(controlType);
        }
        themeManagerBuilder.AddControlThemesProvider(new AtomUIColorPickerThemesProvider());

        var languageProviders = LanguageProviderPool.GetLanguageProviders();
        foreach (var languageProvider in languageProviders)
        {
            themeManagerBuilder.AddLanguageProviders(languageProvider);
        }
        return themeManagerBuilder;
    }
}