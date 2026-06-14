using System;

namespace AtomUIGallery.ShowCases.CustomizeTheme;

public partial class CustomizeThemeDesignTokenDataGrid : GalleryReactiveUserControl<CustomizeThemeViewModel>
{
    public CustomizeThemeDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is CustomizeThemeViewModel viewModel)
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
