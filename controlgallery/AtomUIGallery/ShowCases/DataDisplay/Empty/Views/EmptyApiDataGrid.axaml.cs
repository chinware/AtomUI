using System;

namespace AtomUIGallery.ShowCases.Empty;

public partial class EmptyApiDataGrid : GalleryReactiveUserControl<EmptyViewModel>
{
    public EmptyApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is EmptyViewModel viewModel)
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
