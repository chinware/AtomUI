using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal sealed class NavMenuSelectionCoordinator
{
    private INavMenuNode? _appliedSelectedNode;
    private NavMenuItem? _appliedSelectedItem;

    public void Select(NavMenu menu, NavMenuItem menuItem)
    {
        var selectedNode = ((INavMenuItem)menuItem).Node;
        if (selectedNode is null)
        {
            return;
        }

        if (ReferenceEquals(_appliedSelectedNode, selectedNode) && menuItem.IsSelected)
        {
            _appliedSelectedItem = menuItem;
            return;
        }

        var newItems         = NavMenu.CollectSelectPathItems(menuItem);
        var newSelectedPaths = NavMenu.BuildSelectPathSet(newItems);
        var oldSelectedItem  = ResolveLatestSelectedItem(menu);

        HashSet<NavMenuItem>? oldSelectedPaths = null;
        if (oldSelectedItem != null)
        {
            var oldItems = NavMenu.CollectSelectPathItems(oldSelectedItem);
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

        if (oldSelectedItem != null)
        {
            var oldParentItem = ResolveSelectionOwner(menu, oldSelectedItem);
            oldParentItem.SelectChildItem(oldSelectedItem, false);
        }

        foreach (var newInSelectPathItem in newSelectedPaths)
        {
            newInSelectPathItem.SetCurrentValue(NavMenuItem.IsInSelectedPathProperty, true);
        }

        var parentItem = ResolveSelectionOwner(menu, menuItem);
        parentItem.SelectChildItem(menuItem, true);
        _appliedSelectedNode = selectedNode;
        _appliedSelectedItem = menuItem;
        menu.RaiseNavMenuItemSelected(menuItem);
    }

    public void ClearSelection(NavMenu menu)
    {
        var oldSelectedItem = ResolveLatestSelectedItem(menu);
        if (oldSelectedItem is null)
        {
            Reset();
            return;
        }

        var oldItems = NavMenu.CollectSelectPathItems(oldSelectedItem);
        foreach (var oldInSelectPathItem in oldItems)
        {
            oldInSelectPathItem.SetCurrentValue(NavMenuItem.IsInSelectedPathProperty, false);
        }

        var oldParentItem = ResolveSelectionOwner(menu, oldSelectedItem);
        oldParentItem.SelectChildItem(oldSelectedItem, false);
        Reset();
    }

    public void PrepareContainer(NavMenu menu, NavMenuItem menuItem)
    {
        var node = ((INavMenuItem)menuItem).Node;
        if (node is null)
        {
            return;
        }

        var selectedNode = _appliedSelectedNode ?? menu.SelectedItem;
        var isSelected = ReferenceEquals(node, selectedNode);
        menuItem.SetCurrentValue(NavMenuItem.IsSelectedProperty, isSelected);
        menuItem.SetCurrentValue(
            NavMenuItem.IsInSelectedPathProperty,
            !isSelected && IsAncestorOf(node, selectedNode));

        if (isSelected && ReferenceEquals(node, _appliedSelectedNode))
        {
            _appliedSelectedItem = menuItem;
        }
    }

    public void Forget(NavMenuItem menuItem)
    {
        if (ReferenceEquals(_appliedSelectedItem, menuItem))
        {
            _appliedSelectedItem = null;
        }
    }

    private NavMenuItem? ResolveLatestSelectedItem(NavMenu menu)
    {
        if (_appliedSelectedItem is not null)
        {
            return _appliedSelectedItem;
        }

        var selectedNode = _appliedSelectedNode ?? menu.SelectedItem;
        if (selectedNode is null)
        {
            return null;
        }

        if (!BelongsToMenu(menu, selectedNode))
        {
            Reset();
            return null;
        }

        return menu.FindRealizedMenuItem(selectedNode);
    }

    private static bool BelongsToMenu(NavMenu menu, INavMenuNode node)
    {
        var rootNode = node;
        while (rootNode.ParentNode is INavMenuNode parentNode)
        {
            rootNode = parentNode;
        }

        return NavMenuEntryGraph.ContainsDirectNode(menu.Items, rootNode);
    }

    private void Reset()
    {
        _appliedSelectedNode = null;
        _appliedSelectedItem = null;
    }

    private static bool IsAncestorOf(INavMenuNode candidate, INavMenuNode? selectedNode)
    {
        var current = selectedNode?.ParentNode as INavMenuNode;
        while (current is not null)
        {
            if (ReferenceEquals(current, candidate))
            {
                return true;
            }

            current = current.ParentNode as INavMenuNode;
        }

        return false;
    }

    private static IMenuChildSelectable ResolveSelectionOwner(NavMenu menu, NavMenuItem menuItem)
    {
        return menuItem.SemanticParentItem is not null
            ? menuItem.SemanticParentItem
            : menu;
    }
}
