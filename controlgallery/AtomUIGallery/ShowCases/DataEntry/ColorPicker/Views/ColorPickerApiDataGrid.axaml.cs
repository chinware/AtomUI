namespace AtomUIGallery.ShowCases.ColorPicker;

public partial class ColorPickerApiDataGrid : GalleryReactiveUserControl<ColorPickerViewModel>
{
    public ColorPickerApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ColorPickerViewModel viewModel)
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
