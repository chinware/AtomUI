using System.Collections;
using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using AtomUI.Data;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Templates;
using Avalonia.Input;

namespace AtomUI.Controls;

internal class ListDefaultView : SelectingItemsControl
{
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new VirtualizingStackPanel());

    #region 公共属性定义

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ListDefaultView>();
    
    public static readonly DirectProperty<ListDefaultView, IScrollable?> ScrollProperty =
        AvaloniaProperty.RegisterDirect<ListDefaultView, IScrollable?>(nameof(ListDefaultView), o => o.Scroll);
    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("AvaloniaProperty", "AVP1010",
        Justification = "This property is owned by SelectingItemsControl, but protected there. ListDefaultView changes its visibility.")]
    public new static readonly DirectProperty<SelectingItemsControl, IList?> SelectedItemsProperty =
        SelectingItemsControl.SelectedItemsProperty;
    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("AvaloniaProperty", "AVP1010",
        Justification = "This property is owned by SelectingItemsControl, but protected there. ListDefaultView changes its visibility.")]
    public new static readonly DirectProperty<SelectingItemsControl, ISelectionModel> SelectionProperty =
        SelectingItemsControl.SelectionProperty;
    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("AvaloniaProperty", "AVP1010",
        Justification = "This property is owned by SelectingItemsControl, but protected there. ListDefaultView changes its visibility.")]
    public new static readonly StyledProperty<SelectionMode> SelectionModeProperty =
        SelectingItemsControl.SelectionModeProperty;
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<ListDefaultView>();
    
    public static readonly StyledProperty<bool> DisabledItemHoverEffectProperty =
        List.DisabledItemHoverEffectProperty.AddOwner<ListDefaultView>();
    
    public static readonly StyledProperty<bool> IsShowSelectedIndicatorProperty =
        List.IsShowSelectedIndicatorProperty.AddOwner<ListDefaultView>();
    
    public static readonly StyledProperty<IDataTemplate?> GroupItemTemplateProperty =
        List.GroupItemTemplateProperty.AddOwner<ListDefaultView>();
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    private IScrollable? _scroll;
    public IScrollable? Scroll
    {
        get => _scroll;
        private set => SetAndRaise(ScrollProperty, ref _scroll, value);
    }

    public new IList? SelectedItems
    {
        get => base.SelectedItems;
        set => base.SelectedItems = value;
    }
    
    public new ISelectionModel Selection
    {
        get => base.Selection;
        set => base.Selection = value;
    }
    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("AvaloniaProperty", "AVP1012",
        Justification = "This property is owned by SelectingItemsControl, but protected there. List changes its visibility.")]
    public new SelectionMode SelectionMode
    {
        get => base.SelectionMode;
        set => base.SelectionMode = value;
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public bool DisabledItemHoverEffect
    {
        get => GetValue(DisabledItemHoverEffectProperty);
        set => SetValue(DisabledItemHoverEffectProperty, value);
    }
    
    public bool IsShowSelectedIndicator
    {
        get => GetValue(IsShowSelectedIndicatorProperty);
        set => SetValue(IsShowSelectedIndicatorProperty, value);
    }
    
    public IDataTemplate? GroupItemTemplate
    {
        get => GetValue(GroupItemTemplateProperty);
        set => SetValue(GroupItemTemplateProperty, value);
    }
    #endregion
    
    private protected readonly Dictionary<object, CompositeDisposable> _itemsBindingDisposables = new();

    static ListDefaultView()
    {
        ItemsPanelProperty.OverrideDefaultValue<List>(DefaultPanel);
        KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue(
            typeof(List),
            KeyboardNavigationMode.Once);
    }
   
    public ListDefaultView()
    {
        LogicalChildren.CollectionChanged += HandleChildrenChanged;
    }
    
    private void HandleChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Count > 0)
            {
                foreach (var item in e.OldItems)
                {
                    DisposableListItem(item);
                }
            }
        }
    }

    private protected void DisposableListItem(object item)
    {
        if (_itemsBindingDisposables.TryGetValue(item, out var disposable))
        {
            disposable.Dispose();
            _itemsBindingDisposables.Remove(item);
        }
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        if (item is ListGroupData)
        {
            return new ListGroupItem();
        }
        return new ListItem();
    }
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        var disposables = new CompositeDisposable(4);
        if (container is ListItem listBoxItem)
        {
            if (item != null && item is not Visual)
            {
                if (!listBoxItem.IsSet(ListItem.ContentProperty))
                {
                    listBoxItem.SetCurrentValue(ListItem.ContentProperty, item);
                }
                
                ApplyListItemData(listBoxItem, item);
            }
            
            if (ItemTemplate != null)
            {
                disposables.Add(BindUtils.RelayBind(this, ItemTemplateProperty, listBoxItem, ListItem.ContentTemplateProperty));
            }
            
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, listBoxItem, ListItem.IsMotionEnabledProperty));
            disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, listBoxItem, ListItem.SizeTypeProperty));
            disposables.Add(BindUtils.RelayBind(this, IsShowSelectedIndicatorProperty, listBoxItem, ListItem.IsShowSelectedIndicatorProperty));
            disposables.Add(BindUtils.RelayBind(this, DisabledItemHoverEffectProperty, listBoxItem,
                ListItem.DisabledItemHoverEffectProperty));
            PrepareListBoxItem(listBoxItem, item, index, disposables);
        }
        else if (container is ListGroupItem groupItem)
        {
            if (item != null && item is not Visual)
            {
                if (!groupItem.IsSet(ListGroupItem.ContentProperty))
                {
                    if (GroupItemTemplate != null)
                    {
                        groupItem.SetCurrentValue(ListGroupItem.ContentProperty, item);
                    }
                    else if (item is ListGroupData groupData)
                    {
                        groupItem.SetCurrentValue(ListGroupItem.ContentProperty, groupData.Header);
                    }
                }
                disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, groupItem, ListGroupItem.IsMotionEnabledProperty));
                disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, groupItem, ListGroupItem.SizeTypeProperty));
            }
            
            if (GroupItemTemplate != null)
            {
                disposables.Add(BindUtils.RelayBind(this, GroupItemTemplateProperty, groupItem, ListGroupItem.ContentTemplateProperty));
            }
        }
        
        DisposableListItem(container);
        _itemsBindingDisposables.Add(container, disposables);
    }
    
    protected virtual void PrepareListBoxItem(ListItem listItem, object? item, int index, CompositeDisposable compositeDisposable)
    {
    }

    protected virtual void ApplyListItemData(ListItem listItem, object item)
    {
        if (item is IListBoxItemData listBoxItemData)
        {
            if (!listItem.IsSet(ListItem.IsSelectedProperty))
            {
                listItem.SetCurrentValue(ListItem.IsSelectedProperty, listBoxItemData.IsSelected);
            }
            if (!listItem.IsSet(ListItem.IsEnabledProperty))
            {
                listItem.SetCurrentValue(IsEnabledProperty, listBoxItemData.IsEnabled);
            }
        }
    }
    
    protected override void OnKeyDown(KeyEventArgs e)
    {
        var hotkeys = Application.Current!.PlatformSettings?.HotkeyConfiguration;
        var ctrl    = hotkeys is not null && e.KeyModifiers.HasAllFlags(hotkeys.CommandModifiers);

        if (!ctrl &&
            e.Key.ToNavigationDirection() is { } direction && 
            direction.IsDirectional())
        {
            e.Handled |= MoveSelection(
                direction,
                WrapSelection,
                e.KeyModifiers.HasAllFlags(KeyModifiers.Shift));
        }
        else if (SelectionMode.HasAllFlags(SelectionMode.Multiple) &&
                 hotkeys is not null && hotkeys.SelectAll.Any(x => x.Matches(e)))
        {
            Selection.SelectAll();
            e.Handled = true;
        }
        else if (e.Key == Key.Space || e.Key == Key.Enter)
        {
            UpdateSelectionFromEventSource(
                e.Source,
                true,
                e.KeyModifiers.HasFlag(KeyModifiers.Shift),
                ctrl);
        }

        base.OnKeyDown(e);
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        Scroll = e.NameScope.Find<IScrollable>(ListThemeConstants.ScrollViewerPart);
    }
    
    protected internal virtual bool UpdateSelectionFromPointerEvent(Control source, PointerEventArgs e)
    {
        var hotkeys = Application.Current!.PlatformSettings?.HotkeyConfiguration;
        var toggle  = hotkeys is not null && e.KeyModifiers.HasAllFlags(hotkeys.CommandModifiers);
        return UpdateSelectionFromEventSource(
            source,
            true,
            e.KeyModifiers.HasAllFlags(KeyModifiers.Shift),
            toggle,
            e.GetCurrentPoint(source).Properties.IsRightButtonPressed);
    }
}