using AtomUI.Controls.CalendarView;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class RangeDatePickerPresenter : DatePickerPresenter
{
    #region 公共属性定义

    public static readonly StyledProperty<DateTime?> SecondarySelectedDateTimeProperty =
        AvaloniaProperty.Register<RangeDatePickerPresenter, DateTime?>(nameof(SecondarySelectedDateTime));
    
    public DateTime? SecondarySelectedDateTime
    {
        get => GetValue(SecondarySelectedDateTimeProperty);
        set => SetValue(SecondarySelectedDateTimeProperty, value);
    }

    #endregion

    #region 公共事件定义

    public event EventHandler? RangePartConfirmed;

    #endregion

    protected void EmitRangePartConfirmed()
    {
        RangePartConfirmed?.Invoke(this, EventArgs.Empty);
    }

    internal void NotifySelectRangeStart(bool isStart)
    {
        if (_calendarView is RangeCalendar rangeCalendar)
        {
            rangeCalendar.IsSelectRangeStart = isStart;
            SyncTimeViewTimeValue();
        }
    }

    internal void NotifyRepairReverseRange(bool isRepair)
    {
        if (_calendarView is RangeCalendar rangeCalendar)
        {
            rangeCalendar.IsRepairReverseRange = isRepair;
        }
    }
    
    protected override void NotifyTimeViewHoverChanged(TimeSpan? newTime)
    {
        if (_calendarView is RangeCalendar rangeCalendar)
        {
            DateTime? hoverDateTime = default;
            if (rangeCalendar.IsSelectRangeStart)
            {
                hoverDateTime = CollectDateTime(SelectedDateTime, newTime);
            }
            else
            {
                hoverDateTime = CollectDateTime(SecondarySelectedDateTime, newTime);
            }

            EmitHoverDateTimeChanged(hoverDateTime);
        }
    }

    protected override void NotifyPointerEnterConfirmButton()
    {
        if (_calendarView is RangeCalendar rangeCalendar)
        {
            DateTime? hoverDateTime = default;
            if (rangeCalendar.IsSelectRangeStart)
            {
                hoverDateTime = CollectDateTime(SelectedDateTime, TempSelectedTime ?? _timeView?.SelectedTime);
            }
            else
            {
                hoverDateTime = CollectDateTime(SecondarySelectedDateTime, TempSelectedTime ?? _timeView?.SelectedTime);
            }

            EmitHoverDateTimeChanged(hoverDateTime);
        }
    }

    protected override void NotifyCalendarViewDateSelected()
    {
        if (_calendarView is RangeCalendar rangeCalendar)
        {
            // 部分确认
            if (rangeCalendar.IsSelectRangeStart)
            {
                SelectedDateTime = CollectDateTime(rangeCalendar.SelectedDate, TempSelectedTime ?? _timeView?.SelectedTime);
            }
            else
            {
                SecondarySelectedDateTime = CollectDateTime(rangeCalendar.SecondarySelectedDate, TempSelectedTime ?? _timeView?.SelectedTime);
            }

            if (!IsNeedConfirm)
            {
                OnConfirmed();
            }
        }
    }

    protected override void NotifyConfirmButtonClicked()
    {
        if (SelectedDateTime is not null || SecondarySelectedDateTime is not null)
        {
            OnConfirmed();
        }
    }

    protected override void OnDismiss()
    {
        base.OnDismiss();
        SecondarySelectedDateTime = null;
    }
    
    protected override void OnConfirmed()
    {
        EmitChoosingStatueChanged(false);

        if (SelectedDateTime is null || SecondarySelectedDateTime is null)
        {
            EmitRangePartConfirmed();
        }
        else
        {
            EmitConfirmed();
        }
    }
    
    protected override void SyncTimeViewTimeValue()
    {
        if (_timeView is not null && _calendarView is RangeCalendar rangeCalendar)
        {
            if (rangeCalendar.IsSelectRangeStart)
            {
                _timeView.SelectedTime = SelectedDateTime?.TimeOfDay ?? TimeSpan.Zero;
            }
            else
            {
                _timeView.SelectedTime = SecondarySelectedDateTime?.TimeOfDay ?? TimeSpan.Zero;
            }
        }
    }

    protected override void TimeViewTempTimeSelected(TimeSpan? time)
    {
        base.TimeViewTempTimeSelected(time);
        if (_calendarView is RangeCalendar rangeCalendar)
        {
            // 部分确认
            if (rangeCalendar.IsSelectRangeStart)
            {
                SelectedDateTime = CollectDateTime(rangeCalendar.SelectedDate, TempSelectedTime);
            }
            else
            {
                SecondarySelectedDateTime = CollectDateTime(rangeCalendar.SecondarySelectedDate, TempSelectedTime);
            }
        }
    }
    
    protected override void SetupConfirmButtonEnableStatus()
    {
        if (_confirmButton is null)
        {
            return;
        }
        if (_calendarView is RangeCalendar rangeCalendar)
        {
            if (rangeCalendar.IsSelectRangeStart)
            {
                _confirmButton.IsEnabled = SelectedDateTime is not null;
            }
            else
            {
                _confirmButton.IsEnabled  = SecondarySelectedDateTime is not null;
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SecondarySelectedDateTimeProperty ||
            change.Property == SelectedDateTimeProperty)
        {
            SetupConfirmButtonEnableStatus();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        SetupConfirmButtonEnableStatus();
    }
}