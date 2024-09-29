﻿using System.Globalization;
using AtomUI.Controls.CalendarView;
using AtomUI.Controls.Internal;
using AtomUI.Controls.TimePickerLang;
using AtomUI.Data;
using AtomUI.Theme.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

public class DatePicker : InfoPickerInput
{
    #region 公共属性定义

    public static readonly StyledProperty<DateTime?> SelectedDateTimeProperty =
        AvaloniaProperty.Register<DatePicker, DateTime?>(nameof(SelectedDateTime),
            enableDataValidation: true);
    
    public static readonly StyledProperty<DateTime?> DefaultDateTimeProperty =
        AvaloniaProperty.Register<DatePicker, DateTime?>(nameof(DefaultDateTime),
            enableDataValidation: true);

    public static readonly StyledProperty<string?> FormatProperty = 
        AvaloniaProperty.Register<DatePicker, string?>(nameof(Format));
    
    public static readonly StyledProperty<bool> IsShowTimeProperty =
        AvaloniaProperty.Register<DatePicker, bool>(nameof(IsShowTime), false);
    
    public static readonly StyledProperty<bool> IsNeedConfirmProperty =
        AvaloniaProperty.Register<DatePicker, bool>(nameof(IsNeedConfirm));
    
    public static readonly StyledProperty<bool> IsShowNowProperty =
        AvaloniaProperty.Register<DatePicker, bool>(nameof(IsShowNow), true);
    
    public static readonly StyledProperty<ClockIdentifierType> ClockIdentifierProperty =
        TimePicker.ClockIdentifierProperty.AddOwner<DatePicker>();

    public DateTime? SelectedDateTime
    {
        get => GetValue(SelectedDateTimeProperty);
        set => SetValue(SelectedDateTimeProperty, value);
    }
    
    public DateTime? DefaultDateTime
    {
        get => GetValue(DefaultDateTimeProperty);
        set => SetValue(DefaultDateTimeProperty, value);
    }
    
    public string? Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }
    
    public bool IsShowTime
    {
        get => GetValue(IsShowTimeProperty);
        set => SetValue(IsShowTimeProperty, value);
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
    
    public ClockIdentifierType ClockIdentifier
    {
        get => GetValue(ClockIdentifierProperty);
        set => SetValue(ClockIdentifierProperty, value);
    }
    
    #endregion
    
    private DatePickerPresenter? _pickerPresenter;
    
    /// <summary>
    /// 清除时间选择器的值，不考虑默认值
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        SelectedDateTime = null;
    }

    /// <summary>
    /// 重置时间选择器的值，当有默认值设置的时候，会将当前的值设置成默认值
    /// </summary>
    public void Reset()
    {
        SelectedDateTime = DefaultDateTime;
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
    
    protected override Flyout CreatePickerFlyout()
    {
        return new DatePickerFlyout();
    }
    
    protected override void NotifyFlyoutPresenterCreated(Control flyoutPresenter)
    {
        if (flyoutPresenter is DatePickerFlyoutPresenter datePickerFlyoutPresenter)
        {
            datePickerFlyoutPresenter.AttachedToVisualTree += (sender, args) =>
            {
                _pickerPresenter = datePickerFlyoutPresenter.DatePickerPresenter;
                ConfigurePickerPresenter(_pickerPresenter);
            };
        }
    }

    private void ConfigurePickerPresenter(DatePickerPresenter? presenter)
    {
        if (presenter is null)
        {
            return;
        }
        
        BindUtils.RelayBind(this, SelectedDateTimeProperty, presenter, DatePickerPresenter.SelectedDateTimeProperty);
        BindUtils.RelayBind(this, IsNeedConfirmProperty, presenter, DatePickerPresenter.IsNeedConfirmProperty);
        BindUtils.RelayBind(this, IsShowNowProperty, presenter, DatePickerPresenter.IsShowNowProperty);
        BindUtils.RelayBind(this, IsShowTimeProperty, presenter, DatePickerPresenter.IsShowTimeProperty);
        BindUtils.RelayBind(this, ClockIdentifierProperty, presenter, DatePickerPresenter.ClockIdentifierProperty);
    }
    
    protected override void NotifyFlyoutOpened()
    {
        base.NotifyFlyoutOpened();
        if (_pickerPresenter is not null)
        {
            _pickerPresenter.ChoosingStatueChanged += HandleChoosingStatueChanged;
            _pickerPresenter.HoverDateTimeChanged += HandleHoverDateTimeChanged;
            _pickerPresenter.Confirmed += HandleConfirmed;
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
    
    private void HandleHoverDateTimeChanged(object? sender, DateSelectedEventArgs args)
    {
        if (args.Date.HasValue)
        {
            Text = FormatDateTime(args.Date);
        }
        else
        {
            Text = null;
        }
    }
    
    private void HandleConfirmed(object? sender, EventArgs args)
    {
        SelectedDateTime = _pickerPresenter?.SelectedDateTime;
        ClosePickerFlyout();
    }

    private void ClearHoverSelectedInfo()
    {
        Text = FormatDateTime(SelectedDateTime);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (DefaultDateTime is not null && SelectedDateTime is null)
        {
            SelectedDateTime = DefaultDateTime;
        }
        Text = FormatDateTime(SelectedDateTime);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedDateTimeProperty)
        {
            Text = FormatDateTime(SelectedDateTime);
        }
    }

    protected override bool ShowClearButtonPredicate()
    {
        return SelectedDateTime is not null;
    }
}