using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class DrawerElementBorder : Border
{
    public DrawerElementBorder(Drawer drawer)
    {
        Child      = drawer.Content;
        Background = Brushes.White;
    }
}