using System;

namespace AtomUIGallery.ShowCases.DatePicker;

public partial class DatePickerDesignTokenDataGrid : GalleryReactiveUserControl<DatePickerViewModel>
{
    public DatePickerDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is DatePickerViewModel viewModel)
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
