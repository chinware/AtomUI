using System;

namespace AtomUIGallery.ShowCases.PopupConfirm;

public partial class PopupConfirmApiDataGrid : GalleryReactiveUserControl<PopupConfirmViewModel>
{
    public PopupConfirmApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is PopupConfirmViewModel viewModel)
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
