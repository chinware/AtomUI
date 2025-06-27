using System.Diagnostics;
using AtomUI.Controls.DataGridLang;
using AtomUI.Data;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

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
    
    public static readonly StyledProperty<bool> FilterMultipleProperty =
        AvaloniaProperty.Register<DataGridFilterIndicator, bool>(nameof(FilterMultiple));

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
    
    public bool FilterMultiple
    {
        get => GetValue(FilterMultipleProperty);
        set => SetValue(FilterMultipleProperty, value);
    }
    #endregion
    
    private DataGridColumn? _owningColumn;
    private static int _indicatorSeed = 0;
    private string _treeRadioCheckGroupName;

    internal DataGridColumn? OwningColumn
    {
        get => _owningColumn;
        set
        {
            _owningColumn = value;
            if (_owningColumn != null)
            {
                FilterMode     = _owningColumn.FilterMode;
                FilterMultiple = _owningColumn.FilterMultiple;
            }
        }
    }

    private readonly FlyoutStateHelper _flyoutStateHelper;

    public DataGridFilterIndicator()
    {
        _flyoutStateHelper = new FlyoutStateHelper
        {
            AnchorTarget = this,
            TriggerType  = FlyoutTriggerType.Click,
            ClickHideFlyoutPredicate = (provider, args) =>
            {
                if (provider.PopupHost != args.Root)
                {
                    if (args.Root is Control control)
                    {
                        if (control.Parent is Visual visualParent)
                        {
                            var topLevel = TopLevel.GetTopLevel(visualParent);
                            if (provider.PopupHost == topLevel)
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }

                return false;
            }
        };
        _treeRadioCheckGroupName = $"tree-{nameof(DataGridFilterIndicator)}-{_indicatorSeed++}";
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (Icon is null)
        {
            SetValue(IconProperty, AntDesignIconPackage.FilterFilled(), BindingPriority.Template);
        }

        CreateFlyout();
        base.OnApplyTemplate(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs changed)
    {
        base.OnPropertyChanged(changed);
        if (changed.Property == FilterModeProperty)
        {
            CreateFlyout();
        }
    }

    private void CreateFlyout()
    {
        Debug.Assert(OwningColumn is not null);
        Debug.Assert(OwningColumn.OwningGrid is not null);
        var owningGrid = OwningColumn.OwningGrid;
        if (FilterMode == DataGridFilterMode.Menu && Flyout is not DataGridMenuFilterFlyout)
        {
            var menuFlyout = new DataGridMenuFilterFlyout()
            {
                IsShowArrow               = false,
                Placement                 = PlacementMode.BottomEdgeAlignedRight,
                IsDetectMouseClickEnabled = false
            };
            BindUtils.RelayBind(owningGrid, MotionAwareControlProperty.IsMotionEnabledProperty, menuFlyout,
                MotionAwareControlProperty.IsMotionEnabledProperty);
            var menuItems = BuildMenuItems(OwningColumn.Filters.ToList());
            foreach (var menuItem in menuItems)
            {
                menuFlyout.Items.Add(menuItem);
            }

            Flyout                    = menuFlyout;
            _flyoutStateHelper.Flyout = menuFlyout;
        }
        else if (FilterMode == DataGridFilterMode.Tree && Flyout is not DataGridTreeFilterFlyout)
        {
            var treeFlyout = new DataGridTreeFilterFlyout()
            {
                IsShowArrow               = false,
                Placement                 = PlacementMode.BottomEdgeAlignedRight,
                IsDetectMouseClickEnabled = false
            };
            BindUtils.RelayBind(this, FilterMultipleProperty, treeFlyout, DataGridTreeFilterFlyout.ToggleTypeProperty, (v) =>
            {
                return v ? ItemToggleType.CheckBox : ItemToggleType.Radio;
            });
            BindUtils.RelayBind(owningGrid, MotionAwareControlProperty.IsMotionEnabledProperty, treeFlyout,
                MotionAwareControlProperty.IsMotionEnabledProperty);
            var treeItems         = BuildTreeItems(OwningColumn.Filters.ToList());
            if (FilterMultiple)
            {
                var selectAllTreeItem = new DataGridFilterTreeItem();
                LanguageResourceBinder.CreateBinding(selectAllTreeItem, DataGridFilterTreeItem.HeaderProperty,
                    DataGridLangResourceKey.SelectAllFilterItems);
              
                foreach (var treeItem in treeItems)
                {
                    selectAllTreeItem.Items.Add(treeItem);
                }
                treeFlyout.Items.Add(selectAllTreeItem);
            }
            else
            {
                foreach (var treeItem in treeItems)
                {
                    treeFlyout.Items.Add(treeItem);
                }
            }
            
            Flyout                    = treeFlyout;
            _flyoutStateHelper.Flyout = treeFlyout;
        }
    }

    private List<DataGridFilterMenuItem> BuildMenuItems(List<DataGridFilterItem> filterItems)
    {
        var menuItems = new List<DataGridFilterMenuItem>();
        foreach (var item in filterItems)
        {
            var menuItem = new DataGridFilterMenuItem()
            {
                Header           = item.Text,
                FilterValue      = item.Value,
                StaysOpenOnClick = true
            };
            BindUtils.RelayBind(this, FilterMultipleProperty, menuItem, MenuItem.ToggleTypeProperty, (v) =>
            {
                return v ? MenuItemToggleType.CheckBox : MenuItemToggleType.Radio;
            }, BindingPriority.Template);
            menuItems.Add(menuItem);
            if (item.Children.Count > 0)
            {
                var childItems = BuildMenuItems(item.Children);
                foreach (var childItem in childItems)
                {
                    menuItem.Items.Add(childItem);
                }
            }
        }

        return menuItems;
    }

    private List<DataGridFilterTreeItem> BuildTreeItems(List<DataGridFilterItem> filterItems)
    {
        var treeItems = new List<DataGridFilterTreeItem>();
        foreach (var item in filterItems)
        {
            var treeItem = new DataGridFilterTreeItem()
            {
                Header      = item.Text,
                FilterValue = item.Value,
                GroupName   = _treeRadioCheckGroupName,
            };
  
            treeItems.Add(treeItem);
            if (item.Children.Count > 0)
            {
                var childItems = BuildTreeItems(item.Children);
                foreach (var childItem in childItems)
                {
                    treeItem.Items.Add(childItem);
                }
            }
        }

        return treeItems;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _flyoutStateHelper.NotifyDetachedFromVisualTree();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _flyoutStateHelper.NotifyAttachedToVisualTree();
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
}