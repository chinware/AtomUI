using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Media;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal class StepsItemIndicator : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<StepsItemStatus> StatusProperty =
        AvaloniaProperty.Register<StepsItemIndicator, StepsItemStatus>(nameof(Status), StepsItemStatus.Process);
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<StepsItemIndicator>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<StepsItemIndicator>();
    
    public static readonly StyledProperty<StepsItemIndicatorType> IndicatorTypeProperty =
        StepsItem.IndicatorTypeProperty.AddOwner<StepsItemIndicator>();

    public static readonly StyledProperty<Icon?> IconProperty =
        StepsItem.IconProperty.AddOwner<StepsItemIndicator>();
    
    public static readonly StyledProperty<StepsStyle> StyleProperty =
        Steps.StyleProperty.AddOwner<StepsItemIndicator>();
    
    public StepsItemStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public StepsItemIndicatorType IndicatorType
    {
        get => GetValue(IndicatorTypeProperty);
        set => SetValue(IndicatorTypeProperty, value);
    }
    
    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public StepsStyle Style
    {
        get => GetValue(StyleProperty);
        set => SetValue(StyleProperty, value);
    }

    #endregion
    
    #region 内部属性定义

    internal static readonly DirectProperty<StepsItemIndicator, int> PositionProperty =
        AvaloniaProperty.RegisterDirect<StepsItemIndicator, int>(
            nameof(Position),
            o => o.Position,
            (o, v) => o.Position = v);
    
    internal static readonly DirectProperty<StepsItemIndicator, bool> IsCustomProperty =
        AvaloniaProperty.RegisterDirect<StepsItemIndicator, bool>(
            nameof(IsCustom),
            o => o.IsCustom,
            (o, v) => o.IsCustom = v);
    
    internal static readonly DirectProperty<StepsItemIndicator, bool> IsCurrentProperty =
        AvaloniaProperty.RegisterDirect<StepsItemIndicator, bool>(
            nameof(IsCurrent),
            o => o.IsCurrent,
            (o, v) => o.IsCurrent = v);
    
    internal static readonly DirectProperty<StepsItemIndicator, bool> IsClickableProperty =
        AvaloniaProperty.RegisterDirect<StepsItemIndicator, bool>(
            nameof(IsClickable),
            o => o.IsClickable,
            (o, v) => o.IsClickable = v);
    
    internal static readonly DirectProperty<StepsItemIndicator, bool> IsItemHoverProperty =
        AvaloniaProperty.RegisterDirect<StepsItemIndicator, bool>(
            nameof(IsItemHover),
            o => o.IsItemHover,
            (o, v) => o.IsItemHover = v);
    
    internal static readonly DirectProperty<StepsItemIndicator, bool> IsShowProgressProperty =
        AvaloniaProperty.RegisterDirect<StepsItemIndicator, bool>(
            nameof(IsShowProgress),
            o => o.IsShowProgress,
            (o, v) => o.IsShowProgress = v);
    
    internal static readonly DirectProperty<StepsItemIndicator, double> ProgressValueProperty =
        AvaloniaProperty.RegisterDirect<StepsItemIndicator, double>(
            nameof(ProgressValue),
            o => o.ProgressValue,
            (o, v) => o.ProgressValue = v);
    
    internal static readonly StyledProperty<double> ProgressLineThicknessProperty =
        AvaloniaProperty.Register<StepsItemIndicator, double>(nameof(ProgressLineThickness));
    
    internal static readonly StyledProperty<IBrush?> ProgressGrooveColorProperty =
        AvaloniaProperty.Register<StepsItemIndicator, IBrush?>(nameof(ProgressGrooveColor));

    internal static readonly StyledProperty<IBrush?> ProgressColorProperty =
        AvaloniaProperty.Register<StepsItemIndicator, IBrush?>(nameof(ProgressColor));
    
    private int _position;

    internal int Position
    {
        get => _position;
        set => SetAndRaise(PositionProperty, ref _position, value);
    }
    
    private bool _isCustom;

    internal bool IsCustom
    {
        get => _isCustom;
        set => SetAndRaise(IsCustomProperty, ref _isCustom, value);
    }
    
    private bool _isCurrent;

    internal bool IsCurrent
    {
        get => _isCurrent;
        set => SetAndRaise(IsCurrentProperty, ref _isCurrent, value);
    }
    
    private bool _isClickable;

    internal bool IsClickable
    {
        get => _isClickable;
        set => SetAndRaise(IsClickableProperty, ref _isClickable, value);
    }
    
    private bool _isItemHover;

    internal bool IsItemHover
    {
        get => _isItemHover;
        set => SetAndRaise(IsItemHoverProperty, ref _isItemHover, value);
    }
    
    private bool _isShowProgress;

    internal bool IsShowProgress
    {
        get => _isShowProgress;
        set => SetAndRaise(IsShowProgressProperty, ref _isShowProgress, value);
    }
    
    private double _progressValue;

    internal double ProgressValue
    {
        get => _progressValue;
        set => SetAndRaise(ProgressValueProperty, ref _progressValue, value);
    }
    
    internal double ProgressLineThickness
    {
        get => GetValue(ProgressLineThicknessProperty);
        set => SetValue(ProgressLineThicknessProperty, value);
    }
    
    internal IBrush? ProgressGrooveColor
    {
        get => GetValue(ProgressGrooveColorProperty);
        set => SetValue(ProgressGrooveColorProperty, value);
    }
    
    internal IBrush? ProgressColor
    {
        get => GetValue(ProgressColorProperty);
        set => SetValue(ProgressColorProperty, value);
    }
    #endregion

    static StepsItemIndicator()
    {
        AffectsMeasure<StepsItemIndicator>(SizeTypeProperty, IndicatorTypeProperty, IsCurrentProperty,
            IsShowProgressProperty,
            StyleProperty);
    }
    
    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        SetCurrentValue(CornerRadiusProperty, new CornerRadius(e.NewSize.Width));
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IconProperty)
        {
            SetCurrentValue(IsCustomProperty, Icon != null);
        }
        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);    
            }
        }
    }
    
    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions =
                [
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty),
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty),
                    TransitionUtils.CreateTransition<DoubleTransition>(WidthProperty),
                    TransitionUtils.CreateTransition<DoubleTransition>(HeightProperty),
                ];
            }
        }
        else
        {
            Transitions = null;
        }
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

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (IsShowProgress && IsCurrent)
        {
            var progressRect = new Rect(DesiredSize.Deflate(Margin)).Deflate(ProgressLineThickness / 2);
            {
                var pen          = new Pen(ProgressGrooveColor, ProgressLineThickness);
                context.DrawEllipse(null, pen, progressRect);
            }

            {
                var pen = new Pen(ProgressColor, ProgressLineThickness)
                {
                    LineCap = PenLineCap.Round
                };

                double startAngle = -90;
                var indicatorAngle = 360 * ProgressValue / 100;
                context.DrawArc(pen, progressRect, startAngle, indicatorAngle);
            }
        }
    }
}