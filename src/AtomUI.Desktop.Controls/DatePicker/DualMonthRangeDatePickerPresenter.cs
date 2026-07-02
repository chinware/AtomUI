using AtomUI.Desktop.Controls.CalendarView;
using Avalonia;

namespace AtomUI.Desktop.Controls;

internal class DualMonthRangeDatePickerPresenter : RangeDatePickerPresenter
{
    protected override DateTime ResolveRangeEndDisplayAnchor(DateTime activeEnd)
    {
        return PickerMode switch
        {
            DatePickerMode.Month or DatePickerMode.Quarter =>
                DateTimeHelper.AddYears(activeEnd, -1) ?? activeEnd,
            DatePickerMode.Year => ResolvePreviousDecadeAnchor(activeEnd),
            _                   => DateTimeHelper.AddMonths(activeEnd, -1) ?? activeEnd
        };
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SecondarySelectedDateTimeProperty ||
            change.Property == SelectedDateTimeProperty)
        {
            if (CalendarView is DualMonthRangeCalendar rangeCalendar)
            {
                rangeCalendar.SetCurrentValue(DualMonthRangeCalendar.SecondarySelectedDateProperty, SecondarySelectedDateTime);
            }
        }
    }

    private static DateTime ResolvePreviousDecadeAnchor(DateTime activeEnd)
    {
        var decadeStart = DateTimeHelper.DecadeOfDate(activeEnd) - 10;
        return new DateTime(Math.Max(DateTime.MinValue.Year, decadeStart), 1, 1);
    }
}
