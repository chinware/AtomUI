using AtomUI.Theme;

namespace AtomUI.Controls;

public static class OSSDataGridThemeManagerBuilderExtensions
{
    public static ThemeManagerBuilder UseOSSDataGrid(this ThemeManagerBuilder themeManagerBuilder)
    {
        themeManagerBuilder.AppBuilder.AfterSetup(_ =>
        {
            var controlTokenTypes = ControlTokenTypePool.GetTokenTypes();
            foreach (var controlType in controlTokenTypes)
            {
                themeManagerBuilder.AddControlToken(controlType);
            }
            themeManagerBuilder.AddControlThemesProvider(new AtomUIDataGridThemesProvider());

            var languageProviders = LanguageProviderPool.GetLanguageProviders();
            foreach (var languageProvider in languageProviders)
            {
                themeManagerBuilder.AddLanguageProviders(languageProvider);
            }
        });
        return themeManagerBuilder;
    }
}