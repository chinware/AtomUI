using AtomUI.Desktop.Controls.CalendarView;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

using PickerCalendar = AtomUI.Desktop.Controls.CalendarView.Calendar;

internal class RangeDatePickerPresenter : DatePickerPresenter
{
    #region 公共属性定义

    public static readonly StyledProperty<DateTime?> SecondarySelectedDateTimeProperty =
        AvaloniaProperty.Register<RangeDatePickerPresenter, DateTime?>(nameof(SecondarySelectedDateTime));

    internal static readonly StyledProperty<bool> IsRangeStartActiveProperty =
        AvaloniaProperty.Register<RangeDatePickerPresenter, bool>(nameof(IsRangeStartActive), true);

    public DateTime? SecondarySelectedDateTime
    {
        get => GetValue(SecondarySelectedDateTimeProperty);
        set => SetValue(SecondarySelectedDateTimeProperty, value);
    }

    internal bool IsRangeStartActive
    {
        get => GetValue(IsRangeStartActiveProperty);
        set => SetValue(IsRangeStartActiveProperty, value);
    }

    #endregion

    #region 公共事件定义

    public event EventHandler? RangePartConfirmed;

    #endregion
    
    RangeDatePickState PickState = RangeDatePickState.None;
    private DateTime? _pendingRangeOpenDisplayAnchor;

    protected void EmitRangePartConfirmed()
    {
        RangePartConfirmed?.Invoke(this, EventArgs.Empty);
    }

    internal void NotifySelectRangeStart(bool isStart)
    {
        SetCurrentValue(IsRangeStartActiveProperty, isStart);
        if (CalendarView is RangeCalendar rangeCalendar)
        {
            rangeCalendar.SetCurrentValue(RangeCalendar.IsSelectRangeStartProperty, isStart);
            SyncTimeViewTimeValue();
        }
        SetupConfirmButtonEnableStatus();
    }

    internal void ResetRangePickState()
    {
        PickState = RangeDatePickState.None;
        if (SelectedDateTime is not null)
        {
            PickState |= RangeDatePickState.PartStart;
        }

        if (SecondarySelectedDateTime is not null)
        {
            PickState |= RangeDatePickState.PartEnd;
        }
    }

    internal void ResetRangeOpenPanelState()
    {
        _pendingRangeOpenDisplayAnchor = ResolveRangeOpenDisplayAnchor();
        ApplyPendingRangeOpenPanelState();
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
        if (CalendarView is RangeCalendar)
        {
            var hoverDateTime = CollectDateTime(GetActiveSelectedDateTime(), newTime);
            EmitHoverDateTimeChanged(hoverDateTime);
        }
    }

    protected override void NotifyPointerEnterConfirmButton()
    {
        if (CalendarView is RangeCalendar)
        {
            var hoverDateTime = CollectDateTime(GetActiveSelectedDateTime(), TempSelectedTime ?? TimeView?.SelectedTime);
            EmitHoverDateTimeChanged(hoverDateTime);
        }
    }

    protected override void NotifyCalendarViewDateSelected()
    {
        if (CalendarView is RangeCalendar rangeCalendar)
        {
            var selectedDateTime = CollectDateTime(
                GetActiveCalendarDate(rangeCalendar),
                TempSelectedTime ?? TimeView?.SelectedTime);
            SetActiveSelectedDateTime(selectedDateTime);

            if (!IsNeedConfirm)
            {
                OnConfirmed();
            }
        }
    }

    protected override void NotifyConfirmButtonClicked()
    {
        if (EffectiveDateRange.Contains(GetActiveSelectedDateTime()))
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
        EmitChoosingStatusChanged(false);
        MarkActiveRangePartPicked();

        var pickState    = PickState;
        var hasPartStart = (pickState & RangeDatePickState.PartStart) == RangeDatePickState.PartStart;
        var hasPartEnd   = (pickState & RangeDatePickState.PartEnd) == RangeDatePickState.PartEnd;
        if (hasPartStart && hasPartEnd &&
            SelectedDateTime is not null &&
            SecondarySelectedDateTime is not null)
        {
            EmitConfirmed();
        }
        else
        {
            EmitRangePartConfirmed();
        }
    }

