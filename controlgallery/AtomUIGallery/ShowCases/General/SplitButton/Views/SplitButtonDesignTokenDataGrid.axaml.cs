using System;

namespace AtomUIGallery.ShowCases.SplitButton;

public partial class SplitButtonDesignTokenDataGrid : GalleryReactiveUserControl<SplitButtonViewModel>
{
    public SplitButtonDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is SplitButtonViewModel viewModel)
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
