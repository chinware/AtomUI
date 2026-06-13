using System;

namespace AtomUIGallery.ShowCases.ButtonSpinner;

public partial class ButtonSpinnerApiDataGrid : GalleryReactiveUserControl<ButtonSpinnerViewModel>
{
    public ButtonSpinnerApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ButtonSpinnerViewModel viewModel)
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
