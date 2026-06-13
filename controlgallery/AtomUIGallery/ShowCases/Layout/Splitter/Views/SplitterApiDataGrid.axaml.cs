using System;

namespace AtomUIGallery.ShowCases.Splitter;

public partial class SplitterApiDataGrid : GalleryReactiveUserControl<SplitterViewModel>
{
    public SplitterApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is SplitterViewModel viewModel)
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
