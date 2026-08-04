using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AtomUI.Desktop.Controls.Data;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.Data;

public class DataGridCollectionViewMoveTests
{
    [Fact]
    public void DataGridCollectionView_Exposes_Optional_Move_Capability()
    {
        var moveSupport = typeof(DataGridCollectionView).GetInterface(
            "AtomUI.Desktop.Controls.Data.IDataGridCollectionViewMoveSupport");

        moveSupport.ShouldNotBeNull();
    }

    [Fact]
    public void Mutable_Flat_List_Can_Move_By_View_Index()
    {
        var source = new ObservableCollection<TestItem>
        {
            new("A"),
            new("B"),
            new("C")
        };
        var view        = new DataGridCollectionView(source);
        var moveSupport = (IDataGridCollectionViewMoveSupport)view;

        moveSupport.CanMove.ShouldBeTrue();
        moveSupport.TryMove(0, 2).ShouldBeTrue();

        source.Select(item => item.Name).ShouldBe(["B", "C", "A"]);
        view.Cast<TestItem>().Select(item => item.Name).ShouldBe(["B", "C", "A"]);
    }

    [Fact]
    public void Same_Or_Out_Of_Range_View_Index_Does_Not_Move()
    {
        var source      = new ObservableCollection<int>([1, 2, 3]);
        var view        = new DataGridCollectionView(source);
        var moveSupport = (IDataGridCollectionViewMoveSupport)view;

        moveSupport.TryMove(1, 1).ShouldBeFalse();
        moveSupport.TryMove(-1, 1).ShouldBeFalse();
        moveSupport.TryMove(1, source.Count).ShouldBeFalse();

        source.ShouldBe([1, 2, 3]);
    }

    [Fact]
    public void Read_Only_Fixed_And_Non_List_Sources_Cannot_Move()
    {
        var arrayView    = new DataGridCollectionView(new[] { 1, 2, 3 });
        var readOnlyView = new DataGridCollectionView(ArrayList.ReadOnly([1, 2, 3]));
        var enumerable   = Enumerable.Range(1, 3).Where(static _ => true);
        var enumerableView = new DataGridCollectionView(enumerable);

        ((IDataGridCollectionViewMoveSupport)arrayView).CanMove.ShouldBeFalse();
        ((IDataGridCollectionViewMoveSupport)readOnlyView).CanMove.ShouldBeFalse();
        ((IDataGridCollectionViewMoveSupport)enumerableView).CanMove.ShouldBeFalse();
    }

    [Fact]
    public void Active_View_Transforms_Disable_Move()
    {
        var source = new ObservableCollection<TestItem>
        {
            new("B"),
            new("A")
        };
        var view        = new DataGridCollectionView(source);
        var moveSupport = (IDataGridCollectionViewMoveSupport)view;

        view.SortDescriptions.Add(DataGridSortDescription.FromComparer(new TestItemComparer()));
        moveSupport.CanMove.ShouldBeFalse();
        view.SortDescriptions.Clear();

        view.Filter = static item => ((TestItem)item).Name == "A";
        moveSupport.CanMove.ShouldBeFalse();
        view.Filter = null;

        view.GroupDescriptions.Add(new DataGridPathGroupDescription(nameof(TestItem.Name)));
        moveSupport.CanMove.ShouldBeFalse();
        view.GroupDescriptions.Clear();

        view.PageSize = 1;
        moveSupport.CanMove.ShouldBeFalse();
    }

    [Fact]
    public void DataGrid_Default_Filter_Without_Filter_Descriptions_Does_Not_Disable_Move()
    {
        var source      = new ObservableCollection<int>([1, 2, 3]);
        var view        = new DataGridCollectionView(source);
        var moveSupport = (IDataGridCollectionViewMoveSupport)view;
        view.Filter = new DataGridDefaultFilter(view);

        moveSupport.CanMove.ShouldBeTrue();

        view.FilterDescriptions.Add(new DataGridFilterDescription
        {
            FilterConditions = [1]
        });
        moveSupport.CanMove.ShouldBeFalse();
    }

    [Fact]
    public void Add_Edit_And_Defer_Transactions_Disable_Move()
    {
        var source = new ObservableCollection<EditableItem>
        {
            new() { Name = "A" },
            new() { Name = "B" }
        };
        var view        = new DataGridCollectionView(source);
        var moveSupport = (IDataGridCollectionViewMoveSupport)view;

        view.EditItem(source[0]);
        moveSupport.CanMove.ShouldBeFalse();
        view.CancelEdit();

        view.AddNew();
        moveSupport.CanMove.ShouldBeFalse();
        view.CancelNew();

        using (view.DeferRefresh())
        {
            moveSupport.CanMove.ShouldBeFalse();
        }
    }

