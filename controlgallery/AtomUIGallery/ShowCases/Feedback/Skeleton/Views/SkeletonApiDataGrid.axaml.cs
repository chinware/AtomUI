namespace AtomUIGallery.ShowCases.Skeleton;

public partial class SkeletonApiDataGrid : GalleryReactiveUserControl<SkeletonViewModel>
{
    public SkeletonApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is SkeletonViewModel viewModel)
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
