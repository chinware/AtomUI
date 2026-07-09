namespace AtomUIGallery.ShowCases.ToggleSwitch;

public partial class ToggleSwitchDesignTokenDataGrid : GalleryReactiveUserControl<ToggleSwitchViewModel>
{
    public ToggleSwitchDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ToggleSwitchViewModel viewModel)
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
