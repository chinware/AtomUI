using System.Collections.Specialized;
using System.Diagnostics;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Data;
using AtomUI.Icons.AntDesign;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal class DataGridFilterIndicator : IconButton
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsFilterActivatedProperty =
        AvaloniaProperty.Register<DataGridFilterIndicator, bool>(nameof(IsFilterActivated));

    public static readonly DirectProperty<DataGridFilterIndicator, DataGridFilterMode> FilterModeProperty =
        AvaloniaProperty.RegisterDirect<DataGridFilterIndicator, DataGridFilterMode>(
            nameof(FilterMode),
            o => o.FilterMode,
            (o, v) => o.FilterMode = v);
    
    public static readonly StyledProperty<bool> IsMultipleFilterEnabledProperty =
        AvaloniaProperty.Register<DataGridFilterIndicator, bool>(nameof(IsMultipleFilterEnabled));
    
    public event EventHandler<DataGridColumnFilterEventArgs>? FilterRequest;

    public bool IsFilterActivated
    {
        get => GetValue(IsFilterActivatedProperty);
        set => SetValue(IsFilterActivatedProperty, value);
    }

    private DataGridFilterMode _filterMode;

    public DataGridFilterMode FilterMode
    {
        get => _filterMode;
        set => SetAndRaise(FilterModeProperty, ref _filterMode, value);
    }
    
    public bool IsMultipleFilterEnabled
    {
        get => GetValue(IsMultipleFilterEnabledProperty);
        set => SetValue(IsMultipleFilterEnabledProperty, value);
    }
    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<DataGridFilterIndicator, string?> SelectedAllTextProperty =
        AvaloniaProperty.RegisterDirect<DataGridFilterIndicator, string?>(
            nameof(SelectedAllText),
            o => o.SelectedAllText,
            (o, v) => o.SelectedAllText = v);
    
    internal string? SelectedAllText
    {
        get => _selectedAllText;
        set => SetAndRaise(SelectedAllTextProperty, ref _selectedAllText, value);
    }
    private string? _selectedAllText;

    #endregion
    
    private DataGridColumn? _owningColumn;
    private static int _indicatorSeed = 0;
    private string? _treeRadioCheckGroupName;
    private DataGrid? _subscribedGrid;
    private IDataGridCollectionView? _subscribedCollectionView;
    private INotifyCollectionChanged? _subscribedFilters;
    private bool _isFlyoutContentMaterialized;

    internal DataGridColumn? OwningColumn
    {
        get => _owningColumn;
        set
        {
            if (ReferenceEquals(_owningColumn, value))
            {
                return;
            }

            UnregisterOwningColumnSubscriptions();
            _owningColumn = value;
            if (_owningColumn != null)
            {
                RegisterOwningColumnSubscriptions(_owningColumn);
                FilterMode              = _owningColumn.FilterMode;
                IsMultipleFilterEnabled = _owningColumn.IsMultipleFilterEnabled;
            }
            RefreshFilterFlyoutState();
        }
    }

    private readonly FlyoutStateHelper _flyoutStateHelper;

    public DataGridFilterIndicator()
    {
        _flyoutStateHelper = new FlyoutStateHelper
        {
            AnchorTarget = this,
            TriggerType  = FlyoutTriggerType.Click
        };
        _flyoutStateHelper.FlyoutAboutToShow += HandleFlyoutAboutToShow;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (Icon is null)
        {
            SetValue(IconProperty, new FilterFilled(), BindingPriority.Template);
        }
        base.OnApplyTemplate(e);
        RefreshFilterFlyoutState();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs changed)
    {
        base.OnPropertyChanged(changed);
        if (changed.Property == FilterModeProperty)
        {
            RecreateFlyoutShell();
        }
        else if (changed.Property == IsMultipleFilterEnabledProperty)
        {
            var rematerialize = _isFlyoutContentMaterialized;
            ConfigureTreeFlyoutToggleType();
            ClearFlyoutContent();
            if (rematerialize)
            {
                MaterializeFlyoutContent();
            }
        }
    }

    internal void RefreshFilterFlyoutState()
    {
        if (ShouldProvideFlyout())
        {
            EnsureFlyoutShell();
        }
        else
        {
            ClearFlyout();
        }
    }

    private bool ShouldProvideFlyout()
    {
        return OwningColumn?.OwningGrid is not null &&
               OwningColumn.Filters.Count > 0;
    }

    private void RecreateFlyoutShell()
    {
        var rematerialize = _isFlyoutContentMaterialized;
        ClearFlyout();
        RefreshFilterFlyoutState();
        if (rematerialize)
        {
            MaterializeFlyoutContent();
        }
    }

    private void EnsureFlyoutShell()
    {
        Debug.Assert(OwningColumn is not null);
        Debug.Assert(OwningColumn.OwningGrid is not null);

        if (OwningColumn?.OwningGrid is not { } owningGrid)
        {
            return;
        }

        if (FilterMode == DataGridFilterMode.Menu && Flyout is not DataGridMenuFilterFlyout)
        {
            ClearFlyout();
            var menuFlyout = new DataGridMenuFilterFlyout
            {
                IsArrowVisible             = false,
                Placement                 = PlacementMode.BottomEdgeAlignedRight,
                ShouldUseOverlayPopup     = true
            };
            menuFlyout[!MotionAwareControlProperty.IsMotionEnabledProperty] =
                owningGrid[!MotionAwareControlProperty.IsMotionEnabledProperty];

            Flyout                          =  menuFlyout;
            _flyoutStateHelper.Flyout       =  menuFlyout;
            menuFlyout.FilterValuesSelected += HandleFilterValuesSelected;
        }
        else if (FilterMode == DataGridFilterMode.Tree && Flyout is not DataGridTreeFilterFlyout)
        {
            ClearFlyout();
            var treeFlyout = new DataGridTreeFilterFlyout
            {
                IsArrowVisible             = false,
                Placement                 = PlacementMode.BottomEdgeAlignedRight,
                ShouldUseOverlayPopup     = true
            };
            treeFlyout[!MotionAwareControlProperty.IsMotionEnabledProperty] =
                owningGrid[!MotionAwareControlProperty.IsMotionEnabledProperty];
            ConfigureTreeFlyoutToggleType(treeFlyout);

            Flyout                    = treeFlyout;
            _flyoutStateHelper.Flyout = treeFlyout;
            treeFlyout.FilterValuesSelected += HandleFilterValuesSelected;
        }

        ConfigureFlyoutPassiveCloseFilter();
    }

    private bool ShouldFilterOnPassiveClose()
    {
        return OwningColumn?.FilterOnClose == true;
    }

    private void ConfigureFlyoutPassiveCloseFilter()
    {
        if (Flyout is DataGridMenuFilterFlyout menuFlyout)
        {
            menuFlyout.ShouldFilterOnPassiveClose = ShouldFilterOnPassiveClose;
        }
        else if (Flyout is DataGridTreeFilterFlyout treeFlyout)
        {
            treeFlyout.ShouldFilterOnPassiveClose = ShouldFilterOnPassiveClose;
        }
    }

    private void HandleFlyoutAboutToShow(object? sender, EventArgs args)
    {
        MaterializeFlyoutContent();
    }

    private void MaterializeFlyoutContent()
    {
        if (_isFlyoutContentMaterialized || OwningColumn is null)
        {
            return;
        }

        EnsureFlyoutShell();
        if (Flyout is DataGridMenuFilterFlyout menuFlyout)
        {
            PopulateMenuItems(menuFlyout.Items, OwningColumn.Filters);
            _isFlyoutContentMaterialized = true;
        }
        else if (Flyout is DataGridTreeFilterFlyout treeFlyout)
        {
            if (IsMultipleFilterEnabled)
            {
                var selectAllTreeItem = new DataGridFilterTreeViewItem();
                selectAllTreeItem[!DataGridFilterTreeViewItem.HeaderProperty] = this[!SelectedAllTextProperty];
                PopulateTreeItems(selectAllTreeItem.Items, OwningColumn.Filters);
                treeFlyout.Items.Add(selectAllTreeItem);
            }
            else
            {
                PopulateTreeItems(treeFlyout.Items, OwningColumn.Filters);
            }
            _isFlyoutContentMaterialized = true;
        }
    }

    private string TreeRadioCheckGroupName =>
        _treeRadioCheckGroupName ??= $"tree-{nameof(DataGridFilterIndicator)}-{_indicatorSeed++}";

    private void PopulateMenuItems(ItemCollection targetItems, IEnumerable<DataGridFilterItem> filterItems)
    {
        foreach (var item in filterItems)
        {
            var menuItem = new DataGridFilterMenuItem()
            {
                Header           = item.Text,
                FilterValue      = item.Value,
                StaysOpenOnClick = true,
                ToggleType       = IsMultipleFilterEnabled
                    ? MenuItemToggleType.CheckBox
                    : MenuItemToggleType.Radio
            };
            targetItems.Add(menuItem);
            if (item.HasChildren)
            {
                PopulateMenuItems(menuItem.Items, item.Children);
            }
        }
    }

    private void PopulateTreeItems(ItemCollection targetItems, IEnumerable<DataGridFilterItem> filterItems)
    {
        foreach (var item in filterItems)
        {
            var treeItem = new DataGridFilterTreeViewItem
            {
                Header      = item.Text,
                FilterValue = item.Value,
                GroupName   = TreeRadioCheckGroupName,
            };
  
            targetItems.Add(treeItem);
            if (item.HasChildren)
            {
                PopulateTreeItems(treeItem.Items, item.Children);
            }
        }
    }

    private void HandleFilterValuesSelected(object? sender, DataGridFilterValuesSelectedEventArgs args)
    {
        Debug.Assert(OwningColumn != null);
        if (OwningColumn.FilterOnClose || args.IsConfirmed)
        {
            FilterRequest?.Invoke(this, new DataGridColumnFilterEventArgs(OwningColumn, args.Values));
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _flyoutStateHelper.NotifyDetachedFromVisualTree();
        UnregisterOwningColumnSubscriptions();
        ClearFlyout();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (OwningColumn != null)
        {
            RegisterOwningColumnSubscriptions(OwningColumn);
        }
        _flyoutStateHelper.NotifyAttachedToVisualTree();
        RefreshFilterFlyoutState();
    }

    protected override void OnClick()
    {
        if (IsEffectivelyEnabled)
        {
            var e = new RoutedEventArgs(ClickEvent);
            RaiseEvent(e);
            var (command, parameter) = (Command, CommandParameter);
            if (!e.Handled && command is not null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
                e.Handled = true;
            }
        }
    }

    private void RegisterOwningColumnSubscriptions(DataGridColumn column)
    {
        if (!ReferenceEquals(_subscribedFilters, column.Filters))
        {
            if (_subscribedFilters != null)
            {
                _subscribedFilters.CollectionChanged -= HandleFiltersChanged;
            }
            _subscribedFilters = column.Filters;
            _subscribedFilters.CollectionChanged += HandleFiltersChanged;
        }

        if (column.OwningGrid is { } grid && !ReferenceEquals(_subscribedGrid, grid))
        {
            UnregisterGrid();
            _subscribedGrid = grid;
            _subscribedGrid.PropertyChanged += HandleOwningGridPropertyChanged;
            RegisterCollectionView(grid.CollectionView);
        }
    }

    private void UnregisterOwningColumnSubscriptions()
    {
        if (_subscribedFilters != null)
        {
            _subscribedFilters.CollectionChanged -= HandleFiltersChanged;
            _subscribedFilters = null;
        }
        UnregisterGrid();
    }

    private void UnregisterGrid()
    {
        if (_subscribedGrid != null)
        {
            _subscribedGrid.PropertyChanged -= HandleOwningGridPropertyChanged;
            _subscribedGrid = null;
        }
        RegisterCollectionView(null);
    }

    private void RegisterCollectionView(IDataGridCollectionView? collectionView)
    {
        if (ReferenceEquals(_subscribedCollectionView, collectionView))
        {
            return;
        }

        if (_subscribedCollectionView?.FilterDescriptions != null)
        {
            _subscribedCollectionView.FilterDescriptions.CollectionChanged -= HandleFilterDescriptionsChanged;
        }

        _subscribedCollectionView = collectionView;
        if (_subscribedCollectionView?.FilterDescriptions != null)
        {
            _subscribedCollectionView.FilterDescriptions.CollectionChanged += HandleFilterDescriptionsChanged;
        }
        UpdateFilterActivatedState();
    }

    private void HandleOwningGridPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == DataGrid.CollectionViewProperty)
        {
            RegisterCollectionView(change.NewValue as IDataGridCollectionView);
        }
    }

    private void HandleFiltersChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var rematerialize = _isFlyoutContentMaterialized;
        ClearFlyoutContent();
        RefreshFilterFlyoutState();
        if (rematerialize)
        {
            MaterializeFlyoutContent();
        }
    }

    private void HandleFilterDescriptionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateFilterActivatedState();
    }

    private void UpdateFilterActivatedState()
    {
        Debug.Assert(OwningColumn != null);
        var collectionView = OwningColumn.OwningGrid?.CollectionView;
        if (collectionView is not null)
        {
            var filterDescription = OwningColumn.GetFilterDescription();
            IsFilterActivated = filterDescription != null;
        }
        else
        {
            IsFilterActivated = false;
        }
    }

    private void ConfigureTreeFlyoutToggleType()
    {
        if (Flyout is DataGridTreeFilterFlyout treeFlyout)
        {
            ConfigureTreeFlyoutToggleType(treeFlyout);
        }
    }

    private void ConfigureTreeFlyoutToggleType(DataGridTreeFilterFlyout treeFlyout)
    {
        treeFlyout.ToggleType = IsMultipleFilterEnabled
            ? ItemToggleType.CheckBox
            : ItemToggleType.Radio;
    }

    private void ClearFlyoutContent()
    {
        if (Flyout is DataGridMenuFilterFlyout menuFlyout)
        {
            menuFlyout.Items.Clear();
        }
        else if (Flyout is DataGridTreeFilterFlyout treeFlyout)
        {
            treeFlyout.Items.Clear();
        }
        _isFlyoutContentMaterialized = false;
    }

    private void ClearFlyout()
    {
        if (Flyout is PopupFlyoutBase { IsOpen: true } openFlyout)
        {
            openFlyout.Hide();
        }

        if (Flyout is DataGridMenuFilterFlyout menuFlyout)
        {
            menuFlyout.FilterValuesSelected -= HandleFilterValuesSelected;
            menuFlyout.ShouldFilterOnPassiveClose = null;
            menuFlyout.Items.Clear();
        }
        else if (Flyout is DataGridTreeFilterFlyout treeFlyout)
        {
            treeFlyout.FilterValuesSelected -= HandleFilterValuesSelected;
            treeFlyout.ShouldFilterOnPassiveClose = null;
            treeFlyout.Items.Clear();
        }

        Flyout                    = null;
        _flyoutStateHelper.Flyout = null;
        _isFlyoutContentMaterialized = false;
    }
}
