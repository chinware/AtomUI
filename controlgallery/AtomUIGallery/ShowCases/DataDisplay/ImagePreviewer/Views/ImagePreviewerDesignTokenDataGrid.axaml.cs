using System;

namespace AtomUIGallery.ShowCases.ImagePreviewer;

public partial class ImagePreviewerDesignTokenDataGrid : GalleryReactiveUserControl<ImagePreviewerViewModel>
{
    public ImagePreviewerDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ImagePreviewerViewModel viewModel)
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
