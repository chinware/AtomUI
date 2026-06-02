using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.DataGrid;

public partial class DataGridStructureShowCase : DataGridScenarioShowCase
{
    public DataGridStructureShowCase()
    {
        InitializeComponent();

        ColumnCheckBox1.IsChecked = true;
        ColumnCheckBox2.IsChecked = true;
        ColumnCheckBox3.IsChecked = true;
        ColumnCheckBox4.IsChecked = true;
        ColumnCheckBox5.IsChecked = true;
        ColumnCheckBox6.IsChecked = true;

        ColumnCheckBox1.IsCheckedChanged += HandleColumnVisibleChanged;
        ColumnCheckBox2.IsCheckedChanged += HandleColumnVisibleChanged;
        ColumnCheckBox3.IsCheckedChanged += HandleColumnVisibleChanged;
        ColumnCheckBox4.IsCheckedChanged += HandleColumnVisibleChanged;
        ColumnCheckBox5.IsCheckedChanged += HandleColumnVisibleChanged;
        ColumnCheckBox6.IsCheckedChanged += HandleColumnVisibleChanged;
    }

    protected override void InitializeScenario(DataGridViewModel viewModel)
    {
        DataGridShowCaseDataSources.EnsureExpandableRowDataSource(viewModel);
        DataGridShowCaseDataSources.EnsureGroupHeaderDataSource(viewModel);
        DataGridShowCaseDataSources.EnsureBasicDataSource(viewModel);
        ExpandableDataGrid.ItemsSource            = viewModel.ExpandableRowDataSource;
        OrderSpecificColumnDataGrid.ItemsSource   = viewModel.ExpandableRowDataSource;
        RowAndColumnHeaderDataGrid.ItemsSource    = viewModel.ExpandableRowDataSource;
        GroupHeaderDataGrid.ItemsSource           = viewModel.GroupHeaderDataSource;
        HideColumnDataGrid.ItemsSource            = viewModel.BasicCaseDataSource;
    }

    private void HandleColumnVisibleChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is AtomUICheckBox checkBox)
        {
            var columns     = HideColumnDataGrid.Columns;
            var name        = checkBox.Name;
            var columnIndex = -1;
            if (name == "ColumnCheckBox1")
            {
                columnIndex = 0;
            }
            else if (name == "ColumnCheckBox2")
            {
                columnIndex = 1;
            }
            else if (name == "ColumnCheckBox3")
            {
                columnIndex = 2;
            }
            else if (name == "ColumnCheckBox4")
            {
                columnIndex = 3;
            }
            else if (name == "ColumnCheckBox5")
            {
                columnIndex = 4;
            }
            else if (name == "ColumnCheckBox6")
            {
                columnIndex = 5;
            }

            if (columnIndex != -1)
            {
                var column = columns[columnIndex];
                column.IsVisible = checkBox.IsChecked == true;
            }
        }
    }
}
