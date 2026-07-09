namespace AtomUIGallery.ShowCases.CustomizeTheme;

public partial class CustomizeThemeApiDataGrid : GalleryReactiveUserControl<CustomizeThemeViewModel>
{
    public CustomizeThemeApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is CustomizeThemeViewModel viewModel)
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
