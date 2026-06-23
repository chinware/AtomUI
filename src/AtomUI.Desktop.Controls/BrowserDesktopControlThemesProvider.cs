using AtomUI.Theme;
using Avalonia.Markup.Xaml;

namespace AtomUI.Desktop.Controls;

internal class BrowserDesktopControlThemesProvider : ControlThemesProvider
{
    public BrowserDesktopControlThemesProvider()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
