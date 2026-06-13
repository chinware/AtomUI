using System;

namespace AtomUIGallery.ShowCases.RadioButton;

public partial class RadioButtonApiDataGrid : GalleryReactiveUserControl<RadioButtonViewModel>
{
    public RadioButtonApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is RadioButtonViewModel viewModel)
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
