using System;

namespace AtomUIGallery.ShowCases.Expander;

public partial class ExpanderDesignTokenDataGrid : GalleryReactiveUserControl<ExpanderViewModel>
{
    public ExpanderDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ExpanderViewModel viewModel)
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
