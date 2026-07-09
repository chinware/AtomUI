namespace AtomUIGallery.ShowCases.Watermark;

public partial class WatermarkApiDataGrid : GalleryReactiveUserControl<WatermarkViewModel>
{
    public WatermarkApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is WatermarkViewModel viewModel)
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
