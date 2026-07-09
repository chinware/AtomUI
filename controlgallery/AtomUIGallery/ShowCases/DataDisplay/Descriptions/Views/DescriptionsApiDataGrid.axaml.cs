namespace AtomUIGallery.ShowCases.Descriptions;

public partial class DescriptionsApiDataGrid : GalleryReactiveUserControl<DescriptionsViewModel>
{
    public DescriptionsApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is DescriptionsViewModel viewModel)
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
