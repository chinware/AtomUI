namespace AtomUIGallery.ShowCases.Breadcrumb;

public partial class BreadcrumbDesignTokenDataGrid : GalleryReactiveUserControl<BreadcrumbViewModel>
{
    public BreadcrumbDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is BreadcrumbViewModel viewModel)
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
