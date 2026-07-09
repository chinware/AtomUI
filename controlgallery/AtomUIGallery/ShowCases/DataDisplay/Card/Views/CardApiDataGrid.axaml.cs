namespace AtomUIGallery.ShowCases.Card;

public partial class CardApiDataGrid : GalleryReactiveUserControl<CardViewModel>
{
    public CardApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is CardViewModel viewModel)
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
