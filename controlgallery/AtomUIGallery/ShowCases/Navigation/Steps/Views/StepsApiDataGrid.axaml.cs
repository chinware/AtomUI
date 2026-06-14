using System;

namespace AtomUIGallery.ShowCases.Steps;

public partial class StepsApiDataGrid : GalleryReactiveUserControl<StepsViewModel>
{
    public StepsApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is StepsViewModel viewModel)
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
