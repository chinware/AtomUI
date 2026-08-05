using AtomUI.Generated.AtomUI_Desktop_Controls_DataGrid;
namespace AtomUI.Desktop.Controls;

public static class DataGridThemeManagerBuilderExtensions
{
    public static IAtomUIBuilder UseDesktopDataGrid(this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        GeneratedControlPackageRegistration.Register(
            builder.Theme,
            new AtomUIDataGridThemesProvider());
        return builder;
    }
}
