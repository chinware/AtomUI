using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Media;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Styling;
using Avalonia.Threading;

namespace AtomUI.Controls;

public class Icon : PathIcon, ICustomHitTest, IMotionAwareControl
{
    protected override Type StyleKeyOverride { get; } = typeof(Icon);
    
    public static readonly StyledProperty<IconInfo?> IconInfoProperty =
        AvaloniaProperty.Register<Icon, IconInfo?>(nameof(IconInfo));

    public static readonly StyledProperty<IconAnimation> LoadingAnimationProperty =
        AvaloniaProperty.Register<Icon, IconAnimation>(
            nameof(LoadingAnimation), IconAnimation.None);

    // Fill 和 Outline 支持的颜色
    public static readonly StyledProperty<IBrush?> NormalFilledBrushProperty =
        AvaloniaProperty.Register<Icon, IBrush?>(
            nameof(NormalFilledBrush));

    public static readonly StyledProperty<IBrush?> ActiveFilledBrushProperty =
        AvaloniaProperty.Register<Icon, IBrush?>(
            nameof(ActiveFilledBrush));

    public static readonly StyledProperty<IBrush?> SelectedFilledBrushProperty =
        AvaloniaProperty.Register<Icon, IBrush?>(
            nameof(SelectedFilledBrush));

    public static readonly StyledProperty<IBrush?> DisabledFilledBrushProperty =
        AvaloniaProperty.Register<Icon, IBrush?>(
            nameof(DisabledFilledBrush));

    // TwoTone 类型的颜色
    public static readonly StyledProperty<IBrush?> PrimaryFilledBrushProperty =
        AvaloniaProperty.Register<Icon, IBrush?>(
            nameof(PrimaryFilledBrush));

    public static readonly StyledProperty<IBrush?> SecondaryFilledBrushProperty =
        AvaloniaProperty.Register<Icon, IBrush?>(
            nameof(SecondaryFilledBrush));

    public static readonly StyledProperty<TimeSpan> LoadingAnimationDurationProperty =
        AvaloniaProperty.Register<Icon, TimeSpan>(
            nameof(LoadingAnimationDuration), TimeSpan.FromSeconds(1));

    public static readonly StyledProperty<TimeSpan> FillAnimationDurationProperty =
        AvaloniaProperty.Register<Icon, TimeSpan>(
            nameof(FillAnimationDuration), TimeSpan.FromMilliseconds(200));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Icon>();

    public IconInfo? IconInfo
    {
        get => GetValue(IconInfoProperty);
        set => SetValue(IconInfoProperty, value);
    }

    public IBrush? NormalFilledBrush
    {
        get => GetValue(NormalFilledBrushProperty);
        set => SetValue(NormalFilledBrushProperty, value);
    }

    public IBrush? ActiveFilledBrush
    {
        get => GetValue(ActiveFilledBrushProperty);
        set => SetValue(ActiveFilledBrushProperty, value);
    }

    public IBrush? SelectedFilledBrush
    {
        get => GetValue(SelectedFilledBrushProperty);
        set => SetValue(SelectedFilledBrushProperty, value);
    }

    public IBrush? DisabledFilledBrush
    {
        get => GetValue(DisabledFilledBrushProperty);
        set => SetValue(DisabledFilledBrushProperty, value);
    }

    public IBrush? PrimaryFilledBrush
    {
        get => GetValue(PrimaryFilledBrushProperty);
        set => SetValue(PrimaryFilledBrushProperty, value);
    }

    public IBrush? SecondaryFilledBrush
    {
        get => GetValue(SecondaryFilledBrushProperty);
        set => SetValue(SecondaryFilledBrushProperty, value);
    }

    public TimeSpan LoadingAnimationDuration
    {
        get => GetValue(LoadingAnimationDurationProperty);
        set => SetValue(LoadingAnimationDurationProperty, value);
    }

    public TimeSpan FillAnimationDuration
    {
        get => GetValue(FillAnimationDurationProperty);
        set => SetValue(FillAnimationDurationProperty, value);
    }
    
    public IconThemeType ThemeType => IconInfo?.ThemeType ?? IconThemeType.Filled;

