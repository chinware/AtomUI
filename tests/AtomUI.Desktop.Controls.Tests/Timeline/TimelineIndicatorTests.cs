using System;
using System.Reflection;
using AtomUI.Controls.Commons;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Timeline;

public class TimelineIndicatorTests
{
    static TimelineIndicatorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Horizontal_Render_Draws_Tail_Across_The_Primary_Axis()
    {
        var indicator = CreateIndicator(Orientation.Horizontal, isFirst: false, isLast: false);

        var drawing = Render(indicator, 100, 20);

        var bounds = drawing.GetBounds();
        bounds.Width.ShouldBeGreaterThan(80);
        bounds.Height.ShouldBeLessThan(15);
    }

    [Fact]
    public void Horizontal_First_Item_Does_Not_Draw_Before_The_Node()
    {
        var indicator = CreateIndicator(Orientation.Horizontal, isFirst: true, isLast: false);

        var bounds = Render(indicator, 100, 20).GetBounds();

        bounds.X.ShouldBeGreaterThan(40);
        bounds.Right.ShouldBeGreaterThan(95);
    }

    [Fact]
    public void Horizontal_Last_Item_Does_Not_Draw_After_The_Node()
    {
        var indicator = CreateIndicator(Orientation.Horizontal, isFirst: false, isLast: true);

        var bounds = Render(indicator, 100, 20).GetBounds();

        bounds.X.ShouldBeLessThan(5);
        bounds.Right.ShouldBeLessThan(60);
    }

    [Fact]
    public void Vertical_Render_Preserves_The_Vertical_Axis()
    {
        var indicator = CreateIndicator(Orientation.Vertical, isFirst: false, isLast: false);

        var bounds = Render(indicator, 20, 100).GetBounds();

        bounds.Width.ShouldBeLessThan(15);
        bounds.Height.ShouldBeGreaterThan(80);
    }

    private static Control CreateIndicator(Orientation orientation, bool isFirst, bool isLast)
    {
        var indicatorType = typeof(AbstractTimeline).Assembly.GetType(
            "AtomUI.Controls.Commons.TimelineIndicator",
            throwOnError: true)!;
        var indicator = (Control)Activator.CreateInstance(indicatorType)!;
        SetProperty(indicator, "Orientation", orientation);
        SetProperty(indicator, "IndicatorColor", Brushes.Red);
        SetProperty(indicator, "DefaultIndicatorColor", Brushes.Red);
        SetProperty(indicator, "IndicatorTailColor", Brushes.Red);
        SetProperty(indicator, "IndicatorTailWidth", 2d);
        SetProperty(indicator, "IndicatorDotBorderWidth", 2d);
        SetProperty(indicator, "IndicatorDotSize", 8d);
        SetProperty(indicator, "IndicatorMinHeight", 20d);
        SetProperty(indicator, "IsFirst", isFirst);
        SetProperty(indicator, "IsLast", isLast);
        return indicator;
    }

    private static DrawingGroup Render(Control control, int width, int height)
    {
        control.Measure(new Size(width, height));
        control.Arrange(new Rect(0, 0, width, height));

        var drawing = new DrawingGroup();
        using (var context = drawing.Open())
        {
            control.Render(context);
        }
        return drawing;
    }

    private static void SetProperty(Control control, string propertyName, object value)
    {
        var property = control.GetType().GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property.ShouldNotBeNull().SetValue(control, value);
    }
}
