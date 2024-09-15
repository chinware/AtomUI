using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Button = AtomUI.Controls.Button;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class DrawerShowCase : UserControl
{
    public DrawerShowCase()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (Drawer.GetDrawer(button) is not { } drawer)
        {
            return;
        }

        drawer.IsOpen = false;
    }

    private void ButtonOpenOnCurrentParent_OnClick(object? sender, RoutedEventArgs e)
    {
        Drawer1.OpenOn = Drawer1.OpenOn?.Parent as Visual;
    }
}