using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.Cascader;

public partial class CascaderViewShowCase : GalleryReactiveUserControl<CascaderViewModel>
{
    public CascaderViewShowCase()
    {
        InitializeComponent();
    }

    private void HandleFilterCascaderViewClicked(object? sender, RoutedEventArgs args)
    {
        if (sender is SearchEdit searchEdit)
        {
            SearchCascaderView.FilterValue = searchEdit.Text?.Trim();
        }
    }

    private void HandleFilterCascaderViewItemsSourceClicked(object? sender, RoutedEventArgs args)
    {
        if (sender is SearchEdit searchEdit)
        {
            SearchCascaderViewItemsSource.FilterValue = searchEdit.Text?.Trim();
        }
    }
}
