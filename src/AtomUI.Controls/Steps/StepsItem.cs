using AtomUI.IconPkg;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace AtomUI.Controls;

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
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<StepsItem>();
    
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
    
    internal static readonly DirectProperty<StepsItem, bool> IsLastProperty =
        AvaloniaProperty.RegisterDirect<StepsItem, bool>(
            nameof(IsLast),
            o => o.IsLast,
            (o, v) => o.IsLast = v);
    
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
    
    private bool _isLast;

    internal bool IsLast
    {
        get => _isLast;
        set => SetAndRaise(IsLastProperty, ref _isLast, value);
    }
    #endregion
    
    static StepsItem()
    {
        SelectableMixin.Attach<StepsItem>(IsSelectedProperty);
        PressedMixin.Attach<StepsItem>();
        FocusableProperty.OverrideDefaultValue<StepsItem>(true);
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
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
    }
    
    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions =
                [
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
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StepsPseudoClass.Finished, IsFinished);
    }
}