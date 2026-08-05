using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal sealed class NavMenuSelectionCoordinator
{
    private NavMenuItem? _latestSelectedItem;

    public void Select(NavMenu menu, NavMenuItem menuItem)
    {
        if (ReferenceEquals(_latestSelectedItem, menuItem))
        {
            return;
        }

        var newItems         = NavMenu.CollectSelectPathItems(menuItem);
        var newSelectedPaths = NavMenu.BuildSelectPathSet(newItems);

        HashSet<NavMenuItem>? oldSelectedPaths = null;
        if (_latestSelectedItem != null)
        {
            var oldItems = NavMenu.CollectSelectPathItems(_latestSelectedItem);
            oldSelectedPaths = NavMenu.BuildSelectPathSet(oldItems);
        }

        if (oldSelectedPaths != null)
        {
            foreach (var oldInSelectPathItem in oldSelectedPaths)
            {
                if (!newSelectedPaths.Contains(oldInSelectPathItem))
                {
                    oldInSelectPathItem.SetCurrentValue(NavMenuItem.IsInSelectedPathProperty, false);
                }
            }
        }

        if (_latestSelectedItem != null)
        {
            var oldParentItem = ResolveSelectionOwner(menu, _latestSelectedItem);
            oldParentItem?.SelectChildItem(_latestSelectedItem, false);
        }

        foreach (var newInSelectPathItem in newSelectedPaths)
        {
            newInSelectPathItem.SetCurrentValue(NavMenuItem.IsInSelectedPathProperty, true);
        }

        var parentItem = ResolveSelectionOwner(menu, menuItem);
        parentItem?.SelectChildItem(menuItem, true);
        _latestSelectedItem = menuItem;
        menu.RaiseNavMenuItemSelected(menuItem);
    }

    public void ClearSelection()
    {
        if (_latestSelectedItem is null)
        {
            return;
        }

        var oldItems = NavMenu.CollectSelectPathItems(_latestSelectedItem);
        foreach (var oldInSelectPathItem in oldItems)
        {
            oldInSelectPathItem.SetCurrentValue(NavMenuItem.IsInSelectedPathProperty, false);
        }

        var ownerMenu = _latestSelectedItem.OwnerMenu;
        var oldParentItem = ownerMenu is null
            ? null
            : ResolveSelectionOwner(ownerMenu, _latestSelectedItem);
        oldParentItem?.SelectChildItem(_latestSelectedItem, false);
        Reset();
    }

    public void Reset()
    {
        _latestSelectedItem = null;
    }

    public void Forget(NavMenuItem menuItem)
    {
        if (ReferenceEquals(_latestSelectedItem, menuItem))
        {
            Reset();
        }
    }

    private static IMenuChildSelectable ResolveSelectionOwner(NavMenu menu, NavMenuItem menuItem)
    {
        return menuItem.SemanticParentItem is not null
            ? menuItem.SemanticParentItem
            : menu;
    }
}
