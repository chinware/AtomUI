using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Layout;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

public enum ClockIdentifierType
{
    HourClock12,
    HourClock24
}

[PseudoClasses(FlyoutOpenPC)]
public class TimePicker : LineEdit
{
    private const string FlyoutOpenPC = ":flyout-open";
    private readonly FlyoutStateHelper _flyoutStateHelper;
    private IDisposable? _clearUpButtonDetectDisposable;
    private bool _currentValidSelected;
    private bool _isFlyoutOpen;
    private PickerClearUpButton? _pickerClearUpButton;
    private TimePickerFlyout? _pickerFlyout;

    private TextBoxInnerBox? _textBoxInnerBox;

    static TimePicker()
    {
        HorizontalAlignmentProperty.OverrideDefaultValue<TimePicker>(HorizontalAlignment.Left);
        VerticalAlignmentProperty.OverrideDefaultValue<TimePicker>(VerticalAlignment.Top);
        IsEnableClearButtonProperty.OverrideDefaultValue<TimePicker>(false);
    }

    public TimePicker()
    {
        _flyoutStateHelper = new FlyoutStateHelper
        {
            TriggerType = FlyoutTriggerType.Click
        };
        _flyoutStateHelper.FlyoutAboutToShow        += HandleFlyoutAboutToShow;
        _flyoutStateHelper.FlyoutAboutToClose       += HandleFlyoutAboutToClose;
        _flyoutStateHelper.OpenFlyoutPredicate      =  FlyoutOpenPredicate;
        _flyoutStateHelper.ClickHideFlyoutPredicate =  ClickHideFlyoutPredicate;
    }

    protected override Type StyleKeyOverride => typeof(LineEdit);

    private bool FlyoutOpenPredicate(Point position)
    {
        if (!IsEnabled)
        {
            return false;
        }

        return PositionInEditKernel(position);
    }

    private bool PositionInEditKernel(Point position)
    {
        if (_textBoxInnerBox is not null)
        {
            var pos = _textBoxInnerBox.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
            if (!pos.HasValue)
            {
                return false;
            }

            var targetWidth  = _textBoxInnerBox.Bounds.Width;
            var targetHeight = _textBoxInnerBox.Bounds.Height;
            var startOffsetX = pos.Value.X;
            var endOffsetX   = startOffsetX + targetWidth;
            var offsetY      = pos.Value.Y;
            if (InnerLeftContent is Control leftContent)
            {
                var leftContentPos = leftContent.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
                if (leftContentPos.HasValue)
                {
                    startOffsetX = leftContentPos.Value.X + leftContent.Bounds.Width;
                }
            }

            if (InnerRightContent is Control rightContent)
            {
                var rightContentPos = rightContent.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
                if (rightContentPos.HasValue)
                {
                    endOffsetX = rightContentPos.Value.X;
                }
            }

            targetWidth = endOffsetX - startOffsetX;
            var bounds = new Rect(new Point(startOffsetX, offsetY), new Size(targetWidth, targetHeight));
            if (bounds.Contains(position))
            {
                return true;
            }
        }

        return false;
    }

    private bool ClickHideFlyoutPredicate(IPopupHostProvider hostProvider, RawPointerEventArgs args)
    {
        if (hostProvider.PopupHost != args.Root)
        {
            if (!PositionInEditKernel(args.Position))
            {
                return true;
            }
        }

        return false;
    }

    private void HandleFlyoutAboutToShow(object? sender, EventArgs args)
    {
        _currentValidSelected = false;
    }

    private void HandleFlyoutAboutToClose(object? sender, EventArgs args)
    {
        if (!_currentValidSelected)
        {
            if (SelectedTime.HasValue)
            {
                Text = DateTimeUtils.FormatTimeSpan(SelectedTime.Value,
                    ClockIdentifier == ClockIdentifierType.HourClock12);
            }
            else
            {
                ResetTimeValue();
            }
        }
    }

