using Avalonia;
using Avalonia.Layout;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Steps;

public class StepsPanelTests
{
    static StepsPanelTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Horizontal_Default_Keeps_Last_Item_AutoSized()
    {
        var panel = new Desktop.Controls.StepsPanel
        {
            Type = Desktop.Controls.StepsType.Default,
            Orientation = Orientation.Horizontal
        };
        panel.Children.Add(new FixedSizeControl(40, 24));
        panel.Children.Add(new FixedSizeControl(80, 24));
        panel.Children.Add(new FixedSizeControl(60, 24));

        panel.Measure(new Size(300, 100));
        panel.Arrange(new Rect(0, 0, 300, 100));

        panel.Children.Select(child => child.Bounds.Width).ShouldBe([120d, 120d, 60d]);
        panel.Children.Select(child => child.Bounds.X).ShouldBe([0d, 120d, 240d]);
    }

    [Fact]
    public void Horizontal_Default_Centers_Common_Row_When_Final_Height_Exceeds_Desired_Height()
    {
        var panel = new Desktop.Controls.StepsPanel
        {
            Type        = Desktop.Controls.StepsType.Default,
            Orientation = Orientation.Horizontal
        };
        panel.Children.Add(new FixedSizeControl(40, 20));
        panel.Children.Add(new FixedSizeControl(80, 30));
        panel.Children.Add(new FixedSizeControl(60, 10));

        panel.Measure(new Size(300, 100));
        panel.Arrange(new Rect(0, 0, 300, 100));

        panel.Children.Select(child => child.Bounds.Y).ShouldBe([35d, 35d, 35d]);
        panel.Children.Select(child => child.Bounds.Height).ShouldBe([30d, 30d, 30d]);
    }

    [Fact]
    public void Layout_Panels_Are_Internal_Implementation_Details()
    {
        typeof(Desktop.Controls.StepsPanel).IsPublic.ShouldBeFalse();
        typeof(Desktop.Controls.StepsItemLayoutPanel).IsPublic.ShouldBeFalse();
    }

    [Fact]
    public void Horizontal_Navigation_Arranges_Items_Equally()
    {
        var panel = new Desktop.Controls.StepsPanel
        {
            Type = Desktop.Controls.StepsType.Navigation,
            Orientation = Orientation.Horizontal
        };
        panel.Children.Add(new FixedSizeControl(40, 24));
        panel.Children.Add(new FixedSizeControl(80, 24));
        panel.Children.Add(new FixedSizeControl(20, 24));

        panel.Measure(new Size(300, 100));
        panel.Arrange(new Rect(0, 0, 300, 100));

        panel.Children.Select(child => child.Bounds.Width)
             .ShouldBe([100d, 100d, 100d]);
    }

    [Fact]
    public void Panel_Is_Horizontal_And_Arranges_Items_Equally_Even_When_Orientation_Is_Vertical()
    {
        var panel = new Desktop.Controls.StepsPanel
        {
            Type        = Desktop.Controls.StepsType.Panel,
            Orientation = Avalonia.Layout.Orientation.Vertical
        };
        panel.Children.Add(new FixedSizeControl(40, 24));
        panel.Children.Add(new FixedSizeControl(80, 24));
        panel.Children.Add(new FixedSizeControl(20, 24));

        panel.Measure(new Size(300, 100));
        panel.Arrange(new Rect(0, 0, 300, 100));

        panel.Children.Select(child => child.Bounds.Width)
             .ShouldBe([100d, 100d, 100d]);
        panel.Children.Select(child => child.Bounds.X)
             .ShouldBe([0d, 100d, 200d]);
        panel.Children.Select(child => child.Bounds.Y)
             .ShouldBe([0d, 0d, 0d]);
    }

