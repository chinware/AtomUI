using System;

namespace AtomUIGallery.ShowCases.DataGrid;

public partial class DataGridApiDataGrid : GalleryReactiveUserControl<DataGridViewModel>
{
    public DataGridApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is DataGridViewModel viewModel)
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
