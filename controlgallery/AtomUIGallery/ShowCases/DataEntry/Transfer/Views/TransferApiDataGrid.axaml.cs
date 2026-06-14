using System;

namespace AtomUIGallery.ShowCases.Transfer;

public partial class TransferApiDataGrid : GalleryReactiveUserControl<TransferViewModel>
{
    public TransferApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is TransferViewModel viewModel)
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
