using System;

namespace AtomUIGallery.ShowCases.Modal;

public partial class ModalApiDataGrid : GalleryReactiveUserControl<ModalViewModel>
{
    public ModalApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ModalViewModel viewModel)
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
