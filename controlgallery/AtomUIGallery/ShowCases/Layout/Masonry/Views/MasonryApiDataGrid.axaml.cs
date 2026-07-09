namespace AtomUIGallery.ShowCases.Masonry;

public partial class MasonryApiDataGrid : GalleryReactiveUserControl<MasonryViewModel>
{
    public MasonryApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is MasonryViewModel viewModel)
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
