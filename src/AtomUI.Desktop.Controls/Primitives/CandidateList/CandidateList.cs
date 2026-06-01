using AtomUI.Controls;
using AtomUI.Controls.Data;
using AtomUI.Desktop.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls.Primitives;

public class CandidateList : ListBox, ICandidateList
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsCandidateItemNavigationEnabledProperty =
        AvaloniaProperty.Register<CandidateList, bool>(nameof(IsCandidateItemNavigationEnabled), true);

    public static readonly DirectProperty<CandidateList, object?> CandidateSelectedItemProperty =
        AvaloniaProperty.RegisterDirect<CandidateList, object?>(
            nameof(CandidateSelectedItem),
            o => o.CandidateSelectedItem,
            (o, v) => o.CandidateSelectedItem = v,
            defaultBindingMode: BindingMode.TwoWay, enableDataValidation: true);
    
    public static readonly DirectProperty<CandidateList, int> CandidateSelectedIndexProperty =
        AvaloniaProperty.RegisterDirect<CandidateList, int>(
            nameof(CandidateSelectedIndex),
            o => o.CandidateSelectedIndex,
            (o, v) => o.CandidateSelectedIndex = v,
            defaultBindingMode: BindingMode.TwoWay, enableDataValidation: true);
    
    public static readonly StyledProperty<int> MaxCountProperty =
        AvaloniaProperty.Register<CandidateList, int>(nameof(MaxCount), int.MaxValue);
    
    public bool IsCandidateItemNavigationEnabled
    {
        get => GetValue(IsCandidateItemNavigationEnabledProperty);
        set => SetValue(IsCandidateItemNavigationEnabledProperty, value);
    }

    private object? _candidateSelectedItem;

    public object? CandidateSelectedItem
    {
        get => _candidateSelectedItem;
        set => SetAndRaise(CandidateSelectedItemProperty, ref _candidateSelectedItem, value);
    }

    private int _candidateSelectedIndex = -1;

    public int CandidateSelectedIndex
    {
        get => _candidateSelectedIndex;
        set => SetAndRaise(CandidateSelectedIndexProperty, ref _candidateSelectedIndex, value);
    }
    
    public int MaxCount
    {
        get => GetValue(MaxCountProperty);
        set => SetValue(MaxCountProperty, value);
    }
    #endregion
    
    #region 公共事件定义
    public static readonly RoutedEvent<RoutedEventArgs> CommitEvent =
        RoutedEvent.Register<CandidateList, RoutedEventArgs>(
            nameof(Commit),
            RoutingStrategies.Bubble);
    
    public static readonly RoutedEvent<RoutedEventArgs> CancelEvent =
        RoutedEvent.Register<CandidateList, RoutedEventArgs>(
            nameof(Cancel),
            RoutingStrategies.Bubble);
    
    public event EventHandler<RoutedEventArgs>? Commit
    {
        add => AddHandler(CommitEvent, value);
        remove => RemoveHandler(CommitEvent, value);
    }
    
    public event EventHandler<RoutedEventArgs>? Cancel
    {
        add => AddHandler(CancelEvent, value);
        remove => RemoveHandler(CancelEvent, value);
    }
    #endregion
    
    private static readonly FuncTemplate<Panel?> DefaultPanel = new(() => new CandidateVirtualizingStackPanel());

    private ScrollViewer? _scrollViewer;

    static CandidateList()
    {
        SelectedItemProperty.Changed.AddClassHandler<CandidateList>((list, args) => list.HandleSelectItemChanged(args));
        CandidateSelectedIndexProperty.Changed.AddClassHandler<CandidateList>((list, args) => list.HandleCandidateSelectedIndexChanged(args));
        CandidateSelectedItemProperty.Changed.AddClassHandler<CandidateList>((list, args) => list.HandleCandidateSelectedItemChanged(args));
        SelectionChangedEvent.AddClassHandler<CandidateList>((list, args) => list.HandleSelectionChanged());
        
        ItemsPanelProperty.OverrideDefaultValue<CandidateList>(DefaultPanel);
    }
    
    private void HandleSelectionChanged()
    {
        if (!IsSingleMode())
        {
            ConfigureOptionsForMaxCount();
        }
    }
    
    private void ConfigureOptionsForMaxCount()
    {
        if (SelectedItems?.Count >= MaxCount)
        {
            for (var i = 0; i < ItemCount; i++)
            {
                var item      = Items[i];
                var container = ContainerFromIndex(i);
                if (!SelectedItems.Contains(item))
                {
                    SetContainerEnabledIfChanged(container, false);
                }
                else
                {
                    SetContainerEnabledIfChanged(container, true);
                }
            }
        }
        else
        {
            for (var i = 0; i < ItemCount; i++)
            {
                var item      = Items[i];
                var container = ContainerFromIndex(i);
                if (item is IListItemData selectOption)
                {
                    SetContainerEnabledIfChanged(container, selectOption.IsEnabled);
                }
            }
        }
    }

    private static void SetContainerEnabledIfChanged(Control? container, bool isEnabled)
    {
        if (container is not null && container.IsEnabled != isEnabled)
        {
            container.SetCurrentValue(IsEnabledProperty, isEnabled);
        }
    }
    
    private void HandleCandidateSelectedIndexChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var oldIndex = e.GetOldValue<int>();
        var newIndex = e.GetNewValue<int>();
        if (newIndex == -1)
        {
            ClearCandidateItemSelection(oldIndex);
        }
        else
        {
            TrySetCandidateItemSelected(newIndex, oldIndex);
        }
    }
    
    private void HandleCandidateSelectedItemChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue == null)
        {
            if (e.OldValue != null && ContainerFromItem(e.OldValue) is CandidateListItem listItem)
            {
                SetCandidateItemSelectedIfChanged(listItem, false);
            }
        }
        else
        {
            TrySetCandidateItemSelected(e.NewValue);
        }
    }
    
    public bool TrySetCandidateItemSelected(object item)
    {
        var index = Items.IndexOf(item);
        if (index == -1)
        {
            return false;
        }

        return TrySetCandidateItemSelected(index);
    }
    
    public bool TrySetCandidateItemSelected(int index)
    {
        return TrySetCandidateItemSelected(index, CandidateSelectedIndex);
    }

    private bool TrySetCandidateItemSelected(int index, int oldIndex)
    {
        if (index < 0 || index > ItemCount - 1)
        {
            return false;
        }
        
        if (oldIndex != -1 && oldIndex != index)
        {
            ClearCandidateItemSelection(oldIndex);
        }

        Control? candidateContainer = null;
        if (ItemsPanelRoot is CandidateVirtualizingStackPanel virtualizingStackPanel)
        {
            candidateContainer = virtualizingStackPanel.ScrollCandidateItemIntoView(index);
        }

        candidateContainer ??= ContainerFromIndex(index);
        if (candidateContainer is CandidateListItem childContainer)
        {
            SetCandidateItemSelectedIfChanged(childContainer, true);
        }

        var candidateSelectedItem = Items[index];
        if (!Equals(CandidateSelectedItem, candidateSelectedItem))
        {
            SetCurrentValue(CandidateSelectedItemProperty, candidateSelectedItem);
        }
        if (CandidateSelectedIndex != index)
        {
            SetCurrentValue(CandidateSelectedIndexProperty, index);
        }
        return true;
    }

    private void ClearCandidateItemSelection(int index)
    {
        if (index >= 0 &&
            index < ItemCount &&
            ContainerFromIndex(index) is CandidateListItem listItem)
        {
            SetCandidateItemSelectedIfChanged(listItem, false);
        }
    }

    private static void SetCandidateItemSelectedIfChanged(CandidateListItem item, bool isSelected)
    {
        if (item.IsCandidateSelected != isSelected)
        {
            item.SetCurrentValue(CandidateListItem.IsCandidateSelectedProperty, isSelected);
        }
    }
    
    private void ResetScrollViewer()
    {
        if (_scrollViewer != null)
        {
            _scrollViewer.Offset = new Vector(0, 0);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _scrollViewer = e.NameScope.Find<ScrollViewer>(ListViewThemeConstants.ScrollViewerPart);
    }
    
    public void HandleKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter:
                if (IsSingleMode())
                {
                    HandleSingleModeCommit();
                }
                else
                {
                    HandleMultiModeCommit();
                }
                
                e.Handled = true;
                break;

            case Key.Up:
                SelectPreviousCandidateItem();
                e.Handled = true;
                break;

            case Key.Down:
                SelectNextCandidateItem();
             
                e.Handled = true;
                break;

            case Key.Escape:
                HandleCancel();
                e.Handled = true;
                break;

            default:
                break;
        }
    }

    private void HandleSingleModeCommit()
    {
        NotifyCommit();
        ClearState();
    }
    
    private void HandleMultiModeCommit()
    {
        if (CandidateSelectedItem != null)
        {
            if (ContainerFromItem(CandidateSelectedItem) is CandidateListItem listItem)
            {
                var index = IndexFromContainer(listItem);
                if (index != -1)
                {
                    if (listItem.IsSelected)
                    {
                        Selection.Deselect(index);
                    }
                    else
                    {
                        Selection.Select(index);
                    }
                }
            }
        }
    }
    
    protected virtual void SelectPreviousCandidateItem()
    {
        if (ItemCount == 0)
        {
            return;
        }

        if (_candidateSelectedIndex == -1)
        {
            CandidateSelectedIndex = SelectedIndex != -1 ? FindNextEnabledIndex(SelectedIndex, -1) : ItemCount - 1;
        }
        else
        {
            CandidateSelectedIndex = FindNextEnabledIndex(CandidateSelectedIndex, -1);
        }
    }
    
    protected virtual void SelectNextCandidateItem()
    {
        if (ItemCount == 0)
        {
            return;
        }

        if (_candidateSelectedIndex == -1)
        {
            CandidateSelectedIndex = SelectedIndex != -1 ? FindNextEnabledIndex(SelectedIndex, 1) : 0;
        }
        else
        {
            CandidateSelectedIndex = FindNextEnabledIndex(CandidateSelectedIndex, 1);
        }
    }

    protected virtual void NotifyCommit()
    {
        if (CandidateSelectedItem != null)
        {
            if (!Equals(SelectedItem, CandidateSelectedItem))
            {
                SetCurrentValue(SelectedItemProperty, CandidateSelectedItem);
            }
        }
        RaiseEvent(new RoutedEventArgs(CommitEvent)
        {
            Source = this,
        });
       
    }

    private void HandleCancel()
    {
        NotifyCancel();
        ClearState();
    }
    
    protected virtual void NotifyCancel()
    {
        RaiseEvent(new RoutedEventArgs(CancelEvent)
        {
            Source = this,
        });
    }

    private void HandleSelectItemChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue == null)
        {
            ResetScrollViewer();
        }
    }

    private void ClearState()
    {
        if (SelectedItems is not null)
        {
            SelectedItems = null;
        }
        if (SelectedItem is not null)
        {
            SelectedItem = null;
        }
        if (SelectedIndex != -1)
        {
            SelectedIndex = -1;
        }
    }
    
    protected override void NotifyListBoxItemClicked(ListBoxItem item)
    {
        if (IsSingleMode())
        {
            NotifyCommit();
        }
    }
    
    private int FindNextEnabledIndex(int startIndex, int delta)
    {
        var index     = startIndex;
        var findCycle = false;
        while (true)
        {
            index += delta;
            if (index >= ItemCount)
            {
                index = 0;
                if (!findCycle)
                {
                    findCycle = true;
                }
                else
                {
                    return -1;
                }
            }
            else if (index < 0)
            {
                index = ItemCount - 1;
                if (!findCycle)
                {
                    findCycle = true;
                }
                else
                {
                    return -1;
                }
            }

            if (ContainerFromIndex(index) is ListBoxItem listBoxItem && listBoxItem.IsEnabled)
            {
                return index;
            }
        }
    }
    
    private bool IsSingleMode()
    {
        return (SelectionMode & SelectionMode.Multiple) != SelectionMode.Multiple;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == MaxCountProperty || 
            change.Property == SelectedItemsProperty)
        {
            ConfigureOptionsForMaxCount();
        }

        if (change.Property == SelectedItemsProperty)
        {
            ConfigureEmptyIndicator();
        }
        if (change.Property == FilterValueProperty ||
            change.Property == FilterProperty)
        {
            if (CandidateSelectedIndex != -1)
            {
                CandidateSelectedIndex = -1;
            }
            if (CandidateSelectedItem is not null)
            {
                CandidateSelectedItem = null;
            }
        }
        
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        var listItem = new CandidateListItem();
        NotifyContainerForItemCreated(listItem, item);
        return listItem;
    }
    
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<CandidateListItem>(item, out recycleKey);
    }
    
    protected override void ConfigureEmptyIndicator()
    {
        var isEffectiveEmptyVisible = IsShowEmptyIndicator &&
                                      (ItemCount == 0 || (IsFiltering && FilterResultCount == 0));
        if (IsEffectiveEmptyVisible != isEffectiveEmptyVisible)
        {
            SetCurrentValue(IsEffectiveEmptyVisibleProperty, isEffectiveEmptyVisible);
        }
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (CandidateSelectedIndex != -1)
        {
            CandidateSelectedIndex = -1;
        }
        if (CandidateSelectedItem is not null)
        {
            CandidateSelectedItem = null;
        }
    }

    #region 虚拟化上下文管理

    protected override void NotifyRestoreDefaultContext(ListBoxItem item, IListItemData itemData)
    {
        base.NotifyRestoreDefaultContext(item, itemData);
        if (item is CandidateListItem candidateListItem && item is IListItemVirtualizingContextAware virtualListItem)
        {
            if (CandidateSelectedIndex != -1)
            {
                SetCandidateItemSelectedIfChanged(
                    candidateListItem,
                    virtualListItem.VirtualIndex == CandidateSelectedIndex);
            }
        }
    }
    
    protected override void NotifyClearContainerForVirtualizingContext(ListBoxItem item)
    {
        base.NotifyClearContainerForVirtualizingContext(item);
        if (item is CandidateListItem listItem)
        {
            if (listItem.IsSet(CandidateListItem.IsCandidateSelectedProperty))
            {
                listItem.ClearValue(CandidateListItem.IsCandidateSelectedProperty);
            }
        }
    }
    #endregion
}
