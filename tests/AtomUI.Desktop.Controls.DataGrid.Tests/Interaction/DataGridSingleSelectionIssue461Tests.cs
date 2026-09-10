using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.Interaction;

// https://github.com/AtomUI/AtomUI/issues/461
public class DataGridSingleSelectionIssue461Tests
{
    static DataGridSingleSelectionIssue461Tests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData(ClickTarget.BodyCell)]
    [InlineData(ClickTarget.SelectionIndicator)]
    public void Single_Click_Moves_Selection_To_Another_Row(ClickTarget clickTarget)
    {
        var rows = new[]
        {
            new GridRow("Alice"),
            new GridRow("Bob"),
            new GridRow("Carol")
        };
        var grid = CreateGrid(rows);
        var transitions = new List<DataGridSelectionChangedEventArgs>();
        grid.SelectionChanged += (_, args) => transitions.Add(args);
        var window = new Window
        {
            Width = 600,
            Height = 320,
            Content = grid
        };

        try
        {
            window.Show();
            PumpUntil(() =>
                grid.LoadState == DataGridLoadState.Ready &&
                grid.GetVisualDescendants().OfType<DataGridRow>().Count() == rows.Length);

            Click(GetClickTarget(grid, rows[0], clickTarget), window);
            Dispatcher.UIThread.RunJobs();
            grid.Selection.ExplicitKeys.ShouldBe([DataGridRowKey.FromInt64(1)]);

            Click(GetClickTarget(grid, rows[1], clickTarget), window);
            Dispatcher.UIThread.RunJobs();

            grid.Selection.ExplicitKeys.ShouldBe([DataGridRowKey.FromInt64(2)]);
            GetRow(grid, rows[0]).IsSelected.ShouldBeFalse();
            GetRow(grid, rows[1]).IsSelected.ShouldBeTrue();
            transitions.Count.ShouldBe(2);
            transitions[1].OldSelection.ExplicitKeys.ShouldBe([DataGridRowKey.FromInt64(1)]);
            transitions[1].NewSelection.ExplicitKeys.ShouldBe([DataGridRowKey.FromInt64(2)]);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static global::AtomUI.Desktop.Controls.DataGrid CreateGrid(IReadOnlyList<GridRow> rows)
    {
        var grid = new global::AtomUI.Desktop.Controls.DataGrid
        {
            AutoGenerateColumns = false,
            SelectionMode = DataGridSelectionMode.Single,
            SelectTriggerType = DataGridSelectTriggerType.Cell,
            ItemsSource = new TestDataGridSource<GridRow>(rows),
            Width = 520,
            Height = 240
        };
        grid.Columns.Add(new DataGridSelectionColumn());
        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Name",
            Binding = new Binding(nameof(GridRow.Name)),
            Width = new DataGridLength(240)
        });
        return grid;
    }

    private static Control GetClickTarget(
        global::AtomUI.Desktop.Controls.DataGrid grid,
        GridRow item,
        ClickTarget clickTarget)
    {
        var row = GetRow(grid, item);
        return clickTarget switch
        {
            ClickTarget.BodyCell => row.GetVisualDescendants()
                                       .OfType<DataGridCell>()
                                       .Single(cell => cell.ColumnIndex == 1),
            ClickTarget.SelectionIndicator => row.GetVisualDescendants()
                                                 .OfType<SelectionRadioButton>()
                                                 .Single(),
            _ => throw new ArgumentOutOfRangeException(nameof(clickTarget))
        };
    }

    private static DataGridRow GetRow(
        global::AtomUI.Desktop.Controls.DataGrid grid,
        GridRow item) =>
        grid.GetVisualDescendants()
            .OfType<DataGridRow>()
            .Single(row => ReferenceEquals(row.DataContext, item));

    private static void Click(Control control, Window window)
    {
        var point = control.TranslatePoint(
            new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            window);

        point.ShouldNotBeNull();
        window.MouseMove(point.Value);
        window.MouseDown(point.Value, MouseButton.Left);
        window.MouseUp(point.Value, MouseButton.Left);
    }

    private static void PumpUntil(Func<bool> condition)
    {
        if (!SpinWait.SpinUntil(() =>
            {
                Dispatcher.UIThread.RunJobs();
                return condition();
            }, TimeSpan.FromSeconds(5)))
        {
            throw new TimeoutException("The expected DataGrid rows were not realized.");
        }
    }

    public enum ClickTarget
    {
        BodyCell,
        SelectionIndicator
    }

    private sealed record GridRow(string Name);
}
