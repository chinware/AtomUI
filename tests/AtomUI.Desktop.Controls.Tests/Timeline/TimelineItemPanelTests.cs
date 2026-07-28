using System;
using AtomUI.Controls;
using AtomUI.Controls.Commons;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Timeline;

public class TimelineItemPanelTests
{
    static TimelineItemPanelTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Horizontal_Double_Sided_Start_Places_Label_Above_Axis()
    {
        var layout = CreateLayout(
            Orientation.Horizontal,
            TimelineMode.Start,
            isOdd: false,
            isLabelLayout: true,
            indicatorSpacing: 5);

        layout.Panel.Measure(new Size(100, double.PositiveInfinity));
        layout.Panel.Arrange(new Rect(0, 0, 100, layout.Panel.DesiredSize.Height));

        layout.Panel.DesiredSize.ShouldBe(new Size(100, 80));
        layout.Label.Bounds.ShouldBe(new Rect(0, 0, 100, 30));
        layout.Indicator.Bounds.ShouldBe(new Rect(0, 35, 100, 10));
        layout.Content.Bounds.ShouldBe(new Rect(0, 50, 100, 30));
    }

    [Theory]
    [InlineData(false, 0, 50)]
    [InlineData(true, 50, 0)]
    public void Horizontal_Alternate_Uses_Visual_Parity(bool isOdd, double labelY, double contentY)
    {
        var layout = CreateLayout(
            Orientation.Horizontal,
            TimelineMode.Alternate,
            isOdd,
            isLabelLayout: false,
            indicatorSpacing: 5);

        layout.Panel.Measure(new Size(100, double.PositiveInfinity));
        layout.Panel.Arrange(new Rect(0, 0, 100, layout.Panel.DesiredSize.Height));

        layout.Label.Bounds.Y.ShouldBe(labelY);
        layout.Indicator.Bounds.ShouldBe(new Rect(0, 35, 100, 10));
        layout.Content.Bounds.Y.ShouldBe(contentY);
    }

    [Theory]
    [InlineData(TimelineMode.Start, 0, 15)]
    [InlineData(TimelineMode.End, 35, 0)]
    public void Horizontal_Single_Sided_Mode_Places_Content_On_Requested_Side(
        TimelineMode mode,
        double indicatorY,
        double contentY)
    {
        var layout = CreateLayout(
            Orientation.Horizontal,
            mode,
            isOdd: false,
            isLabelLayout: false,
            indicatorSpacing: 5);

        layout.Panel.Measure(new Size(100, double.PositiveInfinity));
        layout.Panel.Arrange(new Rect(0, 0, 100, layout.Panel.DesiredSize.Height));

        layout.Panel.DesiredSize.ShouldBe(new Size(100, 45));
        layout.Indicator.Bounds.ShouldBe(new Rect(0, indicatorY, 100, 10));
        layout.Content.Bounds.ShouldBe(new Rect(0, contentY, 100, 30));
    }

    [Theory]
    [InlineData(false, 0, 60)]
    [InlineData(true, 60, 0)]
    public void Vertical_Alternate_Starts_From_Start(bool isOdd, double labelX, double contentX)
    {
        var layout = CreateLayout(
            Orientation.Vertical,
            TimelineMode.Alternate,
            isOdd,
            isLabelLayout: false,
            indicatorSpacing: 5);

        layout.Panel.Measure(new Size(110, 40));
        layout.Panel.Arrange(new Rect(0, 0, 110, 40));

        layout.Label.Bounds.X.ShouldBe(labelX);
        layout.Indicator.Bounds.ShouldBe(new Rect(50, 0, 10, 40));
        layout.Content.Bounds.X.ShouldBe(contentX);
    }

    private static LayoutParts CreateLayout(
        Orientation orientation,
        TimelineMode mode,
        bool isOdd,
        bool isLabelLayout,
        double indicatorSpacing)
    {
        var controlsAssembly = typeof(AbstractTimeline).Assembly;
        var panelType = controlsAssembly.GetType(
            "AtomUI.Controls.Commons.TimelineItemPanel",
            throwOnError: true)!;
        var indicatorType = controlsAssembly.GetType(
            "AtomUI.Controls.Commons.TimelineIndicator",
            throwOnError: true)!;
        var panel     = (Panel)Activator.CreateInstance(panelType)!;
        var indicator = (Control)Activator.CreateInstance(indicatorType)!;
        var label = new TextBlock
        {
            Text      = "Label",
            MinHeight = 20
        };
        var content = new ContentPresenter
        {
            Content   = "Content",
            MinHeight = 30
        };
        indicator.MinWidth  = 10;
        indicator.MinHeight = 10;

        SetProperty(panel, "Orientation", orientation);
        SetProperty(panel, "Mode", mode);
        SetProperty(panel, "IsOdd", isOdd);
        SetProperty(panel, "IsLabelLayout", isLabelLayout);
        SetProperty(panel, "IndicatorSpacing", indicatorSpacing);

        panel.Children.Add(label);
        panel.Children.Add(indicator);
        panel.Children.Add(content);
        return new LayoutParts(panel, label, indicator, content);
    }

    private static void SetProperty(Control control, string propertyName, object value)
    {
        var property = control.GetType().GetProperty(propertyName);
        property.ShouldNotBeNull().SetValue(control, value);
    }

    private sealed record LayoutParts(
        Panel Panel,
        TextBlock Label,
        Control Indicator,
        ContentPresenter Content);
}
