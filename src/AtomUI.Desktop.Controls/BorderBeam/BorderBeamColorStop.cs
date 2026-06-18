using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

public class BorderBeamColorStop : AvaloniaObject
{
    #region 公共属性定义

    public static readonly StyledProperty<Color> ColorProperty =
        AvaloniaProperty.Register<BorderBeamColorStop, Color>(nameof(Color));

    public static readonly StyledProperty<double> PercentProperty =
        AvaloniaProperty.Register<BorderBeamColorStop, double>(nameof(Percent));

    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public double Percent
    {
        get => GetValue(PercentProperty);
        set => SetValue(PercentProperty, value);
    }

    #endregion
}
