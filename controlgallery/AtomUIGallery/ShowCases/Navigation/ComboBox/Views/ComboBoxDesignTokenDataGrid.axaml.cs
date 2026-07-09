namespace AtomUIGallery.ShowCases.ComboBox;

public partial class ComboBoxDesignTokenDataGrid : GalleryReactiveUserControl<ComboBoxViewModel>
{
    public ComboBoxDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ComboBoxViewModel viewModel)
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
