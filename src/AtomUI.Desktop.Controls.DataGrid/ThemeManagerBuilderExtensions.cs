using AtomUI.Generated.AtomUI_Desktop_Controls_DataGrid;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public static class DataGridThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseDesktopDataGrid(this IThemeManagerBuilder themeManagerBuilder)
    {
        GeneratedControlPackageRegistration.Register(
            themeManagerBuilder,
            new AtomUIDataGridThemesProvider());
        return themeManagerBuilder;
    }
}
