using System.Collections.ObjectModel;
using AtomUI.Controls.Data;
using Avalonia.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ListView;

public class ListViewSelectionModelTests
{
    [Fact]
    public void Select_Uses_Source_Entry_Identity_For_Duplicate_Occurrences()
    {
        var repeated = new ThrowingEqualityRow();
        var source = new ObservableCollection<object?> { repeated, repeated, 7, 7 };
        var view = new AtomUI.Controls.Data.ListCollectionView(source);
        var bridge = view.ShouldBeAssignableTo<IListCollectionEntryView>();
        var model = new ListViewSelectionModel(bridge);
        model.Mode = SelectionMode.Multiple;

        model.Select(0);
        model.Select(1);
        model.Select(2);
        model.Select(3);

        model.SelectedIndexes.ShouldBe([0, 1, 2, 3]);
        model.SelectedItems.ShouldBe([repeated, repeated, 7, 7]);
        model.IsSelected(0).ShouldBeTrue();
        model.IsSelected(1).ShouldBeTrue();
    }

    [Fact]
    public void Single_Selection_And_Clear_Project_Active_Entry()
    {
        var source = new ObservableCollection<object?> { "first", "second" };
        var view = new AtomUI.Controls.Data.ListCollectionView(source);
        var model = new ListViewSelectionModel(view.ShouldBeAssignableTo<IListCollectionEntryView>());

        model.Select(1);

        model.SelectedIndex.ShouldBe(1);
        model.AnchorIndex.ShouldBe(1);
        model.SelectedItem.ShouldBe("second");
        model.SelectedIndexes.ShouldBe([1]);

        model.Clear();

        model.SelectedIndex.ShouldBe(-1);
        model.AnchorIndex.ShouldBe(-1);
        model.SelectedItems.ShouldBeEmpty();
    }

    private sealed class ThrowingEqualityRow
    {
        public override bool Equals(object? obj) => throw new InvalidOperationException();
        public override int GetHashCode() => throw new InvalidOperationException();
    }
}
