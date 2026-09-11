using System.Reactive.Disposables;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

internal class TabControlScrollViewer : BaseTabScrollViewer
{
    #region 内部属性定义

    internal BaseTabControl? TabControl { get; set; }

    #endregion

    protected override Type StyleKeyOverride => typeof(BaseTabScrollViewer);
    
    private IDisposable? _flyoutBindingDisposable;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (MenuIndicator is not null)
        {
            MenuIndicator.Click -= HandleMenuIndicatorClicked;
        }

        base.OnApplyTemplate(e);
        if (MenuIndicator is not null)
        {
            MenuIndicator.Click += HandleMenuIndicatorClicked;
        }
    }

    private void HandleMenuIndicatorClicked(object? sender, RoutedEventArgs args)
    {
        OpenMenuFlyout();
    }

    private protected override void OpenMenuFlyout()
    {
        if (MenuFlyout != null)
        {
            return;
        }
        MenuFlyout = new MenuFlyout
        {
            IsArrowVisible           = false,
            ShouldUseOverlayPopup = true,
            IsLightDismissEnabled = true,
        };
        MenuFlyout.Closed += HandleMenuFlyoutClosed;
        _flyoutBindingDisposable?.Dispose();
        _flyoutBindingDisposable = new CompositeDisposable(
            BindUtils.RelayBind(this, IsMotionEnabledProperty, MenuFlyout, MenuFlyout.IsMotionEnabledProperty),
            BindUtils.RelayBind(this, IsPopupPinnedOpenProperty, MenuFlyout, Flyout.IsPopupPinnedOpenProperty));

        if (TabStripPlacement == Dock.Top)
        {
            MenuFlyout.RequestedPlacement = PlacementMode.BottomEdgeAlignedLeft;
        }
        else if (TabStripPlacement == Dock.Bottom)
        {
            MenuFlyout.RequestedPlacement = PlacementMode.TopEdgeAlignedLeft;
        }
        else if (TabStripPlacement == Dock.Right)
        {
            MenuFlyout.RequestedPlacement = PlacementMode.LeftEdgeAlignedBottom;
        }
        else
        {
            MenuFlyout.RequestedPlacement = PlacementMode.RightEdgeAlignedBottom;
        }

        // 收集没有完全显示的 Tab 列表
        MenuFlyout.Items.Clear();

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
                            Header         = tabItem.Header,
                            HeaderTemplate = tabItem.HeaderTemplate,
                            TabItem        = tabItem,
                            IsClosable     = tabItem.IsClosable
                        };
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
            else
            {
                HandleMenuFlyoutClosed(MenuFlyout, EventArgs.Empty);
            }
        }
    }

    private protected override void CloseMenuFlyout()
    {
        if (MenuFlyout is { } flyout)
        {
            flyout.CloseForLifecycle();
            HandleMenuFlyoutClosed(flyout, EventArgs.Empty);
        }
    }

    private void HandleMenuFlyoutClosed(object? sender, EventArgs args)
    {
        var flyout = sender as MenuFlyout ?? MenuFlyout;
        if (flyout is null)
        {
            return;
        }

        flyout.Closed -= HandleMenuFlyoutClosed;
        foreach (var flyoutItem in flyout.Items)
        {
            if (flyoutItem is not TabControlOverflowMenuItem item)
            {
                continue;
            }

            item.Click    -= HandleMenuItemClicked;
            item.CloseTab -= HandleCloseTabRequest;
        }
        flyout.Items.Clear();

        if (ReferenceEquals(MenuFlyout, flyout))
        {
            _flyoutBindingDisposable?.Dispose();
            _flyoutBindingDisposable = null;
            MenuFlyout = null;
        }
        flyout.SetCurrentValue(Flyout.IsPopupPinnedOpenProperty, false);
    }

    private void HandleMenuItemClicked(object? sender, RoutedEventArgs args)
    {
        if (TabControl is not null)
        {
            Dispatcher.Post(sender =>
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
        if (sender is TabControlOverflowMenuItem { TabItem: { } tabItem } tabControlMenuItem &&
            TabControl is { } tabControl &&
            tabControl.CloseTab(tabItem))
        {
            tabControlMenuItem.Click    -= HandleMenuItemClicked;
            tabControlMenuItem.CloseTab -= HandleCloseTabRequest;
            tabControlMenuItem.RemoveFromMenu();
        }
    }
}
