namespace AtomUIGallery.ShowCases.Segmented;

public partial class SegmentedDesignTokenDataGrid : GalleryReactiveUserControl<SegmentedViewModel>
{
    public SegmentedDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is SegmentedViewModel viewModel)
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
