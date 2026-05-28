using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Layout;

namespace AtomUI.Performance;

using AtomFlyout = AtomUI.Desktop.Controls.Flyout;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateInfoFlyoutScenarios()
    {
        return
        [
            new PerfScenario("InfoFlyout.Hover.Closed", _ => CreateInfoFlyoutHost(FlyoutTriggerType.Hover)),
            new PerfScenario("InfoFlyout.Click.Closed", _ => CreateInfoFlyoutHost(FlyoutTriggerType.Click)),
            new PerfScenario("InfoFlyout.Focus.Closed", _ => CreateInfoFlyoutHost(FlyoutTriggerType.Focus)),
            new PerfScenario("InfoFlyout.Arrow.Closed", _ => CreateInfoFlyoutHost(FlyoutTriggerType.Hover, isArrowVisible: true)),
            new PerfScenario("InfoFlyout.PlacementGrid.Closed", _ => CreateInfoFlyoutPlacementGrid()),
            new PerfScenario("InfoFlyout.GalleryShape", _ => CreateInfoFlyoutGalleryShape())
        ];
    }

    private static FlyoutHost CreateInfoFlyoutHost(
        FlyoutTriggerType triggerType,
        PlacementMode placement = PlacementMode.Top,
        bool isArrowVisible = false)
    {
        return new FlyoutHost
        {
            Trigger               = triggerType,
            Placement             = placement,
            IsArrowVisible        = isArrowVisible,
            ShouldUseOverlayPopup = false,
            Flyout                = CreateInfoTextFlyout(),
            Content               = new AtomUI.Desktop.Controls.Button
            {
                Content = triggerType.ToString()
            }
        };
    }

    private static AtomFlyout CreateInfoTextFlyout()
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

    private static Control CreateInfoFlyoutGalleryShape()
    {
        var root = new StackPanel
        {
            Spacing = 16
        };

        root.Children.Add(CreateInfoFlyoutRow(
            CreateInfoFlyoutHost(FlyoutTriggerType.Hover),
            CreateInfoFlyoutHost(FlyoutTriggerType.Focus),
            CreateInfoFlyoutHost(FlyoutTriggerType.Click)));

        root.Children.Add(CreateInfoFlyoutPlacementGrid());
        return root;
    }

    private static StackPanel CreateInfoFlyoutRow(params Control[] controls)
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

    private static Grid CreateInfoFlyoutPlacementGrid()
    {
        var grid = new Grid();
        for (var i = 0; i < 5; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        AddInfoFlyoutPlacementControl(grid, PlacementMode.LeftEdgeAlignedTop, 1, 0);
        AddInfoFlyoutPlacementControl(grid, PlacementMode.Left, 2, 0);
        AddInfoFlyoutPlacementControl(grid, PlacementMode.LeftEdgeAlignedBottom, 3, 0);
        AddInfoFlyoutPlacementControl(grid, PlacementMode.TopEdgeAlignedLeft, 0, 1);
        AddInfoFlyoutPlacementControl(grid, PlacementMode.Top, 0, 2);
        AddInfoFlyoutPlacementControl(grid, PlacementMode.TopEdgeAlignedRight, 0, 3);
        AddInfoFlyoutPlacementControl(grid, PlacementMode.RightEdgeAlignedTop, 1, 4);
        AddInfoFlyoutPlacementControl(grid, PlacementMode.Right, 2, 4);
        AddInfoFlyoutPlacementControl(grid, PlacementMode.RightEdgeAlignedBottom, 3, 4);
        AddInfoFlyoutPlacementControl(grid, PlacementMode.BottomEdgeAlignedLeft, 4, 1);
        AddInfoFlyoutPlacementControl(grid, PlacementMode.Bottom, 4, 2);
        AddInfoFlyoutPlacementControl(grid, PlacementMode.BottomEdgeAlignedRight, 4, 3);
        return grid;
    }

    private static void AddInfoFlyoutPlacementControl(Grid grid,
                                                      PlacementMode placement,
                                                      int row,
                                                      int column)
    {
        var control = CreateInfoFlyoutHost(FlyoutTriggerType.Hover, placement, isArrowVisible: true);
        control.Margin = new Avalonia.Thickness(5);
        Grid.SetRow(control, row);
        Grid.SetColumn(control, column);
        grid.Children.Add(control);
    }
}
