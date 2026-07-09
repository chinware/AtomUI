namespace AtomUIGallery.ShowCases.Skeleton;

public partial class SkeletonDesignTokenDataGrid : GalleryReactiveUserControl<SkeletonViewModel>
{
    public SkeletonDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is SkeletonViewModel viewModel)
        {
            viewModel.EnsureDesignTokenRows();
            DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows;
        }
        else
        {
            DesignTokenDataGrid.ItemsSource = null;
        }
    }
}