    internal void ClosePickerFlyout()
    {
        _flyoutStateHelper.HideFlyout(true);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (InnerRightContent is null)
        {
            _pickerClearUpButton = new PickerClearUpButton();
            _pickerClearUpButton.ClearRequest += (sender, args) =>
            {
                ResetTimeValue();
                SelectedTime = null;
            };
            InnerRightContent = _pickerClearUpButton;
        }

        if (_pickerFlyout is null)
        {
            _pickerFlyout = new TimePickerFlyout(this);
            _pickerFlyout.Opened += (sender, args) =>
            {
                _isFlyoutOpen = true;
                UpdatePseudoClasses();
            };
            _pickerFlyout.Closed += (sender, args) =>
            {
                _isFlyoutOpen = false;
                UpdatePseudoClasses();
            };
            _flyoutStateHelper.Flyout = _pickerFlyout;
        }

        _textBoxInnerBox                = e.NameScope.Get<TextBoxInnerBox>(TextBoxTheme.TextBoxInnerBoxPart);
        _flyoutStateHelper.AnchorTarget = _textBoxInnerBox;
        TokenResourceBinder.CreateGlobalTokenBinding(this, MarginToAnchorProperty, GlobalTokenResourceKey.MarginXXS);
        SetupFlyoutProperties();
        ResetTimeValue();
    }

