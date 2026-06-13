using System;

namespace AtomUIGallery.ShowCases.ButtonSpinner;

public partial class ButtonSpinnerDesignTokenDataGrid : GalleryReactiveUserControl<ButtonSpinnerViewModel>
{
    public ButtonSpinnerDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ButtonSpinnerViewModel viewModel)
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
