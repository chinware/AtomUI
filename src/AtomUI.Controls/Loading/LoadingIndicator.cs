using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class LoadingIndicator : TemplatedControl,
                                IControlSharedTokenResourcesHost,
                                ISizeTypeAware
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<LoadingIndicator>();

    public static readonly StyledProperty<string?> LoadingMsgProperty =
        AvaloniaProperty.Register<LoadingIndicator, string?>(nameof(LoadingMsg));

    public static readonly StyledProperty<bool> IsShowLoadingMsgProperty =
        AvaloniaProperty.Register<LoadingIndicator, bool>(nameof(IsShowLoadingMsg));

    public static readonly StyledProperty<Icon?> CustomIndicatorIconProperty =
        AvaloniaProperty.Register<LoadingIndicator, Icon?>(nameof(CustomIndicatorIcon));

    public static readonly StyledProperty<TimeSpan?> MotionDurationProperty =
        AvaloniaProperty.Register<LoadingIndicator, TimeSpan?>(nameof(MotionDuration));

    public static readonly StyledProperty<Easing?> MotionEasingCurveProperty =
        AvaloniaProperty.Register<LoadingIndicator, Easing?>(nameof(MotionEasingCurve));

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public string? LoadingMsg
    {
        get => GetValue(LoadingMsgProperty);
        set => SetValue(LoadingMsgProperty, value);
    }

    public bool IsShowLoadingMsg
    {
        get => GetValue(IsShowLoadingMsgProperty);
        set => SetValue(IsShowLoadingMsgProperty, value);
    }

    public Icon? CustomIndicatorIcon
    {
        get => GetValue(CustomIndicatorIconProperty);
        set => SetValue(CustomIndicatorIconProperty, value);
    }

    public TimeSpan? MotionDuration
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
        AvaloniaProperty.Register<LoadingIndicator, double>(
            nameof(DotSize));

    internal static readonly StyledProperty<IBrush?> DotBgBrushProperty =
        AvaloniaProperty.Register<LoadingIndicator, IBrush?>(
            nameof(DotBgBrush));

    internal static readonly StyledProperty<double> IndicatorTextMarginProperty =
        AvaloniaProperty.Register<LoadingIndicator, double>(
            nameof(IndicatorTextMargin));

    private static readonly DirectProperty<LoadingIndicator, double> IndicatorAngleProperty =
        AvaloniaProperty.RegisterDirect<LoadingIndicator, double>(
            nameof(IndicatorAngle),
            o => o.IndicatorAngle,
            (o, v) => o.IndicatorAngle = v);

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

    internal double IndicatorTextMargin
    {
        get => GetValue(IndicatorTextMarginProperty);
        set => SetValue(IndicatorTextMarginProperty, value);
    }

    private double _indicatorAngle;

    private double IndicatorAngle
    {
        get => _indicatorAngle;
        set => SetAndRaise(IndicatorAngleProperty, ref _indicatorAngle, value);
    }
    
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => LoadingIndicatorToken.ID;

    #endregion

    private Animation? _animation;
    private SingleLineText? _loadingText;
    private Canvas? _mainContainer;
    private RenderInfo? _renderInfo;
    private CancellationTokenSource? _cancellationTokenSource;

    internal const double LARGE_INDICATOR_SIZE = 48;
    internal const double MIDDLE_INDICATOR_SIZE = 32;
    internal const double SMALL_INDICATOR_SIZE = 16;
    internal const double MAX_CONTENT_WIDTH = 120; // 拍脑袋的决定
    internal const double MAX_CONTENT_HEIGHT = 400;
    internal const double DOT_START_OPACITY = 0.3;

    static LoadingIndicator()
    {
        AffectsMeasure<LoadingIndicator>(SizeTypeProperty,
            LoadingMsgProperty,
            IsShowLoadingMsgProperty,
            CustomIndicatorIconProperty);
        AffectsRender<LoadingIndicator>(IndicatorAngleProperty);
    }

    public LoadingIndicator()
    {
        this.RegisterResources();
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        BuildIndicatorAnimation();
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        _animation?.RunAsync(this, _cancellationTokenSource.Token);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _cancellationTokenSource?.Cancel();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (this.IsAttachedToVisualTree())
        {
            if (e.Property == CustomIndicatorIconProperty)
            {
                var oldCustomIcon = e.GetOldValue<Icon?>();
                if (oldCustomIcon is not null)
                {
                    _mainContainer?.Children.Remove(oldCustomIcon);
                }

                SetupCustomIndicator();
            }
        }
      
        if (e.Property == IndicatorAngleProperty)
        {
            if (CustomIndicatorIcon is not null)
            {
                HandleIndicatorAngleChanged();
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        HandleTemplateApplied(e.NameScope);
    }

    private void HandleTemplateApplied(INameScope scope)
    {
        _mainContainer = scope.Find<Canvas>(LoadingIndicatorTheme.MainContainerPart);
        _loadingText   = scope.Find<SingleLineText>(LoadingIndicatorTheme.LoadingTextPart);

        SetupCustomIndicator();
    }

    // 只在使用自定义的 Icon 的时候有效
    private void HandleIndicatorAngleChanged()
    {
        if (CustomIndicatorIcon is not null)
        {
            var builder = new TransformOperations.Builder(1);
            builder.AppendRotate(MathUtils.Deg2Rad(IndicatorAngle));
            CustomIndicatorIcon.RenderTransform = builder.Build();
        }
    }

    private void SetupCustomIndicator()
    {
        if (CustomIndicatorIcon is not null)
        {
            CustomIndicatorIcon.SetTemplatedParent(this);
            _mainContainer?.Children.Insert(0, CustomIndicatorIcon);
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var targetWidth  = 0d;
        var targetHeight = 0d;
        base.MeasureOverride(new Size(Math.Min(availableSize.Width, MAX_CONTENT_WIDTH),
            Math.Min(availableSize.Height, MAX_CONTENT_HEIGHT)));
        if (IsShowLoadingMsg)
        {
            if (_loadingText is not null)
            {
                targetWidth  += _loadingText.DesiredSize.Width;
                targetHeight += _loadingText.DesiredSize.Height;
            }

            if (!string.IsNullOrEmpty(LoadingMsg))
            {
                targetHeight += GetLoadMsgPaddingTop();
            }
        }

        var indicatorSize = GetIndicatorSize(SizeType);
        targetWidth  =  Math.Max(indicatorSize, targetWidth);
        targetHeight += indicatorSize;

        return new Size(targetWidth, targetHeight);
    }

    private double GetLoadMsgPaddingTop()
    {
        return (DotSize - FontSize) / 2 + 2;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (IsShowLoadingMsg)
        {
            var msgRect = GetLoadingMsgRect();
            if (_loadingText is not null)
            {
                Canvas.SetLeft(_loadingText, msgRect.Left);
                Canvas.SetTop(_loadingText, msgRect.Top);
            }
        }

        if (CustomIndicatorIcon is not null)
        {
            var indicatorRect = GetIndicatorRect();
            Canvas.SetLeft(CustomIndicatorIcon, indicatorRect.Left);
            Canvas.SetTop(CustomIndicatorIcon, indicatorRect.Top);
        }

        return base.ArrangeOverride(finalSize);
    }

    private void BuildIndicatorAnimation(bool force = false)
    {
        if (force || _animation is null)
        {
            _cancellationTokenSource?.Cancel();
            _animation = new Animation
            {
                IterationCount = IterationCount.Infinite,
                Easing         = MotionEasingCurve ?? new LinearEasing(),
                Duration       = MotionDuration ?? TimeSpan.FromMilliseconds(300),
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

    private Rect GetIndicatorRect()
    {
        var indicatorSize = GetIndicatorSize(SizeType);
        var offsetX       = (DesiredSize.Width - indicatorSize) / 2;
        var offsetY       = (DesiredSize.Height - indicatorSize) / 2;
        if (IsShowLoadingMsg && LoadingMsg is not null)
        {
            offsetY -= _loadingText!.DesiredSize.Height / 2;
        }

        return new Rect(new Point(offsetX, offsetY), new Size(indicatorSize, indicatorSize));
    }

    private Rect GetLoadingMsgRect()
    {
        if (!IsShowLoadingMsg)
        {
            return default;
        }

        var indicatorRect = GetIndicatorRect();
        var offsetX       = indicatorRect.Left;
        var offsetY       = indicatorRect.Bottom;
        offsetX -= (_loadingText!.DesiredSize.Width - indicatorRect.Width) / 2;
        return new Rect(new Point(offsetX, offsetY), _loadingText.DesiredSize);
    }

    private static double GetIndicatorSize(SizeType sizeType)
    {
        return sizeType switch
        {
            SizeType.Small => SMALL_INDICATOR_SIZE,
            SizeType.Middle => MIDDLE_INDICATOR_SIZE,
            SizeType.Large => LARGE_INDICATOR_SIZE,
            _ => throw new ArgumentOutOfRangeException(nameof(sizeType), sizeType, "Invalid value for SizeType")
        };
    }

    private static double GetOpacityForAngle(double degree)
    {
        var mappedValue = (Math.Sin(MathUtils.Deg2Rad(degree)) + 1) / 2; // 将正弦波的范围从[-1, 1]映射到[0, 1]
        return DOT_START_OPACITY + (1 - DOT_START_OPACITY) * mappedValue;
    }

    public override void Render(DrawingContext context)
    {
        PrepareRenderInfo();
        if (CustomIndicatorIcon is null)
        {
            RenderBuiltInIndicator(context);
        }

        _renderInfo = null;
    }

    private void RenderBuiltInIndicator(DrawingContext context)
    {
        if (_renderInfo is not null)
        {
            var itemSize          = _renderInfo.IndicatorItemSize;
            var rightItemOpacity  = GetOpacityForAngle(_indicatorAngle);
            var bottomItemOpacity = GetOpacityForAngle(_indicatorAngle + 90);
            var leftItemOpacity   = GetOpacityForAngle(_indicatorAngle + 180);
            var topItemOpacity    = GetOpacityForAngle(_indicatorAngle + 270);

            var itemEdgeMargin = _renderInfo.ItemEdgeMargin;

            var indicatorRect = GetIndicatorRect();
            var centerPoint   = indicatorRect.Center;

            var rightItemOffset =
                new Point(indicatorRect.Right - itemEdgeMargin - itemSize, centerPoint.Y - itemSize / 2);
            var bottomItemOffset =
                new Point(centerPoint.X - itemSize / 2, indicatorRect.Bottom - itemEdgeMargin - itemSize);
            var leftItemOffset = new Point(indicatorRect.Left + itemEdgeMargin, centerPoint.Y - itemSize / 2);
            var topItemOffset  = new Point(centerPoint.X - itemSize / 2, indicatorRect.Top + itemEdgeMargin);

            var matrix = Matrix.CreateTranslation(-indicatorRect.Center.X, -indicatorRect.Center.Y);
            matrix *= Matrix.CreateRotation(MathUtils.Deg2Rad(IndicatorAngle));
            matrix *= Matrix.CreateTranslation(indicatorRect.Center.X, indicatorRect.Center.Y);
            using var translateState = context.PushTransform(matrix);

            {
                using var opacityState = context.PushOpacity(rightItemOpacity);
                var       itemRect     = new Rect(rightItemOffset, new Size(itemSize, itemSize));
                context.DrawEllipse(_renderInfo.DotBgBrush, null, itemRect);
            }
            {
                using var opacityState = context.PushOpacity(bottomItemOpacity);
                var       itemRect     = new Rect(bottomItemOffset, new Size(itemSize, itemSize));
                context.DrawEllipse(_renderInfo.DotBgBrush, null, itemRect);
            }
            {
                using var opacityState = context.PushOpacity(leftItemOpacity);
                var       itemRect     = new Rect(leftItemOffset, new Size(itemSize, itemSize));
                context.DrawEllipse(_renderInfo.DotBgBrush, null, itemRect);
            }
            {
                using var opacityState = context.PushOpacity(topItemOpacity);
                var       itemRect     = new Rect(topItemOffset, new Size(itemSize, itemSize));
                context.DrawEllipse(_renderInfo.DotBgBrush, null, itemRect);
            }
        }
    }

    private void PrepareRenderInfo()
    {
        _renderInfo         = new RenderInfo();
        _renderInfo.DotSize = DotSize;
        if (SizeType == SizeType.Large)
        {
            _renderInfo.IndicatorItemSize = (DotSize - IndicatorTextMargin) / 2.5;
        }
        else if (SizeType == SizeType.Middle)
        {
            _renderInfo.IndicatorItemSize = (DotSize - IndicatorTextMargin) / 2;
        }
        else
        {
            _renderInfo.IndicatorItemSize = (DotSize - IndicatorTextMargin) / 2;
        }

        _renderInfo.IndicatorItemSize *= 0.9;
        if (SizeType == SizeType.Large)
        {
            _renderInfo.ItemEdgeMargin = _renderInfo.IndicatorItemSize / 1.5;
        }
        else if (SizeType == SizeType.Middle)
        {
            _renderInfo.ItemEdgeMargin = _renderInfo.IndicatorItemSize / 1.8;
        }
        else
        {
            _renderInfo.ItemEdgeMargin = 0.5;
        }

        _renderInfo.DotBgBrush = DotBgBrush;
    }

    // 跟渲染相关的数据
    private class RenderInfo
    {
        public double DotSize { get; set; }
        public double IndicatorItemSize { get; set; }
        public double ItemEdgeMargin { get; set; }
        public IBrush? DotBgBrush { get; set; }
    }
}