using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls.Switch;

internal class SwitchKnob : Control
{
    #region 公共属性定义

    public static readonly StyledProperty<Size> KnobSizeProperty
        = AvaloniaProperty.Register<SwitchKnob, Size>(nameof(KnobSize));
    
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
    
    public Size KnobSize
    {
        get => GetValue(KnobSizeProperty);
        set => SetValue(KnobSizeProperty, value);
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

    internal static readonly StyledProperty<Size> OriginKnobSizeProperty
        = AvaloniaProperty.Register<SwitchKnob, Size>(nameof(OriginKnobSize));
    
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
    
    internal Size OriginKnobSize
    {
        get => GetValue(OriginKnobSizeProperty);
        set => SetValue(OriginKnobSizeProperty, value);
    }
    
    #endregion
    
    private bool _initialized;
    private bool _isLoading;
    private CancellationTokenSource? _cancellationTokenSource;

    private double _loadingBgOpacity;

    private static readonly DirectProperty<SwitchKnob, double> LoadingBgOpacityTokenProperty
        = AvaloniaProperty.RegisterDirect<SwitchKnob, double>(nameof(_loadingBgOpacity),
            o => o._loadingBgOpacity,
            (o, v) => o._loadingBgOpacity = v);

    // TODO 这个属性可以考虑放出去
    internal static readonly StyledProperty<TimeSpan> LoadingAnimationDurationProperty
        = AvaloniaProperty.Register<SwitchKnob, TimeSpan>(nameof(LoadingAnimationDuration),
            TimeSpan.FromMilliseconds(300));

    internal TimeSpan LoadingAnimationDuration
    {
        get => GetValue(LoadingAnimationDurationProperty);
        set => SetValue(LoadingAnimationDurationProperty, value);
    }
    
    static SwitchKnob()
    {
        AffectsMeasure<SwitchKnob>(KnobSizeProperty);
        AffectsRender<SwitchKnob>(
            RotationProperty);
    }

    public sealed override void ApplyTemplate()
    {
        base.ApplyTemplate();
        if (!_initialized)
        {
            Effect = new DropShadowEffect
            {
                OffsetX    = KnobBoxShadow.OffsetX,
                OffsetY    = KnobBoxShadow.OffsetY,
                Color      = KnobBoxShadow.Color,
                BlurRadius = KnobBoxShadow.Blur
            };
            SetupTokenBindings();
            SetupTransitions();
            _initialized = true;
        }
    }

    private void SetupTransitions()
    {
        var transitions = new Transitions();
        transitions.Add(AnimationUtils.CreateTransition<SizeTransition>(KnobSizeProperty));
        Transitions = transitions;
    }

    public void NotifyStartLoading()
    {
        if (_isLoading)
        {
            return;
        }

        _isLoading = true;
        IsEnabled  = false;
        if (VisualRoot != null)
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

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_isLoading)
        {
            StartLoadingAnimation();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_isLoading)
        {
            _cancellationTokenSource?.Cancel();
        }
    }

    private void SetupTokenBindings()
    {
        TokenResourceBinder.CreateTokenBinding(this, LoadingBgOpacityTokenProperty,
            ToggleSwitchTokenResourceKey.SwitchDisabledOpacity);
        LoadingAnimationDuration = TimeSpan.FromMilliseconds(1200);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return KnobSize;
    }

    public sealed override void Render(DrawingContext context)
    {
        var targetRect = new Rect(new Point(0, 0), KnobSize);
        if (MathUtils.AreClose(KnobSize.Width, KnobSize.Height))
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