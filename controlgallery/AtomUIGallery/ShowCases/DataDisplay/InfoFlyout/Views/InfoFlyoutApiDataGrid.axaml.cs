namespace AtomUIGallery.ShowCases.InfoFlyout;

public partial class InfoFlyoutApiDataGrid : GalleryReactiveUserControl<InfoFlyoutViewModel>
{
    public InfoFlyoutApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is InfoFlyoutViewModel viewModel)
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
