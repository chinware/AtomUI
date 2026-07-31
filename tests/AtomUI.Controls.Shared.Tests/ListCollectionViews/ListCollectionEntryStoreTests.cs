using AtomUI.Controls.Data;
using Shouldly;
using Xunit;

namespace AtomUI.Controls.Shared.Tests.ListCollectionViews;

public class ListCollectionEntryStoreTests
{
    [Fact]
    public void Reset_Assigns_Different_Ids_To_Equal_And_Repeated_Occurrences()
    {
        var equalA   = new EqualRow("same");
        var equalB   = new EqualRow("same");
        var repeated = new object();
        var store    = new ListCollectionEntryStore();

        store.Reset([equalA, equalB, repeated, repeated, 7, 7]);

        store.Entries.Select(entry => entry.Id).Distinct().Count().ShouldBe(6);
        store.Entries[0].Item.ShouldBeSameAs(equalA);
        store.Entries[1].Item.ShouldBeSameAs(equalB);
        store.Entries[2].Item.ShouldBeSameAs(repeated);
        store.Entries[3].Item.ShouldBeSameAs(repeated);
    }

    [Fact]
    public void Indexed_Mutations_Preserve_Occurrence_Identity_And_Use_Indexes()
    {
        var first = new ThrowingEqualityRow("first");
        var second = new ThrowingEqualityRow("second");
        var third = new ThrowingEqualityRow("third");
        var store = new ListCollectionEntryStore();
        store.Reset(new object?[] { first, second, third });

        var secondId = store.Entries[1].Id;
        var add = store.Add(1, new object?[] { "inserted" });
        add.NewEntries.Count.ShouldBe(1);
        store.TryGetSourceIndex(secondId, out var secondIndex).ShouldBeTrue();
        secondIndex.ShouldBe(2);

        var removed = store.Remove(1, 1);
        removed.OldEntries.Count.ShouldBe(1);
        removed.OldEntries[0].Item.ShouldBe("inserted");
        store.TryGetSourceIndex(secondId, out secondIndex).ShouldBeTrue();
        secondIndex.ShouldBe(1);

        var moved = store.Move(1, 2, 1);
        moved.OldEntries[0].Id.ShouldBe(secondId);
        store.TryGetSourceIndex(secondId, out secondIndex).ShouldBeTrue();
        secondIndex.ShouldBe(2);

        var replacement = new ThrowingEqualityRow("replacement");
        var replaced = store.Replace(0, new object?[] { first }, new object?[] { replacement });
        replaced.OldEntries[0].Item.ShouldBeSameAs(first);
        replaced.NewEntries[0].Id.ShouldBe(store.Entries[0].Id);
        store.Entries[0].Item.ShouldBeSameAs(replacement);
    }

    [Fact]
    public void Unequal_Replace_Creates_New_Entries_And_Reset_Never_Reuses_Ids()
    {
        var store = new ListCollectionEntryStore();
        store.Reset(new object?[] { 1, 2, 3 });
        var oldIds = store.Entries.Select(static entry => entry.Id).ToArray();

        var change = store.Replace(1, new object?[] { 2 }, new object?[] { 4, 5 });
        change.OldEntries.Select(static entry => entry.Id).ShouldBe([oldIds[1]]);
        change.NewEntries.Select(static entry => entry.Id).ShouldAllBe(id => id > oldIds.Max());

        store.Reset(new object?[] { 6 });
        store.Entries[0].Id.ShouldBeGreaterThan(change.NewEntries.Max(static entry => entry.Id));
    }

    [Fact]
    public void Invalid_Indexed_Mutations_Do_Not_Partially_Mutate_The_Store()
    {
        var store = new ListCollectionEntryStore();
        store.Reset(new object?[] { 1, 2 });
        var ids = store.Entries.Select(static entry => entry.Id).ToArray();

        Should.Throw<InvalidOperationException>(() => store.Add(3, new object?[] { 4 }));
        Should.Throw<InvalidOperationException>(() => store.Remove(1, 2));
        Should.Throw<InvalidOperationException>(() => store.Move(0, 2, 2));
        Should.Throw<InvalidOperationException>(() => store.Replace(2, new object?[] { 3 }, new object?[] { 4 }));

        store.Entries.Select(static entry => entry.Id).ShouldBe(ids);
    }

    private sealed record EqualRow(string Value);

    private sealed class ThrowingEqualityRow(string value)
    {
        public override bool Equals(object? obj) => throw new InvalidOperationException("Equals must not be called.");
        public override int GetHashCode() => throw new InvalidOperationException("GetHashCode must not be called.");
        public override string ToString() => value;
    }
}
