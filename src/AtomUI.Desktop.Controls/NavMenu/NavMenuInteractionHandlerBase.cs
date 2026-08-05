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

    public void Forget(NavMenuItem menuItem)
    {
        if (ReferenceEquals(_keyboardActiveItem, menuItem))
        {
            ClearKeyboardActiveItem();
        }

        if (ReferenceEquals(_latestClickedItem, menuItem))
        {
            ResetPressState();
        }

        OnForgotten(menuItem);
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

    protected virtual void OnForgotten(NavMenuItem menuItem)
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
        if (navMenu.EffectiveMode == NavMenuMode.Inline)
        {
            if (activeItem is { HasSubMenu: true, IsSubMenuOpen: false })
            {
                activeItem.Open();
                navMenu.ExecutePendingContainerLayout(activeItem);
            }

            return;
        }

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
        if (navMenu.EffectiveMode == NavMenuMode.Inline)
        {
            if (activeItem is { HasSubMenu: true, IsSubMenuOpen: true })
            {
                activeItem.Close();
            }

            return;
        }

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

        var parentItem = activeItem.SemanticParentItem;
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
            ? EnumerateVisibleInlineNavigationItems(navMenu)
            : EnumerateDirectNavigationItems(owner);

        if (activeItem is not null)
        {
            var adjacentItem = FindAdjacentNavigationItem(
                items,
                activeItem,
                delta,
                out _,
                out var activeItemFound);
            if (activeItemFound)
            {
                SetKeyboardActiveItem(adjacentItem!);
                return;
            }
        }

        var initialItem = ResolveInitialKeyboardTarget(navMenu, items, delta);
        if (initialItem is null)
        {
            ClearKeyboardActiveItem();
            return;
        }

        SetKeyboardActiveItem(initialItem);
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

        var parentItem = activeItem?.SemanticParentItem;
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

        var firstChild = FindFirstNavigationItem(EnumerateDirectNavigationItems(item));
        if (firstChild is null)
        {
            SetKeyboardActiveItem(item);
            return true;
        }

        SetKeyboardActiveItem(firstChild);
        return true;
    }

    private ItemsControl ResolveKeyboardNavigationOwner(NavMenu navMenu, NavMenuItem? activeItem)
    {
        if (activeItem is not null &&
            (navMenu.EffectiveMode != NavMenuMode.Horizontal || !activeItem.IsTopLevel))
        {
            return activeItem.SemanticParentItem is not null
                ? activeItem.SemanticParentItem
                : navMenu;
        }

        return navMenu;
    }

    private static IEnumerable<NavMenuItem> EnumerateVisibleInlineNavigationItems(ItemsControl owner)
    {
        foreach (var item in EnumerateDirectNavigationItems(owner))
        {
            yield return item;
            if (item.HasSubMenu && item.IsSubMenuOpen)
            {
                foreach (var child in EnumerateVisibleInlineNavigationItems(item))
                {
                    yield return child;
                }
            }
        }
    }

    private static NavMenuItem? ResolveInitialKeyboardTarget(
        NavMenu navMenu,
        IEnumerable<NavMenuItem> items,
        int delta)
    {
        if (navMenu.SelectedItem is not null &&
            TryFindSelectedNavigationItem(navMenu, navMenu.SelectedItem) is { } selectedItem)
        {
            var adjacentItem = FindAdjacentNavigationItem(
                items,
                selectedItem,
                delta,
                out var firstItem,
                out var selectedItemFound);
            if (selectedItemFound)
            {
                return adjacentItem;
            }

            return firstItem;
        }

        return FindFirstNavigationItem(items);
    }

    private static NavMenuItem? FindAdjacentNavigationItem(
        IEnumerable<NavMenuItem> items,
        NavMenuItem anchor,
        int delta,
        out NavMenuItem? firstItem,
        out bool anchorFound)
    {
        firstItem = null;
        anchorFound = false;
        NavMenuItem? previousItem = null;
        NavMenuItem? lastItem = null;

        foreach (var item in items)
        {
            firstItem ??= item;
            if (delta > 0 && anchorFound)
            {
                return item;
            }

            if (ReferenceEquals(item, anchor))
            {
                anchorFound = true;
                if (delta < 0 && previousItem is not null)
                {
                    return previousItem;
                }
            }

            previousItem = item;
            lastItem = item;
        }

        return anchorFound
            ? delta < 0 ? lastItem : firstItem
            : null;
    }

    private static NavMenuItem? FindFirstNavigationItem(IEnumerable<NavMenuItem> items)
    {
        foreach (var item in items)
        {
            return item;
        }

        return null;
    }

    private static NavMenuItem? TryFindSelectedNavigationItem(ItemsControl owner, INavMenuNode selectedNode)
    {
        foreach (var item in NavMenuSemanticNavigator.EnumerateDirectItems(owner))
        {
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

    private static IEnumerable<NavMenuItem> EnumerateDirectNavigationItems(ItemsControl owner)
    {
        foreach (var container in NavMenuSemanticNavigator.EnumerateDirectItems(owner))
        {
            if (IsInteractiveMenuItem(container))
            {
                yield return container;
            }
        }
    }

    private NavMenuItem? FindDeepestOpenItem(ItemsControl owner)
    {
        NavMenuItem? deepestOpenItem = null;
        foreach (var item in EnumerateDirectNavigationItems(owner))
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
        if (item is not { IsEffectivelyVisible: true, IsEffectivelyEnabled: true })
        {
            return false;
        }

        var parentItem = item.SemanticParentItem;
        while (parentItem is not null)
        {
            if (!parentItem.IsSubMenuOpen)
            {
                return false;
            }

            parentItem = parentItem.SemanticParentItem;
        }

        return true;
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
