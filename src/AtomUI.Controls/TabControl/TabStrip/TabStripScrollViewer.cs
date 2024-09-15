﻿using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AtomUI.Controls;

internal class TabStripScrollViewer : BaseTabScrollViewer
{
    #region 内部属性定义

    internal BaseTabStrip? TabStrip { get; set; }

    #endregion

    protected override Type StyleKeyOverride => typeof(BaseTabScrollViewer);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_menuIndicator is not null)
        {
            _menuIndicator.Click += HandleMenuIndicator;
        }
    }

    private void HandleMenuIndicator(object? sender, RoutedEventArgs args)
    {
        if (_menuFlyout is null)
        {
            _menuFlyout = new MenuFlyout();
        }

        if (TabStripPlacement == Dock.Top)
        {
            _menuFlyout.Placement = PlacementMode.BottomEdgeAlignedLeft;
        }
        else if (TabStripPlacement == Dock.Bottom)
        {
            _menuFlyout.Placement = PlacementMode.TopEdgeAlignedLeft;
        }
        else if (TabStripPlacement == Dock.Right)
        {
            _menuFlyout.Placement = PlacementMode.LeftEdgeAlignedBottom;
        }
        else
        {
            _menuFlyout.Placement = PlacementMode.RightEdgeAlignedBottom;
        }

        // 收集没有完全显示的 Tab 列表
        _menuFlyout.Items.Clear();
        if (TabStrip is not null)
        {
            for (var i = 0; i < TabStrip.ItemCount; i++)
            {
                var itemContainer = TabStrip.ContainerFromIndex(i)!;
                if (itemContainer is TabStripItem tabStripItem)
                {
                    var itemBounds = itemContainer.Bounds;
                    var left       = Math.Floor(itemBounds.Left - Offset.X);
                    var right      = Math.Floor(itemBounds.Right - Offset.X);
                    if (TabStripPlacement == Dock.Top || TabStripPlacement == Dock.Bottom)
                    {
                        if (left < 0 || right > Viewport.Width)
                        {
                            var menuItem = new TabStripOverflowMenuItem
                            {
                                Header       = tabStripItem.Content,
                                TabStripItem = tabStripItem,
                                IsClosable   = tabStripItem.IsClosable
                            };
                            menuItem.Click    += HandleMenuItemClicked;
                            menuItem.CloseTab += HandleCloseTabRequest;
                            _menuFlyout.Items.Add(menuItem);
                        }
                    }
                }
            }

            if (_menuFlyout.Items.Count > 0)
            {
                _menuFlyout.ShowAt(_menuIndicator!);
            }
        }
    }

    private void HandleMenuItemClicked(object? sender, RoutedEventArgs args)
    {
        if (TabStrip is not null)
        {
            Dispatcher.UIThread.Post(sender =>
            {
                if (sender is TabStripOverflowMenuItem tabStripMenuItem)
                {
                    var tabStripItem = tabStripMenuItem.TabStripItem;
                    if (tabStripItem is not null)
                    {
                        tabStripItem.BringIntoView();
                        TabStrip.SelectedItem = tabStripItem;
                    }
                }
            }, sender);
        }
    }

    private void HandleCloseTabRequest(object? sender, RoutedEventArgs args)
    {
        if (sender is TabStripOverflowMenuItem tabStripMenuItem)
        {
            if (TabStrip is not null)
            {
                if (TabStrip.SelectedItem is TabStripItem selectedItem)
                {
                    if (selectedItem == tabStripMenuItem.TabStripItem)
                    {
                        var     selectedIndex   = TabStrip.SelectedIndex;
                        object? newSelectedItem = null;
                        if (selectedIndex != 0)
                        {
                            newSelectedItem = TabStrip.Items[--selectedIndex];
                        }

                        TabStrip.Items.Remove(tabStripMenuItem.TabStripItem);
                        TabStrip.SelectedItem = newSelectedItem;
                    }
                    else
                    {
                        TabStrip.Items.Remove(tabStripMenuItem.TabStripItem);
                    }
                }
            }
        }
    }
}