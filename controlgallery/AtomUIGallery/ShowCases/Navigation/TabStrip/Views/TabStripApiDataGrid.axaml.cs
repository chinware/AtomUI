using System;

namespace AtomUIGallery.ShowCases.TabStrip;

public partial class TabStripApiDataGrid : GalleryReactiveUserControl<TabStripViewModel>
{
    public TabStripApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is TabStripViewModel viewModel)
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
