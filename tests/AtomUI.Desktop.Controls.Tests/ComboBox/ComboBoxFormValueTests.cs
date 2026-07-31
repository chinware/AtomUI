using AtomUI.Controls;
using Shouldly;
using Xunit;
using AtomUIComboBox = AtomUI.Desktop.Controls.ComboBox;

namespace AtomUI.Desktop.Controls.Tests.ComboBox;

public class ComboBoxFormValueTests
{
    static ComboBoxFormValueTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Form_Set_Value_Selects_Item_Instance()
    {
        var firstItem  = new DataItem("Alpha");
        var secondItem = new DataItem("Beta");
        var comboBox = new AtomUIComboBox
        {
            ItemsSource = new List<DataItem> { firstItem, secondItem }
        };
        var formItem = (IFormItemAware)comboBox;

        formItem.SetFormValue(secondItem);

        comboBox.SelectedItem.ShouldBeSameAs(secondItem);
        comboBox.SelectedIndex.ShouldBe(1);
    }

    [Fact]
    public void Form_Get_Value_Returns_Selected_Item_Instance()
    {
        var selectedItem = new DataItem("Alpha");
        var comboBox = new AtomUIComboBox
        {
            ItemsSource   = new List<DataItem> { selectedItem },
            SelectedIndex = 0
        };
        var formItem = (IFormItemAware)comboBox;

        formItem.GetFormValue().ShouldBeSameAs(selectedItem);
    }

    [Fact]
    public void Form_Clear_Value_Clears_Selected_Item()
    {
        var selectedItem = new DataItem("Alpha");
        var comboBox = new AtomUIComboBox
        {
            ItemsSource   = new List<DataItem> { selectedItem },
            SelectedIndex = 0
        };
        var formItem = (IFormItemAware)comboBox;

        formItem.ClearFormValue();

        comboBox.SelectedItem.ShouldBeNull();
        comboBox.SelectedIndex.ShouldBe(-1);
    }

    private sealed record DataItem(string Name);
}
