using Avalonia;
using Avalonia.Controls.Shapes;

namespace AtomUI.Controls.Internal;

public abstract class RangeInfoPickerInput : InfoPickerInput
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> SecondaryWatermarkProperty =
        AvaloniaProperty.Register<InfoPickerInput, string?>(nameof(SecondaryWatermark));
    
    public string? SecondaryWatermark
    {
        get => GetValue(SecondaryWatermarkProperty);
        set => SetValue(SecondaryWatermarkProperty, value);
    }
    
    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<RangeInfoPickerInput, RangeActivatedPart> RangeActivatedPartProperty =
        AvaloniaProperty.RegisterDirect<RangeInfoPickerInput, RangeActivatedPart>(nameof(RangeActivatedPart),
            o => o.RangeActivatedPart);

    private RangeActivatedPart _rangeActivatedPart;

    internal RangeActivatedPart RangeActivatedPart
    {
        get => _rangeActivatedPart;
        set => SetAndRaise(RangeActivatedPartProperty, ref _rangeActivatedPart, value);
    }

    internal static readonly StyledProperty<double> PickerIndicatorOffsetXProperty =
        AvaloniaProperty.Register<RangeInfoPickerInput, double>(nameof(PickerIndicatorOffsetX), double.NaN);

    internal double PickerIndicatorOffsetX
    {
        get => GetValue(PickerIndicatorOffsetXProperty);
        set => SetValue(PickerIndicatorOffsetXProperty, value);
    }

    internal static readonly StyledProperty<double> PickerIndicatorOffsetYProperty =
        AvaloniaProperty.Register<RangeInfoPickerInput, double>(nameof(PickerIndicatorOffsetY));

    internal double PickerIndicatorOffsetY
    {
        get => GetValue(PickerIndicatorOffsetYProperty);
        set => SetValue(PickerIndicatorOffsetYProperty, value);
    }

    #endregion

        
    static RangeInfoPickerInput()
    {
        AffectsArrange<RangeInfoPickerInput>(PickerIndicatorOffsetXProperty, PickerIndicatorOffsetYProperty);
    }
    
    private Rectangle? _rangePickerIndicator;
    private TextBox? _rangeStartTextBox;
    private TextBox? _rangeEndTextBox;
}