    public IconAnimation LoadingAnimation
    {
        get => GetValue(LoadingAnimationProperty);
        set => SetValue(LoadingAnimationProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #region 内部属性定义

    internal static readonly StyledProperty<double> AngleAnimationRotateProperty =
        AvaloniaProperty.Register<Icon, double>(
            nameof(AngleAnimationRotate));

    internal double AngleAnimationRotate
    {
        get => GetValue(AngleAnimationRotateProperty);
        set => SetValue(AngleAnimationRotateProperty, value);
    }

    #endregion

    Control IMotionAwareControl.PropertyBindTarget => this;

    private Animation? _animation;
    private CancellationTokenSource? _animationCancellationTokenSource;
    private readonly List<Matrix> _transforms;
    private readonly List<Geometry> _sourceGeometriesData;
    private Rect _viewBox;

    static Icon()
    {
        AffectsMeasure<Icon>(HeightProperty, WidthProperty, IconInfoProperty);
        AffectsRender<Icon>(ForegroundProperty,
            PrimaryFilledBrushProperty,
            SecondaryFilledBrushProperty);
    }

    public Icon()
    {
        _sourceGeometriesData = new List<Geometry>();
        _transforms           = new List<Matrix>();
    }

    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions = [
                    BaseTransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty, FillAnimationDuration)
                ];
            }
        }
        else
        {
            Transitions = null;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IconInfoProperty)
        {
            BuildSourceRenderData();
        }
        else if (change.Property == AngleAnimationRotateProperty)
        {
            SetCurrentValue(RenderTransformProperty, new RotateTransform(AngleAnimationRotate));
        }

        if (change.Property == LoadingAnimationProperty)
        {
            SetupRotateAnimation();
            if (_animation != null)
            {
                StartLoadingAnimation();
            }
        }
        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);
            }
        }

        if (change.Property == FillAnimationDurationProperty)
        {
            ConfigureTransitions(true);
        }
    }

    private void SetupRotateAnimation()
    {
        if (_animation is not null)
        {
            _animationCancellationTokenSource?.Cancel();
            _animation = null;
        }

        if (LoadingAnimation == IconAnimation.Spin || LoadingAnimation == IconAnimation.Pulse)
        {
            _animation = new Animation
            {
                Duration       = LoadingAnimationDuration,
                IterationCount = new IterationCount(ulong.MaxValue),
                Children =
                {
                    new KeyFrame
                    {
                        Cue     = new Cue(0d),
                        Setters = { new Setter(AngleAnimationRotateProperty, 0d) }
                    },
                    new KeyFrame
                    {
                        Cue     = new Cue(1d),
                        Setters = { new Setter(AngleAnimationRotateProperty, 360d) }
                    }
                }
            };
            if (LoadingAnimation == IconAnimation.Pulse)
            {
                _animation.Easing = new PulseEasing();
            }
        }
    }

    private void BuildSourceRenderData()
    {
        if (_sourceGeometriesData.Count > 0)
        {
            return;
        }

        if (IconInfo is null)
        {
            return;
        }
        
        foreach (var geometryData in IconInfo.Data)
        {
            _sourceGeometriesData.Add(Geometry.Parse(geometryData.PathData));
        }

        _viewBox = IconInfo!.ViewBox;
        // 先求最大的 bounds
        // 裁剪边距算法，暂时先注释掉
        Geometry? combined = null;

        for (var i = 0; i < _sourceGeometriesData.Count; i++)
        {
            var geometry  = _sourceGeometriesData[i];
            if (combined is null)
            {
                combined = geometry;
            }
            else
            {
                combined = new CombinedGeometry(combined, geometry);
            }
        }

        var combinedBounds = combined!.Bounds;

        var marginHorizontal = Math.Min(IconInfo.ViewBox.Right - combinedBounds.Right, combinedBounds.X);
        var marginVertical   = Math.Min(IconInfo.ViewBox.Bottom - combinedBounds.Bottom, combinedBounds.Y);
        var margin           = Math.Min(marginHorizontal, marginVertical);

        var scaleX = 1 - margin / _viewBox.Width;
        var scaleY = 1 - margin / _viewBox.Height;
        
        if (margin > 0)
        {
            _viewBox = combined.Bounds;
            for (var i = 0; i < _sourceGeometriesData.Count; i++)
            {
                var geometry = _sourceGeometriesData[i];
                var cloned   = geometry.Clone();
                var offsetX  = -margin / 2;
                var offsetY  = -margin / 2;
                var matrix   = BuildGeometryItemMatrix(i);
                matrix                   *= Matrix.CreateTranslation(offsetX, offsetY);
                matrix                   *= Matrix.CreateScale(scaleX, scaleY);
                cloned.Transform         =  new MatrixTransform(matrix);
                _sourceGeometriesData[i] =  cloned;
            }
        }
        else
        {
            for (var i = 0; i < _sourceGeometriesData.Count; i++)
            {
                var geometry = _sourceGeometriesData[i];
                var cloned   = geometry.Clone();
                cloned.Transform         = new MatrixTransform(BuildGeometryItemMatrix(i));
                _sourceGeometriesData[i] = cloned;
            }
        }
    }

    private Matrix BuildGeometryItemMatrix(int index)
    {
        Debug.Assert(IconInfo != null);
        var data = IconInfo.Data[index];
        if (string.IsNullOrEmpty(data.Transform))
        {
            return Matrix.Identity;
        }

        return TransformParser.Parse(data.Transform).Value;
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        SetupRotateAnimation();
        BuildSourceRenderData();
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (_animation is not null)
        {
            DispatcherTimer.RunOnce(() =>
            {
                Dispatcher.UIThread.InvokeAsync(StartLoadingAnimationAsync);
            }, TimeSpan.FromMilliseconds(200));
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _animationCancellationTokenSource?.Cancel();
        Transitions = null;
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

    public async Task StartLoadingAnimationAsync()
    {
        if (_animation is not null)
        {
            _animationCancellationTokenSource = new CancellationTokenSource();
            await _animation.RunAsync(this, _animationCancellationTokenSource.Token);
        }
    }

    public void StartLoadingAnimation()
    {
        Dispatcher.UIThread.InvokeAsync(StartLoadingAnimationAsync);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        base.MeasureOverride(availableSize);
        if (_sourceGeometriesData.Count == 0)
        {
            return default;
        }

        Size targetSize = default;
        for (var i = 0; i < _sourceGeometriesData.Count; i++)
        {
            var sourceGeometry = _sourceGeometriesData[i];
            var currentSize    = CalculateSizeAndTransform(availableSize, sourceGeometry.Bounds).size;
            targetSize = new Size(Math.Max(targetSize.Width, currentSize.Width),
                Math.Min(targetSize.Height, currentSize.Height));
        }

        return targetSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _transforms.Clear();
        // This should probably use GetRenderBounds(strokeThickness) but then the calculations
        // will multiply the stroke thickness as well, which isn't correct.
        for (var i = 0; i < _sourceGeometriesData.Count; i++)
        {
            var sourceGeometry = _sourceGeometriesData[i];
            var (_, transform) = CalculateSizeAndTransform(finalSize, sourceGeometry.Bounds);
            
            _transforms.Insert(i, transform);
        }

        return finalSize;
    }

    public override void Render(DrawingContext context)
    {
        if (IsVisible && _transforms.Count == _sourceGeometriesData.Count && _sourceGeometriesData.Count > 0 && DesiredSize != default)
        {
            for (var i = 0; i < _sourceGeometriesData.Count; i++)
            {
                var     renderedGeometry = _sourceGeometriesData[i];
                var     geometryData     = IconInfo!.Data[i];
                IBrush? fillBrush        = null;
                if (IconInfo.ThemeType == IconThemeType.TwoTone)
                {
                    if (geometryData.IsPrimary)
                    {
                        fillBrush = PrimaryFilledBrush;
                    }
                    else
                    {
                        fillBrush = SecondaryFilledBrush;
                    }
                }
                else
                {
                    fillBrush = Foreground;
                }
                using var state = context.PushTransform(_transforms[i]);
                context.DrawGeometry(fillBrush, null, renderedGeometry);
            }
        }
    }

    private (Size size, Matrix transform) CalculateSizeAndTransform(Size availableSize, Rect shapeBounds)
    {
        var shapeSize     = new Size(shapeBounds.Width, shapeBounds.Height);
        var desiredX      = availableSize.Width;
        var desiredY      = availableSize.Height;
        var sx            = 0.0;
        var sy            = 0.0;
        var viewBoxWidth  = _viewBox.Width;
        var viewBoxHeight = _viewBox.Height;

        // 计算大小的比例因子
        var shapeWidthScale  = shapeBounds.Width / viewBoxWidth;
        var shapeHeightScale = shapeBounds.Height / viewBoxHeight;

        // 计算位移的比例因子
        var offsetXScale = Math.Floor(availableSize.Width / viewBoxWidth);
        var offsetYScale = Math.Floor(availableSize.Height / viewBoxHeight);
        
        offsetXScale = offsetXScale > 1 ? 1 / offsetXScale : offsetXScale;
        offsetYScale = offsetYScale > 1 ? 1 / offsetYScale : offsetYScale;

        var offsetX = shapeBounds.X;
        var offsetY = shapeBounds.Y;

        shapeSize = shapeBounds.Size;

        if (double.IsInfinity(availableSize.Width))
        {
            desiredX = shapeSize.Width;
        }
        else
        {
            desiredX =  availableSize.Width * shapeWidthScale;
            offsetX  *= offsetXScale;
        }

        if (double.IsInfinity(availableSize.Height))
        {
            desiredY = shapeSize.Height;
        }
        else
        {
            desiredY =  availableSize.Height * shapeHeightScale;
            offsetY  *= offsetYScale;
        }

        var translate = Matrix.CreateTranslation(-offsetX, -offsetY);
        if (shapeBounds.Width > 0)
        {
            sx = desiredX / shapeSize.Width;
        }

        if (shapeBounds.Height > 0)
        {
            sy = desiredY / shapeSize.Height;
        }
        
        if (double.IsInfinity(availableSize.Width))
        {
            sx = sy;
        }

        if (double.IsInfinity(availableSize.Height))
        {
            sy = sx;
        }

        sx = sy = Math.Min(sx, sy);

        translate *= Matrix.CreateScale(sx, sy);
        var size = new Size(shapeSize.Width * sx, shapeSize.Height * sy);
        return (size, translate);
    }

    public bool HitTest(Point point)
    {
        return true;
    }
}