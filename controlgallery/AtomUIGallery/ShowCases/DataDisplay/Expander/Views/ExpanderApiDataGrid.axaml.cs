namespace AtomUIGallery.ShowCases.Expander;

public partial class ExpanderApiDataGrid : GalleryReactiveUserControl<ExpanderViewModel>
{
    public ExpanderApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ExpanderViewModel viewModel)
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
