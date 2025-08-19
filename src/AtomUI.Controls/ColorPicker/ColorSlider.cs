using Avalonia;
using Avalonia.Layout;

namespace AtomUI.Controls;

using AvaloniaColorSlider = Avalonia.Controls.Primitives.ColorSlider;

public class ColorSlider : AvaloniaColorSlider
{
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == HeightProperty ||
            change.Property == WidthProperty)
        {
            ConfigureCornerRadius();
        }
    }

    protected void ConfigureCornerRadius()
    {
        if (Orientation == Orientation.Vertical)
        {
            CornerRadius = new CornerRadius(Width / 2);
        }
        else
        {
            CornerRadius = new CornerRadius(Height / 2);
        }
    }
}