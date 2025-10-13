using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class ComboBoxSpinnerInnerBox : AddOnDecoratedInnerBox
{
    #region 公共属性定义

    public static readonly StyledProperty<object?> SpinnerContentProperty =
        AvaloniaProperty.Register<ComboBoxSpinnerInnerBox, object?>(nameof(SpinnerContent));

    public object? SpinnerContent
    {
        get => GetValue(SpinnerContentProperty);
        set => SetValue(SpinnerContentProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<ComboBoxSpinnerInnerBox, Thickness> SpinnerBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<ComboBoxSpinnerInnerBox, Thickness>(nameof(SpinnerBorderThickness),
            o => o.SpinnerBorderThickness,
            (o, v) => o.SpinnerBorderThickness = v);

    internal static readonly DirectProperty<ComboBoxSpinnerInnerBox, IBrush?> SpinnerBorderBrushProperty =
        AvaloniaProperty.RegisterDirect<ComboBoxSpinnerInnerBox, IBrush?>(nameof(SpinnerBorderBrush),
            o => o.SpinnerBorderBrush,
            (o, v) => o.SpinnerBorderBrush = v);

    private Thickness _spinnerBorderThickness;

    internal Thickness SpinnerBorderThickness
    {
        get => _spinnerBorderThickness;
        set => SetAndRaise(SpinnerBorderThicknessProperty, ref _spinnerBorderThickness, value);
    }

    private IBrush? _spinnerBorderBrush;

    internal IBrush? SpinnerBorderBrush
    {
        get => _spinnerBorderBrush;
        set => SetAndRaise(SpinnerBorderBrushProperty, ref _spinnerBorderBrush, value);
    }

    #endregion
}