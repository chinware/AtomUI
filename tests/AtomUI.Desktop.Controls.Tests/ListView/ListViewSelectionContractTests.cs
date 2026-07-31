using AtomUI.Controls.Data;
using Shouldly;
using Xunit;
using ListViewControl = AtomUI.Desktop.Controls.ListView;

namespace AtomUI.Desktop.Controls.Tests.ListView;

public class ListViewSelectionContractTests
{
    [Fact]
    public void Selection_Contract_Exposes_Only_Source_Index_Commands()
    {
        typeof(ListViewControl).GetProperty(nameof(ListViewControl.Selection))!.CanWrite.ShouldBeFalse();
        typeof(ListViewControl).GetProperty(nameof(ListViewControl.SelectedIndexes))!.CanWrite.ShouldBeFalse();
        typeof(ListViewControl).GetProperty(nameof(ListViewControl.SelectedItem))!.CanWrite.ShouldBeFalse();
        typeof(ListViewControl).GetProperty(nameof(ListViewControl.SelectedItems))!.CanWrite.ShouldBeFalse();
        typeof(ListViewControl).GetProperty(nameof(ListViewControl.SelectedValue))!.CanWrite.ShouldBeFalse();
        typeof(ListViewControl).GetProperty(nameof(ListViewControl.SelectedIndex))!.CanWrite.ShouldBeTrue();

        typeof(IListViewSelection).GetProperty(nameof(IListViewSelection.SelectedIndexes))!
            .PropertyType.ShouldBe(typeof(IReadOnlyList<int>));
        typeof(IListViewSelection).GetMethod(nameof(IListViewSelection.Select))!
            .GetParameters().Single().ParameterType.ShouldBe(typeof(int));
        typeof(IListItemData).GetProperty("IsSelected").ShouldBeNull();
        new ListItemData { Content = "same" }.Equals(new ListItemData { Content = "same" }).ShouldBeFalse();
    }
}
