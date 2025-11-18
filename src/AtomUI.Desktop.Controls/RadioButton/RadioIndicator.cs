using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Primitives;
using AtomUI.Desktop.Controls.Utils;
using AtomUI.Media;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal class RadioIndicator : TemplatedControl
{
    #region 公共属性定义

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
        AvaloniaProperty.Register<RadioIndicator, double>(
            nameof(DotSizeValue));

    public static readonly StyledProperty<double> DotPaddingProperty =
        AvaloniaProperty.Register<RadioIndicator, double>(
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

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<RadioIndicator>();

    internal static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<RadioIndicator>();

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    internal bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }
    
    #endregion

    private IPen? _cachedPen;
    private WaveSpiritDecorator? _waveSpiritDecorator;
    
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

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureTransitions(false);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Transitions = null;
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        RadioDotEffectSize = CalculateDotSize(IsEnabled, IsChecked.HasValue && IsChecked.Value);
        UpdatePseudoClasses();
    }
    
    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions = [
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(RadioBorderBrushProperty),
                    TransitionUtils.CreateTransition<DoubleTransition>(RadioDotEffectSizeProperty),
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(RadioBackgroundProperty,
                        SharedTokenKey.MotionDurationFast)
                ];
            }
        }
        else
        {
            Transitions = null;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsPointerOverProperty ||
            e.Property == IsCheckedProperty ||
            e.Property == IsEnabledProperty)
        {
            UpdatePseudoClasses();

            if (e.Property == IsCheckedProperty &&
                IsWaveAnimationEnabled &&
                !PseudoClasses.Contains(StdPseudoClass.Disabled) &&
                PseudoClasses.Contains(StdPseudoClass.Checked))
            {
                _waveSpiritDecorator?.Play();
            }
        }

        if (this.IsAttachedToVisualTree())
        {
            if (e.Property == IsPointerOverProperty ||
                e.Property == IsCheckedProperty ||
                e.Property == IsEnabledProperty)
            {
                RadioDotEffectSize = CalculateDotSize(IsEnabled, IsChecked.HasValue && IsChecked.Value);
            }
        }

        if (IsLoaded)
        {
            if (e.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);
            }
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Checked, IsChecked.HasValue && IsChecked.Value);
        PseudoClasses.Set(StdPseudoClass.UnChecked, (IsChecked.HasValue && !IsChecked.Value) || !IsChecked.HasValue);
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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _waveSpiritDecorator = e.NameScope.Find<WaveSpiritDecorator>(WaveSpiritDecorator.WaveSpiritPart);
    }
}