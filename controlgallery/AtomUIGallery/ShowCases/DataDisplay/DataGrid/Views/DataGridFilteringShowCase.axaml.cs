using System.ComponentModel;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.DataGrid;

public partial class DataGridFilteringShowCase : DataGridScenarioShowCase
{
    public DataGridFilteringShowCase()
    {
        InitializeComponent();
        SortAgeBtn.Click                += HandleSortAgeBtnClick;
        ClearFiltersBtn.Click           += HandleClearFiltersBtnClick;
        ClearFiltersAndSortersBtn.Click += HandleClearFiltersAndSortersBtnClick;
    }

    protected override void InitializeScenario(DataGridViewModel viewModel)
    {
        DataGridShowCaseDataSources.EnsureFilterAndSorterDataSource(viewModel);
        DataGridShowCaseDataSources.EnsureMultiSorterDataSource(viewModel);
        DataGridShowCaseDataSources.EnsureBasicDataSource(viewModel);
        FilterAndSortGrid.ItemsSource      = viewModel.FilterAndSorterDataSource;
        FilterInTreeGrid.ItemsSource       = viewModel.FilterAndSorterDataSource;
        MultiSorterDataGrid.ItemsSource    = viewModel.MultiSorterDataSource;
        ResetFilterAndSortGrid.ItemsSource = viewModel.BasicCaseDataSource;
    }

    private void HandleSortAgeBtnClick(object? sender, RoutedEventArgs? eventArgs)
    {
        ResetFilterAndSortGrid.Sort(1, ListSortDirection.Descending);
    }

    private void HandleClearFiltersBtnClick(object? sender, RoutedEventArgs? eventArgs)
    {
        ResetFilterAndSortGrid.ClearFilters();
    }

    private void HandleClearFiltersAndSortersBtnClick(object? sender, RoutedEventArgs? eventArgs)
    {
        ResetFilterAndSortGrid.ClearFilters();
        ResetFilterAndSortGrid.ClearSort();
    }
}
