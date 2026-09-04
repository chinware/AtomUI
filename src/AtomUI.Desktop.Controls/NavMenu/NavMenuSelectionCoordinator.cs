using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal sealed class NavMenuSelectionCoordinator
{
    private INavMenuNode? _appliedSelectedNode;
    private NavMenuItem? _appliedSelectedItem;

    public bool Select(NavMenu menu, NavMenuItem menuItem)
    {
        var selectedNode = ((INavMenuItem)menuItem).Node;
        if (selectedNode is null)
        {
            return false;
        }

        if (ReferenceEquals(_appliedSelectedNode, selectedNode) && menuItem.IsSelected)
        {
            _appliedSelectedItem = menuItem;
            return ReferenceEquals(menu.SelectedItem, selectedNode);
        }

        var newItems         = NavMenu.CollectSelectPathItems(menuItem);
        var newSelectedPaths = NavMenu.BuildSelectPathSet(newItems);
        var oldSelectedNode  = ResolveLatestSelectedNode(menu);
        var oldSelectedItem  = ResolveLatestSelectedItem(menu, oldSelectedNode);
        var oldSelectedPaths = ResolveRealizedSelectedPathItems(menu, oldSelectedNode);

        foreach (var oldInSelectPathItem in oldSelectedPaths)
        {
            if (!newSelectedPaths.Contains(oldInSelectPathItem))
            {
                oldInSelectPathItem.SetCurrentValue(NavMenuItem.IsInSelectedPathProperty, false);
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
        return menu.TryPublishNavMenuItemSelection(menuItem);
    }

    public void ClearSelection(NavMenu menu)
    {
        var oldSelectedNode = ResolveLatestSelectedNode(menu);
        if (oldSelectedNode is null)
        {
            Reset();
            return;
        }

        foreach (var oldInSelectPathItem in ResolveRealizedSelectedPathItems(menu, oldSelectedNode))
        {
            oldInSelectPathItem.SetCurrentValue(NavMenuItem.IsInSelectedPathProperty, false);
        }

        var oldSelectedItem = ResolveLatestSelectedItem(menu, oldSelectedNode);
        if (oldSelectedItem is not null)
        {
            var oldParentItem = ResolveSelectionOwner(menu, oldSelectedItem);
            oldParentItem.SelectChildItem(oldSelectedItem, false);
        }

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

    private INavMenuNode? ResolveLatestSelectedNode(NavMenu menu)
    {
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

        return selectedNode;
    }

    private NavMenuItem? ResolveLatestSelectedItem(NavMenu menu, INavMenuNode? selectedNode)
    {
        if (_appliedSelectedItem is not null)
        {
            return _appliedSelectedItem;
        }

        return selectedNode is not null ? menu.FindRealizedMenuItem(selectedNode) : null;
    }

    private static HashSet<NavMenuItem> ResolveRealizedSelectedPathItems(
        NavMenu menu,
        INavMenuNode? selectedNode)
    {
        var selectedPathItems = new HashSet<NavMenuItem>();
        var current = selectedNode?.ParentNode as INavMenuNode;
        while (current is not null)
        {
            if (menu.FindRealizedMenuItem(current) is { } menuItem)
            {
                selectedPathItems.Add(menuItem);
            }

            current = current.ParentNode as INavMenuNode;
        }

        return selectedPathItems;
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
