using AtomUI.Animations;
using AtomUI.Media;
using AtomUI.Reflection;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Commons;

[PseudoClasses(ProgressBarPseudoClass.Indeterminate, ProgressBarPseudoClass.Completed)]
public abstract class AbstractProgressBar : RangeBase,
                                            ISizeTypeAware,
                                            IMotionAwareControl
{
    protected const double LARGE_STROKE_THICKNESS = 8;
    protected const double MIDDLE_STROKE_THICKNESS = 6;
    protected const double SMALL_STROKE_THICKNESS = 4;

    #region 公共属性定义
    public static readonly StyledProperty<bool> IsIndeterminateProperty =
        AvaloniaProperty.Register<AbstractProgressBar, bool>(nameof(IsIndeterminate));
    
    public static readonly StyledProperty<bool> IsProgressInfoVisibleProperty =
        AvaloniaProperty.Register<AbstractProgressBar, bool>(nameof(IsProgressInfoVisible), true);
    
    public static readonly StyledProperty<string> ProgressTextFormatProperty =
        AvaloniaProperty.Register<AbstractProgressBar, string>(nameof(ProgressTextFormat), "{0:0}%");
    
    public static readonly DirectProperty<AbstractProgressBar, double> PercentageProperty =
        AvaloniaProperty.RegisterDirect<AbstractProgressBar, double>(
            nameof(Percentage),
            o => o.Percentage,
            (o, v) => o.Percentage = v);
    
    public static readonly StyledProperty<IBrush?> StrokeBrushProperty =
        AvaloniaProperty.Register<AbstractProgressBar, IBrush?>(nameof(StrokeBrush));

    public static readonly StyledProperty<Color?> TrailColorProperty =
        AvaloniaProperty.Register<AbstractProgressBar, Color?>(nameof(TrailColor));

    public static readonly StyledProperty<PenLineCap> StrokeLineCapProperty =
        AvaloniaProperty.Register<AbstractProgressBar, PenLineCap>(nameof(StrokeLineCap), PenLineCap.Round);

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<AbstractProgressBar>();

    public static readonly StyledProperty<ProgressStatus> StatusProperty =
        AvaloniaProperty.Register<AbstractProgressBar, ProgressStatus>(nameof(Status));

    public static readonly StyledProperty<double> IndicatorThicknessProperty =
        AvaloniaProperty.Register<AbstractProgressBar, double>(nameof(IndicatorThickness), double.NaN);

    public static readonly StyledProperty<double> SuccessThresholdProperty =
        AvaloniaProperty.Register<AbstractProgressBar, double>(nameof(SuccessThreshold), double.NaN);

    public static readonly StyledProperty<IBrush?> SuccessStrokeBrushProperty =
        AvaloniaProperty.Register<AbstractProgressBar, IBrush?>(nameof(SuccessStrokeBrush));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractProgressBar>();
    
    public static readonly StyledProperty<PathIcon?> ExceptionCompletedIconProperty =
        AvaloniaProperty.Register<AbstractProgressBar, PathIcon?>(nameof(ExceptionCompletedIcon));
    
    public static readonly StyledProperty<PathIcon?> SuccessCompletedIconProperty =
        AvaloniaProperty.Register<AbstractProgressBar, PathIcon?>(nameof(SuccessCompletedIcon));

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
    public bool IsProgressInfoVisible
    {
        get => GetValue(IsProgressInfoVisibleProperty);
        set => SetValue(IsProgressInfoVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets the format string applied to the internally calculated progress text before it is shown.
    /// </summary>
    public string ProgressTextFormat
    {
        get => GetValue(ProgressTextFormatProperty);
        set => SetValue(ProgressTextFormatProperty, value);
    }
    
    public IBrush? StrokeBrush
    {
        get => GetValue(StrokeBrushProperty);
        set => SetValue(StrokeBrushProperty, value);
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

    private double _percentage;

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

    public double IndicatorThickness
    {
        get => GetValue(IndicatorThicknessProperty);
        set => SetValue(IndicatorThicknessProperty, value);
    }

    public IBrush? SuccessStrokeBrush
    {
        get => GetValue(SuccessStrokeBrushProperty);
        set => SetValue(SuccessStrokeBrushProperty, value);
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
    
    public PathIcon? ExceptionCompletedIcon
    {
        get => GetValue(ExceptionCompletedIconProperty);
        set => SetValue(ExceptionCompletedIconProperty, value);
    }

    public PathIcon? SuccessCompletedIcon
    {
        get => GetValue(SuccessCompletedIconProperty);
        set => SetValue(SuccessCompletedIconProperty, value);
    }
    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<AbstractProgressBar, SizeType> EffectiveSizeTypeProperty =
        AvaloniaProperty.RegisterDirect<AbstractProgressBar, SizeType>(nameof(EffectiveSizeType),
            o => o.EffectiveSizeType,
            (o, v) => o.EffectiveSizeType = v);

    internal static readonly DirectProperty<AbstractProgressBar, double> StrokeThicknessProperty =
        AvaloniaProperty.RegisterDirect<AbstractProgressBar, double>(nameof(StrokeThickness),
            o => o.StrokeThickness,
            (o, v) => o.StrokeThickness = v);

    internal static readonly StyledProperty<IBrush?> GrooveBrushProperty =
        AvaloniaProperty.Register<AbstractProgressBar, IBrush?>(nameof(GrooveBrush));

    internal static readonly StyledProperty<bool> IsPercentLabelVisibleProperty =
        AvaloniaProperty.Register<AbstractProgressBar, bool>(nameof(IsPercentLabelVisible), true);

    internal static readonly StyledProperty<bool> IsStatusIconVisibleProperty =
        AvaloniaProperty.Register<AbstractProgressBar, bool>(nameof(IsStatusIconVisible), true);

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

    internal bool IsPercentLabelVisible
    {
        get => GetValue(IsPercentLabelVisibleProperty);
        set => SetValue(IsPercentLabelVisibleProperty, value);
    }

    internal bool IsStatusIconVisible
    {
        get => GetValue(IsStatusIconVisibleProperty);
        set => SetValue(IsStatusIconVisibleProperty, value);
    }

    internal bool IsCompleted
    {
        get => GetValue(IsCompletedProperty);
        set => SetValue(IsCompletedProperty, value);
    }

    #endregion

    protected LayoutTransformControl? LayoutTransformLabel;
    protected Label? PercentageLabel;
    protected Panel? ExtraInfoLayout;
    protected IconPresenter? SuccessCompletedIconPresenter;
    protected IconPresenter? ExceptionCompletedIconPresenter;
    private ProgressStatusIconKind _activeStatusIconKind;
    private bool _isUpdatingStatusIconPresenter;
    private string? _lastTextMeasureKey;
    private Size _lastTextMeasureSize;

    protected enum ProgressStatusIconKind
    {
        None,
        Success,
        Exception
    }

    static AbstractProgressBar()
    {
        AffectsMeasure<AbstractProgressBar>(EffectiveSizeTypeProperty,
            IsProgressInfoVisibleProperty,
            ProgressTextFormatProperty);
        AffectsRender<AbstractProgressBar>(StrokeBrushProperty,
            PercentageProperty,
            StrokeLineCapProperty,
            TrailColorProperty,
            StrokeThicknessProperty,
            SuccessStrokeBrushProperty,
            SuccessThresholdProperty);
        ValueProperty.OverrideMetadata<AbstractProgressBar>(
            new StyledPropertyMetadata<double>(defaultBindingMode: BindingMode.OneWay));
        SizeTypeProperty.OverrideDefaultValue<AbstractProgressBar>(SizeType.Large);
    }

    public AbstractProgressBar()
    {
        _effectiveSizeType = SizeType;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ValueProperty ||
            change.Property == MinimumProperty ||
            change.Property == MaximumProperty ||
            change.Property == IsIndeterminateProperty ||
            change.Property == ProgressTextFormatProperty)
        {
            UpdateProgress();
        }

        HandlePropertyChangedForStyle(change);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ReleaseStatusIconPresenters();
        base.OnApplyTemplate(e);
        ExtraInfoLayout      = e.NameScope.Find<Panel>("PART_ExtraInfoLayout");
        LayoutTransformLabel = e.NameScope.Find<LayoutTransformControl>("PART_LayoutTransformControl");
        PercentageLabel     = e.NameScope.Find<Label>("PART_PercentageLabel");
        // 创建完更新调用一次
        NotifyEffectSizeTypeChanged();
        UpdateProgress();
        UpdateStatusIconPresenter();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdatePseudoClasses();
    }
    
    protected abstract SizeType CalculateEffectiveSizeType(double size);

    protected abstract Rect GetProgressBarRect(Rect controlRect);
    protected abstract Rect GetExtraInfoRect(Rect controlRect);

    protected abstract void RenderGroove(DrawingContext context);
    protected abstract void RenderIndicatorBar(DrawingContext context);
    protected abstract void CalculateStrokeThickness();

    protected virtual void NotifyEffectSizeTypeChanged()
    {
        CalculateStrokeThickness();
    }

    private void UpdateProgress()
    {
        Percentage = CalculatePercentageValue(Value);
        IsCompleted = MathUtils.AreClose(Value, Maximum);
        UpdatePseudoClasses();
        NotifyUpdateProgress();
    }

    protected virtual void NotifyUpdateProgress()
    {
        if (IsProgressInfoVisible && PercentageLabel != null)
        {
            if (Status != ProgressStatus.Exception)
            {
                PercentageLabel.Content = FormatProgressText(Percentage);
            }

            NotifyHandleExtraInfoVisibility();
        }
    }

    protected virtual void NotifyHandleExtraInfoVisibility()
    {
    }

    private void HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == SizeTypeProperty)
        {
            EffectiveSizeType = change.GetNewValue<SizeType>();
        }
        else if (change.Property == IsCompletedProperty)
        {
            InvalidateMeasure();
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == WidthProperty || change.Property == HeightProperty)
            {
                NotifyHandleExtraInfoVisibility();
            }
            else if (change.Property == EffectiveSizeTypeProperty)
            {
                NotifyEffectSizeTypeChanged();
            }
        }
        NotifyPropertyChanged(change);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(ProgressBarPseudoClass.Indeterminate, IsIndeterminate);
        PseudoClasses.Set(ProgressBarPseudoClass.Completed, MathUtils.AreClose(Value, Maximum));
    }

    protected virtual void NotifyPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == TrailColorProperty)
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

        if (change.Property == StatusProperty ||
            change.Property == IsCompletedProperty ||
            change.Property == IsProgressInfoVisibleProperty ||
            change.Property == ExceptionCompletedIconProperty ||
            change.Property == SuccessCompletedIconProperty)
        {
            UpdateStatusIconPresenter();
        }
    }

    protected string FormatProgressText(double value)
    {
        return string.Format(ProgressTextFormat, value);
    }

    protected double CalculatePercentageValue(double value)
    {
        var range   = Maximum - Minimum;
        var percent = Math.Abs(range) < double.Epsilon ? 1.0 : (value - Minimum) / range;
        return percent * 100;
    }

    protected Size CalculateProgressTextSize(double value, double fontSize)
    {
        var text = FormatProgressText(value);
        var key  = $"{text}\u001F{fontSize}\u001F{FontFamily}\u001F{FontStyle}\u001F{FontWeight}";
        if (key == _lastTextMeasureKey)
        {
            return _lastTextMeasureSize;
        }

        _lastTextMeasureKey  = key;
        _lastTextMeasureSize = TextUtils.CalculateTextSize(text, fontSize, FontFamily, FontStyle, FontWeight);
        return _lastTextMeasureSize;
    }

    protected void UpdateStatusIconPresenter()
    {
        if (_isUpdatingStatusIconPresenter)
        {
            return;
        }
        if (ExtraInfoLayout is null)
        {
            return;
        }

        _isUpdatingStatusIconPresenter = true;
        try
        {
            var previousKind = _activeStatusIconKind;
            var targetKind   = GetTargetStatusIconKind();
            if (targetKind == ProgressStatusIconKind.None)
            {
                ReleaseStatusIconPresenters();
                if (previousKind != ProgressStatusIconKind.None)
                {
                    InvalidateMeasure();
                }
                return;
            }

            if (_activeStatusIconKind == targetKind)
            {
                EnsureStatusIconValue(targetKind);
                return;
            }

            ReleaseStatusIconPresenters();
            var presenter = new IconPresenter
            {
                Name = targetKind == ProgressStatusIconKind.Exception
                    ? "PART_ExceptionCompletedIconPresenter"
                    : "PART_SuccessCompletedIconPresenter"
            };
            presenter.SetTemplatedParent(this);
            ConfigureStatusIconPresenter(presenter, targetKind);
            if (targetKind == ProgressStatusIconKind.Exception)
            {
                ExceptionCompletedIconPresenter = presenter;
                presenter[!IconPresenter.IconProperty] = this[!ExceptionCompletedIconProperty];
            }
            else
            {
                SuccessCompletedIconPresenter = presenter;
                presenter[!IconPresenter.IconProperty] = this[!SuccessCompletedIconProperty];
            }

            _activeStatusIconKind = targetKind;
            EnsureStatusIconValue(targetKind);
            ExtraInfoLayout.Children.Add(presenter);
            NotifyStatusIconPresenterCreated(presenter);
            InvalidateMeasure();
        }
        finally
        {
            _isUpdatingStatusIconPresenter = false;
        }
    }

    protected virtual ProgressStatusIconKind GetTargetStatusIconKind()
    {
        if (!IsProgressInfoVisible)
        {
            return ProgressStatusIconKind.None;
        }

        if (Status == ProgressStatus.Exception)
        {
            return ProgressStatusIconKind.Exception;
        }

        if (Status == ProgressStatus.Success)
        {
            return IsCompleted ? ProgressStatusIconKind.None : ProgressStatusIconKind.Success;
        }

        return IsCompleted ? ProgressStatusIconKind.Success : ProgressStatusIconKind.None;
    }

    protected virtual void ConfigureStatusIconPresenter(IconPresenter presenter, ProgressStatusIconKind kind)
    {
    }

    protected virtual void NotifyStatusIconPresenterCreated(IconPresenter presenter)
    {
    }

    protected virtual PathIcon? BuildDefaultExceptionCompletedIcon() => null;

    protected virtual PathIcon? BuildDefaultSuccessCompletedIcon() => null;

    private void EnsureStatusIconValue(ProgressStatusIconKind kind)
    {
        if (kind == ProgressStatusIconKind.Exception && ExceptionCompletedIcon is null)
        {
            SetValue(ExceptionCompletedIconProperty, BuildDefaultExceptionCompletedIcon(), BindingPriority.Template);
        }
        else if (kind == ProgressStatusIconKind.Success && SuccessCompletedIcon is null)
        {
            SetValue(SuccessCompletedIconProperty, BuildDefaultSuccessCompletedIcon(), BindingPriority.Template);
        }
    }

    private void ReleaseStatusIconPresenters()
    {
        ReleaseStatusIconPresenter(ref ExceptionCompletedIconPresenter);
        ReleaseStatusIconPresenter(ref SuccessCompletedIconPresenter);
        _activeStatusIconKind = ProgressStatusIconKind.None;
    }

    private void ReleaseStatusIconPresenter(ref IconPresenter? presenter)
    {
        if (presenter is null)
        {
            return;
        }

        presenter.ClearValue(IconPresenter.IconProperty);
        presenter.ClearValue(IconPresenter.IconBrushProperty);
        if (presenter.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(presenter);
        }
        presenter.SetTemplatedParent(null);
        presenter = null;
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

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Dispatcher.Post(this.EnableTransitions);
    }
}
