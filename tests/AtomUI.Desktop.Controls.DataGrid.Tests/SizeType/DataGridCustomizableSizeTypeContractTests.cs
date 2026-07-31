using System.Reflection;
using Avalonia;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.SizeType;

public class DataGridCustomizableSizeTypeContractTests
{
    public static IEnumerable<object[]> TargetControlTypes()
    {
        yield return [typeof(Desktop.Controls.DataGrid)];
        yield return [typeof(DataGridColumnHeader)];
        yield return [typeof(DataGridColumnGroupHeader)];
        yield return [typeof(DataGridCell)];
        yield return [typeof(DataGridRow)];
        yield return [typeof(DataGridRowHeader)];
    }

    [Theory]
    [MemberData(nameof(TargetControlTypes))]
    public void DataGrid_Controls_Use_Customizable_SizeType_Contract(Type controlType)
    {
        var property = controlType.GetProperty(
            "SizeType",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        property.ShouldNotBeNull($"{controlType.FullName} must expose SizeType.");
        property!.PropertyType.ShouldBe(typeof(CustomizableSizeType), $"{controlType.FullName}.SizeType should support Custom.");

        var field = controlType.GetField(
            "SizeTypeProperty",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        field.ShouldNotBeNull($"{controlType.FullName} must expose SizeTypeProperty.");
        field!.FieldType.ShouldBe(
            typeof(StyledProperty<CustomizableSizeType>),
            $"{controlType.FullName}.SizeTypeProperty should use CustomizableSizeType.");
    }
}
