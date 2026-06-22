using System.Globalization;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AtomUISlider = AtomUI.Desktop.Controls.Slider;
using AtomUISliderMark = AtomUI.Desktop.Controls.SliderMark;
using AtomUISliderTrack = AtomUI.Desktop.Controls.SliderTrack;
using AtomUIToolTip = AtomUI.Desktop.Controls.ToolTip;
using AvaloniaWindow = Avalonia.Controls.Window;

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

    [Fact]
    public void Detaching_Slider_Releases_Template_Pointer_Handlers()
    {
        var slider = new AtomUISlider();
        var window = new AvaloniaWindow
        {
            Width   = 240,
            Height  = 160,
            Content = slider
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            GetPrivateField<IDisposable?>(slider, "_pointerMovedDispose").ShouldNotBeNull();
            GetPrivateField<IDisposable?>(slider, "_pointerPressDispose").ShouldNotBeNull();
            GetPrivateField<IDisposable?>(slider, "_pointerReleaseDispose").ShouldNotBeNull();

            window.Close();
            Dispatcher.UIThread.RunJobs();

            GetPrivateField<IDisposable?>(slider, "_pointerMovedDispose").ShouldBeNull();
            GetPrivateField<IDisposable?>(slider, "_pointerPressDispose").ShouldBeNull();
            GetPrivateField<IDisposable?>(slider, "_pointerReleaseDispose").ShouldBeNull();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Enabling_Range_Mode_After_Template_Configures_End_Thumb_ToolTip()
    {
        var slider = new AtomUISlider
        {
            Minimum             = 0,
            Maximum             = 100,
            Orientation         = Orientation.Vertical,
            RangeValue          = new SliderRangeValue { StartValue = 20, EndValue = 80 },
            ValueFormatTemplate = "{0:0}%"
        };
        var window = new AvaloniaWindow
        {
            Width   = 240,
            Height  = 240,
            Content = slider
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var track = GetInternalProperty<AtomUISliderTrack?>(slider, "SliderTrack");
            track.ShouldNotBeNull();
            track!.EndSliderThumb.ShouldNotBeNull();
            track.EndSliderThumb!.IsVisible.ShouldBeFalse();

            slider.IsRangeMode = true;
            Dispatcher.UIThread.RunJobs();

            var endThumb = track.EndSliderThumb;
            endThumb.ShouldNotBeNull();
            endThumb!.IsVisible.ShouldBeTrue();
            AtomUIToolTip.GetPlacement(endThumb!).ShouldBe(PlacementMode.Right);
            AtomUIToolTip.GetTip(endThumb!).ShouldBe("80%");
            AtomUIToolTip.GetTipHostWidth(endThumb!).ShouldBeGreaterThan(0);
        }
        finally
        {
            window.Close();
        }
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

    private static T GetPrivateField<T>(object target, string fieldName)
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        return (T)field.GetValue(target)!;
    }
}
