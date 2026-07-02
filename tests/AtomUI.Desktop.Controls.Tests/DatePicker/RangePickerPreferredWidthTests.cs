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
    public void DatePicker_Empty_Input_Uses_Format_Reserve_As_Preferred_Width()
    {
        const string placeholder = "Select date";
        var picker = new DatePicker
        {
            PlaceholderText = placeholder
        };

        ShowInWindow(picker, () =>
        {
            var expectedWidth = Math.Max(
                MeasureTextWidth(picker, placeholder),
                MeasureWidestDatePickerTextWidth(picker));

            picker.PreferredInputWidth.ShouldBeGreaterThanOrEqualTo(expectedWidth);
            picker.PreferredInputWidth.ShouldBeLessThanOrEqualTo(expectedWidth + WidthTolerance);
        });
    }

    [Fact]
    public void DatePicker_Empty_Time_Input_Uses_Format_Reserve_As_Preferred_Width()
    {
        const string placeholder = "Select date";
        var picker = new DatePicker
        {
            IsShowTime      = true,
            PlaceholderText = placeholder
        };

        ShowInWindow(picker, () =>
        {
            var expectedWidth = Math.Max(
                MeasureTextWidth(picker, placeholder),
                MeasureWidestDatePickerTextWidth(picker));

            picker.PreferredInputWidth.ShouldBeGreaterThanOrEqualTo(expectedWidth);
            picker.PreferredInputWidth.ShouldBeLessThanOrEqualTo(expectedWidth + WidthTolerance);
        });
    }

    [Fact]
    public void DatePicker_Selected_Input_Uses_Format_Reserve_As_Preferred_Width()
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
            var expectedWidth = Math.Max(
                MeasureTextWidth(picker, picker.PlaceholderText!),
                MeasureWidestDatePickerTextWidth(picker));

            picker.PreferredInputWidth.ShouldBeGreaterThanOrEqualTo(expectedWidth);
            picker.PreferredInputWidth.ShouldBeLessThanOrEqualTo(expectedWidth + WidthTolerance);
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
            var input         = FindPart(picker, "PART_InfoInputBox").ShouldBeOfType<TextBox>();
            var expectedWidth = MeasureWidestDatePickerTextWidth(picker);

            input.FontSize.ShouldBe(picker.FontSize);
            picker.PreferredInputWidth.ShouldBeGreaterThanOrEqualTo(expectedWidth);
            picker.PreferredInputWidth.ShouldBeLessThanOrEqualTo(expectedWidth + WidthTolerance);
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
    public void DatePicker_Selected_Value_Change_Does_Not_Resize_Preferred_Input_Width()
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

            picker.PreferredInputWidth.ShouldBe(emptyWidth, WidthTolerance);
        });
    }

    [Theory]
    [InlineData(DatePickerMode.Date, "2026-07-08")]
    [InlineData(DatePickerMode.Week, "2026-28周")]
    [InlineData(DatePickerMode.Month, "2026-07")]
    [InlineData(DatePickerMode.Quarter, "2026-Q3")]
    [InlineData(DatePickerMode.Year, "2026")]
    public void DatePicker_Formats_Selected_Value_By_Picker_Mode(DatePickerMode pickerMode, string expectedText)
    {
        var picker = new DatePicker
        {
            PickerMode       = pickerMode,
            SelectedDateTime = new DateTime(2026, 7, 8)
        };

        ShowInWindow(picker, () =>
        {
            var input = FindPart(picker, "PART_InfoInputBox").ShouldBeOfType<TextBox>();

            input.Text.ShouldBe(expectedText);
        });
    }

    [Theory]
    [InlineData(DatePickerMode.Date, 2026, 7, 8, 2026, 7, 8)]
    [InlineData(DatePickerMode.Week, 2026, 7, 8, 2026, 7, 6)]
    [InlineData(DatePickerMode.Month, 2026, 7, 8, 2026, 7, 1)]
    [InlineData(DatePickerMode.Quarter, 2026, 8, 8, 2026, 7, 1)]
    [InlineData(DatePickerMode.Year, 2026, 7, 8, 2026, 1, 1)]
    public void DatePicker_Normalizes_Selected_Date_By_Picker_Mode(
        DatePickerMode pickerMode,
        int sourceYear,
        int sourceMonth,
        int sourceDay,
        int expectedYear,
        int expectedMonth,
        int expectedDay)
    {
        var source   = new DateTime(sourceYear, sourceMonth, sourceDay, 15, 30, 45);
        var expected = new DateTime(expectedYear, expectedMonth, expectedDay);

        DatePickerFormattingHelper.NormalizeDateTime(source, pickerMode).ShouldBe(expected);
    }

    [Fact]
    public void DatePicker_Picker_Mode_Change_Recalculates_Preferred_Input_Width()
    {
        var picker = new DatePicker
        {
            PickerMode       = DatePickerMode.Date,
            SelectedDateTime = new DateTime(2026, 7, 8)
        };

        ShowInWindow(picker, () =>
        {
            var dateWidth = picker.PreferredInputWidth;

            picker.PickerMode = DatePickerMode.Year;
            Dispatcher.UIThread.RunJobs();

            picker.PreferredInputWidth.ShouldBeLessThan(dateWidth);
        });
    }

    [Fact]
    public void RangeDatePicker_Empty_Input_Uses_Format_Reserve_As_Preferred_Width()
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
                Math.Max(
                    MeasureTextWidth(picker, placeholder),
                    MeasureTextWidth(picker, secondaryPlaceholder)),
                MeasureWidestDatePickerTextWidth(picker));

            picker.PreferredWidth.ShouldBeGreaterThanOrEqualTo(expectedWidth);
            picker.PreferredWidth.ShouldBeLessThanOrEqualTo(expectedWidth + WidthTolerance);
        });
    }

    [Fact]
    public void RangeDatePicker_Empty_Time_Input_Uses_Format_Reserve_As_Preferred_Width()
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
                Math.Max(
                    MeasureTextWidth(picker, placeholder),
                    MeasureTextWidth(picker, secondaryPlaceholder)),
                MeasureWidestDatePickerTextWidth(picker));

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
    public void RangeDatePicker_With_Time_Reserves_Preferred_Width_For_Both_Input_Parts()
    {
        var picker = new RangeDatePicker
        {
            IsShowTime               = true,
            PlaceholderText          = "Select date",
            SecondaryPlaceholderText = "End date"
        };

        ShowInWindow(picker, () =>
        {
            var startInput = FindPart(picker, "PART_InfoInputBox");
            var endInput   = FindPart(picker, "PART_SecondaryInfoInputBox");

            picker.Bounds.Width.ShouldBeGreaterThanOrEqualTo((picker.PreferredWidth * 2) - WidthTolerance);
            startInput.Bounds.Width.ShouldBeGreaterThanOrEqualTo(picker.PreferredWidth - WidthTolerance);
            endInput.Bounds.Width.ShouldBeGreaterThanOrEqualTo(picker.PreferredWidth - WidthTolerance);
        }, width: 1000);
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
    public void RangeDatePicker_Selected_Value_Change_Does_Not_Resize_Preferred_Width()
    {
        var picker = new RangeDatePicker
        {
            IsShowTime               = true,
            PlaceholderText          = "Start",
            SecondaryPlaceholderText = "End"
        };

        ShowInWindow(picker, () =>
        {
            var emptyWidth = picker.PreferredWidth;

            picker.RangeStartSelectedDate = new DateTime(2026, 6, 26, 0, 0, 0);
            picker.RangeEndSelectedDate   = new DateTime(2026, 12, 31, 23, 59, 59);
            Dispatcher.UIThread.RunJobs();

            picker.PreferredWidth.ShouldBe(emptyWidth, WidthTolerance);
            picker.PreferredInputWidth.ShouldBe(emptyWidth, WidthTolerance);
        });
    }

    [Fact]
    public void RangeDatePicker_Formats_Selected_Values_By_Picker_Mode()
    {
        var picker = new RangeDatePicker
        {
            PickerMode              = DatePickerMode.Quarter,
            RangeStartSelectedDate  = new DateTime(2026, 1, 8),
            RangeEndSelectedDate    = new DateTime(2026, 10, 18)
        };

        ShowInWindow(picker, () =>
        {
            var startInput = FindPart(picker, "PART_InfoInputBox").ShouldBeOfType<TextBox>();
            var endInput   = FindPart(picker, "PART_SecondaryInfoInputBox").ShouldBeOfType<TextBox>();

            startInput.Text.ShouldBe("2026-Q1");
            endInput.Text.ShouldBe("2026-Q4");
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

    private static double MeasureWidestDatePickerTextWidth(DatePicker picker)
    {
        var format     = DatePickerFormattingHelper.GetEffectiveFormat(picker.Format, picker.IsShowTime, picker.ClockIdentifier);
        var formatInfo = DatePickerFormattingHelper.CreateFormatInfo(picker.ClockIdentifier, picker.AmText, picker.PmText);
        return DatePickerFormattingHelper.CalculatePreferredInputWidth(
            null,
            format,
            picker.FontSize,
            picker.FontFamily,
            picker.FontStyle,
            picker.FontWeight,
            formatInfo);
    }

    private static double MeasureWidestDatePickerTextWidth(RangeDatePicker picker)
    {
        var format     = DatePickerFormattingHelper.GetEffectiveFormat(picker.Format, picker.IsShowTime, picker.ClockIdentifier);
        var formatInfo = DatePickerFormattingHelper.CreateFormatInfo(picker.ClockIdentifier, picker.AmText, picker.PmText);
        return DatePickerFormattingHelper.CalculatePreferredInputWidth(
            null,
            format,
            picker.FontSize,
            picker.FontFamily,
            picker.FontStyle,
            picker.FontWeight,
            formatInfo);
    }

    private static void ShowInWindow(Control content, Action assertion, double width = 360)
    {
        var window = new AvaloniaWindow
        {
            Width   = width,
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
