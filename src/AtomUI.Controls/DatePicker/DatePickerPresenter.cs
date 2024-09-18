using System.Reactive.Disposables;
using AtomUI.Controls.CalendarView;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using PickerCalendar = AtomUI.Controls.CalendarView.Calendar;

namespace AtomUI.Controls;

public class ChoosingStatusEventArgs : EventArgs
{
    public bool IsChoosing { get; }
    public ChoosingStatusEventArgs(bool isChoosing)
    {
        IsChoosing = isChoosing;
    }
}

internal class DatePickerPresenter : PickerPresenterBase
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsNeedConfirmProperty =
        DatePicker.IsNeedConfirmProperty.AddOwner<DatePickerPresenter>();
    
    public static readonly StyledProperty<bool> IsShowNowProperty =
        DatePicker.IsShowNowProperty.AddOwner<DatePickerPresenter>();
    
    public static readonly StyledProperty<bool> IsShowTimeProperty =
        DatePicker.IsShowTimeProperty.AddOwner<DatePickerPresenter>();

    public static readonly StyledProperty<DateTime?> SelectedDateTimeProperty =
        DatePicker.SelectedDateTimeProperty.AddOwner<DatePickerPresenter>();
    
    public static readonly StyledProperty<ClockIdentifierType> ClockIdentifierProperty =
        TimePicker.ClockIdentifierProperty.AddOwner<DatePickerPresenter>();
    
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
    
    public DateTime? SelectedDateTime
    {
        get => GetValue(SelectedDateTimeProperty);
        set => SetValue(SelectedDateTimeProperty, value);
    }
    
    public ClockIdentifierType ClockIdentifier
    {
        get => GetValue(ClockIdentifierProperty);
        set => SetValue(ClockIdentifierProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<DatePickerPresenter, bool> ButtonsPanelVisibleProperty =
        AvaloniaProperty.RegisterDirect<DatePickerPresenter, bool>(nameof(ButtonsPanelVisible),
            o => o.ButtonsPanelVisible,
            (o, v) => o.ButtonsPanelVisible = v);
    
    public static readonly StyledProperty<TimeSpan?> TempSelectedTimeProperty =
        AvaloniaProperty.Register<DatePickerPresenter, TimeSpan?>(nameof(TempSelectedTime));

    private bool _buttonsPanelVisible = true;
    internal bool ButtonsPanelVisible
    {
        get => _buttonsPanelVisible;
        set => SetAndRaise(ButtonsPanelVisibleProperty, ref _buttonsPanelVisible, value);
    }
    
    public TimeSpan? TempSelectedTime
    {
        get => GetValue(TempSelectedTimeProperty);
        set => SetValue(TempSelectedTimeProperty, value);
    }

    #endregion

    #region 公共事件定义
    
    /// <summary>
    /// 当前 Pointer 选中的日期和时间的变化事件
    /// </summary>
    public event EventHandler<DateSelectedEventArgs>? HoverDateTimeChanged;
    
    /// <summary>
    /// 当前是否处于选择中状态
    /// </summary>
    public event EventHandler<ChoosingStatusEventArgs>? ChoosingStatueChanged;

    #endregion

    protected Button? _nowButton;
    protected Button? _todayButton;
    protected Button? _confirmButton;
    protected PickerCalendar? _calendarView;
    protected CompositeDisposable? _compositeDisposable;
    protected TimeView? _timeView;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        TokenResourceBinder.CreateGlobalTokenBinding(this, BorderThicknessProperty, GlobalTokenResourceKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this, thickness => new Thickness(0, thickness.Top, 0, 0)));
        _compositeDisposable = new CompositeDisposable();
        if (_calendarView is not null)
        {
            _compositeDisposable.Add(PickerCalendar.IsPointerInMonthViewProperty.Changed.Subscribe(args =>
            {
                EmitChoosingStatueChanged(args.GetNewValue<bool>());
            }));
        }

        if (_timeView is not null)
        {
            _compositeDisposable.Add(TimeView.IsPointerInSelectorProperty.Changed.Subscribe(args =>
            {
                EmitChoosingStatueChanged(args.GetNewValue<bool>());
            }));
            SyncTimeViewTimeValue();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _compositeDisposable?.Dispose();
        _compositeDisposable = null;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsNeedConfirmProperty ||
            change.Property == IsShowNowProperty ||
            change.Property == IsShowTimeProperty)
        {
            SetupButtonStatus();
        }
        else if (change.Property == SelectedDateTimeProperty)
        {
            SetupConfirmButtonEnableStatus();
        }
    }

    protected virtual void SetupConfirmButtonEnableStatus()
    {
        if (_confirmButton is not null)
        {
            _confirmButton.IsEnabled = SelectedDateTime is not null;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _nowButton     = e.NameScope.Get<Button>(DatePickerPresenterTheme.NowButtonPart);
        _todayButton   = e.NameScope.Get<Button>(DatePickerPresenterTheme.TodayButtonPart);
        _confirmButton = e.NameScope.Get<Button>(DatePickerPresenterTheme.ConfirmButtonPart);
        _calendarView  = e.NameScope.Get<PickerCalendar>(DatePickerPresenterTheme.CalendarViewPart);
        _timeView      = e.NameScope.Get<TimeView>(DatePickerPresenterTheme.TimeViewPart);
        SetupButtonStatus();
        if (_calendarView is not null)
        {
            _calendarView.HoverDateChanged += HandleCalendarViewDateHoverChanged;
            _calendarView.DateSelected += HandleCalendarViewDateSelected;
        }

        if (_timeView is not null)
        {
            if (IsShowTime)
            {
                SyncTimeViewTimeValue();
            }
            
            _timeView.HoverTimeChanged += HandleTimeViewHoverChanged;
            _timeView.TimeSelected     += HandleTimeViewTimeSelected;
            _timeView.TempTimeSelected += HandleTimeViewTempTimeSelected;
        }

        if (_todayButton is not null)
        {
            _todayButton.Click += HandleTodayButtonClicked;
        }

        if (_nowButton is not null)
        {
            _nowButton.Click += HandleNowButtonClicked;
        }

        if (_confirmButton is not null)
        {
            _confirmButton.Click     += HandleConfirmButtonClicked;
            _confirmButton.IsEnabled =  SelectedDateTime is not null;
            _confirmButton.PointerEntered += (sender, args) =>
            {
                NotifyPointerEnterConfirmButton();
            };
            _confirmButton.PointerExited += (sender, args) =>
            {
                NotifyPointerExitConfirmButton();
            };
        }

        SetupConfirmButtonEnableStatus();
    }

    protected virtual void NotifyPointerEnterConfirmButton()
    {
        if (_calendarView?.SelectedDate is not null)
        {
            var hoverDateTime = CollectDateTime(_calendarView?.SelectedDate, TempSelectedTime ?? _timeView?.SelectedTime);
            EmitHoverDateTimeChanged(hoverDateTime);
        }
    }
    
    protected virtual void NotifyPointerExitConfirmButton()
    {
        EmitChoosingStatueChanged(false);
    }

    private void HandleTodayButtonClicked(object? sender, RoutedEventArgs args)
    {
        SelectedDateTime = DateTime.Today;
        if (_calendarView is not null)
        {
            _calendarView.DisplayDate = DateTime.Today;
        }
        if (!IsNeedConfirm)
        {
            OnConfirmed();
        }
    }
    
    private void HandleNowButtonClicked(object? sender, RoutedEventArgs args)
    {
        if (_calendarView is not null)
        {
            _calendarView.SelectedDate = DateTime.Now;
        }

        if (IsShowTime && _timeView is not null)
        {
            _timeView.SelectedTime = DateTime.Now.TimeOfDay;
        }

        if (!IsNeedConfirm)
        {
            OnConfirmed();
        }
    }
    
    private void HandleConfirmButtonClicked(object? sender, RoutedEventArgs args)
    {
        NotifyConfirmButtonClicked();
    }

    protected virtual void NotifyConfirmButtonClicked()
    {
        if (SelectedDateTime is not null)
        {
            OnConfirmed();
        }
    }

    private void HandleCalendarViewDateHoverChanged(object? sender, DateSelectedEventArgs args)
    {
        NotifyCalendarViewDateHoverChanged(args.Date);
    }

    protected virtual void NotifyCalendarViewDateHoverChanged(DateTime? newDate)
    {
        // 需要组合日期和时间
        // 暂时没实现
        var hoverDateTime = CollectDateTime(newDate, TempSelectedTime);
        EmitHoverDateTimeChanged(hoverDateTime);
    }

    protected void EmitHoverDateTimeChanged(DateTime? newDate)
    {
        HoverDateTimeChanged?.Invoke(this, new DateSelectedEventArgs(newDate));   
    }
    
    private void HandleCalendarViewDateSelected(object? sender, DateSelectedEventArgs args)
    {
        NotifyCalendarViewDateSelected();
    }

    protected virtual void NotifyCalendarViewDateSelected()
    {
        SelectedDateTime = CollectDateTime(_calendarView?.SelectedDate, TempSelectedTime ?? _timeView?.SelectedTime);
        if (!IsNeedConfirm)
        {
            OnConfirmed();
        }
    }

    protected DateTime? CollectDateTime(DateTime? date, TimeSpan? timeSpan = null)
    {
        if (date is null)
        {
            return null;
        }
        date =   date.Value.Date;
        if (IsShowTime && timeSpan is not null)
        {
            date = date.Value.Add(timeSpan.Value);
        }
        
        return date;
    }

    private void SetupButtonStatus()
    {
        if (_nowButton is null ||
            _todayButton is null ||
            _confirmButton is null)
        {
            return;
        }
        
        _confirmButton.IsVisible = IsNeedConfirm;
        
        _nowButton.IsVisible             = false;
        _todayButton.IsVisible           = false;
        _nowButton.HorizontalAlignment   = HorizontalAlignment.Left;
        _todayButton.HorizontalAlignment = HorizontalAlignment.Left;
        
        if (IsShowNow)
        {
            _nowButton.IsVisible   = false;
            _todayButton.IsVisible = false;
            if (IsShowTime)
            {
                _nowButton.IsVisible = true;
            }
            else
            {
                _todayButton.IsVisible = true;
            }

            if (!IsNeedConfirm)
            {
                _nowButton.HorizontalAlignment   = HorizontalAlignment.Center;
                _todayButton.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else
            {
                _nowButton.HorizontalAlignment   = HorizontalAlignment.Left;
                _todayButton.HorizontalAlignment = HorizontalAlignment.Left;
            }
        }

        ButtonsPanelVisible = _nowButton.IsVisible || _todayButton.IsVisible || _confirmButton.IsVisible;
    }

    protected override void OnConfirmed()
    {
        EmitChoosingStatueChanged(false);
        base.OnConfirmed();
    }

    internal void EmitConfirmed()
    {
        base.OnConfirmed();
    }

    protected void EmitChoosingStatueChanged(bool isChoosing)
    {
        ChoosingStatueChanged?.Invoke(this, new ChoosingStatusEventArgs(isChoosing));
    }

    protected override void OnDismiss()
    {
        base.OnDismiss();
        SelectedDateTime = null;
    }

    protected virtual void SyncTimeViewTimeValue()
    {
        if (_timeView is not null)
        {
            _timeView.SelectedTime = SelectedDateTime?.TimeOfDay ?? TimeSpan.Zero;
        }
    }
    
    private void HandleTimeViewHoverChanged(object? sender, TimeSelectedEventArgs args)
    {
        NotifyTimeViewHoverChanged(args.Time);
    }

    protected virtual void NotifyTimeViewHoverChanged(TimeSpan? newTime)
    {
        var hoverDateTime = CollectDateTime(SelectedDateTime, newTime);
        HoverDateTimeChanged?.Invoke(this, new DateSelectedEventArgs(hoverDateTime));
    }

    private void HandleTimeViewTimeSelected(object? sender, TimeSelectedEventArgs args)
    {
        if (!IsNeedConfirm)
        {
            OnConfirmed();
        }
    }
    
    private void HandleTimeViewTempTimeSelected(object? sender, TimeSelectedEventArgs args)
    {
        TimeViewTempTimeSelected(args.Time);
    }

    protected virtual void TimeViewTempTimeSelected(TimeSpan? time)
    {
        TempSelectedTime = time;
    }
}