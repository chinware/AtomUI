using AtomUI.Theme;
using AtomUI.Theme.Language;
using AtomUI.Generated.AtomUI_Desktop_Controls_DataGrid;

namespace AtomUI.Desktop.Controls;

public static class DataGridThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseDesktopDataGrid(this IThemeManagerBuilder themeManagerBuilder)
    {
        foreach (var descriptor in GeneratedThemeSchema.GetControls())
        {
            themeManagerBuilder.AddControlToken(descriptor);
        }
        themeManagerBuilder.AddControlThemesProvider(new AtomUIDataGridThemesProvider());

        var languageProviders = LanguageProviderPool.GetLanguageProviders();
        foreach (var languageProvider in languageProviders)
        {
            themeManagerBuilder.AddLanguageProviders(languageProvider);
        }
        return themeManagerBuilder;
    }
}
