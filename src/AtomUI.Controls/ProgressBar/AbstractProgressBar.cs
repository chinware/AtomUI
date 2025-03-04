using System.Reactive.Disposables;
using AtomUI.IconPkg;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;
using AnimationUtils = AtomUI.Controls.Utils.AnimationUtils;

namespace AtomUI.Controls;

public enum ProgressStatus
{
    Normal,
    Success,
    Exception,
    Active
}

[PseudoClasses(IndeterminatePC)]
public abstract class AbstractProgressBar : RangeBase,
                                            ISizeTypeAware,
                                            IAnimationAwareControl,
                                            IControlSharedTokenResourcesHost,
                                            ITokenResourceConsumer
{
    protected const double LARGE_STROKE_THICKNESS = 8;
    protected const double MIDDLE_STROKE_THICKNESS = 6;
    protected const double SMALL_STROKE_THICKNESS = 4;

    public const string IndeterminatePC = ":indeterminate";
    public const string CompletedPC = ":completed";

    #region 公共属性定义

    /// <summary>
    /// Defines the <see cref="IsIndeterminate" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsIndeterminateProperty =
        AvaloniaProperty.Register<AbstractProgressBar, bool>(nameof(IsIndeterminate));

    /// <summary>
    /// Defines the <see cref="ShowProgressInfo" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowProgressInfoProperty =
        AvaloniaProperty.Register<AbstractProgressBar, bool>(nameof(ShowProgressInfo), true);

    /// <summary>
    /// Defines the <see cref="ProgressTextFormat" /> property.
    /// </summary>
    public static readonly StyledProperty<string> ProgressTextFormatProperty =
        AvaloniaProperty.Register<AbstractProgressBar, string>(nameof(ProgressTextFormat), "{0:0}%");

    /// <summary>
    /// Defines the <see cref="Percentage" /> property.
    /// </summary>
    public static readonly DirectProperty<AbstractProgressBar, double> PercentageProperty =
        AvaloniaProperty.RegisterDirect<AbstractProgressBar, double>(
            nameof(Percentage),
            o => o.Percentage,
            (o, v) => o.Percentage = v);

    public static readonly StyledProperty<Color?> TrailColorProperty =
        AvaloniaProperty.Register<AbstractProgressBar, Color?>(nameof(TrailColor));

    public static readonly StyledProperty<PenLineCap> StrokeLineCapProperty =
        AvaloniaProperty.Register<AbstractProgressBar, PenLineCap>(nameof(StrokeLineCap), PenLineCap.Round);

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<AbstractProgressBar>();

    public static readonly StyledProperty<ProgressStatus> StatusProperty =
        AvaloniaProperty.Register<AbstractProgressBar, ProgressStatus>(nameof(Status));

    public static readonly StyledProperty<IBrush?> IndicatorBarBrushProperty =
        AvaloniaProperty.Register<AbstractProgressBar, IBrush?>(nameof(IndicatorBarBrush));

    public static readonly StyledProperty<double> IndicatorThicknessProperty =
        AvaloniaProperty.Register<AbstractProgressBar, double>(nameof(IndicatorThickness), double.NaN);

    public static readonly StyledProperty<double> SuccessThresholdProperty =
        AvaloniaProperty.Register<AbstractProgressBar, double>(nameof(SuccessThreshold), double.NaN);

    public static readonly StyledProperty<IBrush?> SuccessThresholdBrushProperty =
        AvaloniaProperty.Register<AbstractProgressBar, IBrush?>(nameof(SuccessThresholdBrush));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AnimationAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractProgressBar>();

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AnimationAwareControlProperty.IsWaveAnimationEnabledProperty.AddOwner<AbstractProgressBar>();

    /// <summary>
    /// Gets or sets a value indicating whether the progress bar shows the actual value or a generic,
    /// continues progress indicator (indeterminate state).
    /// </summary>
    public bool IsIndeterminate
    {
        get => GetValue(IsIndeterminateProperty);
        set => SetValue(IsIndeterminateProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether progress text will be shown.
    /// </summary>
    public bool ShowProgressInfo
    {
        get => GetValue(ShowProgressInfoProperty);
        set => SetValue(ShowProgressInfoProperty, value);
    }

    /// <summary>
    /// Gets or sets the format string applied to the internally calculated progress text before it is shown.
    /// </summary>
    public string ProgressTextFormat
    {
        get => GetValue(ProgressTextFormatProperty);
        set => SetValue(ProgressTextFormatProperty, value);
    }

    public Color? TrailColor
    {
        get => GetValue(TrailColorProperty);
        set => SetValue(TrailColorProperty, value);
    }

    public PenLineCap StrokeLineCap
    {
        get => GetValue(StrokeLineCapProperty);
        set => SetValue(StrokeLineCapProperty, value);
    }

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public ProgressStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    protected double _percentage;

    /// <summary>
    /// Gets the overall percentage complete of the progress
    /// </summary>
    /// <remarks>
    /// This read-only property is automatically calculated using the current <see cref="RangeBase.Value" /> and
    /// the effective range (<see cref="RangeBase.Maximum" /> - <see cref="RangeBase.Minimum" />).
    /// </remarks>
    public double Percentage
    {
        get => _percentage;
        private set => SetAndRaise(PercentageProperty, ref _percentage, value);
    }

    public IBrush? IndicatorBarBrush
    {
        get => GetValue(IndicatorBarBrushProperty);
        set => SetValue(IndicatorBarBrushProperty, value);
    }

    public double IndicatorThickness
    {
        get => GetValue(IndicatorThicknessProperty);
        set => SetValue(IndicatorThicknessProperty, value);
    }

    public IBrush? SuccessThresholdBrush
    {
        get => GetValue(SuccessThresholdBrushProperty);
        set => SetValue(SuccessThresholdBrushProperty, value);
    }

    public double SuccessThreshold
    {
        get => GetValue(SuccessThresholdProperty);
        set => SetValue(SuccessThresholdProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<AbstractProgressBar, SizeType> EffectiveSizeTypeProperty =
        AvaloniaProperty.RegisterDirect<AbstractProgressBar, SizeType>(nameof(EffectiveSizeType),
            o => o.EffectiveSizeType,
            (o, v) => o.EffectiveSizeType = v);

    protected static readonly DirectProperty<AbstractProgressBar, double> StrokeThicknessProperty =
        AvaloniaProperty.RegisterDirect<AbstractProgressBar, double>(nameof(StrokeThickness),
            o => o.StrokeThickness,
            (o, v) => o.StrokeThickness = v);

    internal static readonly StyledProperty<IBrush?> GrooveBrushProperty =
        AvaloniaProperty.Register<AbstractProgressBar, IBrush?>(nameof(GrooveBrush));

    internal static readonly StyledProperty<bool> PercentLabelVisibleProperty =
        AvaloniaProperty.Register<AbstractProgressBar, bool>(nameof(PercentLabelVisible), true);

    internal static readonly StyledProperty<bool> StatusIconVisibleProperty =
        AvaloniaProperty.Register<AbstractProgressBar, bool>(nameof(StatusIconVisible), true);

    internal static readonly StyledProperty<bool> IsCompletedProperty =
        AvaloniaProperty.Register<AbstractProgressBar, bool>(nameof(IsCompleted));

    private SizeType _effectiveSizeType;

    internal SizeType EffectiveSizeType
    {
        get => _effectiveSizeType;
        set => SetAndRaise(EffectiveSizeTypeProperty, ref _effectiveSizeType, value);
    }

    private double _strokeThickness;

    protected double StrokeThickness
    {
        get => _strokeThickness;
        set => SetAndRaise(StrokeThicknessProperty, ref _strokeThickness, value);
    }

    internal IBrush? GrooveBrush
    {
        get => GetValue(GrooveBrushProperty);
        set => SetValue(GrooveBrushProperty, value);
    }

    internal bool PercentLabelVisible
    {
        get => GetValue(PercentLabelVisibleProperty);
        set => SetValue(PercentLabelVisibleProperty, value);
    }

    internal bool StatusIconVisible
    {
        get => GetValue(StatusIconVisibleProperty);
        set => SetValue(StatusIconVisibleProperty, value);
    }

    internal bool IsCompleted
    {
        get => GetValue(IsCompletedProperty);
        set => SetValue(IsCompletedProperty, value);
    }

    Control IAnimationAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ProgressBarToken.ID;
    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;

    #endregion

    private CompositeDisposable? _tokenBindingsDisposable;
    protected LayoutTransformControl? _layoutTransformLabel;
    protected Label? _percentageLabel;
    protected Icon? _successCompletedIcon;
    protected Icon? _exceptionCompletedIcon;

    static AbstractProgressBar()
    {
        AffectsMeasure<AbstractProgressBar>(EffectiveSizeTypeProperty,
            ShowProgressInfoProperty,
            ProgressTextFormatProperty);
        AffectsRender<AbstractProgressBar>(IndicatorBarBrushProperty,
            StrokeLineCapProperty,
            TrailColorProperty,
            StrokeThicknessProperty,
            SuccessThresholdBrushProperty,
            SuccessThresholdProperty,
            ValueProperty);
        ValueProperty.OverrideMetadata<AbstractProgressBar>(
            new StyledPropertyMetadata<double>(defaultBindingMode: BindingMode.OneWay));
    }

    public AbstractProgressBar()
    {
        this.RegisterResources();
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
        _effectiveSizeType = SizeType;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == ValueProperty ||
            e.Property == MinimumProperty ||
            e.Property == MaximumProperty ||
            e.Property == IsIndeterminateProperty ||
            e.Property == ProgressTextFormatProperty)
        {
            UpdateProgress();
        }
        else if (e.Property == IsIndeterminateProperty)
        {
            UpdatePseudoClasses();
        }
        else if (e.Property == IsMotionEnabledProperty)
        {
            SetupTransitions();
        }

        HandlePropertyChangedForStyle(e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        NotifyTemplateApplied(e.NameScope);
        AfterUIStructureReady();
        SetupTransitions();
    }

    protected abstract SizeType CalculateEffectiveSizeType(double size);

    protected abstract Rect GetProgressBarRect(Rect controlRect);
    protected abstract Rect GetExtraInfoRect(Rect controlRect);

    protected abstract void RenderGroove(DrawingContext context);
    protected abstract void RenderIndicatorBar(DrawingContext context);
    protected abstract void CalculateStrokeThickness();

    protected virtual void NotifyTemplateApplied(INameScope scope)
    {
        _layoutTransformLabel = scope.Find<LayoutTransformControl>(AbstractProgressBarTheme.LayoutTransformControlPart);
        _percentageLabel = scope.Find<Label>(AbstractProgressBarTheme.PercentageLabelPart);
        _exceptionCompletedIcon = scope.Find<Icon>(AbstractProgressBarTheme.ExceptionCompletedIconPart);
        _successCompletedIcon = scope.Find<Icon>(AbstractProgressBarTheme.SuccessCompletedIconPart);
        NotifySetupUI();
    }

    protected virtual void NotifyEffectSizeTypeChanged()
    {
        CalculateStrokeThickness();
    }

    private void UpdateProgress()
    {
        var percent = Math.Abs(Maximum - Minimum) < double.Epsilon ? 1.0 : (Value - Minimum) / (Maximum - Minimum);
        Percentage = percent * 100;
        NotifyUpdateProgress();
    }

    protected virtual void NotifyUpdateProgress()
    {
        if (ShowProgressInfo && _percentageLabel != null)
        {
            if (Status != ProgressStatus.Exception)
            {
                _percentageLabel.Content = string.Format(ProgressTextFormat, _percentage);
            }

            NotifyHandleExtraInfoVisibility();
        }
    }

    protected virtual void NotifyHandleExtraInfoVisibility()
    {
    }

    private void AfterUIStructureReady()
    {
        NotifyUiStructureReady();
    }

    protected virtual void NotifyUiStructureReady()
    {
        // 创建完更新调用一次
        NotifyEffectSizeTypeChanged();
        UpdateProgress();
        UpdatePseudoClasses();
    }

    private void SetupTransitions()
    {
        if (IsMotionEnabled)
        {
            var transitions = new Transitions
            {
                AnimationUtils.CreateTransition<DoubleTransition>(ValueProperty,
                    SharedTokenKey.MotionDurationVerySlow, new ExponentialEaseOut()),
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(IndicatorBarBrushProperty,
                    SharedTokenKey.MotionDurationFast),
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty,
                    SharedTokenKey.MotionDurationFast)
            };

            NotifySetupTransitions(ref transitions);
            Transitions = transitions;
        }
        else
        {
            Transitions = null;
        }
    }

    private void SetupTokenBindings()
    {
        ApplyIndicatorBarBackgroundStyleConfig();
        NotifySetupTokenBindings();
    }

    private void HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == SizeTypeProperty)
        {
            EffectiveSizeType = e.GetNewValue<SizeType>();
        }

        if (this.IsAttachedToVisualTree())
        {
            if (e.Property == WidthProperty || e.Property == HeightProperty)
            {
                NotifyHandleExtraInfoVisibility();
            }
            else if (e.Property == EffectiveSizeTypeProperty)
            {
                NotifyEffectSizeTypeChanged();
            }
        }

        if (e.Property == ValueProperty)
        {
            IsCompleted = MathUtils.AreClose(Value, Maximum);
            UpdatePseudoClasses();
        }
        else if (e.Property == IsCompletedProperty)
        {
            InvalidateMeasure();
        }

        NotifyPropertyChanged(e);
    }

    protected virtual void NotifySetupTransitions(ref Transitions transitions)
    {
    }

    protected virtual void ApplyIndicatorBarBackgroundStyleConfig()
    {
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(IndeterminatePC, IsIndeterminate);
        PseudoClasses.Set(CompletedPC, MathUtils.AreClose(Value, Maximum));
    }

    protected virtual void NotifySetupUI()
    {
    }

    protected virtual void NotifySetupTokenBindings()
    {
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, SuccessThresholdBrushProperty,
            SharedTokenKey.ColorSuccess));
    }

    protected virtual void NotifyPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == TrailColorProperty)
        {
            if (TrailColor.HasValue)
            {
                GrooveBrush = new SolidColorBrush(TrailColor.Value);
            }
            else
            {
                ClearValue(GrooveBrushProperty);
            }
        }
    }

    public override void Render(DrawingContext context)
    {
        NotifyPrepareDrawingContext(context);
        RenderGroove(context);
        RenderIndicatorBar(context);
    }

    protected virtual void NotifyPrepareDrawingContext(DrawingContext context)
    {
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingsDisposable = new CompositeDisposable();
        SetupTokenBindings();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
}