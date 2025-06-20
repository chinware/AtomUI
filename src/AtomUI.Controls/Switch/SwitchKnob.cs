using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class SwitchKnob : Control,
                            IResourceBindingManager
{
    #region 公共属性定义
    
    public static readonly StyledProperty<bool> IsCheckedStateProperty
        = AvaloniaProperty.Register<SwitchKnob, bool>(nameof(IsCheckedState));
    
    public static readonly StyledProperty<IBrush?> KnobBackgroundColorProperty
        = AvaloniaProperty.Register<SwitchKnob, IBrush?>(nameof(KnobBackgroundColor));
        
    public static readonly StyledProperty<BoxShadow> KnobBoxShadowProperty
        = AvaloniaProperty.Register<SwitchKnob, BoxShadow>(nameof(KnobBoxShadow));
    
    public IBrush? KnobBackgroundColor
    {
        get => GetValue(KnobBackgroundColorProperty);
        set => SetValue(KnobBackgroundColorProperty, value);
    }

    public bool IsCheckedState
    {
        get => GetValue(IsCheckedStateProperty);
        set => SetValue(IsCheckedStateProperty, value);
    }

    public BoxShadow KnobBoxShadow
    {
        get => GetValue(KnobBoxShadowProperty);
        set => SetValue(KnobBoxShadowProperty, value);
    }

    #endregion
    
    #region 内部属性定义

    internal static readonly StyledProperty<int> RotationProperty
        = AvaloniaProperty.Register<SwitchKnob, int>(nameof(Rotation));
    
    internal static readonly StyledProperty<IBrush?> LoadIndicatorBrushProperty
        = AvaloniaProperty.Register<SwitchKnob, IBrush?>(nameof(LoadIndicatorBrush));
    
    internal static readonly StyledProperty<Size> KnobSizeProperty
        = AvaloniaProperty.Register<SwitchKnob, Size>(nameof(KnobSize));
    
    internal static readonly StyledProperty<double> KnobRenderWidthProperty
        = AvaloniaProperty.Register<SwitchKnob, double>(nameof(KnobRenderWidth));
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<SwitchKnob>();
    
    internal static readonly StyledProperty<TimeSpan> LoadingAnimationDurationProperty
        = AvaloniaProperty.Register<SwitchKnob, TimeSpan>(nameof(LoadingAnimationDuration));

    internal static readonly DirectProperty<SwitchKnob, double> LoadingBgOpacityTokenProperty
        = AvaloniaProperty.RegisterDirect<SwitchKnob, double>(nameof(LoadingBgOpacity),
            o => o.LoadingBgOpacity,
            (o, v) => o.LoadingBgOpacity = v);

    internal int Rotation
    {
        get => GetValue(RotationProperty);
        set => SetValue(RotationProperty, value);
    }
    
    internal IBrush? LoadIndicatorBrush
    {
        get => GetValue(LoadIndicatorBrushProperty);
        set => SetValue(LoadIndicatorBrushProperty, value);
    }
    
    internal Size KnobSize
    {
        get => GetValue(KnobSizeProperty);
        set => SetValue(KnobSizeProperty, value);
    }
    
    internal double KnobRenderWidth
    {
        get => GetValue(KnobRenderWidthProperty);
        set => SetValue(KnobRenderWidthProperty, value);
    }
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    internal TimeSpan LoadingAnimationDuration
    {
        get => GetValue(LoadingAnimationDurationProperty);
        set => SetValue(LoadingAnimationDurationProperty, value);
    }
    
    private double _loadingBgOpacity;
    internal double LoadingBgOpacity
    {
        get => _loadingBgOpacity;
        set => SetAndRaise(LoadingBgOpacityTokenProperty, ref _loadingBgOpacity, value);
    }
    
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable => _resourceBindingsDisposable;
    
    #endregion
    
    private CompositeDisposable? _resourceBindingsDisposable;
    private bool _isLoading;
    private CancellationTokenSource? _cancellationTokenSource;
    
    static SwitchKnob()
    {
        AffectsRender<SwitchKnob>(
            RotationProperty, KnobRenderWidthProperty);
        AffectsMeasure<SwitchKnob>(KnobSizeProperty);
    }
    
    public SwitchKnob()
    {
        LayoutUpdated += HandleLayoutUpdated;
        UseLayoutRounding = false;
    }

    private void HandleLayoutUpdated(object? sender, EventArgs args)
    {
        if (IsMotionEnabled)
        {
            Transitions ??= new Transitions
            {
                TransitionUtils.CreateTransition<DoubleTransition>(KnobRenderWidthProperty),
            };
        }
        else
        {
            Transitions = null;
        }
    }
    
    public void NotifyStartLoading()
    {
        if (_isLoading)
        {
            return;
        }

        _isLoading = true;
        IsEnabled  = false;
        if (this.IsAttachedToVisualTree())
        {
            StartLoadingAnimation();
        }
    }

    private void StartLoadingAnimation()
    {
        var loadingAnimation = new Animation();
        BindUtils.RelayBind(this, LoadingAnimationDurationProperty, loadingAnimation, Animation.DurationProperty);
        loadingAnimation.Duration       = LoadingAnimationDuration;
        loadingAnimation.IterationCount = IterationCount.Infinite;
        loadingAnimation.Easing         = new LinearEasing();
        loadingAnimation.Children.Add(new KeyFrame
        {
            Setters =
            {
                new Setter
                {
                    Property = RotationProperty,
                    Value    = 0
                }
            },
            KeyTime = TimeSpan.FromMilliseconds(0)
        });
        loadingAnimation.Children.Add(new KeyFrame
        {
            Setters =
            {
                new Setter
                {
                    Property = RotationProperty,
                    Value    = 360
                }
            },
            KeyTime = LoadingAnimationDuration
        });
        _cancellationTokenSource = new CancellationTokenSource();
        loadingAnimation.RunAsync(this, _cancellationTokenSource!.Token);
    }

    public void NotifyStopLoading()
    {
        if (!_isLoading)
        {
            return;
        }

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        _isLoading               = false;
        IsEnabled                = true;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return KnobSize;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == KnobSizeProperty)
        {
            KnobRenderWidth = KnobSize.Width;
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
        if (_isLoading)
        {
            StartLoadingAnimation();
        }
        Effect ??= new DropShadowEffect
        {
            OffsetX    = KnobBoxShadow.OffsetX,
            OffsetY    = KnobBoxShadow.OffsetY,
            Color      = KnobBoxShadow.Color,
            BlurRadius = KnobBoxShadow.Blur
        };
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, LoadingBgOpacityTokenProperty,
            ToggleSwitchTokenKey.SwitchDisabledOpacity));
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, LoadingAnimationDurationProperty,
            ToggleSwitchTokenKey.LoadingAnimationDuration));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_isLoading)
        {
            _cancellationTokenSource?.Cancel();
        }
        this.DisposeTokenBindings();
    }

    public sealed override void Render(DrawingContext context)
    {
        var offsetX = 0d;
        var offsetY = 0d;
        if (IsCheckedState)
        {
            offsetX = Bounds.Width - KnobRenderWidth;
        }

        var targetRect = new Rect(offsetX, offsetY, KnobRenderWidth, Bounds.Height);
        if (MathUtils.AreClose(KnobRenderWidth, DesiredSize.Height))
        {
            context.DrawEllipse(KnobBackgroundColor, null, targetRect);
        }
        else
        {
            context.DrawPilledRect(KnobBackgroundColor, null, targetRect);
        }

        if (_isLoading)
        {
            var delta           = 2.5;
            var loadingRectSize = targetRect.Size.Deflate(new Thickness(delta));
            var loadingRect = new Rect(new Point(-loadingRectSize.Width / 2, -loadingRectSize.Height / 2),
                loadingRectSize);
            var       pen                     = new Pen(LoadIndicatorBrush, 1, null, PenLineCap.Round);
            var       translateToCenterMatrix = Matrix.CreateTranslation(targetRect.Center.X, targetRect.Center.Y);
            var       rotationMatrix          = Matrix.CreateRotation(Rotation * Math.PI / 180);
            using var translateToCenterState  = context.PushTransform(translateToCenterMatrix);
            using var rotationMatrixState     = context.PushTransform(rotationMatrix);
            using var bgOpacity               = context.PushOpacity(_loadingBgOpacity);

            context.DrawArc(pen, loadingRect, 0, 90);
        }
    }
}