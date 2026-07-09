namespace AtomUIGallery.ShowCases.Segmented;

public partial class SegmentedApiDataGrid : GalleryReactiveUserControl<SegmentedViewModel>
{
    public SegmentedApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is SegmentedViewModel viewModel)
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
