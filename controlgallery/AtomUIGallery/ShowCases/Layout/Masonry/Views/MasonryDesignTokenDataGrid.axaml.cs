using System;

namespace AtomUIGallery.ShowCases.Masonry;

public partial class MasonryDesignTokenDataGrid : GalleryReactiveUserControl<MasonryViewModel>
{
    public MasonryDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is MasonryViewModel viewModel)
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
