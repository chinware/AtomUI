using System;

namespace AtomUIGallery.ShowCases.Cascader;

public partial class CascaderDesignTokenDataGrid : GalleryReactiveUserControl<CascaderViewModel>
{
    public CascaderDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is CascaderViewModel viewModel)
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
