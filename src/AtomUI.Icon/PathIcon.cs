using AtomUI.Media;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Reactive;
using Avalonia.Rendering;
using Avalonia.Styling;

namespace AtomUI.Icon;

public sealed class PathIcon : Control, ICustomHitTest
{
    public static readonly StyledProperty<string> KindProperty = AvaloniaProperty.Register<PathIcon, string>(
        nameof(Kind), string.Empty);

    public static readonly StyledProperty<IconAnimation> LoadingAnimationProperty =
        AvaloniaProperty.Register<PathIcon, IconAnimation>(
            nameof(LoadingAnimation));

    public static readonly StyledProperty<string?> PackageProviderProperty =
        AvaloniaProperty.Register<PathIcon, string?>(
            nameof(PackageProvider));

    // Fill 和 Outline 支持的颜色
    public static readonly StyledProperty<IBrush?> NormalFilledBrushProperty =
        AvaloniaProperty.Register<PathIcon, IBrush?>(
            nameof(NormalFilledBrush));

    public static readonly StyledProperty<IBrush?> ActiveFilledBrushProperty =
        AvaloniaProperty.Register<PathIcon, IBrush?>(
            nameof(ActiveFilledBrush));

    public static readonly StyledProperty<IBrush?> SelectedFilledBrushProperty =
        AvaloniaProperty.Register<PathIcon, IBrush?>(
            nameof(SelectedFilledBrush));

    public static readonly StyledProperty<IBrush?> DisabledFilledBrushProperty =
        AvaloniaProperty.Register<PathIcon, IBrush?>(
            nameof(DisabledFilledBrush));

    // TwoTone 类型的颜色
    public static readonly StyledProperty<IBrush?> PrimaryFilledBrushProperty =
        AvaloniaProperty.Register<PathIcon, IBrush?>(
            nameof(PrimaryFilledBrush));

    public static readonly StyledProperty<IBrush?> SecondaryFilledBrushProperty =
        AvaloniaProperty.Register<PathIcon, IBrush?>(
            nameof(SecondaryFilledBrush));

    public static readonly StyledProperty<TimeSpan> LoadingAnimationDurationProperty =
        AvaloniaProperty.Register<PathIcon, TimeSpan>(
            nameof(LoadingAnimationDuration), TimeSpan.FromSeconds(1));
    
    public static readonly StyledProperty<TimeSpan> FillAnimationDurationProperty =
        AvaloniaProperty.Register<PathIcon, TimeSpan>(
            nameof(FillAnimationDuration), TimeSpan.FromSeconds(300));

    public static readonly StyledProperty<IconMode> IconModeProperty =
        AvaloniaProperty.Register<PathIcon, IconMode>(nameof(IconMode));

