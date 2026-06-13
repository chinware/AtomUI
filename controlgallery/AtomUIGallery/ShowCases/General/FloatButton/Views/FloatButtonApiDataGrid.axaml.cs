using System;

namespace AtomUIGallery.ShowCases.FloatButton;

public partial class FloatButtonApiDataGrid : GalleryReactiveUserControl<FloatButtonViewModel>
{
    public FloatButtonApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is FloatButtonViewModel viewModel)
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
