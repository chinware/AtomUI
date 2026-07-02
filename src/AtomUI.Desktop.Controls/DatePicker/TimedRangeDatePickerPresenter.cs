using AtomUI.Desktop.Controls.CalendarView;
using Avalonia;

namespace AtomUI.Desktop.Controls;

internal class TimedRangeDatePickerPresenter : RangeDatePickerPresenter
{
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SecondarySelectedDateTimeProperty)
        {
            if (CalendarView is RangeCalendar rangeCalendar)
            {
                rangeCalendar.SetCurrentValue(RangeCalendar.SecondarySelectedDateProperty, SecondarySelectedDateTime);
            }
        }
    }
    
    protected override void NotifyNowButtonClicked()
    {
        SelectNowForActiveRangePart();

        if (!IsNeedConfirm)
        {
            OnConfirmed();
        }
    }
}
