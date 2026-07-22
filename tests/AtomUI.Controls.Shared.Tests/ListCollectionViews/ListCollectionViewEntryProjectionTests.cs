using AtomUI.Controls.Data;
using Shouldly;
using Xunit;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace AtomUI.Controls.Shared.Tests.ListCollectionViews;

public class ListCollectionViewEntryProjectionTests
{
    [Fact]
    public void Bridge_Maps_Duplicate_Occurrences_Without_Item_Lookup()
    {
        var repeated = new ThrowingEqualityRow();
        var view     = new ListCollectionView(new[] { repeated, repeated });
        var bridge   = view.ShouldBeAssignableTo<IListCollectionEntryView>();

        bridge.TryGetSourceEntry(0, out var first).ShouldBeTrue();
        bridge.TryGetSourceEntry(1, out var second).ShouldBeTrue();
        first!.Id.ShouldNotBe(second!.Id);
        bridge.TryGetSourceIndex(first.Id, out var firstIndex).ShouldBeTrue();
        bridge.TryGetSourceIndex(second.Id, out var secondIndex).ShouldBeTrue();
        firstIndex.ShouldBe(0);
        secondIndex.ShouldBe(1);
        bridge.TryGetViewNode(1, out var node).ShouldBeTrue();
        node!.Entry.ShouldBeSameAs(second);
        view.GetItemAt(1).ShouldBeSameAs(repeated);
    }

    [Fact]
    public void ObservableCollection_Changes_Use_Indexed_Entry_Transactions()
    {
        var repeated = new ThrowingEqualityRow();
        var source = new ObservableCollection<object?> { repeated, repeated, "last" };
        var view = new ListCollectionView(source);
        var bridge = view.ShouldBeAssignableTo<IListCollectionEntryView>();
        var callbacks = new List<string>();
        bridge.EntryChangePrepared += (_, e) =>
        {
            callbacks.Add("prepared");
            if (e.ChangeSet.Action == NotifyCollectionChangedAction.Remove)
            {
                e.ChangeSet.OldEntries.Count.ShouldBe(1);
            }
            else
            {
                e.ChangeSet.NewEntries.Count.ShouldBe(1);
                ListCollectionEntry? preparedEntry;
                bridge.TryGetSourceEntry(e.ChangeSet.NewStartingIndex, out preparedEntry).ShouldBeTrue();
            }
        };
        view.CollectionChanged += (_, _) => callbacks.Add("collection");
        bridge.EntryChangeCommitted += (_, _) => callbacks.Add("committed");

        var secondId = bridge.LogicalEntries[1].Id;
        source.RemoveAt(0);

        callbacks.ShouldBe(["prepared", "collection", "committed"]);
        bridge.TryGetSourceIndex(secondId, out var secondIndex).ShouldBeTrue();
        secondIndex.ShouldBe(0);
        view.GetItemAt(0).ShouldBeSameAs(repeated);
    }

