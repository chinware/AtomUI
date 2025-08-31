using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class GradientColorSlider : RangeBase
{
    #region 公共属性定义

    public static readonly StyledProperty<List<GradientStop>?> GradientStopsProperty =
        AvaloniaProperty.Register<GradientColorSlider, List<GradientStop>?>(nameof(GradientStops));
    
    public static readonly StyledProperty<GradientStop?> ActivatedStopProperty =
        AvaloniaProperty.Register<GradientColorSlider, GradientStop?>(nameof(ActivatedStop));
    
    public List<GradientStop>? GradientStops
    {
        get => GetValue(GradientStopsProperty);
        set => SetValue(GradientStopsProperty, value);
    }
    
    public GradientStop? ActivatedStop
    {
        get => GetValue(ActivatedStopProperty);
        set => SetValue(ActivatedStopProperty, value);
    }

    #endregion
    
}