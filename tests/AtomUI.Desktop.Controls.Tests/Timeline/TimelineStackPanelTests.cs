using System;
using AtomUI.Controls.Commons;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Timeline;

public class TimelineStackPanelTests
{
    [Fact]
    public void Horizontal_Uses_Equal_Width_Slots()
    {
        var panel = CreatePanel(Orientation.Horizontal, spacing: 10);
        panel.Children.Add(CreateChild(20, 20));
        panel.Children.Add(CreateChild(50, 30));
        panel.Children.Add(CreateChild(80, 40));

        panel.Measure(new Size(320, 100));
        panel.Arrange(new Rect(0, 0, 320, 100));

        panel.Children[0].Bounds.ShouldBe(new Rect(0, 0, 100, 100));
        panel.Children[1].Bounds.ShouldBe(new Rect(110, 0, 100, 100));
        panel.Children[2].Bounds.ShouldBe(new Rect(220, 0, 100, 100));
    }

    [Fact]
    public void Horizontal_Excludes_Hidden_Items_And_Recomputes_Final_Slots()
    {
        var panel = CreatePanel(Orientation.Horizontal, spacing: 10);
        panel.Children.Add(CreateChild(20, 20));
        panel.Children.Add(new Border { IsVisible = false, Child = CreateChild(50, 30) });
        panel.Children.Add(CreateChild(80, 40));

        panel.Measure(new Size(250, 100));
        panel.Arrange(new Rect(0, 0, 410, 100));

        panel.Children[0].Bounds.ShouldBe(new Rect(0, 0, 200, 100));
        panel.Children[1].Bounds.ShouldBe(default);
        panel.Children[2].Bounds.ShouldBe(new Rect(210, 0, 200, 100));
    }

    [Fact]
    public void Horizontal_Infinite_Width_Uses_Natural_Widths()
    {
        var panel = CreatePanel(Orientation.Horizontal, spacing: 10);
        panel.Children.Add(CreateChild(20, 20));
        panel.Children.Add(CreateChild(50, 30));
        panel.Children.Add(CreateChild(80, 40));

        panel.Measure(new Size(double.PositiveInfinity, 100));

        panel.DesiredSize.ShouldBe(new Size(170, 40));
    }

    [Fact]
    public void Vertical_Preserves_Natural_Item_Heights()
    {
        var panel = CreatePanel(Orientation.Vertical, spacing: 10);
        panel.Children.Add(CreateChild(20, 20));
        panel.Children.Add(CreateChild(50, 30));

        panel.Measure(new Size(200, double.PositiveInfinity));
        panel.Arrange(new Rect(0, 0, 200, 60));

        panel.DesiredSize.ShouldBe(new Size(50, 60));
        panel.Children[0].Bounds.ShouldBe(new Rect(0, 0, 200, 20));
        panel.Children[1].Bounds.ShouldBe(new Rect(0, 30, 200, 30));
    }

    private static Border CreateChild(double width, double height)
    {
        return new Border
        {
            Child = new Border
            {
                Width  = width,
                Height = height
            }
        };
    }

    private static StackPanel CreatePanel(Orientation orientation, double spacing)
    {
        var panelType = typeof(AbstractTimeline).Assembly.GetType(
            "AtomUI.Controls.Commons.TimelineStackPanel",
            throwOnError: true);
        var panel = (StackPanel)Activator.CreateInstance(panelType!)!;
        panel.Orientation = orientation;
        panel.Spacing     = spacing;
        return panel;
    }
}
