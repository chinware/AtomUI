using AtomUI.Controls.Badge;
using AtomUI.MotionScene;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class CountBadgeAdorner : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<IBrush?> BadgeColorProperty =
        AvaloniaProperty.Register<CountBadgeAdorner, IBrush?>(
            nameof(BadgeColor));
    
    internal static readonly StyledProperty<Point> OffsetProperty =
        AvaloniaProperty.Register<CountBadgeAdorner, Point>(
            nameof(Offset));

    public static readonly StyledProperty<int> OverflowCountProperty =
        AvaloniaProperty.Register<CountBadgeAdorner, int>(nameof(OverflowCount));

    public static readonly StyledProperty<CountBadgeSize> SizeProperty =
        AvaloniaProperty.Register<CountBadgeAdorner, CountBadgeSize>(
            nameof(Size));
    
    internal static readonly StyledProperty<int> CountProperty =
        AvaloniaProperty.Register<CountBadgeAdorner, int>(
            nameof(Count));
    
    internal static readonly StyledProperty<IBrush?> BadgeShadowColorProperty =
        AvaloniaProperty.Register<CountBadgeAdorner, IBrush?>(
            nameof(BadgeShadowColor));

    internal static readonly StyledProperty<double> BadgeShadowSizeProperty =
        AvaloniaProperty.Register<CountBadgeAdorner, double>(
            nameof(BadgeShadowSize));

    public IBrush? BadgeColor
    {
        get => GetValue(BadgeColorProperty);
        set => SetValue(BadgeColorProperty, value);
    }

    public Point Offset
    {
        get => GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    public int OverflowCount
    {
        get => GetValue(OverflowCountProperty);
        set => SetValue(OverflowCountProperty, value);
    }

    public CountBadgeSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }
    
    public int Count
    {
        get => GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }
    
    public IBrush? BadgeShadowColor
    {
        get => GetValue(BadgeShadowColorProperty);
        set => SetValue(BadgeShadowColorProperty, value);
    }

    public double BadgeShadowSize
    {
        get => GetValue(BadgeShadowSizeProperty);
        set => SetValue(BadgeShadowSizeProperty, value);
    }
    
    #endregion

    #region 内部属性定义
    
    internal static readonly DirectProperty<CountBadgeAdorner, bool> IsAdornerModeProperty =
        AvaloniaProperty.RegisterDirect<CountBadgeAdorner, bool>(
            nameof(IsAdornerMode),
            o => o.IsAdornerMode,
            (o, v) => o.IsAdornerMode = v);

    internal static readonly DirectProperty<CountBadgeAdorner, BoxShadows> BoxShadowProperty =
        AvaloniaProperty.RegisterDirect<CountBadgeAdorner, BoxShadows>(
            nameof(BoxShadow),
            o => o.BoxShadow,
            (o, v) => o.BoxShadow = v);
    
    internal static readonly DirectProperty<CountBadgeAdorner, string?> CountTextProperty =
        AvaloniaProperty.RegisterDirect<CountBadgeAdorner, string?>(
            nameof(CountText),
            o => o.CountText,
            (o, v) => o.CountText = v);
    
    internal static readonly DirectProperty<CountBadgeAdorner, TimeSpan> MotionDurationProperty =
        AvaloniaProperty.RegisterDirect<CountBadgeAdorner, TimeSpan>(
            nameof(MotionDuration),
            o => o.MotionDuration,
            (o, v) => o.MotionDuration = v);
    
    private bool _isAdornerMode;

    internal bool IsAdornerMode
    {
        get => _isAdornerMode;
        set => SetAndRaise(IsAdornerModeProperty, ref _isAdornerMode, value);
    }
    
    private BoxShadows _boxShadow;
    public BoxShadows BoxShadow
    {
        get => _boxShadow;
        set => SetAndRaise(BoxShadowProperty, ref _boxShadow, value);
    }
    
    private string? _countText;
    public string? CountText
    {
        get => _countText;
        set => SetAndRaise(CountTextProperty, ref _countText, value);
    }
    
    private TimeSpan _motionDuration;
    public TimeSpan MotionDuration
    {
        get => _motionDuration;
        set => SetAndRaise(MotionDurationProperty, ref _motionDuration, value);
    }

    #endregion
    
    private Panel? _rootLayout;
    private MotionActorControl? _indicatorMotionActor;
    private CancellationTokenSource? _motionCancellationTokenSource;
    private bool _needInitialHide;

    static CountBadgeAdorner()
    {
        AffectsMeasure<CountBadgeAdorner>(OverflowCountProperty,
            SizeProperty,
            CountProperty,
            IsAdornerModeProperty);
        AffectsRender<CountBadgeAdorner>(BadgeColorProperty, OffsetProperty);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        TokenResourceBinder.CreateTokenBinding(this, BadgeShadowSizeProperty, BadgeTokenResourceKey.BadgeShadowSize);
        TokenResourceBinder.CreateTokenBinding(this, BadgeShadowColorProperty, BadgeTokenResourceKey.BadgeShadowColor);
        TokenResourceBinder.CreateTokenBinding(this, MotionDurationProperty, GlobalTokenResourceKey.MotionDurationMid);
        TokenResourceBinder.CreateTokenBinding(this, BadgeColorProperty, BadgeTokenResourceKey.BadgeColor);
        _indicatorMotionActor = e.NameScope.Get<MotionActorControl>(CountBadgeAdornerTheme.IndicatorMotionActorPart);
        if (_needInitialHide)
        {
            _indicatorMotionActor.IsVisible = false;
            _needInitialHide                = false;
        }
        BuildBoxShadow();
        BuildCountText();
    }

    private void BuildBoxShadow()
    {
        if (BadgeShadowColor is not null)
        {
            BoxShadow = new BoxShadows(new BoxShadow
            {
                OffsetX = 0,
                OffsetY = 0,
                Blur    = 0,
                Spread  = BadgeShadowSize,
                Color   = ((SolidColorBrush)BadgeShadowColor).Color
            });
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (VisualRoot is not null)
        {
            if (e.Property == BadgeShadowSizeProperty ||
                e.Property == BadgeShadowColorProperty)
            {
                BuildBoxShadow();
            }

            if (e.Property == CountProperty || e.Property == OverflowCountProperty)
            {
                BuildCountText();
            }
        }
    }

    private void BuildCountText()
    {
        CountText = Count > OverflowCount ? $"{OverflowCount}+" : $"{Count}";
    }
    
    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (IsAdornerMode && _indicatorMotionActor is not null)
        {
            var offsetX = Offset.X;
            var offsetY = Offset.Y;
            var indicatorSize = _indicatorMotionActor.DesiredSize;
            offsetX += finalSize.Width - indicatorSize.Width / 2;
            offsetY -= indicatorSize.Height / 2;
            _indicatorMotionActor.Arrange(new Rect(new Point(offsetX, offsetY), indicatorSize));
        }
        return size;
    }
    
    private void ApplyShowMotion()
    {
        if (_indicatorMotionActor is not null)
        {
            _indicatorMotionActor.IsVisible = false;
            var motion = new BadgeZoomBadgeInMotion(MotionDuration);
            MotionInvoker.Invoke(_indicatorMotionActor, motion, () =>
            {
                _indicatorMotionActor.IsVisible = true;
            }, null, _motionCancellationTokenSource!.Token);
        }
    }
    
    private void ApplyHideMotion(Action completedAction)
    {
        if (_indicatorMotionActor is not null)
        {
            var motion = new BadgeZoomBadgeOutMotion(MotionDuration);
            _motionCancellationTokenSource?.Cancel();
            _motionCancellationTokenSource  = new CancellationTokenSource();
            
            MotionInvoker.Invoke(_indicatorMotionActor, motion, null, () =>
            {
                completedAction();
            }, _motionCancellationTokenSource.Token);
        }
        else
        {
            _needInitialHide = true;
        }
    }
    
    internal void ApplyToTarget(AdornerLayer? adornerLayer, Control adorned)
    {
        if (adornerLayer is not null)
        {
            adornerLayer.Children.Remove(this);
        
            AdornerLayer.SetAdornedElement(this, adorned);
            AdornerLayer.SetIsClipEnabled(this, false);
            adornerLayer.Children.Add(this);
        }
        
        _motionCancellationTokenSource?.Cancel();
        _motionCancellationTokenSource  = new CancellationTokenSource();
        
        ApplyShowMotion();
    }

    internal void DetachFromTarget(AdornerLayer? adornerLayer, bool enableMotion = true)
    {

        if (enableMotion)
        {
            ApplyHideMotion(() =>
            {
                if (adornerLayer is not null)
                {
                    adornerLayer.Children.Remove(this);   
                }
            });
        }
        else
        {
            if (adornerLayer is not null)
            {
                adornerLayer.Children.Remove(this);
            }
        }
    }
}