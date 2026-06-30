using AtomUI.Theme;
using Avalonia.Markup.Xaml;

namespace AtomUI.Toolkits.GalleryBase.Controls;

internal class GalleryControlThemesProvider : ControlThemesProvider
{
    public GalleryControlThemesProvider()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
