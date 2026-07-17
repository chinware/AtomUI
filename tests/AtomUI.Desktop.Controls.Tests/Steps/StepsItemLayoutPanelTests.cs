using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Steps;

public class StepsItemLayoutPanelTests
{
    static StepsItemLayoutPanelTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData(Desktop.Controls.StepsType.Default, Orientation.Horizontal, Orientation.Horizontal, Orientation.Horizontal)]
    [InlineData(Desktop.Controls.StepsType.Dot, Orientation.Horizontal, Orientation.Horizontal, Orientation.Vertical)]
    [InlineData(Desktop.Controls.StepsType.OutlineDot, Orientation.Horizontal, Orientation.Horizontal, Orientation.Vertical)]
    [InlineData(Desktop.Controls.StepsType.Navigation, Orientation.Horizontal, Orientation.Vertical, Orientation.Horizontal)]
    [InlineData(Desktop.Controls.StepsType.Inline, Orientation.Horizontal, Orientation.Horizontal, Orientation.Vertical)]
    [InlineData(Desktop.Controls.StepsType.Default, Orientation.Vertical, Orientation.Vertical, Orientation.Horizontal)]
    public void Effective_Title_Placement_Follows_Type_Rules(
        Desktop.Controls.StepsType type,
        Orientation orientation,
        Orientation requested,
        Orientation expected)
    {
        var panel = new Desktop.Controls.StepsItemLayoutPanel
        {
            Type = type,
            Orientation = orientation,
            TitlePlacement = requested
        };

        panel.EffectiveTitlePlacement.ShouldBe(expected);
    }

    [Fact]
    public void Horizontal_Title_Layout_Places_Header_Row_Content_And_Connector_Without_Overlap()
    {
        var panel     = CreatePanel(Desktop.Controls.StepsType.Default, Orientation.Horizontal, Orientation.Horizontal);
        var indicator = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Indicator, 20, 20);
        var header    = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Header, 60, 20);
        var subHeader = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.SubHeader, 40, 12);
        var content   = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Content, 80, 24);
        var connector = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Connector, 10, 2);
        var arrow     = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.NavigationArrow, 16, 16);

        Layout(panel, 300, 100);

        indicator.Bounds.ShouldBe(new Rect(0, 0, 20, 20));
        header.Bounds.ShouldBe(new Rect(20, 0, 60, 20));
        subHeader.Bounds.ShouldBe(new Rect(80, 4, 40, 12));
        content.Bounds.ShouldBe(new Rect(20, 20, 80, 24));
        connector.Bounds.ShouldBe(new Rect(120, 10, 180, 1));
        arrow.Bounds.ShouldBe(default);
    }

    [Fact]
    public void Horizontal_Connector_Start_Follows_Header_Row_Instead_Of_Wider_Content()
    {
        var panel     = CreatePanel(Desktop.Controls.StepsType.Default, Orientation.Horizontal, Orientation.Horizontal);
        AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Indicator, 20, 20);
        var header    = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Header, 60, 20);
        var subHeader = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.SubHeader, 40, 12);
        AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Content, 180, 24);
        var connector = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Connector, 10, 2);

        Layout(panel, 300, 100);

        connector.Bounds.X.ShouldBe(subHeader.Bounds.Right);
        connector.Bounds.X.ShouldBe(header.Bounds.Right + subHeader.Bounds.Width);
        connector.Bounds.Right.ShouldBe(300);
    }

    [Fact]
    public void Vertical_Title_Layout_Centers_Indicator_And_Stretches_Horizontal_Connector()
    {
        var panel     = CreatePanel(Desktop.Controls.StepsType.Dot, Orientation.Horizontal, Orientation.Horizontal);
        var indicator = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Indicator, 20, 20);
        var header    = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Header, 60, 20);
        var subHeader = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.SubHeader, 40, 12);
        var content   = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Content, 80, 24);
        var connector = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Connector, 10, 2);

        Layout(panel, 300, 100);

        indicator.Bounds.ShouldBe(new Rect(140, 0, 20, 20));
        connector.Bounds.ShouldBe(new Rect(160, 10, 280, 1));
        header.Bounds.ShouldBe(new Rect(120, 20, 60, 20));
        subHeader.Bounds.ShouldBe(new Rect(130, 40, 40, 12));
        content.Bounds.ShouldBe(new Rect(110, 52, 80, 24));
    }

    [Fact]
    public void Inline_Item_Wrapper_Fills_The_Item_Cell_For_Hover_Background()
    {
        var panel = CreatePanel(Desktop.Controls.StepsType.Inline, Orientation.Horizontal, Orientation.Horizontal);
        panel.Padding          = new Thickness(6, 9, 6, 0);
        panel.IndicatorSpacing = 8;
        var wrapper  = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.ItemWrapper, 0, 0);
        var indicator = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Indicator, 6, 6);
        var header    = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Header, 48, 18);
        var subHeader = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.SubHeader, 58, 18);
        AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Connector, 10, 1);

        Layout(panel, 240, 60);

        wrapper.Bounds.ShouldBe(new Rect(0, 0, 240, 60));
        wrapper.Bounds.Contains(indicator.Bounds.TopLeft).ShouldBeTrue();
        wrapper.Bounds.Contains(header.Bounds.Center).ShouldBeTrue();
        wrapper.Bounds.Contains(subHeader.Bounds.Center).ShouldBeTrue();
    }

    [Fact]
    public void Vertical_Steps_Stretch_Connector_Below_Indicator_And_Keep_Content_To_The_Right()
    {
        var panel     = CreatePanel(Desktop.Controls.StepsType.Default, Orientation.Vertical, Orientation.Vertical);
        var indicator = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Indicator, 20, 20);
        var header    = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Header, 60, 20);
        var subHeader = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.SubHeader, 40, 12);
        var content   = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Content, 80, 24);
        var connector = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Connector, 2, 10);

        Layout(panel, 200, 120);

        indicator.Bounds.ShouldBe(new Rect(0, 0, 20, 20));
        header.Bounds.ShouldBe(new Rect(20, 0, 60, 20));
        subHeader.Bounds.ShouldBe(new Rect(80, 4, 40, 12));
        content.Bounds.ShouldBe(new Rect(20, 20, 80, 24));
        connector.Bounds.ShouldBe(new Rect(10, 20, 1, 100));
    }

    [Fact]
    public void Navigation_Arrow_Only_Occupies_Layout_In_Navigation_Type()
    {
        var panel = CreatePanel(Desktop.Controls.StepsType.Default, Orientation.Horizontal, Orientation.Horizontal);
        AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Indicator, 20, 20);
        AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Header, 60, 20);
        var connector = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Connector, 10, 2);
        var arrow = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.NavigationArrow, 16, 16);

        Layout(panel, 200, 60);
        connector.Bounds.Width.ShouldBeGreaterThan(0);
        arrow.Bounds.ShouldBe(default);

        panel.Type = Desktop.Controls.StepsType.Navigation;
        Layout(panel, 200, 60);
        connector.Bounds.ShouldBe(default);
        arrow.Bounds.ShouldBe(new Rect(192, 22, 16, 16));
    }

    [Fact]
    public void Vertical_Navigation_Arrow_Occupies_The_Bottom_Edge()
    {
        var panel = CreatePanel(Desktop.Controls.StepsType.Navigation, Orientation.Vertical, Orientation.Vertical);
        AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Indicator, 20, 20);
        AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Header, 60, 20);
        var arrow = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.NavigationArrow, 16, 16);

        Layout(panel, 200, 120);

        arrow.Bounds.ShouldBe(new Rect(95, 115, 11, 11));
    }

    [Fact]
    public void Changing_Child_Role_Invalidates_Parent_Layout()
    {
        var panel = CreatePanel(Desktop.Controls.StepsType.Default, Orientation.Horizontal, Orientation.Horizontal);
        var first = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Indicator, 20, 20);
        var second = AddChild(panel, Desktop.Controls.StepsItemLayoutRole.Header, 60, 20);

        Layout(panel, 200, 60);
        first.Bounds.X.ShouldBe(0);
        second.Bounds.X.ShouldBe(20);

        Desktop.Controls.StepsItemLayoutPanel.SetRole(first, Desktop.Controls.StepsItemLayoutRole.Header);
        Desktop.Controls.StepsItemLayoutPanel.SetRole(second, Desktop.Controls.StepsItemLayoutRole.Indicator);
        Layout(panel, 200, 60);

        second.Bounds.X.ShouldBe(0);
        first.Bounds.X.ShouldBe(60);
    }

    private static Desktop.Controls.StepsItemLayoutPanel CreatePanel(
        Desktop.Controls.StepsType type,
        Orientation orientation,
        Orientation titlePlacement)
    {
        return new Desktop.Controls.StepsItemLayoutPanel
        {
            Type           = type,
            Orientation    = orientation,
            TitlePlacement = titlePlacement
        };
    }

    private static FixedSizeControl AddChild(
        Desktop.Controls.StepsItemLayoutPanel panel,
        Desktop.Controls.StepsItemLayoutRole role,
        double width,
        double height)
    {
        var child = new FixedSizeControl(width, height);
        Desktop.Controls.StepsItemLayoutPanel.SetRole(child, role);
        panel.Children.Add(child);
        return child;
    }

    private static void Layout(Control control, double width, double height)
    {
        control.Measure(new Size(width, height));
        control.Arrange(new Rect(0, 0, width, height));
    }

    private sealed class FixedSizeControl(double width, double height) : Control
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(width, height);
        }
    }
}
