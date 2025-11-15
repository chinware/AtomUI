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
    
    RangeDatePickState PickState = RangeDatePickState.None;
    private bool _isRangeStartActive = false;

    protected void EmitRangePartConfirmed()
    {
        RangePartConfirmed?.Invoke(this, EventArgs.Empty);
    }

    internal void NotifySelectRangeStart(bool isStart)
    {
        if (isStart)
        {
            PickState = RangeDatePickState.PartStart;
        }
        else
        {
            PickState = RangeDatePickState.PartEnd;
        }

        _isRangeStartActive = isStart;
        if (CalendarView is RangeCalendar rangeCalendar)
        {
            rangeCalendar.IsSelectRangeStart = isStart;
            SyncTimeViewTimeValue();
        }
    }

    internal void NotifyRepairReverseRange(bool isRepair)
    {
        if (CalendarView is RangeCalendar rangeCalendar)
        {
            rangeCalendar.IsRepairReverseRange = isRepair;
        }
    }

    protected override void NotifyTimeViewHoverChanged(TimeSpan? newTime)
    {
        if (CalendarView is RangeCalendar rangeCalendar)
        {
            DateTime? hoverDateTime;
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
        if (CalendarView is RangeCalendar rangeCalendar)
        {
            DateTime? hoverDateTime = null;
            if (rangeCalendar.IsSelectRangeStart)
            {
                hoverDateTime = CollectDateTime(SelectedDateTime, TempSelectedTime ?? TimeView?.SelectedTime);
            }
            else
            {
                hoverDateTime = CollectDateTime(SecondarySelectedDateTime, TempSelectedTime ?? TimeView?.SelectedTime);
            }

            EmitHoverDateTimeChanged(hoverDateTime);
        }
    }

    protected override void NotifyCalendarViewDateSelected()
    {
        if (CalendarView is RangeCalendar rangeCalendar)
        {
            // 部分确认
            if (rangeCalendar.IsSelectRangeStart)
            {
                SetCurrentValue(SelectedDateTimeProperty, CollectDateTime(rangeCalendar.SelectedDate, TempSelectedTime ?? TimeView?.SelectedTime));
            }
            else
            {
                SetCurrentValue(SecondarySelectedDateTimeProperty, CollectDateTime(rangeCalendar.SecondarySelectedDate,
                    TempSelectedTime ?? TimeView?.SelectedTime));
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
        SetCurrentValue(SecondarySelectedDateTimeProperty, null);
    }

    protected override void OnConfirmed()
    {
        EmitChoosingStatueChanged(false);

        if (PickState.HasFlag(RangeDatePickState.PartStart) && PickState.HasFlag(RangeDatePickState.PartEnd))
        {
            EmitConfirmed();
        }
        else
        {
            EmitRangePartConfirmed();
            if (PickState.HasFlag(RangeDatePickState.PartEnd))
            {
                PickState |= RangeDatePickState.PartStart;
            }
            else if (PickState.HasFlag(RangeDatePickState.PartStart))
            {
                PickState |= RangeDatePickState.PartEnd;
            }
        }
    }
    
    protected override void NotifyTodayButtonClicked()
    {
        if (_isRangeStartActive)
        {
            SetCurrentValue(SelectedDateTimeProperty, DateTime.Today);
        }
        else
        {
            SetCurrentValue(SecondarySelectedDateTimeProperty, DateTime.Today);
        }

        if (!IsNeedConfirm)
        {
            OnConfirmed();
        }
    }

    protected override void SyncTimeViewTimeValue()
    {
        if (TimeView is not null && CalendarView is RangeCalendar rangeCalendar)
        {
            if (rangeCalendar.IsSelectRangeStart)
            {
                TimeView.SelectedTime = SelectedDateTime?.TimeOfDay ?? TimeSpan.Zero;
            }
            else
            {
                TimeView.SelectedTime = SecondarySelectedDateTime?.TimeOfDay ?? TimeSpan.Zero;
            }
        }
    }

    protected override void TimeViewTempTimeSelected(TimeSpan? time)
    {
        base.TimeViewTempTimeSelected(time);
        if (CalendarView is RangeCalendar rangeCalendar)
        {
            // 部分确认
            if (rangeCalendar.IsSelectRangeStart)
            {
                SetCurrentValue(SelectedDateTimeProperty, CollectDateTime(rangeCalendar.SelectedDate, TempSelectedTime));
            }
            else
            {
                SetCurrentValue(SecondarySelectedDateTimeProperty, CollectDateTime(rangeCalendar.SecondarySelectedDate, TempSelectedTime));
            }
        }
    }

    protected override void SetupConfirmButtonEnableStatus()
    {
        if (ConfirmButton is null)
        {
            return;
        }

        if (CalendarView is RangeCalendar rangeCalendar)
        {
            if (rangeCalendar.IsSelectRangeStart)
            {
                ConfirmButton.IsEnabled = SelectedDateTime is not null;
            }
            else
            {
                ConfirmButton.IsEnabled = SecondarySelectedDateTime is not null;
            }
        }
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
            SetupConfirmButtonEnableStatus();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        SetupConfirmButtonEnableStatus();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        PickState = RangeDatePickState.None;
    }
}

[Flags]
internal enum RangeDatePickState
{
    None = 0x00,
    PartStart = 0x01,
    PartEnd = 0x02,
}