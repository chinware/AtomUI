using AtomUI.Controls.Data;
using AtomUI.Desktop.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.List;

public partial class ListAdvancedShowCase : GalleryReactiveUserControl<ListViewModel>
{
    public ListAdvancedShowCase()
    {
        InitializeComponent();
        OrderedList.SortDescriptions = [ListSortDescription.FromPath(nameof(IListItemData.Content))];
    }

    private void HandleFilterListBoxSearchRequested(object? sender, SearchRequestedEventArgs e)
    {
        SearchListBox.FilterValue = e.Query.Trim();
    }
}
