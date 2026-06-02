
namespace AtomUIGallery.ShowCases.DataGrid;

public partial class DataGridFixedShowCase : DataGridScenarioShowCase
{
    public DataGridFixedShowCase()
    {
        InitializeComponent();
    }

    protected override void InitializeScenario(DataGridViewModel viewModel)
    {
        DataGridShowCaseDataSources.EnsureFixedHeaderDataSource(viewModel);
        DataGridShowCaseDataSources.EnsureFixedColumnsDataSource(viewModel);
        DataGridShowCaseDataSources.EnsureFixedColumnsAndHeadersDataSource(viewModel);
        FixedHeaderDataGrid.ItemsSource            = viewModel.FixedHeaderDataSource;
        FixedColumnsDataGrid1.ItemsSource          = viewModel.FixedColumnsDataSource;
        FixedColumnsDataGrid2.ItemsSource          = viewModel.FixedColumnsDataSource;
        FixedColumnsAndHeadersDataGrid.ItemsSource = viewModel.FixedColumnsAndHeadersDataSource;
    }
}
