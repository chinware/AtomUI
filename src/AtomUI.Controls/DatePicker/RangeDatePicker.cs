﻿using System.Globalization;
using AtomUI.Controls.CalendarView;
using AtomUI.Controls.Internal;
using AtomUI.Controls.TimePickerLang;
using AtomUI.Data;
using AtomUI.Theme.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace AtomUI.Controls;

public class RangeDatePicker : RangeInfoPickerInput
{
    #region 公共属性定义
    
    public static readonly StyledProperty<DateTime?> RangeStartSelectedDateProperty =
        AvaloniaProperty.Register<RangeDatePicker, DateTime?>(nameof(RangeStartSelectedDate),
            defaultBindingMode: BindingMode.TwoWay,
            enableDataValidation: true);
    
    public static readonly StyledProperty<DateTime?> RangeEndSelectedDateProperty =
        AvaloniaProperty.Register<RangeDatePicker, DateTime?>(nameof(RangeEndSelectedDate),
            defaultBindingMode: BindingMode.TwoWay,
            enableDataValidation: true);
    
    public static readonly StyledProperty<DateTime?> RangeStartDefaultDateProperty =
        AvaloniaProperty.Register<RangeDatePicker, DateTime?>(nameof(RangeStartDefaultDate),
            enableDataValidation: true);

    public static readonly StyledProperty<DateTime?> RangeEndDefaultDateProperty =
        AvaloniaProperty.Register<RangeDatePicker, DateTime?>(nameof(RangeEndDefaultDate),
            enableDataValidation: true);
    
    public static readonly StyledProperty<bool> IsNeedConfirmProperty =
        AvaloniaProperty.Register<RangeDatePicker, bool>(nameof(IsNeedConfirm));
    
    public static readonly StyledProperty<bool> IsShowNowProperty =
        DatePicker.IsShowNowProperty.AddOwner<RangeDatePicker>();
    
    public static readonly StyledProperty<bool> IsShowTimeProperty =
        DatePicker.IsShowTimeProperty.AddOwner<RangeDatePicker>();

    public static readonly StyledProperty<ClockIdentifierType> ClockIdentifierProperty =
        DatePicker.ClockIdentifierProperty.AddOwner<RangeDatePicker>();

    public static readonly StyledProperty<string?> FormatProperty =
        DatePicker.FormatProperty.AddOwner<RangeDatePicker>();
    
    public DateTime? RangeStartSelectedDate
    {
        get => GetValue(RangeStartSelectedDateProperty);
        set => SetValue(RangeStartSelectedDateProperty, value);
    }

    public DateTime? RangeEndSelectedDate
    {
        get => GetValue(RangeEndSelectedDateProperty);
        set => SetValue(RangeEndSelectedDateProperty, value);
    }

    public DateTime? RangeStartDefaultDate
    {
        get => GetValue(RangeStartDefaultDateProperty);
        set => SetValue(RangeStartDefaultDateProperty, value);
    }

    public DateTime? RangeEndDefaultDate
    {
        get => GetValue(RangeEndDefaultDateProperty);
        set => SetValue(RangeEndDefaultDateProperty, value);
    }
    
    public bool IsNeedConfirm
    {
        get => GetValue(IsNeedConfirmProperty);
        set => SetValue(IsNeedConfirmProperty, value);
    }
    
    public bool IsShowNow
    {
        get => GetValue(IsShowNowProperty);
        set => SetValue(IsShowNowProperty, value);
    }
    
    public bool IsShowTime
    {
        get => GetValue(IsShowTimeProperty);
        set => SetValue(IsShowTimeProperty, value);
    }
    
    public ClockIdentifierType ClockIdentifier
    {
        get => GetValue(ClockIdentifierProperty);
        set => SetValue(ClockIdentifierProperty, value);
    }
    
