using AtomUI.Controls;
using AtomUI.Controls.Data;
using AtomUI.Controls.Utils;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.Primitives;
using Avalonia.Controls;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateListBoxScenarios()
    {
        return
        [
            new PerfScenario("ListBox.Default.Items5", _ => CreateListBox(CreateListItems(5))),
            new PerfScenario("ListBox.SelectedIndicator.Items5", _ => CreateListBox(
                CreateListItems(5),
                isShowSelectedIndicator: true,
                selectedIndex: 0)),
            new PerfScenario("ListBox.FilterActive.Items20", _ => CreateListBox(
                CreateListItems(20),
                filterValue: "1")),
            new PerfScenario("ListBox.Empty", _ => CreateListBox([])),
            new PerfScenario("CandidateList.Default.Items20", _ => CreateCandidateList(CreateListItems(20))),
            new PerfScenario("ListView.Default.Items20", _ => CreateListView(CreateListItems(20))),
            new PerfScenario("ListView.SelectedIndicator.Items20", _ => CreateListView(
                CreateListItems(20),
                isShowSelectedIndicator: true,
                selectedIndex: 0)),
            new PerfScenario("ListView.Grouped.Items20", _ => CreateListView(
                CreateGroupedListItems(),
                isGroupEnabled: true)),
            new PerfScenario("ListView.FilterActive.Items20", _ => CreateListView(
                CreateGroupedListItems(),
                filterValue: "a")),
            new PerfScenario("ListView.Pagination.Items2000", _ => CreateListView(
                CreateListItems(2000),
                pageSize: 100,
                pagination: new Pagination())),
            new PerfScenario("ListBox.GalleryShape", _ => CreateListGalleryShape())
        ];
    }

    private static AtomUI.Desktop.Controls.ListBox CreateListBox(
        IReadOnlyList<IListItemData> items,
        bool isShowSelectedIndicator = false,
        int selectedIndex = -1,
        object? filterValue = null)
    {
        var listBox = new AtomUI.Desktop.Controls.ListBox
        {
            Width                   = 320,
            ItemsSource             = items,
            IsShowSelectedIndicator = isShowSelectedIndicator,
            SelectedIndex           = selectedIndex,
            FilterValue             = filterValue
        };

        return listBox;
    }

    private static CandidateList CreateCandidateList(IReadOnlyList<IListItemData> items)
    {
        return new CandidateList
        {
            Width       = 320,
            ItemsSource = items
        };
    }

    private static ListView CreateListView(
        IReadOnlyList<IListItemData> items,
        bool isShowSelectedIndicator = false,
        int selectedIndex = -1,
        bool isGroupEnabled = false,
        object? filterValue = null,
        int pageSize = 0,
        AbstractPagination? pagination = null)
    {
        var listView = new ListView
        {
            Width                   = 360,
            IsShowSelectedIndicator = isShowSelectedIndicator,
            IsGroupEnabled          = isGroupEnabled,
            Filter                  = filterValue is null ? null : ValueFilterFactory.BuildFilter(ValueFilterMode.Contains),
            FilterValue             = filterValue,
            PageSize                = pageSize,
            BottomPagination        = pagination
        };
        listView.ItemsSource   = items;
        listView.SelectedIndex = selectedIndex;

        return listView;
    }

    private static Control CreateListGalleryShape()
    {
        var panel = new StackPanel
        {
            Spacing = 8
        };

        panel.Children.Add(CreateListView(CreateListItems(4)));
        panel.Children.Add(CreateListView(CreateListItems(4), isShowSelectedIndicator: true, selectedIndex: 0));
        panel.Children.Add(CreateListView(CreateGroupedListItems(), isGroupEnabled: true));
        panel.Children.Add(CreateListView(CreateDisabledListItems()));
        panel.Children.Add(CreateListView([]));
        panel.Children.Add(CreateListView(CreateGroupedListItems(), filterValue: "a"));
        panel.Children.Add(CreateListView(CreateGroupedListItems(), isGroupEnabled: true));
        panel.Children.Add(CreateListBox(CreateListBoxDemoItems()));
        panel.Children.Add(CreateListBox(CreateListBoxDemoItems(), isShowSelectedIndicator: true, selectedIndex: 0));
        panel.Children.Add(new SearchEdit { Width = 320, PlaceholderText = "Search" });
        panel.Children.Add(CreateListBox(CreateListBoxDemoItems()));
        panel.Children.Add(CreateListView(CreateListItems(2000), pageSize: 100, pagination: new Pagination()));

        return panel;
    }

    private static IReadOnlyList<IListItemData> CreateListItems(int count)
    {
        var items = new List<IListItemData>(count);
        for (var i = 0; i < count; i++)
        {
            items.Add(new ListItemData
            {
                ItemKey = i.ToString(),
                Content = $"Content {i}"
            });
        }
        return items;
    }

    private static IReadOnlyList<IListItemData> CreateDisabledListItems()
    {
        return
        [
            new ListItemData { Content = "Blue" },
            new ListItemData { Content = "Green" },
            new ListItemData { Content = "Red" },
            new ListItemData { Content = "Yellow", IsEnabled = false }
        ];
    }

    private static IReadOnlyList<IListItemData> CreateListBoxDemoItems()
    {
        return
        [
            new ListItemData { Content = "Racing car sprays burning fuel into crowd." },
            new ListItemData { Content = "Japanese princess to wed commoner." },
            new ListItemData { Content = "Australian walks 100km after outback crash." },
            new ListItemData { Content = "Man charged over missing wedding girl." },
            new ListItemData { Content = "Los Angeles battles huge wildfires." }
        ];
    }

    private static IReadOnlyList<IListItemData> CreateGroupedListItems()
    {
        return
        [
            new ListItemData { Content = "Red", Group = "Basic Colors" },
            new ListItemData { Content = "Orange", Group = "Basic Colors" },
            new ListItemData { Content = "Green", Group = "Basic Colors" },
            new ListItemData { Content = "Blue", Group = "Basic Colors" },
            new ListItemData { Content = "Purple", Group = "Basic Colors" },
            new ListItemData { Content = "Pink", Group = "Basic Colors" },
            new ListItemData { Content = "Yellow", Group = "Basic Colors" },
            new ListItemData { Content = "Brown", Group = "Neutral Colors" },
            new ListItemData { Content = "White", Group = "Neutral Colors" },
            new ListItemData { Content = "Black", Group = "Neutral Colors" },
            new ListItemData { Content = "Gray", Group = "Neutral Colors" },
            new ListItemData { Content = "Turquoise", Group = "Specific Shades" },
            new ListItemData { Content = "Violet", Group = "Specific Shades" },
            new ListItemData { Content = "Magenta", Group = "Specific Shades" },
            new ListItemData { Content = "Maroon", Group = "Specific Shades" },
            new ListItemData { Content = "Navy", Group = "Specific Shades" },
            new ListItemData { Content = "Beige", Group = "Specific Shades" },
            new ListItemData { Content = "Cyan", Group = "Specific Shades" },
            new ListItemData { Content = "Lavender", Group = "Specific Shades" },
            new ListItemData { Content = "Olive", Group = "Specific Shades" }
        ];
    }
}
