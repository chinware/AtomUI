using System.Collections;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Metadata;
using Avalonia.VisualTree;

using ItemCollection = AtomUI.Collections.ItemCollection;

namespace AtomUI.Controls.Commons;

[TemplatePart("PART_CheckBoxItems", typeof(SelectingItemsControl))]
public abstract class AbstractCheckBoxGroup: TemplatedControl,
                                             IMotionAwareControl,
                                             IFormItemAware
{
    #region 公共属性定义
    
    public static readonly StyledProperty<double> ItemSpacingProperty = 
        AvaloniaProperty.Register<AbstractCheckBoxGroup, double>(nameof (ItemSpacing));
    
    public static readonly StyledProperty<double> LineSpacingProperty = 
        AvaloniaProperty.Register<AbstractCheckBoxGroup, double>(nameof (LineSpacing));
    
    public static readonly StyledProperty<Orientation> OrientationProperty = 
        StackPanel.OrientationProperty.AddOwner<AbstractCheckBoxGroup>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractCheckBoxGroup>();
    
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        ItemsControl.ItemsSourceProperty.AddOwner<AbstractCheckBoxGroup>();
    
    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
        ItemsControl.ItemTemplateProperty.AddOwner<AbstractCheckBoxGroup>();
    
    public static readonly DirectProperty<AbstractCheckBoxGroup, IList?> CheckedItemsProperty =
        AvaloniaProperty.RegisterDirect<AbstractCheckBoxGroup, IList?>(
            nameof(CheckedItems),
            o => o.CheckedItems,
            (o, v) => o.CheckedItems = v,
            defaultBindingMode: BindingMode.TwoWay,
            enableDataValidation: true);
    
    public double ItemSpacing
    {
        get => GetValue(ItemSpacingProperty);
        set => SetValue(ItemSpacingProperty, value);
    }
    
    public double LineSpacing
    {
        get => GetValue(LineSpacingProperty);
        set => SetValue(LineSpacingProperty, value);
    }
    
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public IDataTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }
    
    private IList? _checkedItems;

    public IList? CheckedItems
    {
        get => _checkedItems;
        set => SetAndRaise(CheckedItemsProperty, ref _checkedItems, value);
    }
    
    [Content]
    public ItemCollection Items => _items;
    #endregion

    #region 公共事件定义

    public static readonly RoutedEvent<CheckBoxGroupCheckedChangedEventArgs> CheckedChangedEvent =
        RoutedEvent.Register<AbstractCheckBoxGroup, CheckBoxGroupCheckedChangedEventArgs>(
            nameof(CheckedChanged),
            RoutingStrategies.Bubble);

    public event EventHandler<CheckBoxGroupCheckedChangedEventArgs>? CheckedChanged
    {
        add => AddHandler(CheckedChangedEvent, value);
        remove => RemoveHandler(CheckedChangedEvent, value);
    }
    #endregion
    
    private readonly ItemCollection _items = new();
    private AbstractCheckBoxItemsControl? _itemsControl;
    private INotifyCollectionChanged? _checkedItemsCollectionChangedSource;
    private IList? _checkedItemsSnapshot;
    
    static AbstractCheckBoxGroup()
    {
        OrientationProperty.OverrideDefaultValue<AbstractCheckBoxGroup>(Orientation.Horizontal);
        ItemsSourceProperty.Changed.AddClassHandler<AbstractCheckBoxGroup>((group, args) => group.HandleItemsSourceChanged(args));
    }
    
    private void HandleItemsSourceChanged(AvaloniaPropertyChangedEventArgs args)
    {
        _items.SetItemsSource(args.GetNewValue<IEnumerable?>());
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_itemsControl != null)
        {
            _itemsControl.SelectionChanged -= HandleItemsSelectedChanged;
        }
        _itemsControl = e.NameScope.Find<AbstractCheckBoxItemsControl>("PART_CheckBoxItems");
        if (_itemsControl != null)
        {
            _itemsControl.ItemsSource      =  _items;
            _itemsControl.SelectionChanged += HandleItemsSelectedChanged;
            SyncCheckedItemsToItemsControl();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ConfigureCheckedItemsCollectionChangedSource(CheckedItems);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ReleaseCheckedItemsCollectionChangedSource();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CheckedItemsProperty)
        {
            HandleCheckedItemsChanged(change.OldValue as IList, change.NewValue as IList);
        }
    }

    private void HandleItemsSelectedChanged(object? sender, SelectionChangedEventArgs change)
    {
        var checkedItems = _itemsControl?.BuildCurrentCheckedItems() ??
                           CopyCheckedItems(CheckedItems) ??
                           new AvaloniaList<object?>();
        foreach (var item in change.RemovedItems)
        {
            checkedItems.Remove(item);
        }

        foreach (var item in change.AddedItems)
        {
            if (!checkedItems.Contains(item))
            {
                checkedItems.Add(item);
            }
        }

        CheckedItems = checkedItems;
    }

    private void HandleCheckedItemsChanged(IList? oldValue, IList? newValue)
    {
        ConfigureCheckedItemsCollectionChangedSource(newValue);
        SyncCheckedItemsToItemsControl();
        _checkedItemsSnapshot = CopyCheckedItems(newValue);
        NotifyCheckedItemsChanged(oldValue, newValue);
        _formValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ConfigureCheckedItemsCollectionChangedSource(IList? checkedItems)
    {
        if (!this.IsAttachedToVisualTree())
        {
            ReleaseCheckedItemsCollectionChangedSource();
            _checkedItemsSnapshot = CopyCheckedItems(checkedItems);
            return;
        }

        if (ReferenceEquals(_checkedItemsCollectionChangedSource, checkedItems))
        {
            _checkedItemsSnapshot = CopyCheckedItems(checkedItems);
            return;
        }

        ReleaseCheckedItemsCollectionChangedSource();

        _checkedItemsCollectionChangedSource = checkedItems as INotifyCollectionChanged;
        if (_checkedItemsCollectionChangedSource != null)
        {
            _checkedItemsCollectionChangedSource.CollectionChanged += HandleCheckedItemsCollectionChanged;
        }

        _checkedItemsSnapshot = CopyCheckedItems(checkedItems);
    }

    private void ReleaseCheckedItemsCollectionChangedSource()
    {
        if (_checkedItemsCollectionChangedSource != null)
        {
            _checkedItemsCollectionChangedSource.CollectionChanged -= HandleCheckedItemsCollectionChanged;
            _checkedItemsCollectionChangedSource = null;
        }
    }

    private void HandleCheckedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (!ReferenceEquals(sender, _checkedItemsCollectionChangedSource))
        {
            return;
        }

        var oldSnapshot = _checkedItemsSnapshot;
        var newSnapshot = CopyCheckedItems(CheckedItems);
        _checkedItemsSnapshot = newSnapshot;

        SyncCheckedItemsToItemsControl();
        NotifyCheckedItemsChanged(oldSnapshot, newSnapshot);
        _formValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SyncCheckedItemsToItemsControl()
    {
        if (_itemsControl == null)
        {
            return;
        }

        _itemsControl.SelectionChanged -= HandleItemsSelectedChanged;
        try
        {
            _itemsControl.CheckedItems = CopyCheckedItems(CheckedItems);
        }
        finally
        {
            _itemsControl.SelectionChanged += HandleItemsSelectedChanged;
        }
    }

    private void NotifyCheckedItemsChanged(IList? oldValue, IList? newValue)
    {
        var removedItems = BuildRemovedItems(oldValue, newValue);
        var addedItems   = BuildRemovedItems(newValue, oldValue);
        if (removedItems.Count == 0 && addedItems.Count == 0)
        {
            return;
        }

        RaiseEvent(new CheckBoxGroupCheckedChangedEventArgs(CheckedChangedEvent, removedItems, addedItems));
    }

    private static IList BuildRemovedItems(IList? source, IList? target)
    {
        var items = new AvaloniaList<object?>();
        if (source == null)
        {
            return items;
        }

        foreach (var item in source)
        {
            if (target == null || !target.Contains(item))
            {
                items.Add(item);
            }
        }
        return items;
    }

    private static AvaloniaList<object?>? CopyCheckedItems(IEnumerable? source)
    {
        if (source == null)
        {
            return null;
        }

        var checkedItems = new AvaloniaList<object?>();
        foreach (var item in source)
        {
            checkedItems.Add(item);
        }
        return checkedItems;
    }
    
    #region 实现 FormItem 接口
    private EventHandler? _formValueChanged;
    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    void IFormItemAware.SetFormValue(object? value) => NotifySetFormValue((IList?)value);

    object? IFormItemAware.GetFormValue() => NotifyGetFormValue();
    void IFormItemAware.ClearFormValue() => NotifyClearFormValue();
    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status) => NotifyValidateStatus(status);
    
    protected virtual void NotifySetFormValue(IList? value)
    {
        SetCurrentValue(CheckedItemsProperty, value);
    }

    protected virtual IList? NotifyGetFormValue()
    {
        return CheckedItems ?? _itemsControl?.BuildCurrentCheckedItems();
    }

    protected virtual void NotifyClearFormValue()
    {
        SetCurrentValue(CheckedItemsProperty, null);
    }
    
    protected virtual void NotifyValidateStatus(FormValidateStatus status)
    {
    }
    #endregion
}