    public string Kind
    {
        get => GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public string? PackageProvider
    {
        get => GetValue(PackageProviderProperty);
        set => SetValue(PackageProviderProperty, value);
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

    /// <summary>
    /// PathIcon 的模式，只对 Outlined 和 Filled 类型有效
    /// </summary>
    public IconMode IconMode
    {
        get => GetValue(IconModeProperty);
        set => SetValue(IconModeProperty, value);
    }

    private static readonly StyledProperty<IBrush?> FilledBrushProperty
        = AvaloniaProperty.Register<ToggleSwitch, IBrush?>(
            nameof(IBrush));

    /// <summary>
    /// 当是非 TwoTone icon 的时候，填充色是支持渐变的
    /// </summary>
    private IBrush? FilledBrush
    {
        get => GetValue(FilledBrushProperty);
        set => SetValue(FilledBrushProperty, value);
    }

    public IconThemeType ThemeType => _iconInfo?.ThemeType ?? IconThemeType.Filled;

    public IconAnimation LoadingAnimation
    {
        get => GetValue(LoadingAnimationProperty);
        set => SetValue(LoadingAnimationProperty, value);
    }

    #region 内部属性定义

    internal static readonly StyledProperty<double> AngleAnimationRotateProperty =
        AvaloniaProperty.Register<PathIcon, double>(
            nameof(AngleAnimationRotate));

    internal double AngleAnimationRotate
    {
        get => GetValue(AngleAnimationRotateProperty);
        set => SetValue(AngleAnimationRotateProperty, value);
    }

    #endregion

    private Animation? _animation;
    private CancellationTokenSource? _animationCancellationTokenSource;
    private WeakReference<IIconPackageProvider>? _iconPackageRef;
    private readonly List<Matrix> _transforms;
    private readonly List<Geometry> _sourceGeometriesData;
    private IconInfo? _iconInfo;
    private Rect _viewBox;

    static PathIcon()
    {
        AffectsGeometry(KindProperty, PackageProviderProperty);
        AffectsMeasure<PathIcon>(HeightProperty, WidthProperty);
        AffectsRender<PathIcon>(IconModeProperty,
            FilledBrushProperty,
            PrimaryFilledBrushProperty,
            SecondaryFilledBrushProperty);
        HorizontalAlignmentProperty.OverrideDefaultValue<PathIcon>(HorizontalAlignment.Left);
        VerticalAlignmentProperty.OverrideDefaultValue<PathIcon>(VerticalAlignment.Center);
    }

    public PathIcon()
    {
        var rotateTransform = new RotateTransform();
        RenderTransform       = rotateTransform;
        _sourceGeometriesData = new List<Geometry>();
        _transforms           = new List<Matrix>();
    }

    private void SetupTransitions()
    {
        Transitions ??= new Transitions()
        {
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(FilledBrushProperty, FillAnimationDuration)
        };
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == KindProperty)
        {
            // TODO 这里不存在记录日志吗？暂时构造一个默认的
            if (VisualRoot is not null)
            {
                BuildSourceRenderData();
            }
        }
        else if (change.Property == IsEnabledProperty)
        {
            // TODO 这个地方需要优化一点，是否需要保存老的，当状态为 Enabled 的时候进行还原
            if (!IsEnabled)
            {
                IconMode = IconMode.Disabled;
            }
        }
        else if (change.Property == NormalFilledBrushProperty ||
                 change.Property == ActiveFilledBrushProperty ||
                 change.Property == SelectedFilledBrushProperty ||
                 change.Property == DisabledFilledBrushProperty ||
                 change.Property == PrimaryFilledBrushProperty ||
                 change.Property == SecondaryFilledBrushProperty ||
                 change.Property == IconModeProperty)
        {
            SetupFilledBrush();
        }
        else if (change.Property == AngleAnimationRotateProperty)
        {
            SetCurrentValue(RenderTransformProperty, new RotateTransform(AngleAnimationRotate));
        }

        if (VisualRoot is not null)
        {
            if (change.Property == LoadingAnimationProperty)
            {
                SetupRotateAnimation();
            }
        }
    }

    private void SetupRotateAnimation()
    {
        if (_animation is not null)
        {
            _animationCancellationTokenSource?.Cancel();
            _animation                        = null;
            _animationCancellationTokenSource = null;
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

            if (VisualRoot is not null)
            {
                _animationCancellationTokenSource = new CancellationTokenSource();
                _animation.RunAsync(this, _animationCancellationTokenSource.Token);
            }
        }
    }

    private void SetupFilledBrush()
    {
        var colorInfo = _iconInfo?.ColorInfo;
        if (IconMode == IconMode.Normal)
        {
            if (NormalFilledBrush is not null)
            {
                FilledBrush = NormalFilledBrush;
            }
            else if (colorInfo.HasValue)
            {
                FilledBrush = new SolidColorBrush(colorInfo.Value.NormalColor);
            }
        }
        else if (IconMode == IconMode.Active)
        {
            if (ActiveFilledBrush is not null)
            {
                FilledBrush = ActiveFilledBrush;
            }
            else if (NormalFilledBrush is not null)
            {
                FilledBrush = NormalFilledBrush;
            }
            else if (colorInfo.HasValue)
            {
                FilledBrush = new SolidColorBrush(colorInfo.Value.ActiveColor);
            }
        }
        else if (IconMode == IconMode.Selected)
        {
            if (SelectedFilledBrush is not null)
            {
                FilledBrush = SelectedFilledBrush;
            }
            else if (NormalFilledBrush is not null)
            {
                FilledBrush = NormalFilledBrush;
            }
            else if (colorInfo.HasValue)
            {
                FilledBrush = new SolidColorBrush(colorInfo.Value.SelectedColor);
            }
        }
        else
        {
            if (DisabledFilledBrush is not null)
            {
                FilledBrush = DisabledFilledBrush;
            }
            else if (NormalFilledBrush is not null)
            {
                FilledBrush = NormalFilledBrush;
            }
            else if (colorInfo.HasValue)
            {
                FilledBrush = new SolidColorBrush(colorInfo.Value.DisabledColor);
            }
        }
    }

    /// <summary>
    /// Invalidates the geometry of this shape.
    /// </summary>
    private void InvalidateGeometry()
    {
        if (_animation is not null)
        {
            _animationCancellationTokenSource?.Cancel();
        }

        _sourceGeometriesData.Clear();
        _transforms.Clear();
        _iconInfo = null;
        InvalidateMeasure();
    }

    /// <summary>
    /// Marks a property as affecting the shape's geometry.
    /// </summary>
    /// <param name="properties">The properties.</param>
    /// <remarks>
    /// After a call to this method in a control's static constructor, any change to the
    /// property will cause <see cref="InvalidateGeometry" /> to be called on the element.
    /// </remarks>
    private static void AffectsGeometry(params AvaloniaProperty[] properties)
    {
        foreach (var property in properties)
        {
            property.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(e =>
            {
                if (e.Sender is PathIcon icon)
                {
                    AffectsGeometryInvalidate(icon, e);
                }
            }));
        }
    }

