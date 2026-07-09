namespace AtomUIGallery.ShowCases.FloatButton;

public partial class FloatButtonDesignTokenDataGrid : GalleryReactiveUserControl<FloatButtonViewModel>
{
    public FloatButtonDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is FloatButtonViewModel viewModel)
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
