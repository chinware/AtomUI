
namespace AtomUIGallery.ShowCases.DataGrid;

public partial class DataGridDragShowCase : DataGridScenarioShowCase
{
    public DataGridDragShowCase()
    {
        InitializeComponent();
    }

    protected override void InitializeScenario(DataGridViewModel viewModel)
    {
        DataGridShowCaseDataSources.EnsureDragColumnDataSource(viewModel);
        DataGridShowCaseDataSources.EnsureDragRowDataSource(viewModel);
        DragColumnDataGrid1.ItemsSource = viewModel.DragColumnDataSource;
        DragColumnDataGrid2.ItemsSource = viewModel.DragColumnDataSource;
        DragColumnDataGrid3.ItemsSource = viewModel.DragColumnDataSource;
        DragRowDataGrid1.ItemsSource    = viewModel.DragRowDataSource;
        DragRowDataGrid2.ItemsSource    = viewModel.DragRowManyDataSource;
    }
}
