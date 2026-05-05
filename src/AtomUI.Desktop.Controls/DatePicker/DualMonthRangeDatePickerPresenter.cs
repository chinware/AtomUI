using AtomUI.Desktop.Controls.CalendarView;
using Avalonia;

namespace AtomUI.Desktop.Controls;

internal class DualMonthRangeDatePickerPresenter : RangeDatePickerPresenter
{
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
}