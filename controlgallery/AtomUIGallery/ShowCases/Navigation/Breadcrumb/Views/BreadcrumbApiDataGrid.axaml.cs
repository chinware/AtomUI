using System;

namespace AtomUIGallery.ShowCases.Breadcrumb;

public partial class BreadcrumbApiDataGrid : GalleryReactiveUserControl<BreadcrumbViewModel>
{
    public BreadcrumbApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is BreadcrumbViewModel viewModel)
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
