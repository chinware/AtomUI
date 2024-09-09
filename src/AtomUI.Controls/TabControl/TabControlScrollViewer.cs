using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AtomUI.Controls;

internal class TabControlScrollViewer : BaseTabScrollViewer
{
    #region 内部属性定义

    internal BaseTabControl? TabControl { get; set; }

    #endregion



    protected override Type StyleKeyOverride => typeof(BaseTabScrollViewer);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_menuIndicator is not null) _menuIndicator.Click += HandleMenuIndicator;
    }

    private void HandleMenuIndicator(object? sender, RoutedEventArgs args)
    {
        if (_menuFlyout is null)
            _menuFlyout = new MenuFlyout
            {
                IsShowArrow = false
            };

        if (TabStripPlacement == Dock.Top)
            _menuFlyout.Placement = PlacementMode.BottomEdgeAlignedLeft;
        else if (TabStripPlacement == Dock.Bottom)
            _menuFlyout.Placement = PlacementMode.TopEdgeAlignedLeft;
        else if (TabStripPlacement == Dock.Right)
            _menuFlyout.Placement = PlacementMode.LeftEdgeAlignedBottom;
        else
            _menuFlyout.Placement = PlacementMode.RightEdgeAlignedBottom;

        // 收集没有完全显示的 Tab 列表
        _menuFlyout.Items.Clear();
        if (TabControl is not null)
        {
            for (var i = 0; i < TabControl.ItemCount; i++)
            {
                var itemContainer = TabControl.ContainerFromIndex(i)!;
                if (itemContainer is TabItem tabItem)
                {
                    var needAddToMenu = false;
                    var itemBounds    = itemContainer.Bounds;
                    if (TabStripPlacement == Dock.Top || TabStripPlacement == Dock.Bottom)
                    {
                        var left                                              = Math.Floor(itemBounds.Left  - Offset.X);
                        var right                                             = Math.Floor(itemBounds.Right - Offset.X);
                        if (left < 0 || right > Viewport.Width) needAddToMenu = true;
                    }
                    else
                    {
                        var top    = Math.Floor(itemBounds.Top    - Offset.Y);
                        var bottom = Math.Floor(itemBounds.Bottom - Offset.Y);

                        if (top < 0 || bottom > Viewport.Height) needAddToMenu = true;
                    }

                    if (needAddToMenu)
                    {
                        var menuItem = new TabControlOverflowMenuItem
                        {
                            Header     = tabItem.Header,
                            TabItem    = tabItem,
                            IsClosable = tabItem.IsClosable
                        };
                        menuItem.Click    += HandleMenuItemClicked;
                        menuItem.CloseTab += HandleCloseTabRequest;
                        _menuFlyout.Items.Add(menuItem);
                    }
                }
            }

            if (_menuFlyout.Items.Count > 0) _menuFlyout.ShowAt(_menuIndicator!);
        }
    }

    private void HandleMenuItemClicked(object? sender, RoutedEventArgs args)
    {
        if (TabControl is not null)
            Dispatcher.UIThread.Post(sender =>
            {
                if (sender is TabControlOverflowMenuItem tabControlMenuItem)
                {
                    var tabItem = tabControlMenuItem.TabItem;
                    if (tabItem is not null)
                    {
                        tabItem.BringIntoView();
                        TabControl.SelectedItem = tabItem;
                    }
                }
            }, sender);
    }

    private void HandleCloseTabRequest(object? sender, RoutedEventArgs args)
    {
        if (sender is TabControlOverflowMenuItem tabControlMenuItem)
            if (TabControl is not null)
                if (TabControl.SelectedItem is TabItem selectedItem)
                {
                    if (selectedItem == tabControlMenuItem.TabItem)
                    {
                        var     selectedIndex                   = TabControl.SelectedIndex;
                        object? newSelectedItem                 = null;
                        if (selectedIndex != 0) newSelectedItem = TabControl.Items[--selectedIndex];
                        TabControl.Items.Remove(tabControlMenuItem.TabItem);
                        TabControl.SelectedItem = newSelectedItem;
                    }
                    else
                    {
                        TabControl.Items.Remove(tabControlMenuItem.TabItem);
                    }
                }
    }
}