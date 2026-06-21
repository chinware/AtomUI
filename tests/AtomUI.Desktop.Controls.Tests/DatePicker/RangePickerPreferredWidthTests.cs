using System;
using System.Linq;
using AtomUI;
using AtomUI.Media;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.DatePickers;

public class RangePickerPreferredWidthTests
{
    private const double WidthTolerance = 1d;

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
    public void DatePicker_Empty_Input_Uses_Placeholder_As_Preferred_Width()
    {
        const string placeholder = "Select date";
        var picker = new DatePicker
        {
            PlaceholderText = placeholder
        };

        ShowInWindow(picker, () =>
        {
            var placeholderWidth = MeasureTextWidth(picker, placeholder);

            picker.PreferredInputWidth.ShouldBeGreaterThanOrEqualTo(placeholderWidth);
            picker.PreferredInputWidth.ShouldBeLessThanOrEqualTo(placeholderWidth + WidthTolerance);
        });
    }

    [Fact]
    public void DatePicker_Empty_Time_Input_Uses_Placeholder_As_Preferred_Width()
    {
        const string placeholder = "Select date";
        var picker = new DatePicker
        {
            IsShowTime      = true,
            PlaceholderText = placeholder
        };

        ShowInWindow(picker, () =>
        {
            var placeholderWidth = MeasureTextWidth(picker, placeholder);

            picker.PreferredInputWidth.ShouldBeGreaterThanOrEqualTo(placeholderWidth);
            picker.PreferredInputWidth.ShouldBeLessThanOrEqualTo(placeholderWidth + WidthTolerance);
        });
    }

    [Fact]
    public void DatePicker_Selected_Input_Uses_Current_Text_As_Preferred_Width()
    {
        var picker = new DatePicker
        {
            IsShowTime       = true,
            ClockIdentifier  = ClockIdentifierType.HourClock12,
            SelectedDateTime = new DateTime(2026, 6, 26, 0, 0, 0),
            PlaceholderText  = "Select date"
        };

        ShowInWindow(picker, () =>
        {
            var input     = FindPart(picker, "PART_InfoInputBox").ShouldBeOfType<TextBox>();
            var textWidth = MeasureTextWidth(picker, input.Text!);

            picker.PreferredInputWidth.ShouldBeGreaterThanOrEqualTo(textWidth);
            picker.PreferredInputWidth.ShouldBeLessThanOrEqualTo(textWidth + WidthTolerance);
        });
    }

    [Fact]
    public void DatePicker_Custom_Font_Size_Is_Applied_To_Input_And_Preferred_Width()
    {
        var picker = new DatePicker
        {
            SizeType         = CustomizableSizeType.Custom,
            FontSize         = 18,
            SelectedDateTime = new DateTime(2026, 6, 26)
        };

        ShowInWindow(picker, () =>
        {
            var input     = FindPart(picker, "PART_InfoInputBox").ShouldBeOfType<TextBox>();
            var textWidth = MeasureTextWidth(input, input.Text!);

            input.FontSize.ShouldBe(picker.FontSize);
            picker.PreferredInputWidth.ShouldBeGreaterThanOrEqualTo(textWidth);
            picker.PreferredInputWidth.ShouldBeLessThanOrEqualTo(textWidth + WidthTolerance);
        });
    }

    [Fact]
    public void DatePicker_Inner_Text_Presenter_Does_Not_Add_Picker_Width_Reserve()
    {
        var picker = new DatePicker
        {
            SelectedDateTime = new DateTime(2026, 6, 26)
        };

        ShowInWindow(picker, () =>
        {
            var input     = FindPart(picker, "PART_InfoInputBox").ShouldBeOfType<TextBox>();
            var presenter = input.GetVisualDescendants()
                                 .OfType<TextPresenter>()
                                 .Single(part => part.Name == "PART_TextPresenter");

            presenter.Margin.ShouldBe(new Thickness(0));
        });
    }

