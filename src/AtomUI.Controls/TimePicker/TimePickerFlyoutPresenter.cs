using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace AtomUI.Controls;

internal class TimePickerFlyoutPresenter : FlyoutPresenter
{
    public static readonly StyledProperty<int> MinuteIncrementProperty =
        TimePicker.MinuteIncrementProperty.AddOwner<TimePickerFlyoutPresenter>();

    public static readonly StyledProperty<int> SecondIncrementProperty =
        TimePicker.SecondIncrementProperty.AddOwner<TimePickerFlyoutPresenter>();

    public static readonly StyledProperty<TimeSpan> TimeProperty =
        AvaloniaProperty.Register<TimePickerPresenter, TimeSpan>(nameof(Time));

    public static readonly StyledProperty<ClockIdentifierType> ClockIdentifierProperty =
        TimePicker.ClockIdentifierProperty.AddOwner<TimePickerFlyoutPresenter>();

    public int MinuteIncrement
    {
        get => GetValue(MinuteIncrementProperty);
        set => SetValue(MinuteIncrementProperty, value);
    }

    public int SecondIncrement
    {
        get => GetValue(SecondIncrementProperty);
        set => SetValue(SecondIncrementProperty, value);
    }

    public TimeSpan Time
    {
        get => GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    public ClockIdentifierType ClockIdentifier
    {
        get => GetValue(ClockIdentifierProperty);
        set => SetValue(ClockIdentifierProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(TimePickerFlyoutPresenter);

    internal TimePicker TimePickerRef { get; set; }

    private TimePickerPresenter? _timePickerPresenter;
    private IDisposable? _disposable;
    private Button? _confirmButton;
    private Button? _nowButton;

    public TimePickerFlyoutPresenter(TimePicker timePicker)
    {
        TimePickerRef       = timePicker;
        HorizontalAlignment = HorizontalAlignment.Left;
        SetCurrentValue(TimeProperty, DateTime.Now.TimeOfDay);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _timePickerPresenter = e.NameScope.Get<TimePickerPresenter>(ArrowDecoratedBoxTheme.ContentPresenterPart);
        _confirmButton       = e.NameScope.Get<Button>(TimePickerFlyoutPresenterTheme.ConfirmButtonPart);
        _nowButton           = e.NameScope.Get<Button>(TimePickerFlyoutPresenterTheme.NowButtonPart);
        if (_timePickerPresenter is not null)
        {
            _timePickerPresenter.Confirmed += (sender, args) =>
            {
                TimePickerRef.NotifyConfirmed(_timePickerPresenter.Time);
            };
            if (TimePickerRef.DefaultTime is not null)
            {
                _timePickerPresenter.Time = TimePickerRef.DefaultTime.Value;
            }
        }

        if (_confirmButton is not null)
        {
            _confirmButton.Click += HandleConfirmButtonClicked;
        }

        if (_nowButton is not null)
        {
            _nowButton.Click += HandleNowButtonClicked;
        }
    }

    private void HandleNowButtonClicked(object? sender, RoutedEventArgs args)
    {
        _timePickerPresenter?.NowConfirm();
        TimePickerRef.ClosePickerFlyout();
    }

    private void HandleConfirmButtonClicked(object? sender, RoutedEventArgs args)
    {
        _timePickerPresenter?.Confirm();
        TimePickerRef.ClosePickerFlyout();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_timePickerPresenter is not null)
        {
            _disposable = TimePickerPresenter.TemporaryTimeProperty.Changed.Subscribe(args =>
            {
                TimePickerRef.NotifyTemporaryTimeSelected(args.GetNewValue<TimeSpan>());
            });
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _disposable?.Dispose();
    }
}