    protected void SetupFlyoutProperties()
    {
        if (_pickerFlyout is not null)
        {
            BindUtils.RelayBind(this, PickerPlacementProperty, _pickerFlyout, PopupFlyoutBase.PlacementProperty);
            BindUtils.RelayBind(this, IsShowArrowProperty, _pickerFlyout);
            BindUtils.RelayBind(this, IsPointAtCenterProperty, _pickerFlyout);
            BindUtils.RelayBind(this, MarginToAnchorProperty, _pickerFlyout);
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        BindUtils.RelayBind(this, MouseEnterDelayProperty, _flyoutStateHelper,
            FlyoutStateHelper.MouseEnterDelayProperty);
        BindUtils.RelayBind(this, MouseLeaveDelayProperty, _flyoutStateHelper,
            FlyoutStateHelper.MouseLeaveDelayProperty);
        if (DefaultTime is not null)
        {
            SelectedTime = DefaultTime;
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _flyoutStateHelper.NotifyAttachedToVisualTree();
        if (_clearUpButtonDetectDisposable is null)
        {
            var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
            _clearUpButtonDetectDisposable = inputManager.Process.Subscribe(DetectClearUpButtonState);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _flyoutStateHelper.NotifyDetachedFromVisualTree();
        _clearUpButtonDetectDisposable?.Dispose();
        _clearUpButtonDetectDisposable = null;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (VisualRoot is not null)
        {
            if (change.Property == SelectedTimeProperty)
            {
                if (SelectedTime.HasValue)
                {
                    Text = DateTimeUtils.FormatTimeSpan(SelectedTime.Value,
                        ClockIdentifier == ClockIdentifierType.HourClock12);
                }
                else
                {
                    ResetTimeValue();
                }
            }
        }
    }

    protected void ResetTimeValue()
    {
        if (DefaultTime is not null)
        {
            Text = DateTimeUtils.FormatTimeSpan(DefaultTime.Value, ClockIdentifier == ClockIdentifierType.HourClock12);
        }
        else
        {
            Clear();
        }
    }

    private void DetectClearUpButtonState(RawInputEventArgs args)
    {
        if (IsEnabled)
        {
            if (args is RawPointerEventArgs pointerEventArgs)
            {
                if (_textBoxInnerBox is not null)
                {
                    var pos = _textBoxInnerBox.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
                    if (!pos.HasValue)
                    {
                        return;
                    }

                    var bounds = new Rect(pos.Value, _textBoxInnerBox.Bounds.Size);
                    if (bounds.Contains(pointerEventArgs.Position))
                    {
                        if (SelectedTime is not null)
                        {
                            _pickerClearUpButton!.IsInClearMode = true;
                        }
                    }
                    else
                    {
                        _pickerClearUpButton!.IsInClearMode = false;
                    }
                }
            }
        }
    }

    private static int CoerceMinuteIncrement(AvaloniaObject sender, int value)
    {
        if (value < 1 || value > 59)
        {
            throw new ArgumentOutOfRangeException(null, "1 >= MinuteIncrement <= 59");
        }

        return value;
    }

    private static int CoerceSecondIncrement(AvaloniaObject sender, int value)
    {
        if (value < 1 || value > 59)
        {
            throw new ArgumentOutOfRangeException(null, "1 >= SecondIncrement <= 59");
        }

        return value;
    }

    internal void NotifyTemporaryTimeSelected(TimeSpan value)
    {
        Text = DateTimeUtils.FormatTimeSpan(value, ClockIdentifier == ClockIdentifierType.HourClock12);
    }

    internal void NotifyConfirmed(TimeSpan value)
    {
        _currentValidSelected = true;
        SelectedTime          = value;
    }

    protected void UpdatePseudoClasses()
    {
        PseudoClasses.Set(FlyoutOpenPC, _isFlyoutOpen);
    }

    #region 公共属性定义

    public static readonly StyledProperty<PlacementMode> PickerPlacementProperty =
        AvaloniaProperty.Register<TimePicker, PlacementMode>(nameof(PickerPlacement),
            PlacementMode.BottomEdgeAlignedLeft);

    public static readonly StyledProperty<bool> IsShowArrowProperty =
        ArrowDecoratedBox.IsShowArrowProperty.AddOwner<TimePicker>();

    public static readonly StyledProperty<bool> IsPointAtCenterProperty =
        Flyout.IsPointAtCenterProperty.AddOwner<TimePicker>();

    public static readonly StyledProperty<int> MinuteIncrementProperty =
        AvaloniaProperty.Register<TimePicker, int>(nameof(MinuteIncrement), 1, coerce: CoerceMinuteIncrement);

    public static readonly StyledProperty<int> SecondIncrementProperty =
        AvaloniaProperty.Register<TimePicker, int>(nameof(SecondIncrement), 1, coerce: CoerceSecondIncrement);

    public static readonly StyledProperty<ClockIdentifierType> ClockIdentifierProperty =
        AvaloniaProperty.Register<TimePicker, ClockIdentifierType>(nameof(ClockIdentifier));

    public static readonly StyledProperty<TimeSpan?> SelectedTimeProperty =
        AvaloniaProperty.Register<TimePicker, TimeSpan?>(nameof(SelectedTime),
            defaultBindingMode: BindingMode.TwoWay,
            enableDataValidation: true);

    public static readonly StyledProperty<TimeSpan?> DefaultTimeProperty =
        AvaloniaProperty.Register<TimePicker, TimeSpan?>(nameof(DefaultTime),
            enableDataValidation: true);

    public static readonly StyledProperty<double> MarginToAnchorProperty =
        Popup.MarginToAnchorProperty.AddOwner<TimePicker>();

    public static readonly StyledProperty<int> MouseEnterDelayProperty =
        FlyoutStateHelper.MouseEnterDelayProperty.AddOwner<TimePicker>();

    public static readonly StyledProperty<int> MouseLeaveDelayProperty =
        FlyoutStateHelper.MouseLeaveDelayProperty.AddOwner<TimePicker>();

    public PlacementMode PickerPlacement
    {
        get => GetValue(PickerPlacementProperty);
        set => SetValue(PickerPlacementProperty, value);
    }

    public bool IsShowArrow
    {
        get => GetValue(IsShowArrowProperty);
        set => SetValue(IsShowArrowProperty, value);
    }

    public bool IsPointAtCenter
    {
        get => GetValue(IsPointAtCenterProperty);
        set => SetValue(IsPointAtCenterProperty, value);
    }

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

    public ClockIdentifierType ClockIdentifier
    {
        get => GetValue(ClockIdentifierProperty);
        set => SetValue(ClockIdentifierProperty, value);
    }

    public TimeSpan? SelectedTime
    {
        get => GetValue(SelectedTimeProperty);
        set => SetValue(SelectedTimeProperty, value);
    }

    public TimeSpan? DefaultTime
    {
        get => GetValue(DefaultTimeProperty);
        set => SetValue(DefaultTimeProperty, value);
    }

    public double MarginToAnchor
    {
        get => GetValue(MarginToAnchorProperty);
        set => SetValue(MarginToAnchorProperty, value);
    }

    public int MouseEnterDelay
    {
        get => GetValue(MouseEnterDelayProperty);
        set => SetValue(MouseEnterDelayProperty, value);
    }

    public int MouseLeaveDelay
    {
        get => GetValue(MouseLeaveDelayProperty);
        set => SetValue(MouseLeaveDelayProperty, value);
    }

    #endregion
}