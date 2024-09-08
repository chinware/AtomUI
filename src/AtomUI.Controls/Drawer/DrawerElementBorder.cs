using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class DrawerElementBorder : Border
{
    private readonly Drawer _drawer;

    public DrawerElementBorder(Drawer drawer)
    {
        Background              =  Brushes.White;
        _drawer                 =  drawer;
        _drawer.PropertyChanged += DrawerOnPropertyChanged;
    }

    private void DrawerOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Drawer.PlacementProperty || e.Property == Drawer.IsOpenProperty)
        {
            UpdateDropShadow();
        }
    }

    private void UpdateDropShadow()
    {
        if (_drawer.IsOpen == false)
        {
            BoxShadow = new BoxShadows();
            return;
        }

        switch (_drawer.Placement)
        {
            case DrawerPlacement.Left:
            {
                var box1 = Avalonia.Media.BoxShadow.Parse("6 0 16 0 rgba(0,0,0,0.08)");
                var box2 = Avalonia.Media.BoxShadow.Parse("3 0 6 -4 rgba(0,0,0,0.12)");
                var box3 = Avalonia.Media.BoxShadow.Parse("9 0 28 8 rgba(0,0,0,0.05)");
                BoxShadow = new BoxShadows(box1, [
                    box2,
                    box3
                ]);
                break;
            }
            case DrawerPlacement.Top:
            {
                var box1 = Avalonia.Media.BoxShadow.Parse("0 6 16 0 rgba(0,0,0,0.08)");
                var box2 = Avalonia.Media.BoxShadow.Parse("0 3 6 -4 rgba(0,0,0,0.12)");
                var box3 = Avalonia.Media.BoxShadow.Parse("0 9 28 8 rgba(0,0,0,0.05)");
                BoxShadow = new BoxShadows(box1, [
                    box2,
                    box3
                ]);
                break;
            }
            case DrawerPlacement.Right:
            {
                var box1 = Avalonia.Media.BoxShadow.Parse("-6 0 16 0 rgba(0,0,0,0.08)");
                var box2 = Avalonia.Media.BoxShadow.Parse("-3 0 6 -4 rgba(0,0,0,0.12)");
                var box3 = Avalonia.Media.BoxShadow.Parse("-9 0 28 8 rgba(0,0,0,0.05)");
                BoxShadow = new BoxShadows(box1, [
                    box2,
                    box3
                ]);
                break;
            }
            case DrawerPlacement.Bottom:
            {
                var box1 = Avalonia.Media.BoxShadow.Parse("0 -6 16 0 rgba(0,0,0,0.08)");
                var box2 = Avalonia.Media.BoxShadow.Parse("0 -3 6 -4 rgba(0,0,0,0.12)");
                var box3 = Avalonia.Media.BoxShadow.Parse("0 -9 28 8 rgba(0,0,0,0.05)");
                BoxShadow = new BoxShadows(box1, [
                    box2,
                    box3
                ]);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}