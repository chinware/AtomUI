namespace AtomUIGallery.ShowCases.DatePicker;

public partial class DatePickerApiDataGrid : GalleryReactiveUserControl<DatePickerViewModel>
{
    public DatePickerApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is DatePickerViewModel viewModel)
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