    [Fact]
    public void Malformed_Indexed_Notification_Is_Rejected()
    {
        var source = new TestCollection { 1 };
        var view = new ListCollectionView(source);

        Should.Throw<InvalidOperationException>(() => source.Raise(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 2)));
    }

    [Fact]
    public void Sorted_Projection_Maps_View_Nodes_Back_To_Source_Occurrences()
    {
        var repeated = new ProjectionRow(2);
        var first    = new ProjectionRow(3);
        var source   = new ObservableCollection<object?> { first, repeated, repeated };
        var view     = new ListCollectionView(source);
        view.SortDescriptions.Add(ListSortDescription.FromComparer(
            Comparer<object>.Create((left, right) =>
                ((ProjectionRow)left).Order.CompareTo(((ProjectionRow)right).Order))));
        var bridge = view.ShouldBeAssignableTo<IListCollectionEntryView>();

        bridge.TryGetSourceEntry(0, out var firstEntry).ShouldBeTrue();
        bridge.TryGetSourceEntry(1, out var repeatedEntry).ShouldBeTrue();
        bridge.TryGetSourceEntry(2, out var secondRepeatedEntry).ShouldBeTrue();
        bridge.TryGetViewNode(0, out var firstNode).ShouldBeTrue();
        bridge.TryGetViewNode(1, out var repeatedNode).ShouldBeTrue();
        bridge.TryGetViewNode(2, out var secondRepeatedNode).ShouldBeTrue();

        firstNode!.Entry!.Id.ShouldBe(repeatedEntry!.Id);
        repeatedNode!.Entry!.Id.ShouldBe(secondRepeatedEntry!.Id);
        secondRepeatedNode!.Entry!.Id.ShouldBe(firstEntry!.Id);
        bridge.TryGetSourceIndex(secondRepeatedNode.Entry.Id, out var secondRepeatedSourceIndex).ShouldBeTrue();
        secondRepeatedSourceIndex.ShouldBe(0);
    }

    [Fact]
    public void Filtered_Projection_Refreshes_Entry_Identity_After_Indexed_Move()
    {
        var first    = new ProjectionRow(1);
        var repeated = new ProjectionRow(2);
        var source   = new ObservableCollection<object?> { first, repeated, repeated };
        var view     = new ListCollectionView(source);
        view.Filter = item => ((ProjectionRow)item).Order > 1;
        var bridge = view.ShouldBeAssignableTo<IListCollectionEntryView>();
        var movedEntryId = bridge.LogicalEntries[0].Id;

        source.Move(1, 2);

        bridge.TryGetViewNode(0, out var node).ShouldBeTrue();
        node!.Entry!.Id.ShouldNotBe(movedEntryId);
        bridge.TryGetViewNode(1, out var movedNode).ShouldBeTrue();
        movedNode!.Entry!.Id.ShouldBe(movedEntryId);
        bridge.TryGetSourceIndex(movedEntryId, out var sourceIndex).ShouldBeTrue();
        sourceIndex.ShouldBe(2);
    }

    [Fact]
    public void Grouped_Projection_Leaves_Group_Headers_Unselectable_And_Maps_Leaves()
    {
        var first  = new GroupedProjectionRow("a", 1);
        var second = new GroupedProjectionRow("b", 2);
        var source = new ObservableCollection<object?> { first, second };
        var view   = new ListCollectionView(source);
        view.GroupDescriptions.Add(new ListGroupDescription(item => ((GroupedProjectionRow)item!).Group));
        var bridge = view.ShouldBeAssignableTo<IListCollectionEntryView>();

        bridge.TryGetViewNode(0, out var firstHeader).ShouldBeTrue();
        firstHeader!.IsGroupHeader.ShouldBeTrue();
        bridge.TryGetViewNode(1, out var firstLeaf).ShouldBeTrue();
        firstLeaf!.Entry!.Item.ShouldBeSameAs(first);
        bridge.TryGetViewNode(2, out var secondHeader).ShouldBeTrue();
        secondHeader!.IsGroupHeader.ShouldBeTrue();
        bridge.TryGetViewNode(3, out var secondLeaf).ShouldBeTrue();
        secondLeaf!.Entry!.Item.ShouldBeSameAs(second);
        bridge.LogicalEntries.ShouldBe([firstLeaf.Entry, secondLeaf.Entry]);
    }

    private sealed class ThrowingEqualityRow
    {
        public override bool Equals(object? obj) => throw new InvalidOperationException("Equals must not be called.");
        public override int GetHashCode() => throw new InvalidOperationException("GetHashCode must not be called.");
    }

    private sealed class TestCollection : ObservableCollection<object?>
    {
        public void Raise(NotifyCollectionChangedEventArgs args) => OnCollectionChanged(args);
    }

    private sealed class ProjectionRow
    {
        public int Order { get; }

        public ProjectionRow(int order)
        {
            Order = order;
        }
    }

    private sealed class GroupedProjectionRow
    {
        public string Group { get; }
        public int Order { get; }

        public GroupedProjectionRow(string group, int order)
        {
            Group = group;
            Order = order;
        }
    }
}
