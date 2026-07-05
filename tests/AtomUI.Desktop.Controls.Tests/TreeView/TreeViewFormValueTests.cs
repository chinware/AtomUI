using AtomUI.Controls;
using Avalonia.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.TreeView;

public class TreeViewFormValueTests
{
    static TreeViewFormValueTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Form_Set_Value_Selects_Item_Instance_In_Single_Mode()
    {
        var firstNode  = new DataNode("Alpha");
        var secondNode = new DataNode("Beta");
        var treeView = new AtomUI.Desktop.Controls.TreeView
        {
            ItemsSource = new List<DataNode> { firstNode, secondNode }
        };
        var formItem = (IFormItemAware)treeView;

        formItem.SetFormValue(secondNode);

        treeView.SelectedItem.ShouldBeSameAs(secondNode);
    }

    [Fact]
    public void Form_Get_Value_Returns_Selected_Item_Instance_In_Single_Mode()
    {
        var selectedNode = new DataNode("Alpha");
        var treeView = new AtomUI.Desktop.Controls.TreeView
        {
            ItemsSource   = new List<DataNode> { selectedNode },
            SelectedItem  = selectedNode
        };
        var formItem = (IFormItemAware)treeView;

        formItem.GetFormValue().ShouldBeSameAs(selectedNode);
    }

    [Fact]
    public void Form_Clear_Value_Clears_Selected_Item_In_Single_Mode()
    {
        var selectedNode = new DataNode("Alpha");
        var treeView = new AtomUI.Desktop.Controls.TreeView
        {
            ItemsSource  = new List<DataNode> { selectedNode },
            SelectedItem = selectedNode
        };
        var formItem = (IFormItemAware)treeView;

        formItem.ClearFormValue();

        treeView.SelectedItem.ShouldBeNull();
    }

    [Fact]
    public void Form_Set_Value_Selects_Items_Instance_In_Multiple_Mode()
    {
        var firstNode  = new DataNode("Alpha");
        var secondNode = new DataNode("Beta");
        var selectedNodes = new List<DataNode> { firstNode, secondNode };
        var treeView = new AtomUI.Desktop.Controls.TreeView
        {
            ItemsSource    = selectedNodes,
            SelectionMode  = SelectionMode.Multiple
        };
        var formItem = (IFormItemAware)treeView;

        formItem.SetFormValue(selectedNodes);

        treeView.SelectedItems.ShouldBeSameAs(selectedNodes);
    }

    [Fact]
    public void Form_Get_Value_Returns_Selected_Items_Instance_In_Multiple_Mode()
    {
        var selectedNodes = new List<DataNode>
        {
            new("Alpha"),
            new("Beta")
        };
        var treeView = new AtomUI.Desktop.Controls.TreeView
        {
            ItemsSource    = selectedNodes,
            SelectionMode  = SelectionMode.Multiple,
            SelectedItems  = selectedNodes
        };
        var formItem = (IFormItemAware)treeView;

        formItem.GetFormValue().ShouldBeSameAs(selectedNodes);
    }

    [Fact]
    public void Form_Clear_Value_Clears_Selected_Items_In_Multiple_Mode()
    {
        var selectedNodes = new List<DataNode>
        {
            new("Alpha"),
            new("Beta")
        };
        var treeView = new AtomUI.Desktop.Controls.TreeView
        {
            ItemsSource    = selectedNodes,
            SelectionMode  = SelectionMode.Multiple,
            SelectedItems  = selectedNodes
        };
        var formItem = (IFormItemAware)treeView;

        formItem.ClearFormValue();

        treeView.SelectedItems.Count.ShouldBe(0);
    }

    private sealed record DataNode(string Name);
}
