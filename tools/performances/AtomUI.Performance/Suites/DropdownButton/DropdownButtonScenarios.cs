using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Layout;

namespace AtomUI.Performance;

using AtomDropdownButton = AtomUI.Desktop.Controls.DropdownButton;
using AtomMenuFlyout = AtomUI.Desktop.Controls.MenuFlyout;
using AtomMenuItem = AtomUI.Desktop.Controls.MenuItem;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateDropdownButtonScenarios()
    {
        return
        [
            new PerfScenario("DropdownButton.Default.Closed", _ => CreateDefaultDropdownButton()),
            new PerfScenario("DropdownButton.Primary.Icon.Closed", _ => new AtomDropdownButton
            {
                ButtonType     = ButtonType.Primary,
                Content        = "Actions",
                Icon           = new SearchOutlined(),
                DropdownFlyout = CreateDropdownButtonMenuFlyout()
            }),
            new PerfScenario("DropdownButton.NoFlyout.Closed", _ => new AtomDropdownButton
            {
                Content = "Actions"
            }),
            new PerfScenario("DropdownButton.NoIndicator.Closed", _ => new AtomDropdownButton
            {
                Content             = "Actions",
                IsShowOpenIndicator = false,
                DropdownFlyout      = CreateDropdownButtonMenuFlyout()
            }),
            new PerfScenario("DropdownButton.Hover.Closed", _ => new AtomDropdownButton
            {
                Content        = "Hover",
                TriggerType    = FlyoutTriggerType.Hover,
                DropdownFlyout = CreateDropdownButtonMenuFlyout()
            }),
            new PerfScenario("DropdownButton.PlacementGrid.Closed", _ => CreateDropdownButtonPlacementGrid()),
            new PerfScenario("DropdownButton.Batch6.Closed", _ => CreateDropdownButtonBatch())
        ];
    }

    private static AtomDropdownButton CreateDefaultDropdownButton()
    {
        return new AtomDropdownButton
        {
            Content        = "Actions",
            DropdownFlyout = CreateDropdownButtonMenuFlyout()
        };
    }

    private static AtomMenuFlyout CreateDropdownButtonMenuFlyout()
    {
        var flyout = new AtomMenuFlyout();
        flyout.Items.Add(new AtomMenuItem
        {
            Header = "1st menu item"
        });
        flyout.Items.Add(new AtomMenuItem
        {
            Header = "2nd menu item"
        });
        flyout.Items.Add(new AtomMenuItem
        {
            Header = "3rd menu item"
        });
        return flyout;
    }

    private static Grid CreateDropdownButtonPlacementGrid()
    {
        var grid = new Grid();
        for (var i = 0; i < 2; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        AddDropdownButtonPlacement(grid, PlacementMode.BottomEdgeAlignedLeft, 0, 0);
        AddDropdownButtonPlacement(grid, PlacementMode.BottomEdgeAlignedRight, 0, 1);
        AddDropdownButtonPlacement(grid, PlacementMode.TopEdgeAlignedLeft, 1, 0);
        AddDropdownButtonPlacement(grid, PlacementMode.TopEdgeAlignedRight, 1, 1);
        return grid;
    }

    private static void AddDropdownButtonPlacement(Grid grid, PlacementMode placement, int row, int column)
    {
        var button = new AtomDropdownButton
        {
            Content        = ShortPlacementLabel(placement),
            Placement      = placement,
            DropdownFlyout = CreateDropdownButtonMenuFlyout(),
            Margin         = new Avalonia.Thickness(5)
        };
        Grid.SetRow(button, row);
        Grid.SetColumn(button, column);
        grid.Children.Add(button);
    }

    private static StackPanel CreateDropdownButtonBatch()
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing     = 8
        };
        panel.Children.Add(CreateDefaultDropdownButton());
        panel.Children.Add(new AtomDropdownButton
        {
            ButtonType     = ButtonType.Primary,
            Content        = "Primary",
            DropdownFlyout = CreateDropdownButtonMenuFlyout()
        });
        panel.Children.Add(new AtomDropdownButton
        {
            Content        = "Hover",
            TriggerType    = FlyoutTriggerType.Hover,
            DropdownFlyout = CreateDropdownButtonMenuFlyout()
        });
        panel.Children.Add(new AtomDropdownButton
        {
            Content             = "No indicator",
            IsShowOpenIndicator = false,
            DropdownFlyout      = CreateDropdownButtonMenuFlyout()
        });
        panel.Children.Add(new AtomDropdownButton
        {
            Content = "No flyout"
        });
        panel.Children.Add(new AtomDropdownButton
        {
            Content        = "Icon",
            Icon           = new SearchOutlined(),
            DropdownFlyout = CreateDropdownButtonMenuFlyout()
        });
        return panel;
    }
}
