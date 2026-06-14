using System;

namespace AtomUIGallery.ShowCases.Drawer;

public partial class DrawerDesignTokenDataGrid : GalleryReactiveUserControl<DrawerViewModel>
{
    public DrawerDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is DrawerViewModel viewModel)
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
