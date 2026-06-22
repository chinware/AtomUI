using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaGrid = Avalonia.Controls.Grid;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Steps;

public class StepsDynamicItemsTests
{
    static StepsDynamicItemsTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Dynamic_StepsItem_Addition_Rebuilds_Grid_Definitions_For_All_Items()
    {
        var steps = new Desktop.Controls.Steps
        {
            Width           = 640,
            CurrentStep     = 1,
            Style           = StepsStyle.Default,
            IsItemClickable = false
        };

        ShowInWindow(steps, () =>
        {
            steps.Items.Clear();
            for (var i = 0; i < 6; i++)
            {
                steps.Items.Add(new StepsItem
                {
                    Header = $"Rule {i + 1}"
                });
            }

            Dispatcher.UIThread.RunJobs();

            var grid = FindItemsPanelGrid(steps);
            grid.Children.Count.ShouldBe(6);
            grid.ColumnDefinitions.Count.ShouldBe(6);

            var stepItems = grid.Children.OfType<StepsItem>().ToList();
            stepItems.Count.ShouldBe(6);
            for (var i = 0; i < stepItems.Count; i++)
            {
                AvaloniaGrid.GetColumn(stepItems[i]).ShouldBe(i);
            }
        });
    }

    private static AvaloniaGrid FindItemsPanelGrid(Desktop.Controls.Steps steps)
    {
        var presenter = steps.GetVisualDescendants()
                             .OfType<ItemsPresenter>()
                             .Single(item => item.Name == "PART_ItemsPresenter");
        return presenter.Panel.ShouldBeOfType<AvaloniaGrid>();
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 760,
            Height  = 220,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }
}
