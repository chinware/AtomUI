using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal sealed class NavMenuSelectionCoordinator
{
    private NavMenuItem? _latestSelectedItem;

    public void Select(INavMenu? menu, NavMenuItem menuItem)
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
            var oldParentItem = ItemsControl.ItemsControlFromItemContainer(_latestSelectedItem) as IMenuChildSelectable;
            oldParentItem?.SelectChildItem(_latestSelectedItem, false);
        }

        foreach (var newInSelectPathItem in newSelectedPaths)
        {
            newInSelectPathItem.SetCurrentValue(NavMenuItem.IsInSelectedPathProperty, true);
        }

        var parentItem = ItemsControl.ItemsControlFromItemContainer(menuItem) as IMenuChildSelectable;
        parentItem?.SelectChildItem(menuItem, true);
        _latestSelectedItem = menuItem;
        (menu as NavMenu)?.RaiseNavMenuItemSelected(menuItem);
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

        var oldParentItem = ItemsControl.ItemsControlFromItemContainer(_latestSelectedItem) as IMenuChildSelectable;
        oldParentItem?.SelectChildItem(_latestSelectedItem, false);
        Reset();
    }

    public void Reset()
    {
        _latestSelectedItem = null;
    }
}
