using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using AtomUI.Controls.Data;
using AtomUI.Desktop.Controls.Data;
using AtomUI.Desktop.Controls.Utils;
using Avalonia.Data;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.DataMemberAccessors;

public class DataGridDataMemberAccessorsTests
{
    [Fact]
    public void DataGridCollectionViewUsesDescriptorFactoryForNonGenericCollections()
    {
        DataMemberAccessorRegistry.TryGet(typeof(SampleRow), out var descriptor).ShouldBeTrue();

        var source = new ArrayList();
        var view   = new DataGridCollectionView(source, descriptor);

        view.CanAddNew.ShouldBeTrue();
        var item = view.AddNew();

        item.ShouldBeOfType<SampleRow>();
        source.Count.ShouldBe(1);
        source[0].ShouldBeSameAs(item);
    }

    [Fact]
    public void GeneratedAccessorsExposeDataGridMetadata()
    {
        DataMemberAccessorRegistry.TryGet(typeof(MetadataRow), out var descriptor).ShouldBeTrue();
        var descriptorMetadata = descriptor.ShouldBeAssignableTo<IDataMemberAccessorDescriptorMetadataProvider>();
        descriptorMetadata.IsDataTypeReadOnly.ShouldBeTrue();

        var visibleAccessor = descriptor.Accessors.Single(accessor => accessor.Path == nameof(MetadataRow.VisibleScore));
        var visibleMetadata = visibleAccessor.ShouldBeAssignableTo<IDataMemberAccessorMetadataProvider>().Metadata;
        visibleAccessor.ValueType.ShouldBe(typeof(int));
        visibleAccessor.CanWrite.ShouldBeTrue();
        visibleMetadata.DisplayName.ShouldBe("Visible score");
        visibleMetadata.AutoGenerateField.ShouldBe(true);
        visibleMetadata.Order.ShouldBe(2);
        visibleMetadata.IsReadOnly.ShouldBeFalse();
        visibleMetadata.IsEditable.ShouldBeTrue();

        var hiddenMetadata = descriptor.Accessors.Single(accessor => accessor.Path == nameof(MetadataRow.HiddenScore))
                                       .ShouldBeAssignableTo<IDataMemberAccessorMetadataProvider>()
                                       .Metadata;
        hiddenMetadata.AutoGenerateField.ShouldBe(false);

        var readOnlyMetadata = descriptor.Accessors.Single(accessor => accessor.Path == nameof(MetadataRow.ReadOnlyScore))
                                         .ShouldBeAssignableTo<IDataMemberAccessorMetadataProvider>()
                                         .Metadata;
        readOnlyMetadata.IsReadOnly.ShouldBeTrue();

        var nonEditableMetadata = descriptor.Accessors.Single(accessor => accessor.Path == nameof(MetadataRow.NonEditableScore))
                                            .ShouldBeAssignableTo<IDataMemberAccessorMetadataProvider>()
                                            .Metadata;
        nonEditableMetadata.IsEditable.ShouldBeFalse();
    }

    [Fact]
    public void DataGridDataConnectionUsesDescriptorMetadataForEmptyNonGenericCollection()
    {
        DataMemberAccessorRegistry.TryGet(typeof(MetadataRow), out var descriptor).ShouldBeTrue();
        var source = new ArrayList();
        var view   = new DataGridCollectionView(source, descriptor);
        var grid   = new global::AtomUI.Desktop.Controls.DataGrid
        {
            ItemsSource = view
        };
        grid.DataConnection.DataSource = view;

        grid.DataConnection.DataType.ShouldBe(typeof(MetadataRow));
        grid.DataConnection.GetDisplayName(nameof(MetadataRow.VisibleScore)).ShouldBe("Visible score");
        grid.DataConnection.GetPropertyType(nameof(MetadataRow.VisibleScore)).ShouldBe(typeof(int));
        grid.DataConnection.GetPropertyIsReadOnly(nameof(MetadataRow.VisibleScore)).ShouldBeTrue();
        grid.DataConnection.GetPropertyIsReadOnly(nameof(MetadataRow.HiddenScore)).ShouldBeTrue();
        grid.DataConnection.GetPropertyIsReadOnly(nameof(MetadataRow.NonEditableScore)).ShouldBeTrue();
    }

    [Fact]
    public void DataGridBoundColumnAppliesDefaultModeAndConverterToCompiledBindingExtension()
    {
        var binding = CreateCompiledBindingExtension(nameof(SampleRow.Name));

        var column = new DataGridTextColumn
        {
            Binding = binding
        };

        column.Binding.ShouldBeSameAs(binding);
        binding.Mode.ShouldBe(BindingMode.TwoWay);
        binding.Converter.ShouldBeSameAs(DataGridValueConverter.Instance);
    }

    [Fact]
    public void DataGridBoundColumnRejectsOneWayToSourceCompiledBindingExtension()
    {
        var binding = CreateCompiledBindingExtension(nameof(SampleRow.Name));
        binding.Mode = BindingMode.OneWayToSource;

        Should.Throw<InvalidOperationException>(() =>
            new DataGridTextColumn
            {
                Binding = binding
            });
    }

    [Fact]
    public void DataGridCollectionViewSortsWithDescriptorAccessorPath()
    {
        var descriptor = CreateManualDescriptor();
        var source = new ArrayList
        {
            new ManualRow { Score = 2 },
            new ManualRow { Score = null },
            new ManualRow { Score = 1 }
        };
        var view = new DataGridCollectionView(source, descriptor);

        view.SortDescriptions.Add(DataGridSortDescription.FromPath("SyntheticScore"));

        view.Cast<ManualRow>().Select(row => row.Score).ShouldBe([null, 1, 2]);
    }

