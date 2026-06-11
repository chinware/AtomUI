using AtomUI.Controls.Data;
using System.Collections;
using Shouldly;
using Xunit;

namespace AtomUI.Controls.Shared.Tests.DataMemberAccessors;

public class GenerateDataMemberAccessorsTests
{
    [Fact]
    public void GeneratedDescriptorProvidesNullableComparerWithDefaultNullOrdering()
    {
        DataMemberAccessorRegistry.TryGet(typeof(SampleRow), out var descriptor).ShouldBeTrue();
        descriptor.ShouldNotBeNull();
        descriptor.TryGetAccessor(nameof(SampleRow.Score), out var accessor).ShouldBeTrue();
        accessor.ShouldNotBeNull();
        accessor.ValueType.ShouldBe(typeof(int?));
        accessor.Comparer.ShouldNotBeNull();
        accessor.Comparer.Compare(null, 1).ShouldBe(-1);
        accessor.Comparer.Compare(1, null).ShouldBe(1);
        accessor.Comparer.Compare(null, null).ShouldBe(0);
    }

    [Fact]
    public void GeneratedDescriptorProvidesGetterSetterAndFactory()
    {
        DataMemberAccessorRegistry.TryGet(typeof(SampleRow), out var descriptor).ShouldBeTrue();
        descriptor.ShouldNotBeNull();
        descriptor.NewItemFactory.ShouldNotBeNull();

        var item = descriptor.NewItemFactory();
        item.ShouldBeOfType<SampleRow>();

        descriptor.TryGetAccessor(nameof(SampleRow.Name), out var accessor).ShouldBeTrue();
        accessor.ShouldNotBeNull();
        accessor.SetValue(item, "updated");
        accessor.GetValue(item).ShouldBe("updated");
    }

    [Fact]
    public void ListCollectionViewUsesDescriptorFactoryForNonGenericCollections()
    {
        DataMemberAccessorRegistry.TryGet(typeof(SampleRow), out var descriptor).ShouldBeTrue();

        var source = new ArrayList();
        var view   = new ListCollectionView(source, descriptor);

        view.CanAddNew.ShouldBeTrue();
        var item = view.AddNew();

        item.ShouldBeOfType<SampleRow>();
        source.Count.ShouldBe(1);
        source[0].ShouldBeSameAs(item);
    }

    [Fact]
    public void ListCollectionViewSortsWithDescriptorAccessorPath()
    {
        var descriptor = new DataMemberAccessorDescriptor<ManualRow>(
            static () => new ManualRow(),
            new IDataMemberAccessor[]
            {
                new DataMemberAccessor<ManualRow, int?>("SyntheticScore", static row => row.Score)
            });

        var source = new ArrayList
        {
            new ManualRow { Score = 2 },
            new ManualRow { Score = null },
            new ManualRow { Score = 1 }
        };
        var view = new ListCollectionView(source, descriptor);

        view.SortDescriptions.Add(ListSortDescription.FromPath("SyntheticScore"));

        view.Cast<ManualRow>().Select(row => row.Score).ShouldBe([null, 1, 2]);
    }

    [Fact]
    public void RuntimeComparerFactoryHandlesNullableNullsWithoutDynamicCode()
    {
        var comparer = DataMemberRuntimeComparerFactory.CreateForNotStringType(typeof(int?), false);

        comparer.ShouldNotBeNull();
        comparer.Compare(null, 1).ShouldBe(-1);
        comparer.Compare(1, null).ShouldBe(1);
        comparer.Compare(null, null).ShouldBe(0);
        comparer.Compare(2, 1).ShouldBe(1);
    }

    [Fact]
    public void RuntimeComparerFactoryDoesNotSilentlyEqualUncomparableTypesWithoutDynamicCode()
    {
        Should.Throw<InvalidOperationException>(() =>
            DataMemberRuntimeComparerFactory.CreateForNotStringType(typeof(UncomparableRow), false));
    }

    [Fact]
    public void PathSortRequiresGeneratedAccessorWhenDynamicCodeIsDisabled()
    {
        var sort = ListSortDescription.FromPath(nameof(UnregisteredRow.Score));

        Should.Throw<InvalidOperationException>(() =>
            sort.Initialize(typeof(UnregisteredRow), isDynamicCodeSupported: false));
    }

    [Fact]
    public void PathSortKeepsRuntimeReflectionFallbackWhenDynamicCodeIsEnabled()
    {
        var source = new ArrayList
        {
            new UnregisteredRow { Score = 2 },
            new UnregisteredRow { Score = 1 }
        };
        var view = new ListCollectionView(source);

        view.SortDescriptions.Add(ListSortDescription.FromPath(nameof(UnregisteredRow.Score)));

        view.Cast<UnregisteredRow>().Select(row => row.Score).ShouldBe([1, 2]);
    }

    [Fact]
    public void PathSortUsesGeneratedAccessorWhenDynamicCodeIsDisabled()
    {
        DataMemberAccessorRegistry.TryGet(typeof(SampleRow), out var descriptor).ShouldBeTrue();

        var sort = ListSortDescription.FromPath(nameof(SampleRow.Score));

        Should.NotThrow(() => sort.Initialize(descriptor.DataType, isDynamicCodeSupported: false));
    }

    [GenerateDataMemberAccessors]
    public partial class SampleRow
    {
        public string? Name { get; set; }

        public int? Score { get; set; }
    }

    private sealed class ManualRow
    {
        public int? Score { get; set; }
    }

    private sealed class UncomparableRow
    {
    }

    private sealed class UnregisteredRow
    {
        public int Score { get; set; }
    }
}
