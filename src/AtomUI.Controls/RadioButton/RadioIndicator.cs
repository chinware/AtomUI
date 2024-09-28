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
using Avalonia.Media;

namespace AtomUI.Controls;

internal class RadioIndicator : Control, IWaveAdornerInfoProvider
{
    public static readonly StyledProperty<bool?> IsCheckedProperty =
        ToggleButton.IsCheckedProperty.AddOwner<CheckBoxIndicator>();

    public static readonly StyledProperty<double> RadioSizeProperty =
        AvaloniaProperty.Register<RadioIndicator, double>(nameof(RadioSize));

    public static readonly StyledProperty<double> PaddingInlineProperty =
        AvaloniaProperty.Register<RadioIndicator, double>(nameof(PaddingInline));

    public static readonly StyledProperty<IBrush?> RadioBorderBrushProperty =
        AvaloniaProperty.Register<RadioIndicator, IBrush?>(nameof(RadioBorderBrush));

    public static readonly StyledProperty<IBrush?> RadioInnerBackgroundProperty =
        AvaloniaProperty.Register<RadioIndicator, IBrush?>(nameof(RadioInnerBackground));

    public static readonly StyledProperty<IBrush?> RadioBackgroundProperty =
        AvaloniaProperty.Register<RadioIndicator, IBrush?>(nameof(RadioBackground));

    public static readonly StyledProperty<Thickness> RadioBorderThicknessProperty =
        AvaloniaProperty.Register<RadioIndicator, Thickness>(nameof(RadioBorderThickness));

    public static readonly StyledProperty<double> RadioDotEffectSizeProperty =
        AvaloniaProperty.Register<RadioIndicator, double>(nameof(RadioDotEffectSize));

    public static readonly StyledProperty<double> DotSizeValueProperty =
        AvaloniaProperty.Register<RadioButton, double>(
            nameof(DotSizeValue));

    public static readonly StyledProperty<double> DotPaddingProperty =
        AvaloniaProperty.Register<RadioButton, double>(
            nameof(DotPadding));

    public bool? IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public double RadioSize
    {
        get => GetValue(RadioSizeProperty);
        set => SetValue(RadioSizeProperty, value);
    }

    public double PaddingInline
    {
        get => GetValue(PaddingInlineProperty);
        set => SetValue(PaddingInlineProperty, value);
    }

    public IBrush? RadioBorderBrush
    {
        get => GetValue(RadioBorderBrushProperty);
        set => SetValue(RadioBorderBrushProperty, value);
    }

    public IBrush? RadioInnerBackground
    {
        get => GetValue(RadioInnerBackgroundProperty);
        set => SetValue(RadioInnerBackgroundProperty, value);
    }

    public IBrush? RadioBackground
    {
        get => GetValue(RadioBackgroundProperty);
        set => SetValue(RadioBackgroundProperty, value);
    }

    public Thickness RadioBorderThickness
    {
        get => GetValue(RadioBorderThicknessProperty);
        set => SetValue(RadioBorderThicknessProperty, value);
    }

    public double RadioDotEffectSize
    {
        get => GetValue(RadioDotEffectSizeProperty);
        set => SetValue(RadioDotEffectSizeProperty, value);
    }

    public double DotSizeValue
    {
        get => GetValue(DotSizeValueProperty);
        set => SetValue(DotSizeValueProperty, value);
    }

    public double DotPadding
    {
        get => GetValue(DotPaddingProperty);
        set => SetValue(DotPaddingProperty, value);
    }

    private IPen? _cachedPen;
    private ControlStyleState _styleState;

    static RadioIndicator()
    {
        AffectsRender<RadioIndicator>(
            IsCheckedProperty,
            RadioBorderBrushProperty,
            RadioInnerBackgroundProperty,
            RadioBackgroundProperty,
            RadioBorderThicknessProperty,
            RadioDotEffectSizeProperty);
    }

    public override void ApplyTemplate()
    {
        base.ApplyTemplate();
        CollectStyleState();
        RadioDotEffectSize = CalculateDotSize(IsEnabled, IsChecked.HasValue && IsChecked.Value);
        TokenResourceBinder.CreateGlobalResourceBinding(this, RadioBorderThicknessProperty,
            GlobalTokenResourceKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));

        Transitions ??= new Transitions
        {
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(RadioBorderBrushProperty),
            AnimationUtils.CreateTransition<DoubleTransition>(RadioDotEffectSizeProperty),
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(RadioBackgroundProperty,
                GlobalTokenResourceKey.MotionDurationFast)
        };
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
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

    private void CollectStyleState()
    {
        ControlStateUtils.InitCommonState(this, ref _styleState);

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

    public sealed override void Render(DrawingContext context)
    {
        var penWidth = RadioBorderThickness.Top;
        PenUtils.TryModifyOrCreate(ref _cachedPen, RadioBorderBrush, RadioBorderThickness.Top);
        var targetRect = new Rect(0, 0, Bounds.Width, Bounds.Height);
        context.DrawEllipse(RadioBackground, _cachedPen, targetRect.Deflate(penWidth / 2));
        if (IsChecked.HasValue && IsChecked.Value)
        {
            var dotDiameter = RadioDotEffectSize / 2;
            context.DrawEllipse(RadioInnerBackground, null, targetRect.Center, dotDiameter, dotDiameter);
        }
    }

    public Rect WaveGeometry()
    {
        return new Rect(Bounds.Size);
    }

    public CornerRadius WaveBorderRadius()
    {
        return new CornerRadius(RadioSize / 2);
    }
}