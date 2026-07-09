namespace AtomUIGallery.ShowCases.SplitButton;

public partial class SplitButtonApiDataGrid : GalleryReactiveUserControl<SplitButtonViewModel>
{
    public SplitButtonApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is SplitButtonViewModel viewModel)
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
