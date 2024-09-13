using AtomUI.Controls.CalendarView;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
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
        AvaloniaProperty.Register<DatePickerPresenter, bool>(nameof(IsNeedConfirm));
    
    public static readonly StyledProperty<bool> IsShowNowProperty =
        AvaloniaProperty.Register<DatePickerPresenter, bool>(nameof(IsShowNow));
    
    public static readonly StyledProperty<bool> IsShowTimeProperty =
        AvaloniaProperty.Register<DatePickerPresenter, bool>(nameof(IsShowTime));
    
    public static readonly StyledProperty<DateTime?> SelectedDateTimeProperty =
        AvaloniaProperty.Register<DatePickerPresenter, DateTime?>(nameof(SelectedDateTime));
    
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

    private Button? _nowButton;
    private Button? _todayButton;
    private Button? _confirmButton;
    private PickerCalendar? _calendarView;
    private IDisposable? _choosingStateDisposable;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        TokenResourceBinder.CreateGlobalTokenBinding(this, BorderThicknessProperty, GlobalTokenResourceKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this, thickness => new Thickness(0, thickness.Top, 0, 0)));
        if (_calendarView is not null)
        {
            _choosingStateDisposable = PickerCalendar.IsPointerInMonthViewProperty.Changed.Subscribe(args =>
            {
                ChoosingStatueChanged?.Invoke(this, new ChoosingStatusEventArgs(args.GetNewValue<bool>()));
            });
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _choosingStateDisposable = null;
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
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _nowButton     = e.NameScope.Get<Button>(DatePickerPresenterTheme.NowButtonPart);
        _todayButton   = e.NameScope.Get<Button>(DatePickerPresenterTheme.TodayButtonPart);
        _confirmButton = e.NameScope.Get<Button>(DatePickerPresenterTheme.ConfirmButtonPart);
        _calendarView  = e.NameScope.Get<PickerCalendar>(DatePickerPresenterTheme.CalendarViewPart);
        SetupButtonStatus();
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
        
        if (IsShowNow)
        {
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
        else
        {
            _nowButton.IsVisible             = false;
            _todayButton.IsVisible           = false;
            _nowButton.HorizontalAlignment   = HorizontalAlignment.Left;
            _todayButton.HorizontalAlignment = HorizontalAlignment.Left;
        }
    }

    protected override void OnConfirmed()
    {
        base.OnConfirmed();
    }

    protected override void OnDismiss()
    {
        base.OnDismiss();
    }
}