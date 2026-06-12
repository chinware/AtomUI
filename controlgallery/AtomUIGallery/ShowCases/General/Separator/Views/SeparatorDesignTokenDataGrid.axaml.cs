using System;

namespace AtomUIGallery.ShowCases.Separator;

public partial class SeparatorDesignTokenDataGrid : GalleryReactiveUserControl<SeparatorViewModel>
{
    public SeparatorDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is SeparatorViewModel viewModel)
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
