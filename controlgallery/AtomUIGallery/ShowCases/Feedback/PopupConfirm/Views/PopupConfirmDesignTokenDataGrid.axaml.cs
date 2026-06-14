using System;

namespace AtomUIGallery.ShowCases.PopupConfirm;

public partial class PopupConfirmDesignTokenDataGrid : GalleryReactiveUserControl<PopupConfirmViewModel>
{
    public PopupConfirmDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is PopupConfirmViewModel viewModel)
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