    private void MarkActiveRangePartPicked()
    {
        if (IsRangeStartActive)
        {
            if (SelectedDateTime is not null)
            {
                PickState |= RangeDatePickState.PartStart;
            }
        }
        else if (SecondarySelectedDateTime is not null)
        {
            PickState |= RangeDatePickState.PartEnd;
        }
    }
    
    protected override void NotifyTodayButtonClicked()
    {
        if (!EffectiveDateRange.Contains(DateTime.Today))
        {
            return;
        }

        SetActiveSelectedDateTime(DateTime.Today);
        OnConfirmed();
    }
    
    protected override void NotifyNowButtonClicked()
    {
        if (!EffectiveDateRange.Contains(DateTime.Now))
        {
            return;
        }

        SelectNowForActiveRangePart();
        OnConfirmed();
    }

    protected void SelectNowForActiveRangePart()
    {
        var now = DateTime.Now;
        SetActiveSelectedDateTime(now);
        CalendarView?.SetCurrentValue(PickerCalendar.SelectedDateProperty, now);

        if (IsShowTime && TimeView is not null)
        {
            TimeView.SelectedTime = now.TimeOfDay;
        }
    }

    protected override void SyncTimeViewTimeValue()
    {
        if (TimeView is not null)
        {
            TimeView.SelectedTime = GetActiveSelectedDateTime()?.TimeOfDay ?? TimeSpan.Zero;
        }
    }

    protected override void TimeViewTempTimeSelected(TimeSpan? time)
    {
        base.TimeViewTempTimeSelected(time);
        if (CalendarView is RangeCalendar rangeCalendar)
        {
            SetActiveSelectedDateTime(CollectDateTime(GetActiveCalendarDate(rangeCalendar), TempSelectedTime));
        }
    }

    protected override void SetupConfirmButtonEnableStatus()
    {
        if (ConfirmButton is null)
        {
            return;
        }

        ConfirmButton.IsEnabled = EffectiveDateRange.Contains(GetActiveSelectedDateTime());
    }

    protected override void SynchronizeCalendarState()
    {
        base.SynchronizeCalendarState();
        if (CalendarView is RangeCalendar rangeCalendar)
        {
            rangeCalendar.SetCurrentValue(
                RangeCalendar.SecondarySelectedDateProperty,
                GetValidCalendarDate(SecondarySelectedDateTime));
        }
    }

    private DateTime? GetActiveSelectedDateTime()
    {
        return IsRangeStartActive ? SelectedDateTime : SecondarySelectedDateTime;
    }

    private void SetActiveSelectedDateTime(DateTime? dateTime)
    {
        if (IsRangeStartActive)
        {
            SetCurrentValue(SelectedDateTimeProperty, dateTime);
        }
        else
        {
            SetCurrentValue(SecondarySelectedDateTimeProperty, dateTime);
        }
    }

    private DateTime? GetActiveCalendarDate(RangeCalendar rangeCalendar)
    {
        return IsRangeStartActive ? rangeCalendar.SelectedDate : rangeCalendar.SecondarySelectedDate;
    }

    private DateTime? ResolveRangeOpenDisplayAnchor()
    {
        var activeDate = GetActiveSelectedDateTime();
        if (activeDate is not null)
        {
            var normalizedActiveDate = DatePickerFormattingHelper.NormalizeDateTime(activeDate.Value, PickerMode);
            return IsRangeStartActive
                ? normalizedActiveDate
                : ResolveRangeEndDisplayAnchor(normalizedActiveDate);
        }

        return PickerDisplayDate.HasValue
            ? DatePickerFormattingHelper.NormalizeDateTime(PickerDisplayDate.Value, PickerMode)
            : null;
    }

    protected virtual DateTime ResolveRangeEndDisplayAnchor(DateTime activeEnd)
    {
        return activeEnd;
    }

    private void ApplyPendingRangeOpenPanelState()
    {
        if (_pendingRangeOpenDisplayAnchor is null || CalendarView is not RangeCalendar rangeCalendar)
        {
            return;
        }

        var anchor = rangeCalendar.NormalizePickerDate(_pendingRangeOpenDisplayAnchor.Value);
        ApplyCalendarDisplayAnchor(rangeCalendar, anchor);
        _pendingRangeOpenDisplayAnchor = null;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SecondarySelectedDateTimeProperty ||
            change.Property == SelectedDateTimeProperty)
        {
            SynchronizeCalendarState();
            SetupConfirmButtonEnableStatus();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ApplyPendingRangeOpenPanelState();
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
