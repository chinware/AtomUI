using System.Reactive.Disposables;
using AtomUI.Data;
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
    
    private IDisposable? _flyoutBindingDisposable;
    private CompositeDisposable? _itemBindingDisposables;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (MenuIndicator is not null)
        {
            MenuIndicator.Click += HandleMenuIndicatorClicked;
        }
    }

    private void HandleMenuIndicatorClicked(object? sender, RoutedEventArgs args)
    {
        if (MenuFlyout is null)
        {
            MenuFlyout = new MenuFlyout
            {
                IsShowArrow = false,
                ClickHideFlyoutPredicate = ClickHideFlyoutPredicate
            };
            _flyoutBindingDisposable?.Dispose();
            _flyoutBindingDisposable = BindUtils.RelayBind(this, IsMotionEnabledProperty, MenuFlyout, MenuFlyout.IsMotionEnabledProperty);
        }

        if (TabStripPlacement == Dock.Top)
        {
            MenuFlyout.Placement = PlacementMode.BottomEdgeAlignedLeft;
        }
        else if (TabStripPlacement == Dock.Bottom)
        {
            MenuFlyout.Placement = PlacementMode.TopEdgeAlignedLeft;
        }
        else if (TabStripPlacement == Dock.Right)
        {
            MenuFlyout.Placement = PlacementMode.LeftEdgeAlignedBottom;
        }
        else
        {
            MenuFlyout.Placement = PlacementMode.RightEdgeAlignedBottom;
        }

        // 收集没有完全显示的 Tab 列表
        MenuFlyout.Items.Clear();
        if (TabControl is not null)
        {
            _itemBindingDisposables?.Dispose();
            _itemBindingDisposables = new CompositeDisposable(TabControl.ItemCount);
            for (var i = 0; i < TabControl.ItemCount; i++)
            {
                var itemContainer = TabControl.ContainerFromIndex(i)!;
                if (itemContainer is TabItem tabItem)
                {
                    var needAddToMenu = false;
                    var itemBounds    = itemContainer.Bounds;
                    if (TabStripPlacement == Dock.Top || TabStripPlacement == Dock.Bottom)
                    {
                        var left  = Math.Floor(itemBounds.Left - Offset.X);
                        var right = Math.Floor(itemBounds.Right - Offset.X);
                        if (left < 0 || right > Viewport.Width)
                        {
                            needAddToMenu = true;
                        }
                    }
                    else
                    {
                        var top    = Math.Floor(itemBounds.Top - Offset.Y);
                        var bottom = Math.Floor(itemBounds.Bottom - Offset.Y);

                        if (top < 0 || bottom > Viewport.Height)
                        {
                            needAddToMenu = true;
                        }
                    }

                    if (needAddToMenu)
                    {
                        var menuItem = new TabControlOverflowMenuItem
                        {
                            Header     = tabItem.Header,
                            TabItem    = tabItem,
                            IsClosable = tabItem.IsClosable
                        };
                        _itemBindingDisposables.Add(BindUtils.RelayBind(TabControl, MotionAwareControlProperty.IsMotionEnabledProperty, menuItem, MotionAwareControlProperty.IsMotionEnabledProperty));
                        menuItem.Click    += HandleMenuItemClicked;
                        menuItem.CloseTab += HandleCloseTabRequest;
                        MenuFlyout.Items.Add(menuItem);
                    }
                }
            }

            if (MenuFlyout.Items.Count > 0)
            {
                MenuFlyout.ShowAt(MenuIndicator!);
            }
        }
    }

    private void HandleMenuItemClicked(object? sender, RoutedEventArgs args)
    {
        if (TabControl is not null)
        {
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
    }

    private void HandleCloseTabRequest(object? sender, RoutedEventArgs args)
    {
        if (sender is TabControlOverflowMenuItem tabControlMenuItem)
        {
            if (TabControl is not null)
            {
                if (TabControl.SelectedItem is TabItem selectedItem)
                {
                    if (selectedItem == tabControlMenuItem.TabItem)
                    {
                        var     selectedIndex   = TabControl.SelectedIndex;
                        object? newSelectedItem = null;
                        if (selectedIndex != 0)
                        {
                            newSelectedItem = TabControl.Items[--selectedIndex];
                        }

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
    }
}