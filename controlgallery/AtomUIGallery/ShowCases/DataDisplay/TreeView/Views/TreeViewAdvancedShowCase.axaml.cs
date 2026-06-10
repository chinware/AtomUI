using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia.Interactivity;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.TreeView;

public partial class TreeViewAdvancedShowCase : GalleryReactiveUserControl<TreeViewViewModel>
{
    private TreeViewItem? _contextMenuTargetItem;

    public TreeViewAdvancedShowCase()
    {
        InitializeComponent();
    }

    private void HandleFilterItemsSourceTreeClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is SearchEdit searchEdit)
        {
            SearchTreeViewByItemsSource.FilterValue = searchEdit.Text?.Trim();
        }
    }

    private void HandleFilterTreeClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is SearchEdit searchEdit)
        {
            SearchTreeView.FilterValue = searchEdit.Text?.Trim();
        }
    }

    private void HandleContextMenuTreeItemContextMenuRequest(object? sender, TreeItemContextMenuEventArgs e)
    {
        _contextMenuTargetItem = e.ViewItem;
        if (ContextMenuTree.Resources.TryGetValue("TreeItemContextMenu", out var resource) &&
            resource is MenuFlyout flyout)
        {
            flyout.ShowAt(e.ViewItem);
        }
    }

    private void HandleContextMenuNewNodeClick(object? sender, RoutedEventArgs e)
    {
        if (_contextMenuTargetItem is null)
        {
            return;
        }

        var header = _contextMenuTargetItem.Header?.ToString() ??
                     TreeViewShowCase.Lang(TreeViewShowCaseLangResourceKind.P2HeaderNodeFallback, "node");
        var newItem = new TreeViewItem
        {
            Header = string.Format(
                TreeViewShowCase.Lang(TreeViewShowCaseLangResourceKind.P2HeaderNewNodeFormat, "{0} / new ({1})"),
                header,
                _contextMenuTargetItem.Items.Count + 1)
        };
        _contextMenuTargetItem.Items.Add(newItem);
        _contextMenuTargetItem.IsExpanded = true;
    }

    private void HandleContextMenuRenameClick(object? sender, RoutedEventArgs e)
    {
        if (_contextMenuTargetItem is null)
        {
            return;
        }

        var header = _contextMenuTargetItem.Header?.ToString() ??
                     TreeViewShowCase.Lang(TreeViewShowCaseLangResourceKind.P2HeaderNodeFallback, "node");
        _contextMenuTargetItem.Header = string.Format(
            TreeViewShowCase.Lang(TreeViewShowCaseLangResourceKind.P2HeaderRenamedFormat, "{0} (renamed)"),
            header);
    }

    private void HandleContextMenuDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (_contextMenuTargetItem is null)
        {
            return;
        }

        if (_contextMenuTargetItem.Parent is TreeViewItem parentItem)
        {
            parentItem.Items.Remove(_contextMenuTargetItem);
        }
        else if (_contextMenuTargetItem.Parent is AtomUITreeView parentTree)
        {
            parentTree.Items.Remove(_contextMenuTargetItem);
        }
        _contextMenuTargetItem = null;
    }
}
