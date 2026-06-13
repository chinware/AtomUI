using System;

namespace AtomUIGallery.ShowCases.NumberUpDown;

public partial class NumberUpDownApiDataGrid : GalleryReactiveUserControl<NumberUpDownViewModel>
{
    public NumberUpDownApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is NumberUpDownViewModel viewModel)
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
