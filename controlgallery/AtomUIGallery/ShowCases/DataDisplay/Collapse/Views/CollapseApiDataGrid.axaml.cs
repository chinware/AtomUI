using System;

namespace AtomUIGallery.ShowCases.Collapse;

public partial class CollapseApiDataGrid : GalleryReactiveUserControl<CollapseViewModel>
{
    public CollapseApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is CollapseViewModel viewModel)
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
