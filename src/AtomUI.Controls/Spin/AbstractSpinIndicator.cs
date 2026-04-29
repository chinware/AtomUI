using AtomUI.Animations;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Metadata;
using Avalonia.Styling;
using Avalonia.Threading;

namespace AtomUI.Controls.Commons;

public abstract class AbstractSpinIndicator : TemplatedControl, ISizeTypeAware
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<AbstractSpinIndicator>();

    public static readonly StyledProperty<object?> CustomIndicatorProperty =
        AbstractSpin.CustomIndicatorProperty.AddOwner<AbstractSpinIndicator>();

    public static readonly StyledProperty<IDataTemplate?> CustomIndicatorTemplateProperty =
        AbstractSpin.CustomIndicatorTemplateProperty.AddOwner<AbstractSpinIndicator>();

    public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<AbstractSpinIndicator>();

    public static readonly StyledProperty<Easing?> MotionEasingCurveProperty =
        AbstractSpin.MotionEasingCurveProperty.AddOwner<AbstractSpinIndicator>();

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    [DependsOn(nameof(CustomIndicatorTemplate))]
    public object? CustomIndicator
    {
        get => GetValue(CustomIndicatorProperty);
        set => SetValue(CustomIndicatorProperty, value);
    }
    
    public IDataTemplate? CustomIndicatorTemplate
    {
        get => GetValue(CustomIndicatorTemplateProperty);
        set => SetValue(CustomIndicatorTemplateProperty, value);
    }

    public TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }

    public Easing? MotionEasingCurve
    {
        get => GetValue(MotionEasingCurveProperty);
        set => SetValue(MotionEasingCurveProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<double> DotSizeProperty =
        AvaloniaProperty.Register<AbstractSpinIndicator, double>(
            nameof(DotSize));

    internal static readonly StyledProperty<IBrush?> DotBgBrushProperty =
        AvaloniaProperty.Register<AbstractSpinIndicator, IBrush?>(
            nameof(DotBgBrush));

    internal static readonly DirectProperty<AbstractSpinIndicator, double> IndicatorAngleProperty =
        AvaloniaProperty.RegisterDirect<AbstractSpinIndicator, double>(
            nameof(IndicatorAngle),
            o => o.IndicatorAngle,
            (o, v) => o.IndicatorAngle = v);
    
    internal static readonly StyledProperty<double> CustomIndicatorSizeProperty =
        AvaloniaProperty.Register<AbstractSpinIndicator, double>(nameof(CustomIndicatorSize), double.NaN);

    internal double DotSize
    {
        get => GetValue(DotSizeProperty);
        set => SetValue(DotSizeProperty, value);
    }

    internal IBrush? DotBgBrush
    {
        get => GetValue(DotBgBrushProperty);
        set => SetValue(DotBgBrushProperty, value);
    }

    private double _indicatorAngle;

    internal double IndicatorAngle
    {
        get => _indicatorAngle;
        set => SetAndRaise(IndicatorAngleProperty, ref _indicatorAngle, value);
    }
    
    internal double CustomIndicatorSize
    {
        get => GetValue(CustomIndicatorSizeProperty);
        set => SetValue(CustomIndicatorSizeProperty, value);
    }

    #endregion

    private Animation? _animation;
    private ContentPresenter? _customIndicatorPresenter;
    private CancellationTokenSource? _cancellationTokenSource;
    
    internal const double DOT_START_OPACITY = 0.3;

    static AbstractSpinIndicator()
    {
        AffectsMeasure<AbstractSpinIndicator>(SizeTypeProperty,
            CustomIndicatorProperty,
            CustomIndicatorTemplateProperty,
            CustomIndicatorSizeProperty);
        AffectsRender<AbstractSpinIndicator>(IndicatorAngleProperty);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        BuildIndicatorAnimation();
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
        if (_animation != null)
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await _animation.RunInfiniteAsync(this, _cancellationTokenSource.Token);
            });
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_customIndicatorPresenter != null)
        {
            _customIndicatorPresenter.PropertyChanged -= HandleIndicatorPresenterPropertyChanged;
        }

        _customIndicatorPresenter = e.NameScope.Find<ContentPresenter>("PART_CustomIndicatorPresenter");

        if (_customIndicatorPresenter != null)
        {
            _customIndicatorPresenter.PropertyChanged += HandleIndicatorPresenterPropertyChanged;
            UpdateCustomIndicatorSize();
        }
    }
    
    private void HandleIndicatorAngleChanged()
    {
        if (_customIndicatorPresenter is not null && _customIndicatorPresenter.IsVisible)
        {
            var builder = new TransformOperations.Builder(1);
            builder.AppendRotate(MathUtils.Deg2Rad(IndicatorAngle));
            _customIndicatorPresenter.RenderTransform = builder.Build();
            _customIndicatorPresenter.RenderTransformOrigin = RelativePoint.Center;
        }
    }

    private void BuildIndicatorAnimation(bool force = false)
    {
        if (force || _animation is null)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _animation = new Animation
            {
                Easing         = MotionEasingCurve ?? new LinearEasing(),
                Duration       = MotionDuration,
                Children =
                {
                    new KeyFrame
                    {
                        Setters = { new Setter(IndicatorAngleProperty, 0d) }, Cue = new Cue(0.0d)
                    },
                    new KeyFrame
                    {
                        Setters = { new Setter(IndicatorAngleProperty, 360d) }, Cue = new Cue(1.0d)
                    }
                }
            };
            _cancellationTokenSource = null;
        }
    }
    
    private static double GetOpacityForAngle(double degree)
    {
        var mappedValue = (Math.Sin(MathUtils.Deg2Rad(degree)) + 1) / 2; // 将正弦波的范围从[-1, 1]映射到[0, 1]
        return DOT_START_OPACITY + (1 - DOT_START_OPACITY) * mappedValue;
    }

    public override void Render(DrawingContext context)
    {
        if (CustomIndicator is null)
        {
            RenderBuiltInIndicator(context);
        }
    }

    private void RenderBuiltInIndicator(DrawingContext context)
    {
        var rightItemOpacity  = GetOpacityForAngle(_indicatorAngle);
        var bottomItemOpacity = GetOpacityForAngle(_indicatorAngle + 90);
        var leftItemOpacity   = GetOpacityForAngle(_indicatorAngle + 180);
        var topItemOpacity    = GetOpacityForAngle(_indicatorAngle + 270);
            
        var indicatorRect = new Rect(DesiredSize);
        var centerPoint   = indicatorRect.Center;

        var rightItemOffset =
            new Point(indicatorRect.Right - DotSize, centerPoint.Y - DotSize / 2);
        var bottomItemOffset =
            new Point(centerPoint.X - DotSize / 2, indicatorRect.Bottom - DotSize);
        var leftItemOffset = new Point(indicatorRect.Left, centerPoint.Y - DotSize / 2);
        var topItemOffset  = new Point(centerPoint.X - DotSize / 2, indicatorRect.Top);

        var matrix = Matrix.CreateTranslation(-indicatorRect.Center.X, -indicatorRect.Center.Y);
        matrix *= Matrix.CreateRotation(MathUtils.Deg2Rad(IndicatorAngle));
        matrix *= Matrix.CreateTranslation(indicatorRect.Center.X, indicatorRect.Center.Y);
        using var translateState = context.PushTransform(matrix);

        {
            using var opacityState = context.PushOpacity(rightItemOpacity);
            var       itemRect     = new Rect(rightItemOffset, new Size(DotSize, DotSize));
            context.DrawEllipse(DotBgBrush, null, itemRect);
        }
        {
            using var opacityState = context.PushOpacity(bottomItemOpacity);
            var       itemRect     = new Rect(bottomItemOffset, new Size(DotSize, DotSize));
            context.DrawEllipse(DotBgBrush, null, itemRect);
        }
        {
            using var opacityState = context.PushOpacity(leftItemOpacity);
            var       itemRect     = new Rect(leftItemOffset, new Size(DotSize, DotSize));
            context.DrawEllipse(DotBgBrush, null, itemRect);
        }
        {
            using var opacityState = context.PushOpacity(topItemOpacity);
            var       itemRect     = new Rect(topItemOffset, new Size(DotSize, DotSize));
            context.DrawEllipse(DotBgBrush, null, itemRect);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CustomIndicatorSizeProperty)
        {
            UpdateCustomIndicatorSize();
        }
        else if (change.Property == IndicatorAngleProperty)
        {
            HandleIndicatorAngleChanged();
        }
    }

    private void HandleIndicatorPresenterPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == ContentPresenter.ChildProperty)
        {
            UpdateCustomIndicatorSize();
        }
    }

    private void UpdateCustomIndicatorSize()
    {
        if (_customIndicatorPresenter?.Child is not PathIcon child)
        {
            return;
        }

        var size = CustomIndicatorSize;
        if (!double.IsNaN(size))
        {
            child.SetValue(WidthProperty, size);
            child.SetValue(HeightProperty, size);
        }
    }
}