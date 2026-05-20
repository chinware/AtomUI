using System.Reflection;
using Avalonia.Controls;

namespace AtomUI.Performance;

using AtomDatePicker = AtomUI.Desktop.Controls.DatePicker;
using AtomRangeDatePicker = AtomUI.Desktop.Controls.RangeDatePicker;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateDatePickerScenarios()
    {
        return
        [
            new PerfScenario("DatePicker.Default.Closed", _ => CreateDatePicker()),
            new PerfScenario("DatePicker.Selected.Closed", _ => CreateDatePicker(selectedDateTime: new DateTime(2024, 1, 20))),
            new PerfScenario("DatePicker.NeedConfirm.Closed", _ => CreateDatePicker(isNeedConfirm: true)),
            new PerfScenario("DatePicker.ShowTime.Closed", _ => CreateDatePicker(
                selectedDateTime: new DateTime(2024, 1, 20, 12, 22, 23),
                isNeedConfirm: true,
                isShowTime: true)),
            new PerfScenario("RangeDatePicker.Default.Closed", _ => CreateRangeDatePicker()),
            new PerfScenario("RangeDatePicker.Selected.Closed", _ => CreateRangeDatePicker(
                rangeStartSelectedDate: new DateTime(2024, 1, 20),
                rangeEndSelectedDate: new DateTime(2024, 3, 20))),
            new PerfScenario("RangeDatePicker.ShowTime.Closed", _ => CreateRangeDatePicker(
                rangeStartSelectedDate: new DateTime(2024, 1, 20, 12, 22, 23),
                rangeEndSelectedDate: new DateTime(2024, 2, 20, 7, 22, 23),
                isNeedConfirm: true,
                isShowTime: true)),
            new PerfScenario("DatePicker.Default.PresenterOnly", _ => CreatePickerPresenterForTest(CreateDatePicker())),
            new PerfScenario("DatePicker.ShowTime.PresenterOnly", _ => CreatePickerPresenterForTest(CreateDatePicker(
                selectedDateTime: new DateTime(2024, 1, 20, 12, 22, 23),
                isNeedConfirm: true,
                isShowTime: true))),
            new PerfScenario("RangeDatePicker.Default.PresenterOnly", _ => CreatePickerPresenterForTest(CreateRangeDatePicker())),
            new PerfScenario("RangeDatePicker.ShowTime.PresenterOnly", _ => CreatePickerPresenterForTest(CreateRangeDatePicker(
                rangeStartSelectedDate: new DateTime(2024, 1, 20, 12, 22, 23),
                rangeEndSelectedDate: new DateTime(2024, 2, 20, 7, 22, 23),
                isNeedConfirm: true,
                isShowTime: true)))
        ];
    }

    private static AtomDatePicker CreateDatePicker(
        DateTime? selectedDateTime = null,
        bool isNeedConfirm = false,
        bool isShowTime = false)
    {
        return new AtomDatePicker
        {
            Width            = 260,
            PlaceholderText  = "Select date",
            SelectedDateTime = selectedDateTime,
            IsNeedConfirm    = isNeedConfirm,
            IsShowTime       = isShowTime
        };
    }

    private static AtomRangeDatePicker CreateRangeDatePicker(
        DateTime? rangeStartSelectedDate = null,
        DateTime? rangeEndSelectedDate = null,
        bool isNeedConfirm = false,
        bool isShowTime = false)
    {
        return new AtomRangeDatePicker
        {
            Width                  = 320,
            PlaceholderText        = "Select date",
            SecondaryPlaceholderText = "End date",
            RangeStartSelectedDate = rangeStartSelectedDate,
            RangeEndSelectedDate   = rangeEndSelectedDate,
            IsNeedConfirm          = isNeedConfirm,
            IsShowTime             = isShowTime
        };
    }

    private static Control CreatePickerPresenterForTest(Control picker)
    {
        var method = picker.GetType().GetMethod(
            "CreatePickerPresenter",
            BindingFlags.Instance | BindingFlags.NonPublic);
        return (Control)method!.Invoke(picker, null)!;
    }
}
