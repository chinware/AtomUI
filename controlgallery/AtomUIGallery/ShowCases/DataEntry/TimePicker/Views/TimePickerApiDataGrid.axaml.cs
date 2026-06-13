using System;

namespace AtomUIGallery.ShowCases.TimePicker;

public partial class TimePickerApiDataGrid : GalleryReactiveUserControl<TimePickerViewModel>
{
    public TimePickerApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is TimePickerViewModel viewModel)
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
