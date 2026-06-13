using System;

namespace AtomUIGallery.ShowCases.ProgressBar;

public partial class ProgressBarDesignTokenDataGrid : GalleryReactiveUserControl<ProgressBarViewModel>
{
    public ProgressBarDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ProgressBarViewModel viewModel)
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
