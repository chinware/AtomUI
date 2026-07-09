namespace AtomUIGallery.ShowCases.Descriptions;

public partial class DescriptionsDesignTokenDataGrid : GalleryReactiveUserControl<DescriptionsViewModel>
{
    public DescriptionsDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is DescriptionsViewModel viewModel)
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
