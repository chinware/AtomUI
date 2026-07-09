namespace AtomUIGallery.ShowCases.DropdownButton;

public partial class DropdownButtonDesignTokenDataGrid : GalleryReactiveUserControl<DropdownButtonViewModel>
{
    public DropdownButtonDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is DropdownButtonViewModel viewModel)
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
