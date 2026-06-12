using System;

namespace AtomUIGallery.ShowCases.QRCode;

public partial class QRCodeDesignTokenDataGrid : GalleryReactiveUserControl<QRCodeViewModel>
{
    public QRCodeDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is QRCodeViewModel viewModel)
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
