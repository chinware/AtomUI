using System;

namespace AtomUIGallery.ShowCases.Mentions;

public partial class MentionsApiDataGrid : GalleryReactiveUserControl<MentionsViewModel>
{
    public MentionsApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is MentionsViewModel viewModel)
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
