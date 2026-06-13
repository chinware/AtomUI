using System;

namespace AtomUIGallery.ShowCases.ToggleSwitch;

public partial class ToggleSwitchApiDataGrid : GalleryReactiveUserControl<ToggleSwitchViewModel>
{
    public ToggleSwitchApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ToggleSwitchViewModel viewModel)
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
