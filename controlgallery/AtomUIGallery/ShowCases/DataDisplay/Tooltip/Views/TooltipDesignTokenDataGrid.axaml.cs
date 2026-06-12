using System;

namespace AtomUIGallery.ShowCases.Tooltip;

public partial class TooltipDesignTokenDataGrid : GalleryReactiveUserControl<TooltipViewModel>
{
    public TooltipDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is TooltipViewModel viewModel)
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
