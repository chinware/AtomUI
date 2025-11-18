using AtomUI.Desktop.Controls.CalendarView;
using Avalonia;

namespace AtomUI.Desktop.Controls;

using PickerCalendar = AtomUI.Desktop.Controls.CalendarView.Calendar;

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
        if (IsRangeStartActive)
        {
            SetCurrentValue(SelectedDateTimeProperty, DateTime.Now);
        }
        else
        {
            SetCurrentValue(SecondarySelectedDateTimeProperty, DateTime.Now);
        }
        if (CalendarView is not null)
        {
            CalendarView?.SetCurrentValue(PickerCalendar.SelectedDateProperty, DateTime.Now);
        }

        if (IsShowTime && TimeView is not null)
        {
            TimeView.SelectedTime = DateTime.Now.TimeOfDay;
        }

        if (!IsNeedConfirm)
        {
            OnConfirmed();
        }
    }
}