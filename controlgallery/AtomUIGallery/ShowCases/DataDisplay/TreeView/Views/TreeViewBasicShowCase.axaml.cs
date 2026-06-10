using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.TreeView;

public partial class TreeViewBasicShowCase : GalleryReactiveUserControl<TreeViewViewModel>
{
    public TreeViewBasicShowCase()
    {
        InitializeComponent();
    }

    private void HandleHoverModeChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is AtomUI.Desktop.Controls.RadioButton radioButton &&
            radioButton.IsChecked == true &&
            radioButton.Tag is TreeItemHoverMode hoverMode &&
            DataContext is TreeViewViewModel viewModel)
        {
            viewModel.TreeViewNodeHoverMode = hoverMode;
        }
    }
}
