using System.Runtime.CompilerServices;
using AtomUI.Controls.Data;
using AtomUIGallery.ShowCases.DataGrid;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class DataGridShowCaseAotTests
{
    public static TheoryData<Type, string[]> SortableDataGridModelPaths => new()
    {
        {
            typeof(DataGridBaseInfo),
            [
                nameof(DataGridBaseInfo.Name),
                nameof(DataGridBaseInfo.Age),
                nameof(DataGridBaseInfo.Address)
            ]
        },
        {
            typeof(MultiSorterDataType),
            [
                nameof(MultiSorterDataType.Name),
                nameof(MultiSorterDataType.Chinese),
                nameof(MultiSorterDataType.Math),
                nameof(MultiSorterDataType.English)
            ]
        }
    };

    [Theory]
    [MemberData(nameof(SortableDataGridModelPaths))]
    public void Sortable_DataGrid_ShowCase_Models_Register_Aot_DataMember_Accessors(Type modelType, string[] paths)
    {
        RuntimeHelpers.RunModuleConstructor(modelType.Module.ModuleHandle);

        DataMemberAccessorRegistry.TryGetCompatible(modelType, out var descriptor).ShouldBeTrue();

        foreach (var path in paths)
        {
            descriptor.TryGetAccessor(path, out _).ShouldBeTrue($"Missing AOT data member accessor for {modelType.Name}.{path}.");
        }
    }
}
