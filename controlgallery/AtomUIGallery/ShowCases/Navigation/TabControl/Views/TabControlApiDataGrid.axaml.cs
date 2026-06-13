using System;

namespace AtomUIGallery.ShowCases.TabControl;

public partial class TabControlApiDataGrid : GalleryReactiveUserControl<TabControlViewModel>
{
    public TabControlApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is TabControlViewModel viewModel)
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
