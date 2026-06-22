using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.Media;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.VisualTree;

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

    internal static readonly StyledProperty<double> IndicatorSizeProperty =
        AvaloniaProperty.Register<AbstractSpinIndicator, double>(nameof(IndicatorSize), double.NaN);

    internal static readonly StyledProperty<IBrush?> DotBgBrushProperty =
        AvaloniaProperty.Register<AbstractSpinIndicator, IBrush?>(nameof(DotBgBrush));

    internal static readonly StyledProperty<double> CustomIndicatorSizeProperty =
        AvaloniaProperty.Register<AbstractSpinIndicator, double>(nameof(CustomIndicatorSize), double.NaN);

    internal static readonly DirectProperty<AbstractSpinIndicator, bool> IsCustomIndicatorProperty =
        AvaloniaProperty.RegisterDirect<AbstractSpinIndicator, bool>(
            nameof(IsCustomIndicator),
            o => o.IsCustomIndicator,
            (o, v) => o.IsCustomIndicator = v);

    internal double IndicatorSize
    {
        get => GetValue(IndicatorSizeProperty);
        set => SetValue(IndicatorSizeProperty, value);
    }

    internal IBrush? DotBgBrush
    {
        get => GetValue(DotBgBrushProperty);
        set => SetValue(DotBgBrushProperty, value);
    }

    internal double CustomIndicatorSize
    {
        get => GetValue(CustomIndicatorSizeProperty);
        set => SetValue(CustomIndicatorSizeProperty, value);
    }

    private bool _isCustomIndicator;

    internal bool IsCustomIndicator
    {
        get => _isCustomIndicator;
        set => SetAndRaise(IsCustomIndicatorProperty, ref _isCustomIndicator, value);
    }

    #endregion

    private const double DOT_START_OPACITY     = 0.3;
    private const float FULL_ROTATION_RADIANS  = (float)(Math.PI * 2);
    private const string ROTATION_PROPERTY     = "RotationAngle";
    private const string OPACITY_PROPERTY      = "Opacity";

    private SpinIndicatorDotPanel? _builtInIndicatorLayout;
    private ContentPresenter? _customIndicatorPresenter;
    private Control? _animatedIndicatorTarget;

    static AbstractSpinIndicator()
    {
        AffectsMeasure<AbstractSpinIndicator>(SizeTypeProperty,
            CustomIndicatorProperty,
            CustomIndicatorTemplateProperty,
            IndicatorSizeProperty,
            CustomIndicatorSizeProperty);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        StartIndicatorAnimation();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ReleaseTemplateParts();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ReleaseTemplateParts();

        _builtInIndicatorLayout = e.NameScope.Find<SpinIndicatorDotPanel>("BuiltInIndicatorLayout");
        _customIndicatorPresenter = e.NameScope.Find<ContentPresenter>("PART_CustomIndicatorPresenter");

        if (_customIndicatorPresenter is not null)
        {
            _customIndicatorPresenter.PropertyChanged += HandleIndicatorPresenterPropertyChanged;
            UpdateCustomIndicatorSize();
        }

        SyncCustomIndicatorState();
        if (IsLoaded)
        {
            StartIndicatorAnimation();
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CustomIndicatorSizeProperty)
        {
            UpdateCustomIndicatorSize();
            UpdateAnimatedTargetCenterPoint();
        }
        else if (change.Property == IndicatorSizeProperty)
        {
            UpdateAnimatedTargetCenterPoint();
        }
        else if (change.Property == CustomIndicatorProperty ||
                 change.Property == CustomIndicatorTemplateProperty)
        {
            SyncCustomIndicatorState();
            RestartIndicatorAnimation();
        }
        else if (change.Property == IsVisibleProperty)
        {
            if (change.GetNewValue<bool>())
            {
                StartIndicatorAnimation();
            }
            else
            {
                StopIndicatorAnimation();
            }
        }
        else if (change.Property == MotionDurationProperty ||
                 change.Property == MotionEasingCurveProperty)
        {
            RestartIndicatorAnimation();
        }
    }

    private void SyncCustomIndicatorState()
    {
        SetCurrentValue(IsCustomIndicatorProperty, CustomIndicator is not null);
    }

    private void StartIndicatorAnimation()
    {
        if (!IsVisible || !IsLoaded || !this.IsAttachedToVisualTree())
        {
            return;
        }

        var target = GetActiveAnimationTarget();
        if (target is null)
        {
            return;
        }

        if (ReferenceEquals(_animatedIndicatorTarget, target))
        {
            StartTargetAnimation(target);
            return;
        }

        StopIndicatorAnimation();
        if (StartTargetAnimation(target))
        {
            _animatedIndicatorTarget = target;
        }
    }

    private void StopIndicatorAnimation()
    {
        if (_animatedIndicatorTarget is not null)
        {
            StopTargetAnimation(_animatedIndicatorTarget);
            _animatedIndicatorTarget = null;
            return;
        }

        StopTargetAnimation(_builtInIndicatorLayout);
        StopTargetAnimation(_customIndicatorPresenter);
    }

    private void RestartIndicatorAnimation()
    {
        StopIndicatorAnimation();
        StartIndicatorAnimation();
    }

    private Control? GetActiveAnimationTarget()
    {
        return IsCustomIndicator ? _customIndicatorPresenter : _builtInIndicatorLayout;
    }

    private bool StartTargetAnimation(Control target)
    {
        var visual = ElementComposition.GetElementVisual(target);
        if (visual?.Compositor is null)
        {
            return false;
        }

        UpdateTargetCenterPoint(target, visual);
        var easing = MotionEasingCurve ?? new LinearEasing();
        var rotationAnimation = visual.Compositor.CreateScalarKeyFrameAnimation();
        rotationAnimation.Target            = ROTATION_PROPERTY;
        rotationAnimation.Duration          = GetCompositionDuration();
        rotationAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
        rotationAnimation.StopBehavior      = AnimationStopBehavior.SetToInitialValue;
        rotationAnimation.InsertKeyFrame(0, 0, easing);
        rotationAnimation.InsertKeyFrame(1, FULL_ROTATION_RADIANS, easing);
        visual.StartAnimation(ROTATION_PROPERTY, rotationAnimation);

        if (ReferenceEquals(target, _builtInIndicatorLayout))
        {
            StartBuiltInDotOpacityAnimations(visual.Compositor);
        }

        return true;
    }

    private void StopTargetAnimation(Control? target)
    {
        if (target is null)
        {
            return;
        }

        var visual = ElementComposition.GetElementVisual(target);
        visual?.StopAnimation(ROTATION_PROPERTY);
        if (visual is not null)
        {
            visual.RotationAngle = 0;
        }

        if (ReferenceEquals(target, _builtInIndicatorLayout))
        {
            StopBuiltInDotOpacityAnimations();
            ResetBuiltInDotOpacities();
        }
    }

    private void UpdateAnimatedTargetCenterPoint()
    {
        if (_animatedIndicatorTarget is null)
        {
            return;
        }

        var visual = ElementComposition.GetElementVisual(_animatedIndicatorTarget);
        if (visual is not null)
        {
            UpdateTargetCenterPoint(_animatedIndicatorTarget, visual);
        }
    }

    private void UpdateTargetCenterPoint(Control target, CompositionVisual visual)
    {
        var size = GetAnimationTargetSize(target);
        visual.CenterPoint = new Vector3D(size.Width / 2, size.Height / 2, 0);
    }

    private Size GetAnimationTargetSize(Control target)
    {
        if (ReferenceEquals(target, _builtInIndicatorLayout) && !double.IsNaN(IndicatorSize))
        {
            return new Size(IndicatorSize, IndicatorSize);
        }

        if (ReferenceEquals(target, _customIndicatorPresenter) && !double.IsNaN(CustomIndicatorSize))
        {
            return new Size(CustomIndicatorSize, CustomIndicatorSize);
        }

        return target.Bounds.Size;
    }

    private TimeSpan GetCompositionDuration()
    {
        return MotionDuration < TimeSpan.FromMilliseconds(1)
            ? TimeSpan.FromMilliseconds(1)
            : MotionDuration;
    }

    private void StartBuiltInDotOpacityAnimations(Compositor compositor)
    {
        if (_builtInIndicatorLayout is null)
        {
            return;
        }

        var easing = MotionEasingCurve ?? new LinearEasing();
        var index = 0;
        foreach (var child in _builtInIndicatorLayout.Children)
        {
            if (child is not Control dot || index >= 4)
            {
                continue;
            }

            var dotVisual = ElementComposition.GetElementVisual(dot);
            if (dotVisual is null)
            {
                index++;
                continue;
            }

            var opacityAnimation = compositor.CreateScalarKeyFrameAnimation();
            opacityAnimation.Target            = OPACITY_PROPERTY;
            opacityAnimation.Duration          = GetCompositionDuration();
            opacityAnimation.DelayBehavior     = AnimationDelayBehavior.SetInitialValueBeforeDelay;
            opacityAnimation.DelayTime         = GetDotOpacityDelay(index);
            opacityAnimation.Direction         = PlaybackDirection.Alternate;
            opacityAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
            opacityAnimation.StopBehavior      = AnimationStopBehavior.SetToInitialValue;
            opacityAnimation.InsertKeyFrame(0, (float)DOT_START_OPACITY, easing);
            opacityAnimation.InsertKeyFrame(1, 1, easing);

            dotVisual.StartAnimation(OPACITY_PROPERTY, opacityAnimation);
            index++;
        }
    }

    private void StopBuiltInDotOpacityAnimations()
    {
        if (_builtInIndicatorLayout is null)
        {
            return;
        }

        foreach (var child in _builtInIndicatorLayout.Children)
        {
            if (child is Control dot)
            {
                ElementComposition.GetElementVisual(dot)?.StopAnimation(OPACITY_PROPERTY);
            }
        }
    }

    private void ResetBuiltInDotOpacities()
    {
        if (_builtInIndicatorLayout is null)
        {
            return;
        }

        var index = 0;
        foreach (var child in _builtInIndicatorLayout.Children)
        {
            if (child is Control dot && index < 4)
            {
                dot.Opacity = DOT_START_OPACITY;
                index++;
            }
        }
    }

    private TimeSpan GetDotOpacityDelay(int index)
    {
        return TimeSpan.FromTicks(GetCompositionDuration().Ticks * index / 3);
    }

    private void HandleIndicatorPresenterPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == ContentPresenter.ChildProperty)
        {
            UpdateCustomIndicatorSize();
        }
    }

    private void ReleaseTemplateParts()
    {
        StopIndicatorAnimation();
        if (_customIndicatorPresenter is not null)
        {
            _customIndicatorPresenter.PropertyChanged -= HandleIndicatorPresenterPropertyChanged;
        }

        _builtInIndicatorLayout   = null;
        _customIndicatorPresenter = null;
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
