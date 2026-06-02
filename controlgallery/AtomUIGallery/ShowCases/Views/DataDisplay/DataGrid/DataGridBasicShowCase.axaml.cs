using AtomUIGallery.ShowCases.ViewModels;

namespace AtomUIGallery.ShowCases.Views;

public partial class DataGridBasicShowCase : DataGridScenarioShowCase
{
    public DataGridBasicShowCase()
    {
        InitializeComponent();
    }

    protected override void InitializeScenario(DataGridViewModel viewModel)
    {
        DataGridShowCaseDataSources.EnsureBasicDataSource(viewModel);
        BasicCaseGrid.ItemsSource = viewModel.BasicCaseDataSource;
    }
}