    [Fact]
    public void DataGridCollectionViewFiltersWithDescriptorAccessorPath()
    {
        var descriptor = CreateManualDescriptor();
        var source = new ArrayList
        {
            new ManualRow { Name = "alpha", Score = 1 },
            new ManualRow { Name = "beta", Score = 2 }
        };
        var view = new DataGridCollectionView(source, descriptor);
        view.Filter = new DataGridDefaultFilter(view);

        view.FilterDescriptions.Add(new DataGridFilterDescription
        {
            PropertyPath = "SyntheticName",
            FilterConditions = ["alp"]
        });

        view.Cast<ManualRow>().Select(row => row.Name).ShouldBe(["alpha"]);
    }

    [Fact]
    public void DataGridPathGroupDescriptionUsesDescriptorAccessorPath()
    {
        var descriptor = CreateManualDescriptor();
        var group = new DataGridPathGroupDescription("SyntheticName", descriptor);

        var key = group.GroupKeyFromItem(new ManualRow { Name = "alpha" }, 0, CultureInfo.InvariantCulture);

        key.ShouldBe("alpha");
    }

    [Fact]
    public void DataGridPathSortRequiresGeneratedAccessorWhenDynamicCodeIsDisabled()
    {
        var sort = DataGridSortDescription.FromPath(nameof(UnregisteredRow.Score));

        Should.Throw<InvalidOperationException>(() =>
            sort.Initialize(typeof(UnregisteredRow), isDynamicCodeSupported: false));
    }

    [Fact]
    public void DataGridPathSortUsesBaseGeneratedAccessorForDerivedRowsWhenDynamicCodeIsDisabled()
    {
        var sort = DataGridSortDescription.FromPath(nameof(SampleRow.Score));

        sort.Initialize(typeof(DerivedSampleRow), isDynamicCodeSupported: false);

        sort.Comparer.Compare(
            new DerivedSampleRow { Score = 1 },
            new DerivedSampleRow { Score = 2 }).ShouldBeLessThan(0);
    }

    [Fact]
    public void DataGridPathSortKeepsRuntimeReflectionFallbackWhenDynamicCodeIsEnabled()
    {
        var source = new ArrayList
        {
            new UnregisteredRow { Score = 2 },
            new UnregisteredRow { Score = 1 }
        };
        var view = new DataGridCollectionView(source);

        view.SortDescriptions.Add(DataGridSortDescription.FromPath(nameof(UnregisteredRow.Score)));

        view.Cast<UnregisteredRow>().Select(row => row.Score).ShouldBe([1, 2]);
    }

    [Fact]
    public void DataGridPathFilterRequiresGeneratedAccessorWhenDynamicCodeIsDisabled()
    {
        var filter = new DataGridFilterDescription
        {
            PropertyPath = nameof(UnregisteredRow.Name),
            FilterConditions = ["alpha"]
        };

        Should.Throw<InvalidOperationException>(() =>
            filter.Initialize(typeof(UnregisteredRow), null, isDynamicCodeSupported: false));
    }

    [Fact]
    public void DataGridPathGroupRequiresGeneratedAccessorWhenDynamicCodeIsDisabled()
    {
        var group = new DataGridPathGroupDescription(nameof(UnregisteredRow.Name));

        Should.Throw<InvalidOperationException>(() =>
            group.Initialize(typeof(UnregisteredRow), null, isDynamicCodeSupported: false));
    }

    private static DataMemberAccessorDescriptor<ManualRow> CreateManualDescriptor()
    {
        return new DataMemberAccessorDescriptor<ManualRow>(
            static () => new ManualRow(),
            new IDataMemberAccessor[]
            {
                new DataMemberAccessor<ManualRow, int?>("SyntheticScore", static row => row.Score),
                new DataMemberAccessor<ManualRow, string?>("SyntheticName", static row => row.Name)
            });
    }

    private static CompiledBindingExtension CreateCompiledBindingExtension(string propertyName)
    {
        var pathBuilder = new CompiledBindingPathBuilder();
        var property = new ClrPropertyInfo(
            propertyName,
            target => ((SampleRow)target).Name,
            (target, value) => ((SampleRow)target).Name = (string?)value,
            typeof(string));
        pathBuilder.Property(property, static (_, _) => throw new NotSupportedException());
        return new CompiledBindingExtension(pathBuilder.Build());
    }

    [GenerateDataMemberAccessors]
    public partial class SampleRow
    {
        public string? Name { get; set; }

        public int? Score { get; set; }
    }

    public sealed class DerivedSampleRow : SampleRow
    {
    }

    [GenerateDataMemberAccessors]
    [ReadOnly(true)]
    public partial class MetadataRow
    {
        [Display(ShortName = "Visible score", AutoGenerateField = true, Order = 2)]
        public int VisibleScore { get; set; }

        [Display(AutoGenerateField = false)]
        public int HiddenScore { get; set; }

        [ReadOnly(true)]
        public int ReadOnlyScore { get; set; }

        [Editable(false)]
        public int NonEditableScore { get; set; }
    }

    private sealed class ManualRow
    {
        public string? Name { get; set; }

        public int? Score { get; set; }
    }

    private sealed class UnregisteredRow
    {
        public string? Name { get; set; }

        public int Score { get; set; }
    }
}
