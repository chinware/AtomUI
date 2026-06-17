using System.Runtime.CompilerServices;
using AtomUI.Controls.Data;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class ListShowCaseAotTests
{
    [Fact]
    public void Ordered_List_ShowCase_Model_Registers_Aot_Content_Accessor()
    {
        RuntimeHelpers.RunModuleConstructor(typeof(IListItemData).Module.ModuleHandle);

        DataMemberAccessorRegistry.TryGetCompatible(typeof(IListItemData), out var descriptor).ShouldBeTrue();
        descriptor.TryGetAccessor(nameof(IListItemData.Content), out _)
                  .ShouldBeTrue($"Missing AOT data member accessor for {nameof(IListItemData)}.{nameof(IListItemData.Content)}.");
    }
}
