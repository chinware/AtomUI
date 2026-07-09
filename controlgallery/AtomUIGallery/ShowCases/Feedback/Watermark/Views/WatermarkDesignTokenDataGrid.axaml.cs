namespace AtomUIGallery.ShowCases.Watermark;

public partial class WatermarkDesignTokenDataGrid : GalleryReactiveUserControl<WatermarkViewModel>
{
    public WatermarkDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is WatermarkViewModel viewModel)
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
