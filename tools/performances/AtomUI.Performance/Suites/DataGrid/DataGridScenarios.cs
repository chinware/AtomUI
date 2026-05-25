using System.Collections.ObjectModel;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.Data;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
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
            new PerfScenario("DataGrid.RowHeaders", _ => CreateRowHeadersDataGrid()),
            new PerfScenario("DataGrid.RowDetails.Collapsed", _ => CreateRowDetailsDataGrid()),
            new PerfScenario("DataGrid.GroupHeaders", _ => CreateColumnGroupDataGrid()),
            new PerfScenario("DataGrid.RowGroups", _ => CreateRowGroupDataGrid()),
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

    private static DataGrid CreateRowHeadersDataGrid()
    {
        var grid = CreateBasicDataGrid();
        grid.HeadersVisibility = DataGridHeadersVisibility.All;
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

    private static DataGrid CreateRowDetailsDataGrid()
    {
        var grid = CreateDataGridShell(8);
        grid.Columns.Add(new DataGridDetailExpanderColumn());
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Name",
            Binding = new Binding(nameof(PerfDataGridRow.Name))
        });
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Age",
            Binding = new Binding(nameof(PerfDataGridRow.Age))
        });
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Address",
            Binding = new Binding(nameof(PerfDataGridRow.Address))
        });
        grid.RowDetailsTemplate = new FuncDataTemplate<PerfDataGridRow>((row, _) =>
            new Avalonia.Controls.TextBlock
            {
                Text         = row?.Address,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            });
        return grid;
    }

    private static DataGrid CreateColumnGroupDataGrid()
    {
        var grid = CreateDataGridShell(8);
        grid.CanUserResizeColumns = true;
        grid.LeftFrozenColumnCount = 1;
        grid.RightFrozenColumnCount = 1;

        var addressGroup = new DataGridColumnGroupItem { Header = "Address" };
        addressGroup.GroupChildren.Add(new DataGridTextColumn
        {
            Header  = "Address",
            Binding = new Binding(nameof(PerfDataGridRow.Address))
        });
        addressGroup.GroupChildren.Add(new DataGridTextColumn
        {
            Header  = "Score",
            Binding = new Binding(nameof(PerfDataGridRow.Score))
        });

        var profileGroup = new DataGridColumnGroupItem { Header = "Profile" };
        profileGroup.GroupChildren.Add(new DataGridTextColumn
        {
            Header  = "Name",
            Binding = new Binding(nameof(PerfDataGridRow.Name))
        });
        profileGroup.GroupChildren.Add(new DataGridTextColumn
        {
            Header  = "Age",
            Binding = new Binding(nameof(PerfDataGridRow.Age))
        });
        profileGroup.GroupChildren.Add(addressGroup);

        grid.ColumnGroups.Add(profileGroup);
        grid.ColumnGroups.Add(new DataGridTextColumn
        {
            Header  = "Score",
            Binding = new Binding(nameof(PerfDataGridRow.Score))
        });
        return grid;
    }

    private static DataGrid CreateRowGroupDataGrid()
    {
        var view = new DataGridCollectionView(CreateDataGridRows(12));
        view.GroupDescriptions.Add(new DataGridPathGroupDescription(nameof(PerfDataGridRow.Name)));

        var grid = CreateDataGridShell(view);
        grid.SelectionMode = DataGridSelectionMode.Single;
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Name",
            Binding = new Binding(nameof(PerfDataGridRow.Name))
        });
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Age",
            Binding = new Binding(nameof(PerfDataGridRow.Age))
        });
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Address",
            Binding = new Binding(nameof(PerfDataGridRow.Address))
        });
        return grid;
    }

    private static DataGrid CreateDataGridShell(int rowCount)
    {
        return CreateDataGridShell(CreateDataGridRows(rowCount));
    }

    private static DataGrid CreateDataGridShell(System.Collections.IEnumerable rows)
    {
        return new DataGrid
        {
            Width       = 720,
            Height      = 220,
            ItemsSource = rows
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
