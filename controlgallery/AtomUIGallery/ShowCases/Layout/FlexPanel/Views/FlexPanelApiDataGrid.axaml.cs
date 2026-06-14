using System;

namespace AtomUIGallery.ShowCases.FlexPanel;

public partial class FlexPanelApiDataGrid : GalleryReactiveUserControl<FlexPanelViewModel>
{
    public FlexPanelApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is FlexPanelViewModel viewModel)
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
