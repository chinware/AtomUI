using AtomUI.Controls;
using AtomUI.Controls.Commons;
using Shouldly;
using Xunit;
using AtomUITag = AtomUI.Desktop.Controls.Tag;

namespace AtomUI.Desktop.Controls.Tests.Tag;

public class TagContractTests
{
    [Fact]
    public void TagVariant_Uses_Ant_Design_V6_Names()
    {
        Enum.GetNames<TagVariant>().ShouldBe(["Filled", "Solid", "Outlined"]);
    }

    [Fact]
    public void Tag_Defaults_To_Filled_And_Has_No_Legacy_Border_Switch()
    {
        var tag = new AtomUITag();

        tag.Variant.ShouldBe(TagVariant.Filled);
        typeof(AbstractTag).GetProperty("IsBordered").ShouldBeNull();
        typeof(AbstractTag).GetField("IsBorderedProperty").ShouldBeNull();
    }
}
