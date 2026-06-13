using System;

namespace AtomUIGallery.ShowCases.ColorPicker;

public partial class ColorPickerDesignTokenDataGrid : GalleryReactiveUserControl<ColorPickerViewModel>
{
    public ColorPickerDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ColorPickerViewModel viewModel)
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