    [Theory]
    [InlineData(HorizontalAlignment.Left, 0d, 80d)]
    [InlineData(HorizontalAlignment.Center, 110d, 80d)]
    [InlineData(HorizontalAlignment.Right, 220d, 80d)]
    [InlineData(HorizontalAlignment.Stretch, 0d, 300d)]
    public void Vertical_Navigation_Honors_Horizontal_Content_Alignment(
        HorizontalAlignment alignment,
        double expectedX,
        double expectedWidth)
    {
        var panel = new Desktop.Controls.StepsPanel
        {
            Type                       = Desktop.Controls.StepsType.Navigation,
            Orientation                = Orientation.Vertical,
            HorizontalContentAlignment = alignment
        };
        panel.Children.Add(new FixedSizeControl(40, 20));
        panel.Children.Add(new FixedSizeControl(80, 30));
        panel.Children.Add(new FixedSizeControl(20, 10));

        panel.Measure(new Size(300, 100));
        panel.Arrange(new Rect(0, 0, 300, 100));

        panel.Children.Select(child => child.Bounds.X).ShouldBe([expectedX, expectedX, expectedX]);
        panel.Children.Select(child => child.Bounds.Width).ShouldBe([expectedWidth, expectedWidth, expectedWidth]);
    }

    [Fact]
    public void Horizontal_Inline_Offset_Reserves_Leading_Cells()
    {
        var panel = new Desktop.Controls.StepsPanel
        {
            Type           = Desktop.Controls.StepsType.Inline,
            Orientation    = Orientation.Horizontal,
            TitlePlacement = Orientation.Vertical,
            Offset         = 2
        };
        panel.Children.Add(new FixedSizeControl(40, 24));
        panel.Children.Add(new FixedSizeControl(40, 24));
        panel.Children.Add(new FixedSizeControl(40, 24));

        panel.Measure(new Size(500, 100));
        panel.Arrange(new Rect(0, 0, 500, 100));

        panel.Children.Select(child => child.Bounds.X).ShouldBe([200d, 300d, 400d]);
        panel.Children.Select(child => child.Bounds.Width).ShouldBe([100d, 100d, 100d]);
    }

    [Fact]
    public void Vertical_Stacks_Items_At_Their_Desired_Heights()
    {
        var panel = new Desktop.Controls.StepsPanel
        {
            Orientation = Orientation.Vertical
        };
        panel.Children.Add(new FixedSizeControl(40, 20));
        panel.Children.Add(new FixedSizeControl(80, 30));
        panel.Children.Add(new FixedSizeControl(20, 10));

        panel.Measure(new Size(300, 100));
        panel.Arrange(new Rect(0, 0, 300, 100));

        panel.Children.Select(child => child.Bounds.Y).ShouldBe([0d, 20d, 50d]);
        panel.Children.Select(child => child.Bounds.Width).ShouldBe([300d, 300d, 300d]);
    }

    [Fact]
    public void Runtime_Type_And_Orientation_Changes_Rearrange_Items()
    {
        var panel = new Desktop.Controls.StepsPanel
        {
            Type = Desktop.Controls.StepsType.Default,
            Orientation = Orientation.Horizontal
        };
        panel.Children.Add(new FixedSizeControl(40, 20));
        panel.Children.Add(new FixedSizeControl(80, 30));

        panel.Measure(new Size(300, 100));
        panel.Arrange(new Rect(0, 0, 300, 100));
        panel.Children.Select(child => child.Bounds.Width).ShouldBe([220d, 80d]);

        panel.Type = Desktop.Controls.StepsType.Navigation;
        panel.Measure(new Size(300, 100));
        panel.Arrange(new Rect(0, 0, 300, 100));
        panel.Children.Select(child => child.Bounds.Width).ShouldBe([150d, 150d]);

        panel.Orientation = Orientation.Vertical;
        panel.Measure(new Size(300, 100));
        panel.Arrange(new Rect(0, 0, 300, 100));
        panel.Children.Select(child => child.Bounds.Y).ShouldBe([0d, 20d]);
        panel.Children.Select(child => child.Bounds.X).ShouldBe([110d, 110d]);
        panel.Children.Select(child => child.Bounds.Width).ShouldBe([80d, 80d]);
    }

    private sealed class FixedSizeControl(double width, double height) : Avalonia.Controls.Control
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(width, height);
        }
    }
}
