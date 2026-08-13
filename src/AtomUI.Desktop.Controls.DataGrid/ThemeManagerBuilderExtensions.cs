using AtomUI.Generated.AtomUIDesktopControlsDataGrid;
using AtomUI.Registration;
namespace AtomUI.Desktop.Controls;

public static class DataGridThemeManagerBuilderExtensions
{
    internal const string PackageId = "AtomUI.Desktop.Controls.DataGrid";

    public static IAtomUIBuilder UseDesktopDataGrid(this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var provider = new AtomUIDataGridThemesProvider();
        if (AotTrimRegistration.IsEnabled)
        {
            AotTrimRegistrationPlanRegistry.ApplyPackage(
                builder,
                PackageId,
                provider);
        }
        else
        {
            GeneratedControlPackageRegistration.Register(builder.Theme, provider);
        }
        GeneratedLanguageModuleRegistration.Register(builder.Localization);
        return builder;
    }
}
