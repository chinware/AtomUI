namespace AtomUIGallery.ShowCases.ImagePreviewer;

public partial class ImagePreviewerApiDataGrid : GalleryReactiveUserControl<ImagePreviewerViewModel>
{
    public ImagePreviewerApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ImagePreviewerViewModel viewModel)
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
