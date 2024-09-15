using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaRadioButton = Avalonia.Controls.RadioButton;

public class RadioButton : AvaloniaRadioButton,
                           ICustomHitTest,
                           IWaveAdornerInfoProvider
{
    #region 内部属性定义

     internal static readonly StyledProperty<double> RadioSizeProperty =
        AvaloniaProperty.Register<Button, double>(nameof(RadioSize));

    internal static readonly StyledProperty<double> PaddingInlineProperty =
        AvaloniaProperty.Register<Button, double>(nameof(PaddingInline));

    internal static readonly StyledProperty<IBrush?> RadioBorderBrushProperty =
        AvaloniaProperty.Register<Button, IBrush?>(nameof(RadioBorderBrush));

    internal static readonly StyledProperty<IBrush?> RadioInnerBackgroundProperty =
        AvaloniaProperty.Register<Button, IBrush?>(nameof(RadioInnerBackground));

    internal static readonly StyledProperty<IBrush?> RadioBackgroundProperty =
        AvaloniaProperty.Register<Button, IBrush?>(nameof(RadioBackground));

    internal static readonly StyledProperty<Thickness> RadioBorderThicknessProperty =
        AvaloniaProperty.Register<Button, Thickness>(nameof(RadioBorderThickness));

    internal static readonly StyledProperty<double> RadioDotEffectSizeProperty =
        AvaloniaProperty.Register<Button, double>(nameof(RadioDotEffectSize));

    internal static readonly StyledProperty<double> DotSizeValueProperty =
        AvaloniaProperty.Register<RadioButton, double>(
            nameof(DotSizeValue));

    internal static readonly StyledProperty<double> DotPaddingProperty =
        AvaloniaProperty.Register<RadioButton, double>(
            nameof(DotPadding));

    internal double RadioSize
    {
        get => GetValue(RadioSizeProperty);
        set => SetValue(RadioSizeProperty, value);
    }

    internal double PaddingInline
    {
        get => GetValue(PaddingInlineProperty);
        set => SetValue(PaddingInlineProperty, value);
    }

    internal IBrush? RadioBorderBrush
    {
        get => GetValue(RadioBorderBrushProperty);
        set => SetValue(RadioBorderBrushProperty, value);
    }

    internal IBrush? RadioInnerBackground
    {
        get => GetValue(RadioInnerBackgroundProperty);
        set => SetValue(RadioInnerBackgroundProperty, value);
    }

    internal IBrush? RadioBackground
    {
        get => GetValue(RadioBackgroundProperty);
        set => SetValue(RadioBackgroundProperty, value);
    }

    internal Thickness RadioBorderThickness
    {
        get => GetValue(RadioBorderThicknessProperty);
        set => SetValue(RadioBorderThicknessProperty, value);
    }

    internal double RadioDotEffectSize
    {
        get => GetValue(RadioDotEffectSizeProperty);
        set => SetValue(RadioDotEffectSizeProperty, value);
    }

    internal double DotSizeValue
    {
        get => GetValue(DotSizeValueProperty);
        set => SetValue(DotSizeValueProperty, value);
    }

    internal double DotPadding
    {
        get => GetValue(DotPaddingProperty);
        set => SetValue(DotPaddingProperty, value);
    }

    #endregion
    
    private IPen? _cachedPen;
    private ControlStyleState _styleState;

    static RadioButton()
    {
        AffectsRender<RadioButton>(
            RadioBorderBrushProperty,
            RadioInnerBackgroundProperty,
            RadioBackgroundProperty,
            RadioBorderThicknessProperty,
            RadioDotEffectSizeProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        HandlePropertyChangedForStyle(e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        HandleTemplateApplied(e.NameScope);
    }

    private void HandleTemplateApplied(INameScope scope)
    {
        TokenResourceBinder.CreateGlobalResourceBinding(this, RadioBorderThicknessProperty,
            GlobalTokenResourceKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
        Cursor = new Cursor(StandardCursorType.Hand);
        CollectStyleState();
        RadioDotEffectSize = CalculateDotSize(IsEnabled, IsChecked.HasValue && IsChecked.Value);
        SetupTransitions();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size         = base.MeasureOverride(availableSize);
        var targetWidth  = size.Width + RadioSize + PaddingInline;
        var targetHeight = Math.Max(size.Height, RadioSize);
        return new Size(targetWidth, targetHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var arrangeRect = RadioTextRect();

        var visualChildren = VisualChildren;
        var visualCount    = visualChildren.Count;

        for (var i = 0; i < visualCount; i++)
        {
            var visual = visualChildren[i];
            if (visual is Layoutable layoutable)
            {
                layoutable.Arrange(arrangeRect);
            }
        }

        return finalSize;
    }

    public sealed override void Render(DrawingContext context)
    {
        var radioRect = RadioRect();
        var penWidth  = RadioBorderThickness.Top;
        PenUtils.TryModifyOrCreate(ref _cachedPen, RadioBorderBrush, RadioBorderThickness.Top);
        context.DrawEllipse(RadioBackground, _cachedPen, radioRect.Deflate(penWidth / 2));
        if (IsChecked.HasValue && IsChecked.Value)
        {
            var dotDiameter = RadioDotEffectSize / 2;
            context.DrawEllipse(RadioInnerBackground, null, radioRect.Center, dotDiameter, dotDiameter);
        }
    }

    public bool HitTest(Point point)
    {
        return true;
    }

    private void CollectStyleState()
    {
        ControlStateUtils.InitCommonState(this, ref _styleState);
        if (IsPressed)
        {
            _styleState |= ControlStyleState.Sunken;
        }
        else
        {
            _styleState |= ControlStyleState.Raised;
        }

        if (IsChecked.HasValue && IsChecked.Value)
        {
            _styleState |= ControlStyleState.On;
        }
        else
        {
            _styleState |= ControlStyleState.Off;
        }
    }

    private double CalculateDotSize(bool isEnabled, bool isChecked)
    {
        double targetValue;
        if (isChecked)
        {
            if (isEnabled)
            {
                targetValue = DotSizeValue;
            }
            else
            {
                targetValue = RadioSize - DotPadding * 2;
            }
        }
        else
        {
            targetValue = DotSizeValue * 0.6;
        }

        return targetValue;
    }

    private void SetupTransitions()
    {
        Transitions = new Transitions
        {
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(RadioBorderBrushProperty),
            AnimationUtils.CreateTransition<DoubleTransition>(RadioDotEffectSizeProperty),
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(RadioBackgroundProperty,
                GlobalTokenResourceKey.MotionDurationFast)
        };
    }

    // Measure 之后才有值
    private Rect RadioRect()
    {
        var offsetY = (DesiredSize.Height - Margin.Top - Margin.Bottom - RadioSize) / 2;
        return new Rect(0d, offsetY, RadioSize, RadioSize);
    }

    private Rect RadioTextRect()
    {
        var offsetX = RadioSize + PaddingInline;
        return new Rect(offsetX, 0d, DesiredSize.Width - offsetX, DesiredSize.Height);
    }

    private void HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == IsPointerOverProperty ||
            e.Property == IsCheckedProperty ||
            e.Property == IsEnabledProperty)
        {
            CollectStyleState();
            if (VisualRoot is not null)
            {
                RadioDotEffectSize = CalculateDotSize(IsEnabled, IsChecked.HasValue && IsChecked.Value);
            }

            if (e.Property == IsCheckedProperty &&
                _styleState.HasFlag(ControlStyleState.Enabled) &&
                _styleState.HasFlag(ControlStyleState.On))
            {
                WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.CircleWave);
            }
        }
    }

    public Rect WaveGeometry()
    {
        return RadioRect();
    }

    public CornerRadius WaveBorderRadius()
    {
        return new CornerRadius(RadioSize / 2);
    }
}