using System;

namespace AtomUIGallery.ShowCases.Carousel;

public partial class CarouselApiDataGrid : GalleryReactiveUserControl<CarouselViewModel>
{
    public CarouselApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is CarouselViewModel viewModel)
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
