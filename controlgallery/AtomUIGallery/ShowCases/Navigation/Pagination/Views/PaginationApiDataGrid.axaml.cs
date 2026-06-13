using System;

namespace AtomUIGallery.ShowCases.Pagination;

public partial class PaginationApiDataGrid : GalleryReactiveUserControl<PaginationViewModel>
{
    public PaginationApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is PaginationViewModel viewModel)
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
