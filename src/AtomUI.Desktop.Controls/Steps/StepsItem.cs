using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Themes;
using AtomUI.Desktop.Controls.Utils;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[PseudoClasses(StepsPseudoClass.Finished)]
public class StepsItem : HeaderedContentControl, ISelectable
{
    #region 公共属性定义
    
    public static readonly StyledProperty<object?> SubHeaderProperty =
        AvaloniaProperty.Register<StepsItem, object?>(nameof(SubHeader));
    
    public static readonly StyledProperty<IDataTemplate?> SubHeaderTemplateProperty =
        AvaloniaProperty.Register<StepsItem, IDataTemplate?>(nameof(SubHeaderTemplate));
    
    public static readonly StyledProperty<object?> DescriptionProperty =
        AvaloniaProperty.Register<StepsItem, object?>(nameof(Description));
    
    public static readonly StyledProperty<IDataTemplate?> DescriptionTemplateProperty =
        AvaloniaProperty.Register<StepsItem, IDataTemplate?>(nameof(DescriptionTemplate));

    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<StepsItem>();
    
    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<StepsItem, Icon?>(nameof(Icon));
    
    public static readonly StyledProperty<StepsItemStatus> StatusProperty =
        AvaloniaProperty.Register<StepsItem, StepsItemStatus>(nameof(Status), StepsItemStatus.Process);
    
    public static readonly StyledProperty<Orientation> LabelPlacementProperty =
        Steps.LabelPlacementProperty.AddOwner<StepsItem>();
    
    public object? SubHeader
    {
        get => GetValue(SubHeaderProperty);
        set => SetValue(SubHeaderProperty, value);
    }
    
    public IDataTemplate? SubHeaderTemplate
    {
        get => GetValue(SubHeaderTemplateProperty);
        set => SetValue(SubHeaderTemplateProperty, value);
    }
    
    public object? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
    
