using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.DatePickers;

public class RangePickerPreferredWidthTests
{
    static RangePickerPreferredWidthTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void DatePicker_Recalculates_Preferred_Input_Width_When_ShowTime_Changes()
    {
        var picker = new DatePicker
        {
            SelectedDateTime = new DateTime(2024, 1, 20, 0, 22, 23)
        };

        ShowInWindow(picker, () =>
        {
            var dateOnlyWidth = picker.PreferredInputWidth;

            picker.IsShowTime = true;
            Dispatcher.UIThread.RunJobs();

            picker.PreferredInputWidth.ShouldBeGreaterThan(dateOnlyWidth);
        });
    }

    [Fact]
    public void DatePicker_Recalculates_Preferred_Input_Width_When_Format_Changes()
    {
        var picker = new DatePicker
        {
            SelectedDateTime = new DateTime(2024, 1, 20, 0, 22, 23)
        };

        ShowInWindow(picker, () =>
        {
            var dateOnlyWidth = picker.PreferredInputWidth;

            picker.Format = "yyyy-MM-dd HH:mm:ss";
            Dispatcher.UIThread.RunJobs();

            picker.PreferredInputWidth.ShouldBeGreaterThan(dateOnlyWidth);
        });
    }

    [Fact]
    public void RangeDatePicker_With_Time_Uses_Calculated_Input_Width_For_Overall_Measure()
    {
        var picker = new RangeDatePicker
        {
            IsShowTime            = true,
            RangeStartDefaultDate = new DateTime(2024, 1, 20, 0, 22, 23),
            RangeEndDefaultDate   = new DateTime(2024, 2, 20, 7, 22, 23)
        };

        ShowInWindow(picker, () =>
        {
            picker.PreferredInputWidth.ShouldBeGreaterThan(0);
            picker.PreferredWidth.ShouldBe(picker.PreferredInputWidth, 0.001);
        });
    }

    [Fact]
    public void RangeDatePicker_Recalculates_Preferred_Width_When_ShowTime_Changes()
    {
        var picker = new RangeDatePicker
        {
            RangeStartDefaultDate = new DateTime(2024, 1, 20, 0, 22, 23),
            RangeEndDefaultDate   = new DateTime(2024, 2, 20, 7, 22, 23)
        };

        ShowInWindow(picker, () =>
        {
            var dateOnlyWidth = picker.PreferredWidth;

            picker.IsShowTime = true;
            Dispatcher.UIThread.RunJobs();

            picker.PreferredWidth.ShouldBeGreaterThan(dateOnlyWidth);
        });
    }

    [Fact]
    public void RangeDatePicker_Recalculates_Preferred_Width_When_Format_Changes()
    {
        var picker = new RangeDatePicker
        {
            RangeStartDefaultDate = new DateTime(2024, 1, 20, 0, 22, 23),
            RangeEndDefaultDate   = new DateTime(2024, 2, 20, 7, 22, 23)
        };

        ShowInWindow(picker, () =>
        {
            var dateOnlyWidth = picker.PreferredWidth;

            picker.Format = "yyyy-MM-dd HH:mm:ss";
            Dispatcher.UIThread.RunJobs();

            picker.PreferredWidth.ShouldBeGreaterThan(dateOnlyWidth);
        });
    }

    [Fact]
    public void RangeTimePicker_Uses_Calculated_Input_Width_For_Overall_Measure()
    {
        var picker = new RangeTimePicker
        {
            RangeStartDefaultTime = new TimeSpan(0, 22, 23),
            RangeEndDefaultTime   = new TimeSpan(7, 22, 23)
        };

        ShowInWindow(picker, () =>
        {
            picker.PreferredInputWidth.ShouldBeGreaterThan(0);
            picker.PreferredWidth.ShouldBe(picker.PreferredInputWidth, 0.001);
        });
    }

    [Fact]
    public void RangeDatePicker_Constrained_Layout_Does_Not_Overlap_Range_Input_Parts()
    {
        var picker = new RangeDatePicker
        {
            IsShowTime            = true,
            RangeStartDefaultDate = new DateTime(2024, 1, 20, 0, 22, 23),
            RangeEndDefaultDate   = new DateTime(2024, 2, 20, 7, 22, 23)
        };
        var host = new Border
        {
            Width = 420,
            Child = picker
        };

        ShowInWindow(host, () => AssertRangePartsDoNotOverlap(picker));
    }

    private static void AssertRangePartsDoNotOverlap(Control picker)
    {
        var startInput = FindPart(picker, "PART_InfoInputBox");
        var arrow      = FindPart(picker, "PART_RangePickerArrow");
        var endInput   = FindPart(picker, "PART_SecondaryInfoInputBox");

        startInput.Bounds.Right.ShouldBeLessThanOrEqualTo(arrow.Bounds.Left + 0.001);
        arrow.Bounds.Right.ShouldBeLessThanOrEqualTo(endInput.Bounds.Left + 0.001);
    }

    private static Control FindPart(Control control, string name)
    {
        return control.GetVisualDescendants()
                      .OfType<Control>()
                      .Single(part => part.Name == name);
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 160,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }
}