    private static void AffectsGeometryInvalidate(PathIcon control, AvaloniaPropertyChangedEventArgs e)
    {
        // If the geometry is invalidated when Bounds changes, only invalidate when the Size
        // portion changes.
        if (e.Property == BoundsProperty)
        {
            var oldBounds = (Rect)e.OldValue!;
            var newBounds = (Rect)e.NewValue!;
            if (oldBounds.Size == newBounds.Size)
            {
                return;
            }
        }

        control.InvalidateGeometry();
    }

    private void BuildSourceRenderData()
    {
        if (_sourceGeometriesData.Count > 0)
        {
            return;
        }

        var manager = IconManager.Current;
        PackageProvider ??= manager.DefaultPackage;

        var iconPackage = manager.GetIconProvider(PackageProvider);
        // 这里报错还是？
        if (iconPackage is not null)
        {
            _iconPackageRef = new WeakReference<IIconPackageProvider>(iconPackage);
        }

        if (_iconPackageRef != null && _iconPackageRef.TryGetTarget(out var iconPackageProvider))
        {
            // TODO 这里可能需要优化，针对 IconInfo 的拷贝问题
            _iconInfo = iconPackageProvider.GetIcon(Kind) ?? new IconInfo();
            foreach (var geometryData in _iconInfo.Data)
            {
                _sourceGeometriesData.Add(Geometry.Parse(geometryData.PathData));
            }

            _viewBox = _iconInfo!.ViewBox;
            // 先求最大的 bounds
            // 裁剪边距算法，暂时先注释掉
            Geometry? combined = null;
            foreach (var geometry in _sourceGeometriesData)
            {
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

            var marginHorizontal = Math.Min(_iconInfo.ViewBox.Right - combinedBounds.Right, combinedBounds.X);
            var marginVertical   = Math.Min(_iconInfo.ViewBox.Bottom - combinedBounds.Bottom, combinedBounds.Y);
            var margin           = Math.Min(marginHorizontal, marginVertical);

            var scaleX = 1 - margin / _viewBox.Width;
            var scaleY = 1 - margin / _viewBox.Height;

            if (margin > 0)
            {
                for (var i = 0; i < _sourceGeometriesData.Count; i++)
                {
                    var geometry = _sourceGeometriesData[i];
                    var cloned   = geometry.Clone();
                    var offsetX  = -margin / 2;
                    var offsetY  = -margin / 2;
                    var matrix   = Matrix.CreateTranslation(offsetX, offsetY);
                    matrix                   = matrix * Matrix.CreateScale(scaleX, scaleY);
                    cloned.Transform         = new MatrixTransform(matrix);
                    _sourceGeometriesData[i] = cloned;
                }

                _viewBox = combined!.Bounds;
            }
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        SetupTransitions();
        SetupRotateAnimation();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_sourceGeometriesData.Count == 0)
        {
            BuildSourceRenderData();
            SetupFilledBrush();
        }
        if (_animation is not null && _animationCancellationTokenSource is null)
        {
            _animationCancellationTokenSource = new CancellationTokenSource();
            _animation.RunAsync(this, _animationCancellationTokenSource.Token);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _animationCancellationTokenSource?.Cancel();
        _animationCancellationTokenSource = null;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
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
        if (IsVisible && 
            _sourceGeometriesData.Count > 0 &&
            Bounds.Width > 0 &&
            Bounds.Width > 0)
        {
            for (var i = 0; i < _sourceGeometriesData.Count; i++)
            {
                var     renderedGeometry = _sourceGeometriesData[i];
                var     geometryData     = _iconInfo!.Data[i];
                IBrush? fillBrush        = null;
                if (_iconInfo.ThemeType == IconThemeType.TwoTone)
                {
                    var colorInfo = _iconInfo.TwoToneColorInfo;
                    if (colorInfo.HasValue)
                    {
                        if (geometryData.IsPrimary)
                        {
                            if (PrimaryFilledBrush is not null)
                            {
                                fillBrush = PrimaryFilledBrush;
                            }
                            else
                            {
                                fillBrush = new SolidColorBrush(colorInfo.Value.PrimaryColor);
                            }
                        }
                        else
                        {
                            if (SecondaryFilledBrush is not null)
                            {
                                fillBrush = SecondaryFilledBrush;
                            }
                            else
                            {
                                fillBrush = new SolidColorBrush(colorInfo.Value.SecondaryColor);
                            }
                        }
                    }
                }
                else
                {
                    fillBrush = FilledBrush;
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

        translate = translate * Matrix.CreateScale(sx, sy);
        var size = new Size(shapeSize.Width * sx, shapeSize.Height * sy);
        return (size, translate);
    }

    public bool HitTest(Point point)
    {
        var targetRect = new Rect(0, 0, DesiredSize.Width, DesiredSize.Height);
        return targetRect.Contains(point);
    }
}