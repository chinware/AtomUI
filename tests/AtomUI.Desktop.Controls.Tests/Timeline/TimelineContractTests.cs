using AtomUI.Controls;
using Avalonia.Layout;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Timeline;

public class TimelineContractTests
{
    [Fact]
    public void TimelineMode_Uses_Logical_Placement_Names()
    {
        Enum.GetNames<TimelineMode>().ShouldBe(["Start", "End", "Alternate"]);
    }

    [Fact]
    public void Timeline_Defaults_To_Vertical_Start_Layout()
    {
        var timeline            = new Desktop.Controls.Timeline();
        var orientationProperty = timeline.GetType().GetProperty("Orientation");

        orientationProperty.ShouldNotBeNull();
        orientationProperty.PropertyType.ShouldBe(typeof(Orientation));
        orientationProperty.GetValue(timeline).ShouldBe(Orientation.Vertical);
        timeline.Mode.ShouldBe(TimelineMode.Start);
    }
}
