using System;

namespace AtomUIGallery.ShowCases.Separator;

public partial class SeparatorApiDataGrid : GalleryReactiveUserControl<SeparatorViewModel>
{
    public SeparatorApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is SeparatorViewModel viewModel)
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
