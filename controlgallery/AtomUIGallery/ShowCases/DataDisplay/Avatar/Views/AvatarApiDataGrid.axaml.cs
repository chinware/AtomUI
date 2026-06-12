using System;

namespace AtomUIGallery.ShowCases.Avatar;

public partial class AvatarApiDataGrid : GalleryReactiveUserControl<AvatarViewModel>
{
    public AvatarApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is AvatarViewModel viewModel)
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
