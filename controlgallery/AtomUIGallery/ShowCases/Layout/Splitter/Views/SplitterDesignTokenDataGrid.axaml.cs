using System;

namespace AtomUIGallery.ShowCases.Splitter;

public partial class SplitterDesignTokenDataGrid : GalleryReactiveUserControl<SplitterViewModel>
{
    public SplitterDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is SplitterViewModel viewModel)
        {
            viewModel.EnsureDesignTokenRows();
            DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows;
        }
        else
        {
            DesignTokenDataGrid.ItemsSource = null;
        }
    }
}
