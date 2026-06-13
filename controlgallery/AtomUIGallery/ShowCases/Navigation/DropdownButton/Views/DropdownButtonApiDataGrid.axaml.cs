using System;

namespace AtomUIGallery.ShowCases.DropdownButton;

public partial class DropdownButtonApiDataGrid : GalleryReactiveUserControl<DropdownButtonViewModel>
{
    public DropdownButtonApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is DropdownButtonViewModel viewModel)
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
