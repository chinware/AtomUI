using AtomUI.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal abstract class NavMenuInteractionHandlerBase : INavMenuInteractionHandler
{
    private bool _currentPressedIsValid;
    private NavMenuItem? _latestClickedItem;
    private NavMenuItem? _keyboardActiveItem;

    internal INavMenu? Menu { get; private set; }

    public void Attach(NavMenu navMenu) => AttachCore(navMenu);

    public void Detach(NavMenu navMenu) => DetachCore(navMenu);

    public abstract void Select(NavMenuItem menuItem);

    public void ClearSelection()
    {
        (Menu as NavMenu)?.ClearSelectionState();
    }

    internal void AttachCore(INavMenu navMenu)
    {
        if (Menu != null)
        {
            throw new NotSupportedException("NavMenu interaction handler is already attached.");
        }

        Menu                 =  navMenu;
        Menu.PointerPressed  += PointerPressed;
        Menu.PointerReleased += PointerReleased;
        if (Menu is InputElement inputElement)
        {
            inputElement.KeyDown += KeyDown;
        }
        OnAttached(navMenu);
    }

    internal void DetachCore(INavMenu navMenu)
    {
        if (Menu != navMenu)
        {
            throw new NotSupportedException("NavMenu interaction handler is not attached to the navMenu.");
        }

        Menu.PointerPressed  -= PointerPressed;
        Menu.PointerReleased -= PointerReleased;
        if (Menu is InputElement inputElement)
        {
            inputElement.KeyDown -= KeyDown;
        }
        OnDetached(navMenu);

        ClearKeyboardActiveItem();
        Menu = null;
        ResetPressState();
    }

    protected virtual void OnAttached(INavMenu navMenu)
    {
    }

    protected virtual void OnDetached(INavMenu navMenu)
    {
    }

    protected virtual void PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        ResetPressState();

        var sourceControl = e.Source as Control;
        var menuItem      = GetMenuItemCore(sourceControl);
        if (menuItem is null || !menuItem.ItemHeader.IsVisualAncestorOf(sourceControl))
        {
            return;
        }

        _currentPressedIsValid = true;
        _latestClickedItem     = menuItem;
        if (sender is Visual visual &&
            e.GetCurrentPoint(visual).Properties.IsLeftButtonPressed)
        {
            Select(menuItem);
            e.Handled = true;
        }
    }

    protected virtual void PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_latestClickedItem is null || !_currentPressedIsValid)
        {
            return;
        }

        try
        {
            if (e.InitialPressMouseButton == MouseButton.Left)
            {
                Click(_latestClickedItem);
                e.Handled = true;
            }
        }
        finally
        {
            ResetPressState();
        }
    }

    protected virtual void KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Handled ||
            e.KeyModifiers != KeyModifiers.None ||
            Menu is not NavMenu navMenu)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Down:
                HandleDownKey(navMenu);
                e.Handled = true;
                break;

            case Key.Up:
                HandleUpKey(navMenu);
                e.Handled = true;
                break;

            case Key.Right:
                HandleRightKey(navMenu);
                e.Handled = true;
                break;

            case Key.Left:
                HandleLeftKey(navMenu);
                e.Handled = true;
                break;

            case Key.Enter:
            case Key.Space:
                CommitKeyboardActiveItem(navMenu);
                e.Handled = true;
                break;

            case Key.Escape:
                CloseKeyboardActiveBranch(navMenu);
                e.Handled = true;
                break;
        }
    }

    private void HandleDownKey(NavMenu navMenu)
    {
        var activeItem = GetValidKeyboardActiveItem();
        if (navMenu.EffectiveMode == NavMenuMode.Horizontal &&
            activeItem is { IsTopLevel: true, HasSubMenu: true })
        {
            if (TryOpenSubmenuAndActivateFirstChild(navMenu, activeItem))
            {
                return;
            }
        }

        MoveKeyboardActiveItem(navMenu, 1);
    }

    private void HandleUpKey(NavMenu navMenu)
    {
        MoveKeyboardActiveItem(navMenu, -1);
    }

    private void HandleRightKey(NavMenu navMenu)
    {
        var activeItem = GetValidKeyboardActiveItem();
        if (activeItem is null)
        {
            MoveKeyboardActiveItem(navMenu, 1);
            return;
        }

        if (navMenu.EffectiveMode == NavMenuMode.Horizontal && activeItem.IsTopLevel)
        {
            MoveKeyboardActiveItem(navMenu, 1, navMenu);
            return;
        }

        if (activeItem.HasSubMenu &&
            TryOpenSubmenuAndActivateFirstChild(navMenu, activeItem))
        {
            return;
        }

        MoveKeyboardActiveItem(navMenu, 1);
    }

    private void HandleLeftKey(NavMenu navMenu)
    {
        var activeItem = GetValidKeyboardActiveItem();
        if (activeItem is null)
        {
            MoveKeyboardActiveItem(navMenu, -1);
            return;
        }

        if (navMenu.EffectiveMode == NavMenuMode.Horizontal && activeItem.IsTopLevel)
        {
            MoveKeyboardActiveItem(navMenu, -1, navMenu);
            return;
        }

        var parentItem = activeItem.Parent as NavMenuItem;
        if (parentItem is not null)
        {
            if (navMenu.EffectiveMode != NavMenuMode.Inline)
            {
                parentItem.Close();
            }
            SetKeyboardActiveItem(parentItem);
            return;
        }

        if (activeItem.HasSubMenu && activeItem.IsSubMenuOpen)
        {
            activeItem.Close();
            return;
        }

        MoveKeyboardActiveItem(navMenu, -1);
    }

    private void MoveKeyboardActiveItem(NavMenu navMenu, int delta, ItemsControl? forcedOwner = null)
    {
        var activeItem = GetValidKeyboardActiveItem();
        var owner      = forcedOwner ?? ResolveKeyboardNavigationOwner(navMenu, activeItem);
        var items      = navMenu.EffectiveMode == NavMenuMode.Inline && forcedOwner is null
            ? CollectVisibleInlineItems(navMenu)
            : CollectDirectNavigationItems(owner);

        if (items.Count == 0)
        {
            ClearKeyboardActiveItem();
            return;
        }

        var activeIndex = activeItem is null ? -1 : items.IndexOf(activeItem);
        if (activeIndex == -1)
        {
            SetKeyboardActiveItem(items[ResolveInitialKeyboardTargetIndex(navMenu, items, delta)]);
            return;
        }

        SetKeyboardActiveItem(items[WrapIndex(activeIndex + delta, items.Count)]);
    }

    private void CommitKeyboardActiveItem(NavMenu navMenu)
    {
        var activeItem = GetValidKeyboardActiveItem();
        if (activeItem is null)
        {
            MoveKeyboardActiveItem(navMenu, 1);
            return;
        }

        if (activeItem.HasSubMenu)
        {
            if (navMenu.EffectiveMode == NavMenuMode.Inline)
            {
                Select(activeItem);
            }
            else
            {
                TryOpenSubmenuAndActivateFirstChild(navMenu, activeItem);
            }
            return;
        }

        Select(activeItem);
        Click(activeItem);
    }

    private void CloseKeyboardActiveBranch(NavMenu navMenu)
    {
        var activeItem = GetValidKeyboardActiveItem();
        if (activeItem is { HasSubMenu: true, IsSubMenuOpen: true })
        {
            activeItem.Close();
            SetKeyboardActiveItem(activeItem);
            return;
        }

        var parentItem = activeItem?.Parent as NavMenuItem;
        if (parentItem is not null && parentItem.IsSubMenuOpen)
        {
            parentItem.Close();
            SetKeyboardActiveItem(parentItem);
            return;
        }

        var openItem = FindDeepestOpenItem(navMenu);
        if (openItem is not null)
        {
            openItem.Close();
            SetKeyboardActiveItem(openItem);
        }
    }

    private bool TryOpenSubmenuAndActivateFirstChild(NavMenu navMenu, NavMenuItem item)
    {
        if (!item.HasSubMenu)
        {
            return false;
        }

        item.Open();
        navMenu.ExecutePendingContainerLayout(item);

        var children = CollectDirectNavigationItems(item);
        if (children.Count == 0)
        {
            SetKeyboardActiveItem(item);
            return true;
        }

        SetKeyboardActiveItem(children[0]);
        return true;
    }

    private ItemsControl ResolveKeyboardNavigationOwner(NavMenu navMenu, NavMenuItem? activeItem)
    {
        if (activeItem?.Parent is ItemsControl owner &&
            (navMenu.EffectiveMode != NavMenuMode.Horizontal || !activeItem.IsTopLevel))
        {
            return owner;
        }

        return navMenu;
    }

    private List<NavMenuItem> CollectVisibleInlineItems(ItemsControl owner)
    {
        var items = new List<NavMenuItem>();
        CollectVisibleInlineItems(owner, items);
        return items;
    }

    private void CollectVisibleInlineItems(ItemsControl owner, List<NavMenuItem> items)
    {
        var directItems = CollectDirectNavigationItems(owner);
        foreach (var item in directItems)
        {
            items.Add(item);
            if (item.HasSubMenu && item.IsSubMenuOpen)
            {
                CollectVisibleInlineItems(item, items);
            }
        }
    }

    private static int ResolveInitialKeyboardTargetIndex(
        NavMenu navMenu,
        IReadOnlyList<NavMenuItem> items,
        int delta)
    {
        if (navMenu.SelectedItem is not null &&
            TryFindSelectedNavigationItem(navMenu, navMenu.SelectedItem) is { } selectedItem)
        {
            var selectedIndex = IndexOfItem(items, selectedItem);
            if (selectedIndex != -1)
            {
                return WrapIndex(selectedIndex + delta, items.Count);
            }
        }

        return 0;
    }

    private static int IndexOfItem(IReadOnlyList<NavMenuItem> items, NavMenuItem target)
    {
        for (var i = 0; i < items.Count; i++)
        {
            if (ReferenceEquals(items[i], target))
            {
                return i;
            }
        }

        return -1;
    }

    private static NavMenuItem? TryFindSelectedNavigationItem(ItemsControl owner, INavMenuNode selectedNode)
    {
        for (var i = 0; i < owner.ItemCount; i++)
        {
            if (owner.ContainerFromIndex(i) is not NavMenuItem item)
            {
                continue;
            }

            if (IsInteractiveMenuItem(item) &&
                ReferenceEquals(((INavMenuItem)item).Node, selectedNode))
            {
                return item;
            }

            if (item.HasSubMenu && item.IsSubMenuOpen &&
                TryFindSelectedNavigationItem(item, selectedNode) is { } selectedChild)
            {
                return selectedChild;
            }
        }

        return null;
    }

    private static List<NavMenuItem> CollectDirectNavigationItems(ItemsControl owner)
    {
        var items = new List<NavMenuItem>();
        for (var i = 0; i < owner.ItemCount; i++)
        {
            if (owner.ContainerFromIndex(i) is NavMenuItem container &&
                IsInteractiveMenuItem(container))
            {
                items.Add(container);
            }
        }

        return items;
    }

    private NavMenuItem? FindDeepestOpenItem(ItemsControl owner)
    {
        NavMenuItem? deepestOpenItem = null;
        var directItems = CollectDirectNavigationItems(owner);
        foreach (var item in directItems)
        {
            if (!item.IsSubMenuOpen)
            {
                continue;
            }

            deepestOpenItem = FindDeepestOpenItem(item) ?? item;
        }

        return deepestOpenItem;
    }

    private NavMenuItem? GetValidKeyboardActiveItem()
    {
        return IsInteractiveMenuItem(_keyboardActiveItem) ? _keyboardActiveItem : null;
    }

    private void SetKeyboardActiveItem(NavMenuItem item)
    {
        if (ReferenceEquals(_keyboardActiveItem, item))
        {
            return;
        }

        ClearKeyboardActiveItem();
        _keyboardActiveItem = item;
        item.SetCurrentValue(NavMenuItem.IsKeyboardActiveProperty, true);
    }

    private void ClearKeyboardActiveItem()
    {
        if (_keyboardActiveItem is not null)
        {
            _keyboardActiveItem.SetCurrentValue(NavMenuItem.IsKeyboardActiveProperty, false);
            _keyboardActiveItem = null;
        }
    }

    private static bool IsInteractiveMenuItem(NavMenuItem? item)
    {
        return item is { IsVisible: true, IsEffectivelyEnabled: true };
    }

    private static int WrapIndex(int index, int count)
    {
        if (index < 0)
        {
            return count - 1;
        }

        if (index >= count)
        {
            return 0;
        }

        return index;
    }

    protected virtual void Click(INavMenuItem item)
    {
        (item as IClickableControl)?.RaiseClick();
        if (Menu is NavMenu navMenu)
        {
            navMenu.RaiseNavMenuItemClick(item);
        }
    }

    protected static INavMenuItem? FindTopLevelMenuItem(INavMenuItem item)
    {
        if (item.IsTopLevel)
        {
            return item;
        }

        var current = item;
        while (current != null && !current.IsTopLevel)
        {
            current = current.Parent as INavMenuItem;
        }

        return current;
    }

    protected static NavMenuItem? GetMenuItemCore(StyledElement? item)
    {
        NavMenuItem? target  = null;
        var          current = item;
        while (current != null)
        {
            if (current is NavMenuItem menuItem)
            {
                target = menuItem;
                break;
            }

            current = current.Parent;
        }

        return target;
    }

    private void ResetPressState()
    {
        _currentPressedIsValid = false;
        _latestClickedItem     = null;
    }
}