    public IDataTemplate? DescriptionTemplate
    {
        get => GetValue(DescriptionTemplateProperty);
        set => SetValue(DescriptionTemplateProperty, value);
    }
    
    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }
    
    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public StepsItemStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
    
    public Orientation LabelPlacement
    {
        get => GetValue(LabelPlacementProperty);
        set => SetValue(LabelPlacementProperty, value);
    }
    
    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<StepsItem>();
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<StepsItem>();
    
    internal static readonly StyledProperty<bool> IsClickableProperty =
        AvaloniaProperty.Register<StepsItem, bool>(nameof(IsClickable), false);
    
    internal static readonly StyledProperty<StepsStyle> StyleProperty =
        Steps.StyleProperty.AddOwner<StepsItem>();
    
    internal static readonly StyledProperty<StepsItemIndicatorType> IndicatorTypeProperty =
        AvaloniaProperty.Register<StepsItem, StepsItemIndicatorType>(nameof(IndicatorType), StepsItemIndicatorType.Default);
    
    internal static readonly DirectProperty<StepsItem, bool> IsFinishedProperty =
        AvaloniaProperty.RegisterDirect<StepsItem, bool>(
            nameof(IsFinished),
            o => o.IsFinished,
            (o, v) => o.IsFinished = v);
    
    internal static readonly DirectProperty<StepsItem, int> PositionProperty =
        AvaloniaProperty.RegisterDirect<StepsItem, int>(
            nameof(Position),
            o => o.Position,
            (o, v) => o.Position = v);
    
    internal static readonly DirectProperty<StepsItem, bool> IsFirstProperty =
        AvaloniaProperty.RegisterDirect<StepsItem, bool>(
            nameof(IsFirst),
            o => o.IsFirst,
            (o, v) => o.IsFirst = v);
    
    internal static readonly DirectProperty<StepsItem, bool> IsLastProperty =
        AvaloniaProperty.RegisterDirect<StepsItem, bool>(
            nameof(IsLast),
            o => o.IsLast,
            (o, v) => o.IsLast = v);
    
    internal static readonly DirectProperty<StepsItem, bool> IsShowProgressProperty =
        AvaloniaProperty.RegisterDirect<StepsItem, bool>(
            nameof(IsShowProgress),
            o => o.IsShowProgress,
            (o, v) => o.IsShowProgress = v);
    
    internal static readonly DirectProperty<StepsItem, bool> IsEffectiveShowProgressProperty =
        AvaloniaProperty.RegisterDirect<StepsItem, bool>(
            nameof(IsEffectiveShowProgress),
            o => o.IsEffectiveShowProgress,
            (o, v) => o.IsEffectiveShowProgress = v);
    
    internal static readonly DirectProperty<StepsItem, double> ProgressValueProperty =
        AvaloniaProperty.RegisterDirect<StepsItem, double>(
            nameof(ProgressValue),
            o => o.ProgressValue,
            (o, v) => o.ProgressValue = v);
    
    internal static readonly StyledProperty<Orientation> OrientationProperty =
        ScrollBar.OrientationProperty.AddOwner<StepsItem>();
    
    internal static readonly StyledProperty<IBrush?> SubTitleForegroundProperty =
        AvaloniaProperty.Register<StepsItem, IBrush?>(nameof(SubTitleForeground));
    
    internal static readonly StyledProperty<IBrush?> DescriptionForegroundProperty =
        AvaloniaProperty.Register<StepsItem, IBrush?>(nameof(DescriptionForeground));
    
    internal static readonly StyledProperty<IBrush?> NavIndicatorLineColorProperty =
        AvaloniaProperty.Register<StepsItem, IBrush?>(nameof(NavIndicatorLineColor));
    
    internal static readonly StyledProperty<ITransform?> NavIndicatorLineRenderTransformProperty = 
        AvaloniaProperty.Register<StepsItem, ITransform?>(nameof (NavIndicatorLineRenderTransform));
    
    internal SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    internal bool IsClickable
    {
        get => GetValue(IsClickableProperty);
        set => SetValue(IsClickableProperty, value);
    }
    
    internal StepsStyle Style
    {
        get => GetValue(StyleProperty);
        set => SetValue(StyleProperty, value);
    }

    internal StepsItemIndicatorType IndicatorType
    {
        get => GetValue(IndicatorTypeProperty);
        set => SetValue(IndicatorTypeProperty, value);
    }

    private bool _isFinished;

    internal bool IsFinished
    {
        get => _isFinished;
        set => SetAndRaise(IsFinishedProperty, ref _isFinished, value);
    }
    
    private int _position;

    internal int Position
    {
        get => _position;
        set => SetAndRaise(PositionProperty, ref _position, value);
    }
    
    private bool _isFirst;

    internal bool IsFirst
    {
        get => _isFirst;
        set => SetAndRaise(IsFirstProperty, ref _isFirst, value);
    }
    
    private bool _isLast;

    internal bool IsLast
    {
        get => _isLast;
        set => SetAndRaise(IsLastProperty, ref _isLast, value);
    }
    
    internal Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
    
    internal IBrush? SubTitleForeground
    {
        get => GetValue(SubTitleForegroundProperty);
        set => SetValue(SubTitleForegroundProperty, value);
    }
    
    internal IBrush? DescriptionForeground
    {
        get => GetValue(DescriptionForegroundProperty);
        set => SetValue(DescriptionForegroundProperty, value);
    }
    
    internal IBrush? NavIndicatorLineColor
    {
        get => GetValue(NavIndicatorLineColorProperty);
        set => SetValue(NavIndicatorLineColorProperty, value);
    }
    
    internal ITransform? NavIndicatorLineRenderTransform
    {
        get => GetValue(NavIndicatorLineRenderTransformProperty);
        set => SetValue(NavIndicatorLineRenderTransformProperty, value);
    }
    
    private bool _isShowProgress;

    internal bool IsShowProgress
    {
        get => _isShowProgress;
        set => SetAndRaise(IsShowProgressProperty, ref _isShowProgress, value);
    }
    
    private bool _isEffectiveShowProgress;

    internal bool IsEffectiveShowProgress
    {
        get => _isEffectiveShowProgress;
        set => SetAndRaise(IsEffectiveShowProgressProperty, ref _isEffectiveShowProgress, value);
    }
    
    private double _progressValue;

    internal double ProgressValue
    {
        get => _progressValue;
        set => SetAndRaise(ProgressValueProperty, ref _progressValue, value);
    }
    #endregion
    
    private StepsItemIndicator? _indicator;
    
    static StepsItem()
    {
        SelectableMixin.Attach<StepsItem>(IsSelectedProperty);
        PressedMixin.Attach<StepsItem>();
        FocusableProperty.OverrideDefaultValue<StepsItem>(true);
        AffectsRender<StepsItem>(SubTitleForegroundProperty, DescriptionForegroundProperty);
        AffectsMeasure<StepsItem>(OrientationProperty, StyleProperty, SizeTypeProperty, OrientationProperty);
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _indicator = e.NameScope.Find<StepsItemIndicator>(StepsItemThemeConstants.IndicatorPart);
        UpdatePseudoClasses();
        ConfigureEffectiveShowProgress();
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);    
            }
        }

        if (change.Property == IsFinishedProperty)
        {
            UpdatePseudoClasses();
        }
        else if (change.Property == IsSelectedProperty)
        {
            if (_indicator != null)
            {
                _indicator.IsItemHover = false;
            }
        }
        else if (change.Property == IsShowProgressProperty ||
                 change.Property == StyleProperty ||
                 change.Property == IndicatorTypeProperty)
        {
            ConfigureEffectiveShowProgress();
        }
    }

    private void ConfigureEffectiveShowProgress()
    {
        SetCurrentValue(IsEffectiveShowProgressProperty, IsShowProgress && Style != StepsStyle.Inline && Icon == null &&IndicatorType != StepsItemIndicatorType.Dot);
    }
    
    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions =
                [
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty),
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(SubTitleForegroundProperty),
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(DescriptionForegroundProperty),
                    TransitionUtils.CreateTransition<TransformOperationsTransition>(NavIndicatorLineRenderTransformProperty, SharedTokenKey.MotionDurationMid, new CubicEaseOut())
                ];
            }
        }
        else
        {
            Transitions = null;
        }
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

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        if (_indicator != null && !IsSelected)
        {
            _indicator.IsItemHover = true;
        }
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        if (_indicator != null && !IsSelected)
        {
            _indicator.IsItemHover = false;
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StepsPseudoClass.Finished, IsFinished);
    }
}