using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

using AvaloniaMenu = Avalonia.Controls.Menu;

public class Menu : AvaloniaMenu,
                    ICustomizableSizeTypeAware,
                    IMotionAwareControl,
                    IScrollAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<CustomizableSizeType> SizeTypeProperty =
        CustomizableSizeTypeControlProperty.SizeTypeProperty.AddOwner<Menu>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Menu>();

    public static readonly StyledProperty<bool> IsScrollEnabledProperty =
        ScrollAwareControlProperty.IsScrollEnabledProperty.AddOwner<Menu>();

    public static readonly StyledProperty<int> DisplayPageSizeProperty =
        AvaloniaProperty.Register<Menu, int>(nameof(DisplayPageSize), 10);

    public static readonly StyledProperty<bool> ShouldUseOverlayPopupProperty =
        Flyout.ShouldUseOverlayPopupProperty.AddOwner<Menu>();

    internal static readonly StyledProperty<bool> IsPopupPinnedOpenProperty =
        Popup.IsPopupPinnedOpenProperty.AddOwner<Menu>();

    public CustomizableSizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsScrollEnabled
    {
        get => GetValue(IsScrollEnabledProperty);
        set => SetValue(IsScrollEnabledProperty, value);
    }

    public int DisplayPageSize
    {
        get => GetValue(DisplayPageSizeProperty);
        set => SetValue(DisplayPageSizeProperty, value);
    }

    public bool ShouldUseOverlayPopup
    {
        get => GetValue(ShouldUseOverlayPopupProperty);
        set => SetValue(ShouldUseOverlayPopupProperty, value);
    }

    internal bool IsPopupPinnedOpen
    {
        get => GetValue(IsPopupPinnedOpenProperty);
        set => SetCurrentValue(IsPopupPinnedOpenProperty, value);
    }

    #endregion

    private bool _isClosing;
    private bool _isClosingForLifecycle;
    private bool _isSyncingDetachedTitleBarRadioGroup;
    private IDisposable? _detachedTitleBarPopupDismissRoot;
    private MenuItem? _pinnedOpenMenuItem;

    static Menu()
    {
        AutoScrollToSelectedItemProperty.OverrideDefaultValue<Menu>(false);
    }

    public Menu()
        : base(new DefaultMenuInteractionHandler(false))
    {
        AddHandler(MenuItem.ClickEvent, RelayDetachedTitleBarPopupClickToHostWindow);
        AddHandler(MenuItem.IsCheckStateChangedEvent, SyncDetachedTitleBarRadioGroup);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        if (item is MenuSeparatorData)
        {
            return new MenuSeparator();
        }

        if (item is MenuItemGroupData)
        {
            return new MenuItemGroup();
        }

        return new MenuItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        if (item is MenuItem or MenuSeparator or MenuItemGroup)
        {
            recycleKey = null;
            return false;
        }

        recycleKey = DefaultRecycleKey;
        return true;
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is MenuItem menuItem)
        {
            if (item != null && item is not Visual)
            {
                if (!menuItem.IsSet(MenuItem.HeaderProperty))
                {
                    menuItem.SetCurrentValue(MenuItem.HeaderProperty, item);
                }

                if (item is IMenuItemData menuItemData)
                {
                    if (!menuItem.IsSet(MenuItem.IconProperty))
                    {
                        menuItem.SetCurrentValue(MenuItem.IconProperty, menuItemData.Icon);
                    }

                    if (menuItem.ItemKey == null)
                    {
                        menuItem.ItemKey = menuItemData.ItemKey;
                    }

                    if (!menuItem.IsSet(MenuItem.IsEnabledProperty))
                    {
                        menuItem.SetCurrentValue(IsEnabledProperty, menuItemData.IsEnabled);
                    }

                    if (!menuItem.IsSet(MenuItem.InputGestureProperty))
                    {
                        menuItem.SetCurrentValue(MenuItem.InputGestureProperty, menuItemData.InputGesture);
                    }
                }
            }

            if (ItemTemplate != null)
            {
                menuItem[!MenuItem.HeaderTemplateProperty] = this[!ItemTemplateProperty];
            }

            menuItem[!MenuItem.DisplayPageSizeProperty]       = this[!DisplayPageSizeProperty];
            menuItem[!MenuItem.ItemTemplateProperty]           = this[!ItemTemplateProperty];
            menuItem[!SizeTypeProperty]                        = this[!SizeTypeProperty];
            menuItem[!IsMotionEnabledProperty]                 = this[!IsMotionEnabledProperty];
            menuItem[!MenuItem.ShouldUseOverlayPopupProperty]  = this[!ShouldUseOverlayPopupProperty];

            PrepareMenuItem(menuItem, item, index);
        }
        else if (container is MenuSeparator menuSeparator)
        {
            menuSeparator.Orientation = Orientation.Vertical;
        }
        else if (container is MenuItemGroup)
        {
            // 分组标题与子项的样式由 MenuItemGroup 自身的模板与容器逻辑处理。
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container),
                "The container type is incorrect, it must be type MenuItem or MenuSeparator.");
        }
    }

    protected override void ClearContainerForItemOverride(Control container)
    {
        if (container is MenuItem menuItem)
        {
            menuItem.IsPopupPinnedOpen = false;
            menuItem.CloseForLifecycle();
        }

        base.ClearContainerForItemOverride(container);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsPopupPinnedOpenProperty)
        {
            if (change.GetNewValue<bool>())
            {
                EnsurePinnedOpenMenuItem();
            }
            else
            {
                ClearPinnedOpenMenuItem();
            }
        }
        else if (change.Property == SelectedIndexProperty && IsPopupPinnedOpen && !_isClosingForLifecycle)
        {
            EnsurePinnedOpenMenuItem();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        EnsurePinnedOpenMenuItem();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (IsPopupPinnedOpen)
        {
            ClosePinnedForLifecycle();
        }

        base.OnDetachedFromVisualTree(e);
    }

    private void EnsurePinnedOpenMenuItem()
    {
        if (!IsPopupPinnedOpen || !this.IsAttachedToVisualTree())
        {
            return;
        }

        MenuItem? menuItem = null;
        var index = SelectedIndex;
        if (index >= 0 && ContainerFromIndex(index) is MenuItem selectedItem && selectedItem.HasSubMenu)
        {
            menuItem = selectedItem;
        }
        else
        {
            for (var i = 0; i < ItemCount; i++)
            {
                if (ContainerFromIndex(i) is MenuItem candidate && candidate.HasSubMenu)
                {
                    menuItem = candidate;
                    index = i;
                    break;
                }
            }
        }

        if (menuItem == null)
        {
            return;
        }

        if (!ReferenceEquals(_pinnedOpenMenuItem, menuItem))
        {
            if (_pinnedOpenMenuItem != null)
            {
                _pinnedOpenMenuItem.IsPopupPinnedOpen = false;
                _pinnedOpenMenuItem.CloseForLifecycle();
            }
            _pinnedOpenMenuItem = menuItem;
        }

        SetCurrentValue(SelectedIndexProperty, index);
        menuItem.IsPopupPinnedOpen = true;
    }

    private void ClearPinnedOpenMenuItem()
    {
        if (_pinnedOpenMenuItem == null)
        {
            return;
        }

        _pinnedOpenMenuItem.IsPopupPinnedOpen = false;
        _pinnedOpenMenuItem = null;
    }

    protected virtual void PrepareMenuItem(MenuItem menuItem, object? item, int index)
    {
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        ConfigureItemContainerTheme(false);
        ConfigureDetachedTitleBarPopupDismissRoot();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        DetachedTitleBarPopupSupport.ClearDismissRoot(ref _detachedTitleBarPopupDismissRoot);
        base.OnDetachedFromLogicalTree(e);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureDetachedTitleBarPopupDismissRoot();
    }

    private void ConfigureItemContainerTheme(bool force)
    {
        if (Theme == null || force)
        {
            if (Application.Current != null)
            {
                if (Application.Current.TryFindResource("TopLevelMenuItemTheme", out var resource))
                {
                    if (resource is ControlTheme theme)
                    {
                        ItemContainerTheme = theme;
                    }
                }
            }
        }
    }

    public override void Close()
    {
        if (IsPopupPinnedOpen)
        {
            return;
        }

        if (InteractionHandler is DefaultMenuInteractionHandler interactionHandler)
        {
            interactionHandler.CancelPendingHoverOperations();
        }

        if (!IsOpen || _isClosing)
        {
            return;
        }

        _isClosing = true;
        if (IsMotionEnabled)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                for (var i = 0; i < ItemCount; i++)
                {
                    var container = ContainerFromIndex(i);
                    if (container is MenuItem menuItem)
                    {
                        await menuItem.CloseItemAsync();
                    }
                }

                HandleMenuClosed();
                _isClosing = false;
            });
        }
        else
        {
            for (var i = 0; i < ItemCount; i++)
            {
                var container = ContainerFromIndex(i);
                if (container is MenuItem menuItem)
                {
                    menuItem.Close();
                }
            }

            HandleMenuClosed();
            _isClosing = false;
        }
    }

    internal void CloseImmediately()
    {
        if (IsPopupPinnedOpen)
        {
            return;
        }

        if (InteractionHandler is DefaultMenuInteractionHandler interactionHandler)
        {
            interactionHandler.CancelPendingHoverOperations();
        }

        if (!IsOpen && !_isClosing)
        {
            return;
        }

        _isClosing = false;
        for (var i = 0; i < ItemCount; i++)
        {
            var container = ContainerFromIndex(i);
            if (container is MenuItem menuItem)
            {
                menuItem.Close();
            }
        }

        HandleMenuClosed();
    }

    private void ClosePinnedForLifecycle()
    {
        _isClosingForLifecycle = true;
        try
        {
            if (InteractionHandler is DefaultMenuInteractionHandler interactionHandler)
            {
                interactionHandler.CancelPendingHoverOperations();
            }

            _isClosing = false;
            ClearPinnedOpenMenuItem();

            for (var i = 0; i < ItemCount; i++)
            {
                if (ContainerFromIndex(i) is MenuItem menuItem)
                {
                    menuItem.CloseForLifecycle();
                }
            }

            if (IsOpen)
            {
                HandleMenuClosed();
            }
            else
            {
                SetCurrentValue(SelectedIndexProperty, -1);
            }
        }
        finally
        {
            _isClosingForLifecycle = false;
        }
    }

    private void HandleMenuClosed()
    {
        IsOpen        = false;
        SelectedIndex = -1;
        RaiseEvent(new RoutedEventArgs
        {
            RoutedEvent = ClosedEvent,
            Source      = this
        });
    }

    private void ConfigureDetachedTitleBarPopupDismissRoot()
    {
        _detachedTitleBarPopupDismissRoot =
            DetachedTitleBarPopupSupport.UpdateDismissRoot(
                this,
                _detachedTitleBarPopupDismissRoot,
                () => IsOpen,
                Close,
                IsInteractionInsideMenu);
    }

    // Title-bar overlay menus are outside the host Window's visual tree, so the
    // default menu root cannot relay clicks or manage radio groups for this case.
    private void RelayDetachedTitleBarPopupClickToHostWindow(object? sender, RoutedEventArgs e)
    {
        if (e.Source is not MenuItem ||
            !DetachedTitleBarPopupSupport.TryResolveHostWindow(this, out var hostWindow))
        {
            return;
        }

        var relayedArgs = new RoutedEventArgs(MenuItem.ClickEvent)
        {
            Source = e.Source
        };
        hostWindow.RaiseRoutedEventFromOverlay((MenuItem)e.Source, relayedArgs);
        e.Handled = relayedArgs.Handled;
    }

    private void SyncDetachedTitleBarRadioGroup(object? sender, RoutedEventArgs e)
    {
        if (_isSyncingDetachedTitleBarRadioGroup ||
            e.Source is not MenuItem checkedItem ||
            !checkedItem.IsChecked ||
            checkedItem.ToggleType != MenuItemToggleType.Radio ||
            !DetachedTitleBarPopupSupport.TryResolveHostWindow(this, out _))
        {
            return;
        }

        _isSyncingDetachedTitleBarRadioGroup = true;
        try
        {
            if (string.IsNullOrEmpty(checkedItem.GroupName))
            {
                UncheckSiblingRadioItems(checkedItem);
            }
            else
            {
                UncheckNamedRadioGroup(checkedItem);
            }
        }
        finally
        {
            _isSyncingDetachedTitleBarRadioGroup = false;
        }
    }

    private static void UncheckSiblingRadioItems(MenuItem checkedItem)
    {
        var parent = ((ILogical)checkedItem).LogicalParent;
        if (parent is null)
        {
            return;
        }

        foreach (var sibling in parent.LogicalChildren)
        {
            if (sibling is MenuItem menuItem &&
                !ReferenceEquals(menuItem, checkedItem) &&
                menuItem.ToggleType == MenuItemToggleType.Radio &&
                string.IsNullOrEmpty(menuItem.GroupName) &&
                menuItem.IsChecked)
            {
                menuItem.SetCurrentValue(MenuItem.IsCheckedProperty, false);
            }
        }
    }

    private void UncheckNamedRadioGroup(MenuItem checkedItem)
    {
        foreach (var menuItem in EnumerateMenuItems(this))
        {
            if (!ReferenceEquals(menuItem, checkedItem) &&
                menuItem.ToggleType == MenuItemToggleType.Radio &&
                menuItem.GroupName == checkedItem.GroupName &&
                menuItem.IsChecked)
            {
                menuItem.SetCurrentValue(MenuItem.IsCheckedProperty, false);
            }
        }
    }

    private static IEnumerable<MenuItem> EnumerateMenuItems(ILogical owner)
    {
        foreach (var child in owner.LogicalChildren)
        {
            if (child is MenuItem menuItem)
            {
                yield return menuItem;
                foreach (var descendant in EnumerateMenuItems(menuItem))
                {
                    yield return descendant;
                }
            }
        }
    }

    private bool IsInteractionInsideMenu(ILogical control)
    {
        if (this.IsLogicalAncestorOf(control))
        {
            return true;
        }

        var current = control as StyledElement;
        while (current is not null)
        {
            if (ReferenceEquals(current, this))
            {
                return true;
            }

            current = current.Parent;
        }

        return false;
    }
}
