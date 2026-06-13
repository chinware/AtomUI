using System;

namespace AtomUIGallery.ShowCases.AutoComplete;

public partial class AutoCompleteApiDataGrid : GalleryReactiveUserControl<AutoCompleteViewModel>
{
    public AutoCompleteApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is AutoCompleteViewModel viewModel)
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
