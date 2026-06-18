namespace AtomUIGallery.ShowCases.BorderBeam;

public partial class BorderBeamApiDataGrid : GalleryReactiveUserControl<BorderBeamViewModel>
{
    public BorderBeamApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is BorderBeamViewModel viewModel)
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
