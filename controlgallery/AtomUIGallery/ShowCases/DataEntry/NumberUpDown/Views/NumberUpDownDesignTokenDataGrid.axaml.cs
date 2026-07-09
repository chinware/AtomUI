namespace AtomUIGallery.ShowCases.NumberUpDown;

public partial class NumberUpDownDesignTokenDataGrid : GalleryReactiveUserControl<NumberUpDownViewModel>
{
    public NumberUpDownDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is NumberUpDownViewModel viewModel)
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
