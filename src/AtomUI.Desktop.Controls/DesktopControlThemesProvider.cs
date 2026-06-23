using AtomUI.Theme;
using Avalonia.Markup.Xaml;

namespace AtomUI.Desktop.Controls;

internal class DesktopControlThemesProvider : ControlThemesProvider
{
    public DesktopControlThemesProvider()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