    [Fact]
    public void Move_Uses_Index_Identity_For_Duplicate_Equal_Items()
    {
        var first       = new TestItem("Same");
        var second      = new TestItem("Same");
        var third       = new TestItem("Other");
        var source      = new ObservableCollection<TestItem>([first, second, third]);
        var view        = new DataGridCollectionView(source);
        var moveSupport = (IDataGridCollectionViewMoveSupport)view;

        moveSupport.TryMove(1, 2).ShouldBeTrue();

        source[0].ShouldBeSameAs(first);
        source[1].ShouldBeSameAs(third);
        source[2].ShouldBeSameAs(second);
    }

    [Fact]
    public void Null_Item_Can_Move()
    {
        var source      = new ObservableCollection<object?>(["A", null, "B"]);
        var view        = new DataGridCollectionView(source);
        var moveSupport = (IDataGridCollectionViewMoveSupport)view;

        moveSupport.TryMove(1, 2).ShouldBeTrue();

        source.ShouldBe(["A", "B", null]);
    }

    [Fact]
    public void Insert_Failure_Restores_Original_Order_And_Rethrows()
    {
        var source      = new ThrowingInsertList(new[] { "A", "B", "C" }, throwAtIndex: 2);
        var view        = new DataGridCollectionView(source);
        var moveSupport = (IDataGridCollectionViewMoveSupport)view;

        Should.Throw<InvalidOperationException>(() => moveSupport.TryMove(0, 2));

        source.Cast<string>().ShouldBe(["A", "B", "C"]);
        view.Cast<string>().ShouldBe(["A", "B", "C"]);
    }

    [Fact]
    public void Reentrant_Source_Change_Aborts_Move_And_Restores_Dragged_Item()
    {
        var source      = new ObservableCollection<string>(["A", "B", "C"]);
        var view        = new DataGridCollectionView(source);
        var moveSupport = (IDataGridCollectionViewMoveSupport)view;
        source.CollectionChanged += HandleCollectionChanged;

        moveSupport.TryMove(0, 2).ShouldBeFalse();

        source.ShouldBe(["A", "B", "C", "External"]);
        view.Cast<string>().ShouldBe(["A", "B", "C", "External"]);
        return;

        void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                source.CollectionChanged -= HandleCollectionChanged;
                source.Add("External");
            }
        }
    }

    [Fact]
    public void View_Notification_Failure_Rolls_Back_Source_And_View()
    {
        var source      = new ObservableCollection<string>(["A", "B", "C"]);
        var view        = new DataGridCollectionView(source);
        var moveSupport = (IDataGridCollectionViewMoveSupport)view;
        NotifyCollectionChangedEventHandler? handler = null;
        handler = (_, args) =>
        {
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                view.CollectionChanged -= handler;
                throw new InvalidOperationException("Reset observer failed.");
            }
        };
        view.CollectionChanged += handler;

        Should.Throw<InvalidOperationException>(() => moveSupport.TryMove(0, 2));

        source.ShouldBe(["A", "B", "C"]);
        view.Cast<string>().ShouldBe(["A", "B", "C"]);
    }

    private sealed record TestItem(string Name);

    private sealed class TestItemComparer : IComparer
    {
        public int Compare(object? x, object? y)
        {
            return string.Compare(
                ((TestItem?)x)?.Name,
                ((TestItem?)y)?.Name,
                StringComparison.Ordinal);
        }
    }

    private sealed class EditableItem : IEditableObject
    {
        public string? Name { get; set; }

        public void BeginEdit()
        {
        }

        public void CancelEdit()
        {
        }

        public void EndEdit()
        {
        }
    }

    private sealed class ThrowingInsertList : IList
    {
        private readonly ArrayList _items;
        private readonly int _throwAtIndex;

        public ThrowingInsertList(IEnumerable items, int throwAtIndex)
        {
            _items        = new ArrayList(items.Cast<object>().ToArray());
            _throwAtIndex = throwAtIndex;
        }

        public int Add(object? value)
        {
            return _items.Add(value);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(object? value)
        {
            return _items.Contains(value);
        }

        public int IndexOf(object? value)
        {
            return _items.IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            if (index == _throwAtIndex)
            {
                throw new InvalidOperationException("Insert failed.");
            }

            _items.Insert(index, value);
        }

        public void Remove(object? value)
        {
            _items.Remove(value);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public object? this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public bool IsReadOnly => false;
        public bool IsFixedSize => false;
        public int Count => _items.Count;
        public object SyncRoot => _items.SyncRoot;
        public bool IsSynchronized => false;

        public void CopyTo(Array array, int index)
        {
            _items.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
