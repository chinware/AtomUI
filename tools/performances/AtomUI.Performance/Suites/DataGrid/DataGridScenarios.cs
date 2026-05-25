using System.Collections.ObjectModel;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Data;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateDataGridScenarios()
    {
        return
        [
            new PerfScenario("DataGrid.Basic", _ => CreateBasicDataGrid()),
            new PerfScenario("DataGrid.Filter.Menu.Closed", _ => CreateFilterDataGrid(DataGridFilterMode.Menu)),
            new PerfScenario("DataGrid.Filter.Tree.Closed", _ => CreateFilterDataGrid(DataGridFilterMode.Tree)),
            new PerfScenario("DataGrid.GalleryShape", _ => CreateDataGridGalleryShape())
        ];
    }

    private static DataGrid CreateBasicDataGrid(int rowCount = 8, int columnCount = 4)
    {
        var grid = CreateDataGridShell(rowCount);
        for (var i = 0; i < columnCount; i++)
        {
            grid.Columns.Add(new DataGridTextColumn
            {
                Header  = $"Column {i + 1}",
                Binding = new Binding(GetDataGridBindingPath(i))
            });
        }
        return grid;
    }

    private static DataGrid CreateFilterDataGrid(DataGridFilterMode filterMode)
    {
        var grid = CreateDataGridShell(8);
        grid.CanUserFilterColumns = true;
        grid.Columns.Add(CreateFilterColumn("Name", nameof(PerfDataGridRow.Name), filterMode,
        [
            new DataGridFilterItem { Text = "Joe", Value = "Joe" },
            new DataGridFilterItem { Text = "Jim", Value = "Jim" },
            new DataGridFilterItem
            {
                Text = "Nested",
                Value = "Nested",
                Children =
                {
                    new DataGridFilterItem { Text = "Green", Value = "Green" },
                    new DataGridFilterItem { Text = "Black", Value = "Black" }
                }
            }
        ]));
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Age",
            Binding = new Binding(nameof(PerfDataGridRow.Age))
        });
        grid.Columns.Add(CreateFilterColumn("Address", nameof(PerfDataGridRow.Address), filterMode,
        [
            new DataGridFilterItem { Text = "London", Value = "London" },
            new DataGridFilterItem { Text = "New York", Value = "New York" }
        ]));
        return grid;
    }

    private static Control CreateDataGridGalleryShape()
    {
        var panel = new StackPanel
        {
            Spacing = 8
        };

        panel.Children.Add(CreateBasicDataGrid(rowCount: 8, columnCount: 4));
        panel.Children.Add(CreateFilterDataGrid(DataGridFilterMode.Menu));
        panel.Children.Add(CreateFilterDataGrid(DataGridFilterMode.Tree));
        panel.Children.Add(CreateBasicDataGrid(rowCount: 20, columnCount: 8));

        return panel;
    }

    private static DataGrid CreateDataGridShell(int rowCount)
    {
        return new DataGrid
        {
            Width       = 720,
            Height      = 220,
            ItemsSource = CreateDataGridRows(rowCount)
        };
    }

    private static DataGridTextColumn CreateFilterColumn(
        string header,
        string bindingPath,
        DataGridFilterMode filterMode,
        IEnumerable<DataGridFilterItem> filters)
    {
        var column = new DataGridTextColumn
        {
            Header           = header,
            Binding          = new Binding(bindingPath),
            FilterMode       = filterMode,
            FilterMemberPath = bindingPath
        };
        foreach (var filter in filters)
        {
            column.Filters.Add(filter);
        }
        return column;
    }

    private static ObservableCollection<PerfDataGridRow> CreateDataGridRows(int count)
    {
        var rows = new ObservableCollection<PerfDataGridRow>();
        for (var i = 0; i < count; i++)
        {
            rows.Add(new PerfDataGridRow
            {
                Name = i % 2 == 0 ? "Joe" : "Jim",
                Age = 20 + i % 30,
                Address = i % 3 == 0 ? "London" : "New York",
                Score = 60 + i % 40
            });
        }
        return rows;
    }

    private static string GetDataGridBindingPath(int index)
    {
        return (index % 4) switch
        {
            0 => nameof(PerfDataGridRow.Name),
            1 => nameof(PerfDataGridRow.Age),
            2 => nameof(PerfDataGridRow.Address),
            _ => nameof(PerfDataGridRow.Score)
        };
    }

    private sealed class PerfDataGridRow
    {
        public string? Name { get; init; }
        public int Age { get; init; }
        public string? Address { get; init; }
        public int Score { get; init; }
    }
}
