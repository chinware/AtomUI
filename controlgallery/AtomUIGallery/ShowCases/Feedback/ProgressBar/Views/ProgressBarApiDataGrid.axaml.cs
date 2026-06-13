using System;

namespace AtomUIGallery.ShowCases.ProgressBar;

public partial class ProgressBarApiDataGrid : GalleryReactiveUserControl<ProgressBarViewModel>
{
    public ProgressBarApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ProgressBarViewModel viewModel)
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
