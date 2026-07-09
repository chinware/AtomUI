namespace AtomUIGallery.ShowCases.TimePicker;

public partial class TimePickerDesignTokenDataGrid : GalleryReactiveUserControl<TimePickerViewModel>
{
    public TimePickerDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is TimePickerViewModel viewModel)
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
