using AtomUI.Theme.Resources;

namespace AtomUI.Desktop.Controls;

internal class AtomUIExtrasThemesProvider : ControlThemesProvider
{
    public AtomUIExtrasThemesProvider()
    {
        Id = ExtrasThemeManagerBuilderExtensions.PackageId;
    }
}
