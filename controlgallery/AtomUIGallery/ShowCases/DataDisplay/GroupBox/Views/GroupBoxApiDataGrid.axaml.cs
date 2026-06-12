using System;

namespace AtomUIGallery.ShowCases.GroupBox;

public partial class GroupBoxApiDataGrid : GalleryReactiveUserControl<GroupBoxViewModel>
{
    public GroupBoxApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is GroupBoxViewModel viewModel)
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
