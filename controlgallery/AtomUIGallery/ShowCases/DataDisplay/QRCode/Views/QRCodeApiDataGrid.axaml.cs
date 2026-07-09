namespace AtomUIGallery.ShowCases.QRCode;

public partial class QRCodeApiDataGrid : GalleryReactiveUserControl<QRCodeViewModel>
{
    public QRCodeApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is QRCodeViewModel viewModel)
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
