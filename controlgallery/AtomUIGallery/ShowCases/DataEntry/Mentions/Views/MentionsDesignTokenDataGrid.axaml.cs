using System;

namespace AtomUIGallery.ShowCases.Mentions;

public partial class MentionsDesignTokenDataGrid : GalleryReactiveUserControl<MentionsViewModel>
{
    public MentionsDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is MentionsViewModel viewModel)
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
