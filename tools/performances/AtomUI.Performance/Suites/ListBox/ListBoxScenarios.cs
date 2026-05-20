using AtomUI.Controls.Data;
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
            new PerfScenario("ListBox.GalleryShape", _ => CreateListBoxGalleryShape())
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

    private static Control CreateListBoxGalleryShape()
    {
        var panel = new StackPanel
        {
            Spacing = 8
        };

        panel.Children.Add(CreateListBox(CreateListBoxDemoItems()));
        panel.Children.Add(CreateListBox(CreateListBoxDemoItems(), isShowSelectedIndicator: true, selectedIndex: 0));
        panel.Children.Add(new SearchEdit { Width = 320, PlaceholderText = "Search" });
        panel.Children.Add(CreateListBox(CreateListBoxDemoItems()));

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
}