    public string? Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }
    
    #endregion
    
    private RangeDatePickerPresenter? _pickerPresenter;
    private bool? _isNeedConfirmedBackup;
    
    protected override Flyout CreatePickerFlyout()
    {
        return new RangeDatePickerFlyout();
    }
    
    public override void Clear()
    {
        base.Clear();
        
        RangeStartSelectedDate = null;
        RangeEndSelectedDate   = null;
    }
    
    public void Reset()
    {
        RangeStartSelectedDate = RangeStartDefaultDate;
        RangeEndSelectedDate   = RangeEndDefaultDate;
    }
    
    protected override void NotifyFlyoutPresenterCreated(Control flyoutPresenter)
    {
        if (flyoutPresenter is RangeDatePickerFlyoutPresenter rangeDatePickerFlyoutPresenter)
        {
            BindUtils.RelayBind(this, IsShowTimeProperty, rangeDatePickerFlyoutPresenter, RangeDatePickerFlyoutPresenter.IsShowTimeProperty);
            rangeDatePickerFlyoutPresenter.AttachedToVisualTree += (sender, args) =>
            {
                _pickerPresenter = rangeDatePickerFlyoutPresenter.DatePickerPresenter;
                ConfigurePickerPresenter(_pickerPresenter);
            };
        }
    }
    
    private void ConfigurePickerPresenter(RangeDatePickerPresenter? presenter)
    {
        if (presenter is null)
        {
            return;
        }
        presenter.NotifyRepairReverseRange(true);
        BindUtils.RelayBind(this, RangeStartSelectedDateProperty, presenter, RangeDatePickerPresenter.SelectedDateTimeProperty);
        BindUtils.RelayBind(this, RangeEndSelectedDateProperty, presenter, RangeDatePickerPresenter.SecondarySelectedDateTimeProperty);
        BindUtils.RelayBind(this, ClockIdentifierProperty, presenter, RangeDatePickerPresenter.ClockIdentifierProperty);
        BindUtils.RelayBind(this, IsNeedConfirmProperty, presenter, RangeDatePickerPresenter.IsNeedConfirmProperty);
        BindUtils.RelayBind(this, IsShowNowProperty, presenter, RangeDatePickerPresenter.IsShowNowProperty);
    }
    
    protected override void NotifyFlyoutOpened()
    {
        base.NotifyFlyoutOpened();
        if (_pickerPresenter is not null)
        {
            _pickerPresenter.ChoosingStatueChanged += HandleChoosingStatueChanged;
            _pickerPresenter.HoverDateTimeChanged  += HandleHoverDateTimeChanged;
            _pickerPresenter.Confirmed             += HandleConfirmed;
            _pickerPresenter.RangePartConfirmed    += HandleRangePartConfirmed;
            
            _pickerPresenter.SelectedDateTime          = RangeStartSelectedDate;
            _pickerPresenter.SecondarySelectedDateTime = RangeEndSelectedDate;
            if (RangeActivatedPart == RangeActivatedPart.Start)
            {
                _pickerPresenter.NotifySelectRangeStart(true);
            }
            else
            {
                _pickerPresenter.NotifySelectRangeStart(false);
            }

        }
    }
    
    protected override void NotifyFlyoutAboutToClose(bool selectedIsValid)
    {
        base.NotifyFlyoutAboutToClose(selectedIsValid);
        if (_pickerPresenter is not null)
        {
            _pickerPresenter.ChoosingStatueChanged -= HandleChoosingStatueChanged;
            _pickerPresenter.HoverDateTimeChanged  -= HandleHoverDateTimeChanged;
            _pickerPresenter.Confirmed             -= HandleConfirmed;
            _pickerPresenter.RangePartConfirmed    -= HandleRangePartConfirmed;

            _pickerPresenter.SelectedDateTime          = null;
            _pickerPresenter.SecondarySelectedDateTime = null;
        }
    }

    private void HandleChoosingStatueChanged(object? sender, ChoosingStatusEventArgs args)
    {
        _isChoosing = args.IsChoosing;
        UpdatePseudoClasses();
        if (!args.IsChoosing)
        {
            ClearHoverSelectedInfo();
        }
    }
    
    private void ClearHoverSelectedInfo()
    {
        if (RangeActivatedPart == RangeActivatedPart.Start)
        {
            Text = FormatDateTime(RangeStartSelectedDate);
        }
        else if (RangeActivatedPart == RangeActivatedPart.End)
        {
            SecondaryText = FormatDateTime(RangeEndSelectedDate);
        }
    }
    
    private string GetEffectiveFormat()
    {
        if (Format is not null)
        {
            return Format;
        }

        var format = "yyyy-MM-dd";
        if (IsShowTime)
        {
           
            if (ClockIdentifier == ClockIdentifierType.HourClock12)
            {
                format = $"{format} hh:mm:ss tt";
            }
            else
            {
                format = $"{format} HH:mm:ss";
            }
        }

        return format;
    }
    
    protected string FormatDateTime(DateTime? dateTime)
    {
        if (dateTime is null)
        {
            return string.Empty;
        }

        var format = GetEffectiveFormat();
        if (ClockIdentifier == ClockIdentifierType.HourClock12)
        {
            var formatInfo = new DateTimeFormatInfo();
            formatInfo.AMDesignator = LanguageResourceBinder.GetLangResource(TimePickerLangResourceKey.AMText)!;
            formatInfo.PMDesignator = LanguageResourceBinder.GetLangResource(TimePickerLangResourceKey.PMText)!;
            return dateTime.Value.ToString(format, formatInfo);
        }

        return dateTime.Value.ToString(format);
    }
    
    private void HandleHoverDateTimeChanged(object? sender, DateSelectedEventArgs args)
    {
        if (args.Date.HasValue)
        {
            if (RangeActivatedPart == RangeActivatedPart.Start)
            {
                Text = FormatDateTime(args.Date);
            }
            else if (RangeActivatedPart == RangeActivatedPart.End)
            {
                SecondaryText = FormatDateTime(args.Date);
            }
        }
    }

    private void HandleRangePartConfirmed(object? sender, EventArgs args)
    {
        if (RangeActivatedPart == RangeActivatedPart.Start)
        {
            RangeStartSelectedDate = _pickerPresenter?.SelectedDateTime;
            if (RangeEndSelectedDate is null)
            {
                RangeActivatedPart = RangeActivatedPart.End;
                _pickerPresenter?.NotifySelectRangeStart(false);
            }
        }
        else if (RangeActivatedPart == RangeActivatedPart.End)
        {
            RangeEndSelectedDate = _pickerPresenter?.SecondarySelectedDateTime;
            if (RangeStartSelectedDate is null)
            {
                RangeActivatedPart = RangeActivatedPart.Start;
                _pickerPresenter?.NotifySelectRangeStart(true);
            }
        }
    }
    
    private void HandleConfirmed(object? sender, EventArgs args)
    {
        var rangeStart = _pickerPresenter?.SelectedDateTime;
        var rangeEnd   = _pickerPresenter?.SecondarySelectedDateTime;
        if (rangeStart is not null && rangeEnd is not null)
        {
            if (DateTimeHelper.CompareDays(rangeEnd.Value, rangeStart.Value) < 0)
            {
                RangeStartSelectedDate = rangeEnd;
                RangeEndSelectedDate   = rangeStart;
            }
            else
            {
                RangeStartSelectedDate = rangeStart;
                RangeEndSelectedDate   = rangeEnd;
            }
        }

        ClosePickerFlyout();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == RangeActivatedPartProperty)
        {
            HandleRangeActivatedPartChanged();
        } else if (change.Property == IsShowTimeProperty)
        {
            if (IsShowTime)
            {
                _isNeedConfirmedBackup = IsNeedConfirm;
                IsNeedConfirm          = true;
            }
            else
            {
                if (_isNeedConfirmedBackup is not null)
                {
                    IsNeedConfirm = _isNeedConfirmedBackup.Value;
                }
            }
        }

        if (VisualRoot is not null)
        {
            if (change.Property == RangeStartSelectedDateProperty)
            {
                if (RangeStartSelectedDate.HasValue)
                {
                    Text = FormatDateTime(RangeStartSelectedDate.Value);
                }
                else
                {
                    ResetRangeStartDateValue();
                }
            }
            else if (change.Property == RangeEndSelectedDateProperty)
            {
                if (RangeEndSelectedDate.HasValue)
                {
                    SecondaryText = FormatDateTime(RangeEndSelectedDate.Value);
                }
                else
                {
                    ResetRangeEndDateValue();
                }
            }
        }
    }
    
     protected void ResetRangeStartDateValue()
    {
        if (_infoInputBox is not null)
        {
            if (RangeStartDefaultDate is not null)
            {
                _infoInputBox.Text = FormatDateTime(RangeStartDefaultDate.Value);
            }
            else
            {
                _infoInputBox.Clear();
            }
        }
    }
    
    protected void ResetRangeEndDateValue()
    {
        if (_secondaryInfoInputBox is not null)
        {
            if (RangeEndDefaultDate is not null)
            {
                _secondaryInfoInputBox.Text = FormatDateTime(RangeEndDefaultDate.Value);;
            }
            else
            {
                _secondaryInfoInputBox.Clear();
            }
        }
    }
    
    protected override void HandleRangeActivatedPartChanged()
    {
        SetupPickerIndicatorPosition();
        if (RangeActivatedPart == RangeActivatedPart.Start)
        {
            _infoInputBox!.Focus();
            if (RangeEndSelectedDate is null)
            {
                ResetRangeStartDateValue();
            }
            _pickerPresenter?.NotifySelectRangeStart(true);
        }
        else if (RangeActivatedPart == RangeActivatedPart.End)
        {
            _secondaryInfoInputBox!.Focus();
            if (RangeStartSelectedDate is null)
            {
                ResetRangeEndDateValue();
            }
            _pickerPresenter?.NotifySelectRangeStart(false);
        }
        else
        {
            if (RangeStartSelectedDate is null)
            {
                ResetRangeStartDateValue();
            }
    
            if (RangeEndSelectedDate is null)
            {
                ResetRangeEndDateValue();
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (RangeStartDefaultDate is not null && RangeStartSelectedDate is null)
        {
            RangeStartSelectedDate = RangeStartDefaultDate;
        }
        
        if (RangeEndDefaultDate is not null && RangeEndSelectedDate is null)
        {
            RangeEndSelectedDate = RangeEndDefaultDate;
        }
    }

    protected override bool ShowClearButtonPredicate()
    {
        return RangeStartSelectedDate is not null || RangeEndSelectedDate is not null;
    }
    
}