using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public enum StepsItemIndicatorType
{
    Default,
    Dot
}

public enum StepsItemStatus
{
    Wait,
    Process,
    Finish,
    Error
}

public enum StepsStyle
{
    Default,
    Navigation,
    Inline
}

[PseudoClasses(StdPseudoClass.Vertical, StdPseudoClass.Horizontal)]
public class Steps : SelectingItemsControl,
                     ISizeTypeAware,
                     IMotionAwareControl,
                     IControlSharedTokenResourcesHost
{
    #region 公共属性定义
    
    public static readonly StyledProperty<int> CurrentStepProperty =
        AvaloniaProperty.Register<Steps, int>(nameof(CurrentStep), 0);
    
    public static readonly StyledProperty<int> InitialStepProperty =
        AvaloniaProperty.Register<Steps, int>(nameof(InitialStep), -1);
    
    public static readonly StyledProperty<double> PercentValueProperty =
        AvaloniaProperty.Register<Steps, double>(nameof(PercentValue), 0);
    
    public static readonly StyledProperty<StepsItemStatus> CurrentStepStatusProperty =
        AvaloniaProperty.Register<Steps, StepsItemStatus>(nameof(CurrentStepStatus), StepsItemStatus.Process);
    
    public static readonly StyledProperty<Orientation> OrientationProperty =
        ScrollBar.OrientationProperty.AddOwner<Steps>();
    
    public static readonly StyledProperty<Orientation> LabelPlacementProperty =
        AvaloniaProperty.Register<Steps, Orientation>(nameof(LabelPlacement), Orientation.Horizontal);

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<Steps>();
    
    public static readonly StyledProperty<StepsItemIndicatorType> ItemIndicatorTypeProperty =
        AvaloniaProperty.Register<Steps, StepsItemIndicatorType>(nameof(ItemIndicatorType), StepsItemIndicatorType.Default);
    
    public static readonly StyledProperty<StepsStyle> StyleProperty =
        AvaloniaProperty.Register<Steps, StepsStyle>(nameof(Style), StepsStyle.Default);
        
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Steps>();
    
    public static readonly StyledProperty<bool> IsItemClickableProperty =
        AvaloniaProperty.Register<Steps, bool>(nameof(IsItemClickable), false);
    
    public int CurrentStep
    {
        get => GetValue(CurrentStepProperty);
        set => SetValue(CurrentStepProperty, value);
    }
    
    public int InitialStep
    {
        get => GetValue(InitialStepProperty);
        set => SetValue(InitialStepProperty, value);
    }
    
    public double PercentValue
    {
        get => GetValue(PercentValueProperty);
        set => SetValue(PercentValueProperty, value);
    }
    
    public StepsItemStatus CurrentStepStatus
    {
        get => GetValue(CurrentStepStatusProperty);
        set => SetValue(CurrentStepStatusProperty, value);
    }
    
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
    
    public Orientation LabelPlacement
    {
        get => GetValue(LabelPlacementProperty);
        set => SetValue(LabelPlacementProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
        
    public StepsItemIndicatorType ItemIndicatorType
    {
        get => GetValue(ItemIndicatorTypeProperty);
        set => SetValue(ItemIndicatorTypeProperty, value);
    }
    
    public StepsStyle Style
    {
        get => GetValue(StyleProperty);
        set => SetValue(StyleProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public bool IsItemClickable
    {
        get => GetValue(IsItemClickableProperty);
        set => SetValue(IsItemClickableProperty, value);
    }

    #endregion
    
    #region 内部属性定义
    
    internal static readonly StyledProperty<double> HorizontalItemSpacingProperty =
        AvaloniaProperty.Register<Steps, double>(nameof(HorizontalItemSpacing));
    
    internal static readonly StyledProperty<double> VerticalItemSpacingProperty =
        AvaloniaProperty.Register<Steps, double>(nameof(VerticalItemSpacing));
    
    internal double HorizontalItemSpacing
    {
        get => GetValue(HorizontalItemSpacingProperty);
        set => SetValue(HorizontalItemSpacingProperty, value);
    }
    
    internal double VerticalItemSpacing
    {
        get => GetValue(VerticalItemSpacingProperty);
        set => SetValue(VerticalItemSpacingProperty, value);
    }

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => StepsToken.ID;

    #endregion

    private readonly Dictionary<StepsItem, CompositeDisposable> _itemsBindingDisposables = new();
    private Grid? _grid;
    
    static Steps()
    {
        AffectsMeasure<Steps>(SizeTypeProperty);
        AutoScrollToSelectedItemProperty.OverrideDefaultValue<Steps>(false);
        OrientationProperty.OverrideDefaultValue<Steps>(Orientation.Horizontal);
    }
    
    public Steps()
    {
        this.RegisterResources();
        Items.CollectionChanged += HandleCollectionChanged;
        SelectionMode           =  SelectionMode.Single;
    }
    
    private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Count > 0)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is StepsItem stepsItem)
                    {
                        if (_itemsBindingDisposables.TryGetValue(stepsItem, out var disposable))
                        {
                            disposable.Dispose();
                            _itemsBindingDisposables.Remove(stepsItem);
                        }
                    }
                }
            }
        }
        ConfigureItemsPanel();
        ConfigureCurrentStepsItem();
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new StepsItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<StepsItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is StepsItem stepsItem)
        {
            var disposables = new CompositeDisposable(2);
            
            if (item != null && item is not Visual)
            {
                if (!stepsItem.IsSet(StepsItem.ContentProperty))
                {
                    stepsItem.SetCurrentValue(StepsItem.ContentProperty, item);
                }
            }
            
            if (ItemTemplate != null)
            {
                disposables.Add(BindUtils.RelayBind(this, ItemTemplateProperty, stepsItem, StepsItem.ContentTemplateProperty));
            }
            
            disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, stepsItem, StepsItem.SizeTypeProperty));
            disposables.Add(BindUtils.RelayBind(this, StyleProperty, stepsItem, StepsItem.StyleProperty));
            disposables.Add(BindUtils.RelayBind(this, ItemIndicatorTypeProperty, stepsItem, StepsItem.IndicatorTypeProperty));
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, stepsItem, StepsItem.IsMotionEnabledProperty));
            
            PrepareStepsItem(stepsItem, item, index, disposables);
            
            if (_itemsBindingDisposables.TryGetValue(stepsItem, out var oldDisposables))
            {
                oldDisposables.Dispose();
                _itemsBindingDisposables.Remove(stepsItem);
            }
            _itemsBindingDisposables.Add(stepsItem, disposables);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type StepsItem.");
        }
    }
    
    protected virtual void PrepareStepsItem(StepsItem stepsItem, object? item, int index, CompositeDisposable compositeDisposable)
    {
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == OrientationProperty)
        {
            UpdatePseudoClasses();
            ConfigureItemsPanel();
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == SelectedIndexProperty)
            {
                ConfigureCurrentStepsItem();
            }
        }
        if (change.Property == CurrentStepProperty)
        {
            SyncCurrentStepToSelectedItem();    
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var itemsPresenter = e.NameScope.Find<ItemsPresenter>(StepsThemeConstants.ItemsPresenterPart);
        itemsPresenter?.ApplyTemplate();
        if (itemsPresenter?.Panel != null)
        {
            _grid = itemsPresenter.Panel as Grid;
        }
        ConfigureItemsPanel();
        UpdatePseudoClasses();
        if (InitialStep != -1)
        {
            SetCurrentValue(CurrentStepProperty, InitialStep);
        }

        SyncCurrentStepToSelectedItem();
        ConfigureCurrentStepsItem();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Vertical, Orientation == Orientation.Vertical);
        PseudoClasses.Set(StdPseudoClass.Horizontal, Orientation == Orientation.Horizontal);
    }

    private void ConfigureItemsPanel()
    {
        if (_grid != null)
        {
            var count = _grid.Children.Count;
            _grid.RowDefinitions.Clear();
            _grid.RowDefinitions.Clear();
            if (Orientation == Orientation.Horizontal)
            {
                var columnDefinitions = new ColumnDefinitions();
                for (var i = 0; i < count; i++)
                {
                    if (i != count - 1)
                    {
                        columnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                    }
                    else
                    {
                        columnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                    }
                }
                _grid.ColumnDefinitions = columnDefinitions;
            }
            else
            {
                var rowDefinitions = new RowDefinitions();
                for (var i = 0; i < count; i++)
                {
                    if (i != count - 1)
                    {
                        rowDefinitions.Add(new RowDefinition(GridLength.Star));
                    }
                    else
                    {
                        rowDefinitions.Add(new RowDefinition(GridLength.Auto));
                    }
                }
                _grid.RowDefinitions = rowDefinitions;
            }
        }
    }

    private void ConfigureCurrentStepsItem()
    {
        for (var i = 0; i < ItemCount; ++i)
        {
            if (ContainerFromIndex(i) is StepsItem stepsItem)
            {
                stepsItem.SetCurrentValue(StepsItem.PositionProperty, i + 1);
                stepsItem.SetCurrentValue(StepsItem.IsLastProperty, i == ItemCount - 1);
                if (SelectedIndex != -1)
                {
                    if (i < SelectedIndex)
                    {
                        stepsItem.SetCurrentValue(StepsItem.IsFinishedProperty, true);
                        stepsItem.SetValue(StepsItem.StatusProperty, StepsItemStatus.Finish, BindingPriority.Template);
                    }
                    else if (i == SelectedIndex)
                    {
                        stepsItem.SetValue(StepsItem.StatusProperty, CurrentStepStatus, BindingPriority.Template);
                    }
                    else
                    {
                        stepsItem.SetCurrentValue(StepsItem.IsFinishedProperty, false);
                        stepsItem.SetValue(StepsItem.StatusProperty, StepsItemStatus.Wait, BindingPriority.Template);
                    }
                }
                else
                {
                    if (CurrentStep >= ItemCount)
                    {
                        stepsItem.SetCurrentValue(StepsItem.IsFinishedProperty, true);
                        stepsItem.SetValue(StepsItem.StatusProperty, StepsItemStatus.Finish, BindingPriority.Template);
                    }
                }

                if (Orientation == Orientation.Horizontal)
                {
                    Grid.SetRow(stepsItem, 0);
                    Grid.SetColumn(stepsItem, i);
                }
                else
                {
                    Grid.SetRow(stepsItem, i);
                    Grid.SetColumn(stepsItem, 0);
                }
            }
        }
    }

    private void SyncCurrentStepToSelectedItem()
    {
        if (CurrentStep >= 0 && CurrentStep < ItemCount)
        {
            SetCurrentValue(SelectedIndexProperty, CurrentStep);
        }
        else
        {
            SetCurrentValue(SelectedIndexProperty, -1);
        }
    }
}