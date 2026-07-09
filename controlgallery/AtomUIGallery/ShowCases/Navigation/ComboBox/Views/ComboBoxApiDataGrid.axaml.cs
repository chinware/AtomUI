namespace AtomUIGallery.ShowCases.ComboBox;

public partial class ComboBoxApiDataGrid : GalleryReactiveUserControl<ComboBoxViewModel>
{
    public ComboBoxApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ComboBoxViewModel viewModel)
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
