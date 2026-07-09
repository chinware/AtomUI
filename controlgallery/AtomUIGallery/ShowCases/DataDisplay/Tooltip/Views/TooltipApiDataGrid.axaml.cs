namespace AtomUIGallery.ShowCases.Tooltip;

public partial class TooltipApiDataGrid : GalleryReactiveUserControl<TooltipViewModel>
{
    public TooltipApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is TooltipViewModel viewModel)
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
