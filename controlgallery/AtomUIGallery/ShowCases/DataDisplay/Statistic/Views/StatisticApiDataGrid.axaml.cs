namespace AtomUIGallery.ShowCases.Statistic;

public partial class StatisticApiDataGrid : GalleryReactiveUserControl<StatisticViewModel>
{
    public StatisticApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is StatisticViewModel viewModel)
        {
            viewModel.EnsureApiRows();
            ApiDataGrid.ItemsSource = viewModel.ApiRows;
        }
        else
        {
            ApiDataGrid.ItemsSource = null;
        }
    }
}
