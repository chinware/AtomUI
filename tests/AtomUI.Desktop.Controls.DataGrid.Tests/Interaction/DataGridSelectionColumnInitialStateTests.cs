using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.Interaction;

public class DataGridSelectionColumnInitialStateTests
{
    static DataGridSelectionColumnInitialStateTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData(DataGridPaginationVisibility.Bottom)]
    [InlineData(DataGridPaginationVisibility.None)]
    public void Initial_SelectedItems_State_Is_Applied_To_Selection_Column_CheckBoxes(
        DataGridPaginationVisibility paginationVisibility)
    {
        var selectedRow = new GridRow("zzz", "aaa");
        var grid        = CreateGrid([selectedRow], paginationVisibility);
        grid.SelectedItems.Add(selectedRow);

        var window = new Window
        {
            Width   = 640,
            Height  = 320,
            Content = grid
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var dataGridRow = grid.GetVisualDescendants()
                                  .OfType<DataGridRow>()
                                  .Single(row => ReferenceEquals(row.DataContext, selectedRow));
            var headerCheckBox = grid.GetVisualDescendants()
                                     .OfType<SelectionHeaderCheckBox>()
                                     .Single();
            var rowCheckBox = grid.GetVisualDescendants()
                                  .OfType<SelectionCheckBox>()
                                  .Single();

            grid.SelectedItems.Count.ShouldBe(1);
            dataGridRow.IsSelected.ShouldBeTrue();
            rowCheckBox.IsChecked.ShouldBe(true);
            headerCheckBox.IsChecked.ShouldBe(true);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static global::AtomUI.Desktop.Controls.DataGrid CreateGrid(
        IReadOnlyList<GridRow> rows,
        DataGridPaginationVisibility paginationVisibility)
    {
        var grid = new global::AtomUI.Desktop.Controls.DataGrid
        {
            AutoGenerateColumns  = false,
            SelectionMode        = DataGridSelectionMode.Extended,
            SelectTriggerType    = DataGridSelectTriggerType.SelectIndicator,
            IsHideOnSinglePage   = true,
            PaginationVisibility = paginationVisibility,
            PageSize             = 10,
            ItemsSource          = rows,
            Width                = 540,
            Height               = 240
        };

        grid.Columns.Add(new DataGridSelectionColumn());
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Name",
            Binding = new Binding(nameof(GridRow.Name)),
            Width   = new DataGridLength(160)
        });
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "ProjectName",
            Binding = new Binding(nameof(GridRow.ProjectName)),
            Width   = new DataGridLength(180)
        });

        return grid;
    }

    private sealed record GridRow(string Name, string ProjectName);
}
