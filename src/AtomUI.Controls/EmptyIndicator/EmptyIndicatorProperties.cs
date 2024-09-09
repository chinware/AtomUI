using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class EmptyIndicator
{
    internal static readonly StyledProperty<double> DescriptionMarginProperty =
        AvaloniaProperty.Register<EmptyIndicator, double>(
            nameof(DescriptionMargin));

    internal double DescriptionMargin
    {
        get => GetValue(DescriptionMarginProperty);
        set => SetValue(DescriptionMarginProperty, value);
    }



    #region Control token 值绑定属性

    private IBrush? _colorFillToken;

    private static readonly DirectProperty<EmptyIndicator, IBrush?> ColorFillTokenProperty =
        AvaloniaProperty.RegisterDirect<EmptyIndicator, IBrush?>(
            nameof(_colorFillToken),
            o => o._colorFillToken,
            (o, v) => o._colorFillToken = v);

    private IBrush? _colorFillTertiary;

    private static readonly DirectProperty<EmptyIndicator, IBrush?> ColorFillTertiaryTokenProperty =
        AvaloniaProperty.RegisterDirect<EmptyIndicator, IBrush?>(
            nameof(_colorFillTertiary),
            o => o._colorFillTertiary,
            (o, v) => o._colorFillTertiary = v);

    private IBrush? _colorFillQuaternary;

    private static readonly DirectProperty<EmptyIndicator, IBrush?> ColorFillQuaternaryTokenProperty =
        AvaloniaProperty.RegisterDirect<EmptyIndicator, IBrush?>(
            nameof(_colorFillQuaternary),
            o => o._colorFillQuaternary,
            (o, v) => o._colorFillQuaternary = v);

    private IBrush? _colorBgContainer;

    private static readonly DirectProperty<EmptyIndicator, IBrush?> ColorBgContainerTokenProperty =
        AvaloniaProperty.RegisterDirect<EmptyIndicator, IBrush?>(
            nameof(_colorBgContainer),
            o => o._colorBgContainer,
            (o, v) => o._colorBgContainer = v);

    #endregion
}