    [Fact]
    public void DatePicker_Recalculates_Preferred_Input_Width_When_Selected_Value_Changes()
    {
        const string placeholder = "Select date";
        var picker = new DatePicker
        {
            IsShowTime      = true,
            PlaceholderText = placeholder
        };

        ShowInWindow(picker, () =>
        {
            var emptyWidth = picker.PreferredInputWidth;

            picker.SelectedDateTime = new DateTime(2026, 6, 26, 0, 0, 0);
            Dispatcher.UIThread.RunJobs();

            var input     = FindPart(picker, "PART_InfoInputBox").ShouldBeOfType<TextBox>();
            var textWidth = MeasureTextWidth(picker, input.Text!);

            picker.PreferredInputWidth.ShouldBeGreaterThan(emptyWidth);
            picker.PreferredInputWidth.ShouldBeGreaterThanOrEqualTo(textWidth);
            picker.PreferredInputWidth.ShouldBeLessThanOrEqualTo(textWidth + WidthTolerance);
        });
    }

    [Fact]
    public void RangeDatePicker_Empty_Input_Uses_Placeholders_As_Preferred_Width()
    {
        const string placeholder          = "Select date";
        const string secondaryPlaceholder = "End date";
        var picker = new RangeDatePicker
        {
            PlaceholderText          = placeholder,
            SecondaryPlaceholderText = secondaryPlaceholder
        };

        ShowInWindow(picker, () =>
        {
            var expectedWidth = Math.Max(
                MeasureTextWidth(picker, placeholder),
                MeasureTextWidth(picker, secondaryPlaceholder));

            picker.PreferredWidth.ShouldBeGreaterThanOrEqualTo(expectedWidth);
            picker.PreferredWidth.ShouldBeLessThanOrEqualTo(expectedWidth + WidthTolerance);
        });
    }

    [Fact]
    public void RangeDatePicker_Empty_Time_Input_Uses_Placeholders_As_Preferred_Width()
    {
        const string placeholder          = "Select date";
        const string secondaryPlaceholder = "End date";
        var picker = new RangeDatePicker
        {
            IsShowTime               = true,
            PlaceholderText          = placeholder,
            SecondaryPlaceholderText = secondaryPlaceholder
        };

        ShowInWindow(picker, () =>
        {
            var expectedWidth = Math.Max(
                MeasureTextWidth(picker, placeholder),
                MeasureTextWidth(picker, secondaryPlaceholder));

            picker.PreferredWidth.ShouldBeGreaterThanOrEqualTo(expectedWidth);
            picker.PreferredWidth.ShouldBeLessThanOrEqualTo(expectedWidth + WidthTolerance);
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
    public void TimePicker_Selected_Input_Uses_Current_Text_As_Preferred_Width()
    {
        var picker = new TimePicker
        {
            SelectedTime = new TimeSpan(12, 8, 23)
        };

        ShowInWindow(picker, () =>
        {
            var input     = FindPart(picker, "PART_InfoInputBox").ShouldBeOfType<TextBox>();
            var textWidth = MeasureTextWidth(input, input.Text!);

            picker.PreferredInputWidth.ShouldBeGreaterThanOrEqualTo(textWidth);
            picker.PreferredInputWidth.ShouldBeLessThanOrEqualTo(textWidth + WidthTolerance);
        });
    }

    [Fact]
    public void RangeTimePicker_Selected_Input_Uses_Current_Text_As_Preferred_Width()
    {
        var picker = new RangeTimePicker
        {
            RangeStartSelectedTime = new TimeSpan(10, 9, 20),
            RangeEndSelectedTime   = new TimeSpan(12, 12, 20)
        };

        ShowInWindow(picker, () =>
        {
            var startInput = FindPart(picker, "PART_InfoInputBox").ShouldBeOfType<TextBox>();
            var endInput   = FindPart(picker, "PART_SecondaryInfoInputBox").ShouldBeOfType<TextBox>();
            var textWidth  = Math.Max(
                MeasureTextWidth(startInput, startInput.Text!),
                MeasureTextWidth(endInput, endInput.Text!));

            picker.PreferredWidth.ShouldBeGreaterThanOrEqualTo(textWidth);
            picker.PreferredWidth.ShouldBeLessThanOrEqualTo(textWidth + WidthTolerance);
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

    private static double MeasureTextWidth(Control control, string text)
    {
        return TextUtils.CalculateTextSize(
            text,
            control.GetValue(TemplatedControl.FontSizeProperty),
            control.GetValue(TemplatedControl.FontFamilyProperty),
            control.GetValue(TemplatedControl.FontStyleProperty),
            control.GetValue(TemplatedControl.FontWeightProperty)).Width;
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
