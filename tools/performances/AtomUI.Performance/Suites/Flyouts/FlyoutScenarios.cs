using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Layout;

namespace AtomUI.Performance;

using AtomFlyout = AtomUI.Desktop.Controls.Flyout;
using AtomMenuFlyout = AtomUI.Desktop.Controls.MenuFlyout;
using AtomSplitButton = AtomUI.Desktop.Controls.SplitButton;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateFlyoutScenarios()
    {
        return
        [
            new PerfScenario("FlyoutHost.Hover.Closed", _ => CreateFlyoutHost(FlyoutTriggerType.Hover)),
            new PerfScenario("FlyoutHost.Click.Closed", _ => CreateFlyoutHost(FlyoutTriggerType.Click)),
            new PerfScenario("FlyoutHost.Focus.Closed", _ => CreateFlyoutHost(FlyoutTriggerType.Focus)),
            new PerfScenario("PopupConfirm.Closed", _ => CreatePopupConfirm()),
            new PerfScenario("DropdownButton.MenuFlyout.Closed", _ => CreateFlyoutDropdownButton()),
            new PerfScenario("SplitButton.MenuFlyout.Closed", _ => CreateFlyoutSplitButton()),
            new PerfScenario("Flyouts.GalleryShape", _ => CreateFlyoutGalleryShape())
        ];
    }

    private static FlyoutHost CreateFlyoutHost(FlyoutTriggerType triggerType)
    {
        return new FlyoutHost
        {
            Trigger               = triggerType,
            Placement             = PlacementMode.Top,
            IsArrowVisible        = true,
            ShouldUseOverlayPopup = false,
            Flyout                = CreateTextFlyout(),
            Content               = new AtomUI.Desktop.Controls.Button
            {
                Content = triggerType.ToString()
            }
        };
    }

    private static AtomFlyout CreateTextFlyout()
    {
        return new AtomFlyout
        {
            Content = new Avalonia.Controls.TextBlock
            {
                Width   = 200,
                Height  = 100,
                Padding = new Avalonia.Thickness(20),
                Text    = "The most basic example."
            }
        };
    }

    private static PopupConfirm CreatePopupConfirm()
    {
        return new PopupConfirm
        {
            Title          = "Delete the task",
            ConfirmContent = "Are you sure to delete this task?",
            OkText         = "Ok",
            CancelText     = "Cancel",
            Placement      = PlacementMode.Top,
            IsArrowVisible = true,
            Content        = new AtomUI.Desktop.Controls.Button
            {
                ButtonType = ButtonType.Default,
                IsDanger   = true,
                Content    = "Delete"
            }
        };
    }

    private static DropdownButton CreateFlyoutDropdownButton()
    {
        return new DropdownButton
        {
            Content        = "Actions",
            DropdownFlyout = CreateMenuFlyout()
        };
    }

    private static AtomSplitButton CreateFlyoutSplitButton()
    {
        return new AtomSplitButton
        {
            Content = "Split",
            Icon    = new EllipsisOutlined(),
            Flyout  = CreateMenuFlyout()
        };
    }

    private static AtomMenuFlyout CreateMenuFlyout()
    {
        var flyout = new AtomMenuFlyout();
        flyout.Items.Add(new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "1st menu item"
        });
        flyout.Items.Add(new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "2nd menu item"
        });
        flyout.Items.Add(new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "3rd menu item"
        });
        return flyout;
    }

    private static Control CreateFlyoutGalleryShape()
    {
        var root = new StackPanel
        {
            Spacing = 16
        };

        root.Children.Add(CreateFlyoutRow(
            CreateFlyoutHost(FlyoutTriggerType.Hover),
            CreateFlyoutHost(FlyoutTriggerType.Focus),
            CreateFlyoutHost(FlyoutTriggerType.Click)));

        root.Children.Add(CreatePlacementGrid<FlyoutHost>(placement => new FlyoutHost
        {
            Trigger        = FlyoutTriggerType.Hover,
            Placement      = placement,
            Flyout         = CreateTextFlyout(),
            Content        = new AtomUI.Desktop.Controls.Button { Content = ShortPlacementLabel(placement) }
        }));

        root.Children.Add(CreatePlacementGrid<PopupConfirm>(placement => new PopupConfirm
        {
            Trigger        = FlyoutTriggerType.Click,
            Placement      = placement,
            Title          = "Delete the task",
            ConfirmContent = "Are you sure to delete this task?",
            OkText         = "Ok",
            CancelText     = "Cancel",
            Content        = new AtomUI.Desktop.Controls.Button { Content = ShortPlacementLabel(placement) }
        }));

        root.Children.Add(CreateFlyoutRow(
            CreateFlyoutDropdownButton(),
            CreateFlyoutSplitButton()));

        return root;
    }

    private static StackPanel CreateFlyoutRow(params Control[] controls)
    {
        var row = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing     = 10
        };

        foreach (var control in controls)
        {
            row.Children.Add(control);
        }

        return row;
    }

    private static Grid CreatePlacementGrid<TControl>(Func<PlacementMode, TControl> factory)
        where TControl : Control
    {
        var grid = new Grid();
        for (var i = 0; i < 5; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        AddPlacementControl(grid, factory, PlacementMode.LeftEdgeAlignedTop, 1, 0);
        AddPlacementControl(grid, factory, PlacementMode.Left, 2, 0);
        AddPlacementControl(grid, factory, PlacementMode.LeftEdgeAlignedBottom, 3, 0);
        AddPlacementControl(grid, factory, PlacementMode.TopEdgeAlignedLeft, 0, 1);
        AddPlacementControl(grid, factory, PlacementMode.Top, 0, 2);
        AddPlacementControl(grid, factory, PlacementMode.TopEdgeAlignedRight, 0, 3);
        AddPlacementControl(grid, factory, PlacementMode.RightEdgeAlignedTop, 1, 4);
        AddPlacementControl(grid, factory, PlacementMode.Right, 2, 4);
        AddPlacementControl(grid, factory, PlacementMode.RightEdgeAlignedBottom, 3, 4);
        AddPlacementControl(grid, factory, PlacementMode.BottomEdgeAlignedLeft, 4, 1);
        AddPlacementControl(grid, factory, PlacementMode.Bottom, 4, 2);
        AddPlacementControl(grid, factory, PlacementMode.BottomEdgeAlignedRight, 4, 3);
        return grid;
    }

    private static void AddPlacementControl<TControl>(Grid grid,
                                                      Func<PlacementMode, TControl> factory,
                                                      PlacementMode placement,
                                                      int row,
                                                      int column)
        where TControl : Control
    {
        var control = factory(placement);
        control.Margin = new Avalonia.Thickness(5);
        Grid.SetRow(control, row);
        Grid.SetColumn(control, column);
        grid.Children.Add(control);
    }

    private static string ShortPlacementLabel(PlacementMode placement)
    {
        return placement switch
        {
            PlacementMode.LeftEdgeAlignedTop => "LT",
            PlacementMode.Left => "Left",
            PlacementMode.LeftEdgeAlignedBottom => "LB",
            PlacementMode.TopEdgeAlignedLeft => "TL",
            PlacementMode.Top => "Top",
            PlacementMode.TopEdgeAlignedRight => "TR",
            PlacementMode.RightEdgeAlignedTop => "RT",
            PlacementMode.Right => "Right",
            PlacementMode.RightEdgeAlignedBottom => "RB",
            PlacementMode.BottomEdgeAlignedLeft => "BL",
            PlacementMode.Bottom => "Bottom",
            PlacementMode.BottomEdgeAlignedRight => "BR",
            _ => placement.ToString()
        };
    }
}
