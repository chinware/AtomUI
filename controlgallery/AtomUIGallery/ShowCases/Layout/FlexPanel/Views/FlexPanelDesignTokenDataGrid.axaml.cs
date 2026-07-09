namespace AtomUIGallery.ShowCases.FlexPanel;

public partial class FlexPanelDesignTokenDataGrid : GalleryReactiveUserControl<FlexPanelViewModel>
{
    public FlexPanelDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is FlexPanelViewModel viewModel)
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
