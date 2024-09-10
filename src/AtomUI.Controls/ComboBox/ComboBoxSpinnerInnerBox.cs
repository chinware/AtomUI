using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Rendering;

namespace AtomUI.Controls;

[TemplatePart(ComboBoxSpinnerInnerBoxTheme.SpinnerHandlePart, typeof(ContentPresenter))]
internal class ComboBoxSpinnerInnerBox : AddOnDecoratedInnerBox, ICustomHitTest
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

    internal static readonly DirectProperty<ComboBoxSpinnerInnerBox, double> SpinnerHandleWidthTokenProperty =
        AvaloniaProperty.RegisterDirect<ComboBoxSpinnerInnerBox, double>(nameof(SpinnerHandleWidthToken),
            o => o.SpinnerHandleWidthToken,
            (o, v) => o.SpinnerHandleWidthToken = v);

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

    private double _spinnerHandleWidthToken;

    internal double SpinnerHandleWidthToken
    {
        get => _spinnerHandleWidthToken;
        set => SetAndRaise(SpinnerHandleWidthTokenProperty, ref _spinnerHandleWidthToken, value);
    }

    #endregion

    protected override void BuildEffectiveInnerBoxPadding()
    {
        var padding = _spinnerHandleWidthToken + InnerBoxPadding.Right;
        EffectiveInnerBoxPadding =
            new Thickness(InnerBoxPadding.Left, InnerBoxPadding.Top, padding, InnerBoxPadding.Bottom);
    }

    public bool HitTest(Point point)
    {
        return true;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        TokenResourceBinder.CreateGlobalTokenBinding(this, SpinnerBorderThicknessProperty,
            GlobalTokenResourceKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
        TokenResourceBinder.CreateGlobalTokenBinding(this, SpinnerBorderBrushProperty,
            GlobalTokenResourceKey.ColorBorder);
        TokenResourceBinder.CreateTokenBinding(this, SpinnerHandleWidthTokenProperty,
            ButtonSpinnerTokenResourceKey.HandleWidth);
        base.OnApplyTemplate(e);
    }
}