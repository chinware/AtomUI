using System;

namespace AtomUIGallery.ShowCases.Slider;

public partial class SliderApiDataGrid : GalleryReactiveUserControl<SliderViewModel>
{
    public SliderApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is SliderViewModel viewModel)
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
