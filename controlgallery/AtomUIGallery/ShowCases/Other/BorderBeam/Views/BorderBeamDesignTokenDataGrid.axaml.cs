namespace AtomUIGallery.ShowCases.BorderBeam;

public partial class BorderBeamDesignTokenDataGrid : GalleryReactiveUserControl<BorderBeamViewModel>
{
    public BorderBeamDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is BorderBeamViewModel viewModel)
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
