using AtomUI.Controls.Data;
using AtomUI.Desktop.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.List;

public partial class ListAdvancedShowCase : GalleryReactiveUserControl<ListViewModel>
{
    public ListAdvancedShowCase()
    {
        InitializeComponent();
        OrderedList.SortDescriptions = [ListSortDescription.FromPath("Content")];
    }

    private void HandleFilterListBoxClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is SearchEdit searchEdit)
        {
            SearchListBox.FilterValue = searchEdit.Text?.Trim();
        }
    }
}
