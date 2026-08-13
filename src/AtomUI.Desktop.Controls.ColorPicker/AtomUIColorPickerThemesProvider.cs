using AtomUI.Theme.Resources;

namespace AtomUI.Desktop.Controls;

internal class AtomUIColorPickerThemesProvider : ControlThemesProvider
{
    public AtomUIColorPickerThemesProvider()
    {
        Id = ColorPickerThemeManagerBuilderExtensions.PackageId;
    }
}
