using AtomUI.Desktop.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.DataGrid;

public partial class DataGridEditingShowCase : DataGridScenarioShowCase
{
    private static int s_cellsEditableNewRowIndex = 1;

    public DataGridEditingShowCase()
    {
        InitializeComponent();
    }

    protected override void InitializeScenario(DataGridViewModel viewModel)
    {
        DataGridShowCaseDataSources.EnsureCustomEmptyDataSource(viewModel);
        DataGridShowCaseDataSources.EnsureEditableCellsDataSource(viewModel);
        CustomEmptyDataGrid.ItemsSource   = viewModel.CustomEmptyDataSource;
        EditableCellsDataGrid.ItemsSource = viewModel.EditableCellsDataSource;
    }

    private void HandleToggleEmptyGridItemsSource(object? sender, RoutedEventArgs? eventArgs)
    {
        if (CustomEmptyDataGrid.ItemsSource != null)
        {
            CustomEmptyDataGrid.ItemsSource = null;
        }
        else if (DataContext is DataGridViewModel viewModel)
        {
            DataGridShowCaseDataSources.EnsureCustomEmptyDataSource(viewModel);
            CustomEmptyDataGrid.ItemsSource = viewModel.CustomEmptyDataSource;
        }
    }

    private void HandleToggleLoadingState(object? sender, RoutedEventArgs? eventArgs)
    {
        CustomEmptyDataGrid.IsOperating = !CustomEmptyDataGrid.IsOperating;
    }

    private void HandleAddARowToCellsEditableGrid(object? sender, RoutedEventArgs? eventArgs)
    {
        if (DataContext is DataGridViewModel viewModel)
        {
            DataGridShowCaseDataSources.EnsureEditableCellsDataSource(viewModel);
            viewModel.EditableCellsDataSource?.Add(new DataGridBaseInfo
            {
                Address = $"London, Park Lane no. {s_cellsEditableNewRowIndex}",
                Name    = $"Edward King {s_cellsEditableNewRowIndex}",
                Age     = 32
            });
            s_cellsEditableNewRowIndex++;
        }
    }

    private void HandleRemoveRowCellsEditableGrid(object? sender, RoutedEventArgs? eventArgs)
    {
        if (sender is AtomUIPopupConfirm popupConfirm &&
            popupConfirm.DataContext is int index)
        {
            EditableCellsDataGrid.CollectionView?.RemoveAt(index);
        }
    }
}
