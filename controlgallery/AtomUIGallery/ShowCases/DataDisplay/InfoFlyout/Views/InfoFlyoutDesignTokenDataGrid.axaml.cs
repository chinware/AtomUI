namespace AtomUIGallery.ShowCases.InfoFlyout;

public partial class InfoFlyoutDesignTokenDataGrid : GalleryReactiveUserControl<InfoFlyoutViewModel>
{
    public InfoFlyoutDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is InfoFlyoutViewModel viewModel)
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
