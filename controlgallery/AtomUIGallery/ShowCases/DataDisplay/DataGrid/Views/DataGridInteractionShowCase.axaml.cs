using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.DataGrid;

public partial class DataGridInteractionShowCase : DataGridScenarioShowCase
{
    public DataGridInteractionShowCase()
    {
        InitializeComponent();
        ExtendedSelection.IsCheckedChanged += SelectionModeCheckedChanged;
        SingleSelection.IsCheckedChanged   += SelectionModeCheckedChanged;
    }

    protected override void InitializeScenario(DataGridViewModel viewModel)
    {
        DataGridShowCaseDataSources.EnsureBasicDataSource(viewModel);
        SelectionDataGrid.ItemsSource              = viewModel.BasicCaseDataSource;
        DragResizeColumn.ItemsSource              = viewModel.BasicCaseDataSource;
        LargeSizeDataGrid.ItemsSource             = viewModel.BasicCaseDataSource;
        MiddleSizeDataGrid.ItemsSource            = viewModel.BasicCaseDataSource;
        SmallSizeDataGrid.ItemsSource             = viewModel.BasicCaseDataSource;
        CustomHeaderAndFooterDataGrid.ItemsSource = viewModel.BasicCaseDataSource;
    }

    private void SelectionModeCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is AtomUIRadioButton radioButton)
        {
            if (radioButton == ExtendedSelection && ExtendedSelection.IsChecked == true)
            {
                SelectionDataGrid.SelectionMode = DataGridSelectionMode.Extended;
            }
            else if (radioButton == SingleSelection && SingleSelection.IsChecked == true)
            {
                SelectionDataGrid.SelectionMode = DataGridSelectionMode.Single;
            }
        }
    }
}
