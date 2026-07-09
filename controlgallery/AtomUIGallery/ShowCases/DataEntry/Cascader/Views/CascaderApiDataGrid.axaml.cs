namespace AtomUIGallery.ShowCases.Cascader;

public partial class CascaderApiDataGrid : GalleryReactiveUserControl<CascaderViewModel>
{
    public CascaderApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is CascaderViewModel viewModel)
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
