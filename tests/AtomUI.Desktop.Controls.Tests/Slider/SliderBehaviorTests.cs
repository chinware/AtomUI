using System.Globalization;
using System.Reflection;
using Avalonia.Media;
using Shouldly;
using Xunit;
using AtomUISlider = AtomUI.Desktop.Controls.Slider;
using AtomUISliderMark = AtomUI.Desktop.Controls.SliderMark;
using AtomUISliderTrack = AtomUI.Desktop.Controls.SliderTrack;

namespace AtomUI.Desktop.Controls.Tests.Slider;

public class SliderBehaviorTests
{
    static SliderBehaviorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Snap_To_Decimal_Tick_Normalizes_Value_Written_By_Step_Move()
    {
        var slider = new AtomUISlider
        {
            Minimum             = 0,
            Maximum             = 1,
            SmallChange         = 0.1,
            TickFrequency       = 0.1,
            IsSnapToTickEnabled = true,
            Value               = 0.2
        };

        InvokePrivate(slider, "MoveToNextTick", 0.1);

        slider.Value.ToString("R", CultureInfo.InvariantCulture).ShouldBe("0.3");
    }

    [Fact]
    public void Mark_Label_Brush_Change_Rebuilds_Cached_Formatted_Text()
    {
        var mark = new AtomUISliderMark("30", 30);
        var track = new AtomUISliderTrack
        {
            Marks             = [mark],
            MarkLabelFontSize = 12,
            MarkLabelBrush    = Brushes.Red
        };

        InvokePrivate(track, "CalculateMaxMarkSize", false);
        var formattedText = GetInternalProperty<object>(mark, "FormattedText");

        track.MarkLabelBrush = Brushes.Blue;

        GetInternalProperty<object>(mark, "FormattedText")
            .ShouldNotBeSameAs(formattedText);
    }

    [Fact]
    public void MarkLabelFontFamily_Setter_Stores_FontFamily()
    {
        var track      = new AtomUISliderTrack();
        var fontFamily = new FontFamily("Arial");

        track.MarkLabelFontFamily = fontFamily;

        track.MarkLabelFontFamily.ShouldBe(fontFamily);
    }

    private static void InvokePrivate(object target, string methodName, params object[] args)
    {
        var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        method.ShouldNotBeNull();
        method.Invoke(target, args);
    }

    private static T GetInternalProperty<T>(object target, string propertyName)
    {
        var property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
        property.ShouldNotBeNull();
        return (T)property.GetValue(target)!;
    }
}
