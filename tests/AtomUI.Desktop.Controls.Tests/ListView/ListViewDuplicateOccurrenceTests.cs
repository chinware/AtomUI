using AtomUI.Controls;
using AtomUI.Controls.Data;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ListView;

public class ListViewDuplicateOccurrenceTests
{
    static ListViewDuplicateOccurrenceTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Equal_Items_Project_Selection_To_Only_The_Addressed_Container()
    {
        var first  = new EqualListItemData("same");
        var second = new EqualListItemData("same");
        var list   = CreateList(first, second);

        list.Selection.Select(1);
        var firstContainer  = list.PrepareContainer(0);
        var secondContainer = list.PrepareContainer(1);

        firstContainer.IsSelected.ShouldBeFalse();
        secondContainer.IsSelected.ShouldBeTrue();
        firstContainer.EntryId.ShouldNotBe(secondContainer.EntryId);
    }

    [Fact]
    public void Repeated_Reference_Projects_Selection_To_Only_The_Addressed_Container()
    {
        var repeated = CreateItem("same");
        var list     = CreateList(repeated, repeated);

        list.Selection.Select(1);
        var firstContainer  = list.PrepareContainer(0);
        var secondContainer = list.PrepareContainer(1);

        firstContainer.IsSelected.ShouldBeFalse();
        secondContainer.IsSelected.ShouldBeTrue();
        firstContainer.EntryId.ShouldNotBe(secondContainer.EntryId);
    }

    [Fact]
    public void Recycled_Container_Drops_The_Previous_Entry_Context_And_Selected_State()
    {
        var list = CreateList(CreateItem("first"), CreateItem("second"));
        list.Selection.Select(1);
        var container = list.PrepareContainer(1);
        container.IsSelected.ShouldBeTrue();
        container.EntryId.ShouldNotBeNull();

        list.RecycleContainer(container);

        container.EntryId.ShouldBeNull();
        container.IsSelected.ShouldBeFalse();

        list.PrepareContainer(0, container);
        container.IsSelected.ShouldBeFalse();
        container.EntryId.ShouldNotBeNull();
    }

    [Fact]
    public void Dynamic_Items_With_Matching_Content_Project_Selection_To_Only_The_Addressed_Container()
    {
        var first = CreateItem("Dynamic item");
        var list  = CreateList(first);
        var second = CreateItem("Dynamic item");
        list.ItemsSource = new IListItemData[] { first, second };

        list.Selection.Select(1);
        var firstContainer  = list.PrepareContainer(0);
        var secondContainer = list.PrepareContainer(1);

        firstContainer.IsSelected.ShouldBeFalse();
        secondContainer.IsSelected.ShouldBeTrue();

        var third = CreateItem("Dynamic item");
        list.ItemsSource = new IListItemData[] { first, second, third };
        list.Selection.Select(2);
        var secondAfterAddContainer = list.PrepareContainer(1);
        var thirdContainer          = list.PrepareContainer(2);

        secondAfterAddContainer.IsSelected.ShouldBeFalse();
        thirdContainer.IsSelected.ShouldBeTrue();
    }

    private static ListItemData CreateItem(string content)
    {
        return new ListItemData
        {
            Content = content
        };
    }

    private static TestListView CreateList(params IListItemData[] items)
    {
        return new TestListView
        {
            ItemsSource = items
        };
    }

    private sealed class TestListView : AtomUI.Desktop.Controls.ListView
    {
        public ListViewItem PrepareContainer(int index, ListViewItem? container = null)
        {
            container ??= new ListViewItem();
            var item = ItemsView[index];
            PrepareContainerForItemOverride(container, item, index);
            ContainerForItemPreparedOverride(container, item, index);
            return container;
        }

        public void RecycleContainer(ListViewItem container)
        {
            ClearContainerForItemOverride(container);
        }
    }

    private sealed record EqualListItemData(string Value) : IListItemData
    {
        public bool IsEnabled { get; set; } = true;
        public object? Content { get; set; } = Value;
        public EntityKey? ItemKey { get; init; }
        public string? Group { get; init; }
    }
}
