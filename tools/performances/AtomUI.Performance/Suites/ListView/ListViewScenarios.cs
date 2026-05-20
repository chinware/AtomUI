using System.ComponentModel;
using AtomUI.Controls;
using AtomUI.Controls.Data;
using AtomUI.Controls.Utils;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using AtomListSortDescription = AtomUI.Controls.Data.ListSortDescription;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateListViewScenarios()
    {
        return
        [
            new PerfScenario("ListView.Default.Items20", _ => CreateListView(CreateListViewItems(20))),
            new PerfScenario("ListView.SelectedIndicator.Items20", _ => CreateListView(
                CreateListViewItems(20),
                isShowSelectedIndicator: true,
                selectedIndex: 0)),
            new PerfScenario("ListView.Disabled.Items20", _ => CreateListView(CreateDisabledListViewItems())),
            new PerfScenario("ListView.Empty", _ => CreateListView([])),
            new PerfScenario("ListView.Grouped.Items20", _ => CreateListView(
                CreateGroupedListViewItems(),
                isGroupEnabled: true)),
            new PerfScenario("ListView.FilterActive.Items20", _ => CreateListView(
                CreateGroupedListViewItems(),
                filterValue: "a")),
            new PerfScenario("ListView.SortActive.Items20", _ => CreateListView(
                CreateGroupedListViewItems(),
                isGroupEnabled: true,
                sortDescriptions:
                [
                    AtomListSortDescription.FromPath(nameof(IListItemData.Content), ListSortDirection.Ascending)
                ])),
            new PerfScenario("ListView.Pagination.Items2000", _ => CreateListView(
                CreateListViewItems(2000),
                pageSize: 100,
                pagination: new Pagination())),
            new PerfScenario("ListView.GalleryListViewShape", _ => CreateListViewGalleryShape())
        ];
    }

    private static ListView CreateListView(
        IReadOnlyList<IListItemData> items,
        bool isShowSelectedIndicator = false,
        int selectedIndex = -1,
        bool isGroupEnabled = false,
        object? filterValue = null,
        int pageSize = 0,
        AbstractPagination? pagination = null,
        IList<IListSortDescription>? sortDescriptions = null)
    {
        var listView = new ListView
        {
            Width                   = 360,
            IsShowSelectedIndicator = isShowSelectedIndicator,
            IsGroupEnabled          = isGroupEnabled,
            Filter                  = filterValue is null ? null : ValueFilterFactory.BuildFilter(ValueFilterMode.Contains),
            FilterValue             = filterValue,
            PageSize                = pageSize,
            BottomPagination        = pagination,
            SortDescriptions        = sortDescriptions
        };
        listView.ItemsSource   = items;
        listView.SelectedIndex = selectedIndex;

        return listView;
    }

    private static Control CreateListViewGalleryShape()
    {
        var panel = new StackPanel
        {
            Spacing = 8
        };

        panel.Children.Add(CreateListView(CreateListViewItems(4)));
        panel.Children.Add(CreateListView(CreateListViewItems(4), isShowSelectedIndicator: true, selectedIndex: 0));
        panel.Children.Add(CreateListView(CreateGroupedListViewItems(), isGroupEnabled: true));
        panel.Children.Add(CreateListView(CreateDisabledListViewItems()));
        panel.Children.Add(CreateListView([]));
        panel.Children.Add(CreateListView(CreateGroupedListViewItems(), filterValue: "a"));
        panel.Children.Add(CreateListView(
            CreateGroupedListViewItems(),
            isGroupEnabled: true,
            sortDescriptions:
            [
                AtomListSortDescription.FromPath(nameof(IListItemData.Content), ListSortDirection.Ascending)
            ]));
        panel.Children.Add(CreateListView(CreateListViewItems(2000), pageSize: 100, pagination: new Pagination()));

        return panel;
    }

    private static IReadOnlyList<IListItemData> CreateListViewItems(int count)
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

    private static IReadOnlyList<IListItemData> CreateDisabledListViewItems()
    {
        return
        [
            new ListItemData { Content = "Blue" },
            new ListItemData { Content = "Green" },
            new ListItemData { Content = "Red" },
            new ListItemData { Content = "Yellow", IsEnabled = false }
        ];
    }

    private static IReadOnlyList<IListItemData> CreateGroupedListViewItems()
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
