using System;

namespace AtomUIGallery.ShowCases.Badge;

public partial class BadgeApiDataGrid : GalleryReactiveUserControl<BadgeViewModel>
{
    public BadgeApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is BadgeViewModel viewModel)
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
