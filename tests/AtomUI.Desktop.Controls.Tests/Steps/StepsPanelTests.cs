using System.Linq;
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
            Type = Desktop.Controls.StepsType.Inline,
            Orientation = Orientation.Horizontal
        };
        panel.Children.Add(new FixedSizeControl(40, 20));
        panel.Children.Add(new FixedSizeControl(80, 30));

        panel.Measure(new Size(300, 100));
        panel.Arrange(new Rect(0, 0, 300, 100));
        panel.Children.Select(child => child.Bounds.Width).ShouldBe([40d, 80d]);

        panel.Type = Desktop.Controls.StepsType.Navigation;
        panel.Measure(new Size(300, 100));
        panel.Arrange(new Rect(0, 0, 300, 100));
        panel.Children.Select(child => child.Bounds.Width).ShouldBe([150d, 150d]);

        panel.Orientation = Orientation.Vertical;
        panel.Measure(new Size(300, 100));
        panel.Arrange(new Rect(0, 0, 300, 100));
        panel.Children.Select(child => child.Bounds.Y).ShouldBe([0d, 20d]);
        panel.Children.Select(child => child.Bounds.Width).ShouldBe([300d, 300d]);
    }

    private sealed class FixedSizeControl(double width, double height) : Avalonia.Controls.Control
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(width, height);
        }
    }
}
