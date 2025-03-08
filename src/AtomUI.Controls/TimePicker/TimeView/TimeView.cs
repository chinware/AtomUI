using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class TimeSelectedEventArgs : EventArgs
{
    public TimeSpan? Time { get; }

    public TimeSelectedEventArgs(TimeSpan? value)
    {
        Time = value;
    }
}

[TemplatePart(TimeViewTheme.HourSelectorPart, typeof(DateTimePickerPanel), IsRequired = true)]
[TemplatePart(TimeViewTheme.MinuteSelectorPart, typeof(DateTimePickerPanel), IsRequired = true)]
[TemplatePart(TimeViewTheme.SecondSelectorPart, typeof(DateTimePickerPanel), IsRequired = true)]
[TemplatePart(TimeViewTheme.PeriodHostPart, typeof(Panel), IsRequired = true)]
[TemplatePart(TimeViewTheme.PeriodSelectorPart, typeof(DateTimePickerPanel), IsRequired = true)]
[TemplatePart(TimeViewTheme.PickerSelectorContainerPart, typeof(Grid), IsRequired = true)]
[TemplatePart(TimeViewTheme.SecondSpacerPart, typeof(Rectangle), IsRequired = true)]
internal class TimeView : TemplatedControl,
                          ITokenResourceConsumer
{
    #region 公共属性定义

    public static readonly StyledProperty<int> MinuteIncrementProperty =
        TimePicker.MinuteIncrementProperty.AddOwner<TimeView>();

    public static readonly StyledProperty<int> SecondIncrementProperty =
        TimePicker.SecondIncrementProperty.AddOwner<TimeView>();

    public static readonly StyledProperty<ClockIdentifierType> ClockIdentifierProperty =
        TimePicker.ClockIdentifierProperty.AddOwner<TimeView>();

    public static readonly StyledProperty<TimeSpan?> SelectedTimeProperty =
        AvaloniaProperty.Register<TimeView, TimeSpan?>(nameof(SelectedTime));

    public static readonly StyledProperty<bool> IsShowHeaderProperty =
        AvaloniaProperty.Register<TimeView, bool>(nameof(IsShowHeader), true);

    public static readonly StyledProperty<int> SelectorRowCountProperty =
        AvaloniaProperty.Register<TimeView, int>(nameof(SelectorRowCount), 7);

    /// <summary>
    /// Gets or sets the minute increment in the selector
    /// </summary>
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

    /// <summary>
    /// Gets or sets the current clock identifier, either 12HourClock or 24HourClock
    /// </summary>
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

    public bool IsShowHeader
    {
        get => GetValue(IsShowHeaderProperty);
        set => SetValue(IsShowHeaderProperty, value);
    }

    public int SelectorRowCount
    {
        get => GetValue(SelectorRowCountProperty);
        set => SetValue(SelectorRowCountProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<TimeView, double> SpacerThicknessProperty =
        AvaloniaProperty.RegisterDirect<TimeView, double>(nameof(SpacerWidth),
            o => o.SpacerWidth,
            (o, v) => o.SpacerWidth = v);

    internal static readonly DirectProperty<TimeView, double> ItemHeightProperty =
        AvaloniaProperty.RegisterDirect<TimeView, double>(nameof(ItemHeight),
            o => o.ItemHeight,
            (o, v) => o.ItemHeight = v);

    internal static readonly StyledProperty<bool> IsPointerInSelectorProperty =
        AvaloniaProperty.Register<TimeView, bool>(nameof(IsPointerInSelector), false);

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AnimationAwareControlProperty.IsMotionEnabledProperty.AddOwner<TimeView>();

    private double _spacerWidth;

    internal double SpacerWidth
    {
        get => _spacerWidth;
        set => SetAndRaise(SpacerThicknessProperty, ref _spacerWidth, value);
    }

    internal bool IsPointerInSelector
    {
        get => GetValue(IsPointerInSelectorProperty);
        set => SetValue(IsPointerInSelectorProperty, value);
    }

    private double _itemHeight;

    internal double ItemHeight
    {
        get => _itemHeight;
        set => SetAndRaise(ItemHeightProperty, ref _itemHeight, value);
    }

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;

    #endregion

    #region 公共事件定义

    public event EventHandler<TimeSelectedEventArgs>? TimeSelected;
    public event EventHandler<TimeSelectedEventArgs>? TempTimeSelected;
    public event EventHandler<TimeSelectedEventArgs>? HoverTimeChanged;

    #endregion

    private CompositeDisposable? _tokenBindingsDisposable;

    static TimeView()
    {
        KeyboardNavigation.TabNavigationProperty
                          .OverrideDefaultValue<TimeView>(KeyboardNavigationMode.Cycle);
    }

    // TemplateItems
    private Grid? _pickerSelectorContainer;
    private Rectangle? _spacer3;
    private Panel? _periodHost;
    private TextBlock? _headerText;
    private DateTimePickerPanel? _hourSelector;
    private DateTimePickerPanel? _minuteSelector;
    private DateTimePickerPanel? _secondSelector;
    private DateTimePickerPanel? _periodSelector;

    private IDisposable? _pointerPositionDisposable;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
        _pointerPositionDisposable = inputManager.Process.Subscribe(DetectPointerPosition);
        SyncTimeValueToPanel(SelectedTime ?? TimeSpan.Zero);
    }

    private void DetectPointerPosition(RawInputEventArgs args)
    {
        if (args is RawPointerEventArgs pointerEventArgs)
        {
            if (!CheckPointerInSelectors(pointerEventArgs.Position))
            {
                IsPointerInSelector = false;
            }
            else
            {
                IsPointerInSelector = true;
            }
        }
    }

    protected virtual bool CheckPointerInSelectors(Point position)
    {
        if (ClockIdentifier == ClockIdentifierType.HourClock12)
        {
            return CheckPointerInSelector(_hourSelector, position) ||
                   CheckPointerInSelector(_minuteSelector, position) ||
                   CheckPointerInSelector(_secondSelector, position) ||
                   CheckPointerInSelector(_periodSelector, position);
        }

        return CheckPointerInSelector(_hourSelector, position) ||
               CheckPointerInSelector(_minuteSelector, position) ||
               CheckPointerInSelector(_secondSelector, position);
    }

    private bool CheckPointerInSelector(DateTimePickerPanel? selector, Point position)
    {
        if (selector is null)
        {
            return false;
        }

        var globalRect = GetSelectorGlobalRect(selector);
        return globalRect.Contains(position);
    }

    private Rect GetSelectorGlobalRect(DateTimePickerPanel selector)
    {
        var pos = selector.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(selector)!) ?? default;
        return new Rect(pos, selector.Bounds.Size);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _pointerPositionDisposable?.Dispose();
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingsDisposable = new CompositeDisposable();
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, SpacerThicknessProperty, SharedTokenKey.LineWidth,
            BindingPriority.Template,
            new RenderScaleAwareDoubleConfigure(this)));
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _pickerSelectorContainer = e.NameScope.Get<Grid>(TimeViewTheme.PickerSelectorContainerPart);
        _periodHost              = e.NameScope.Get<Panel>(TimeViewTheme.PeriodHostPart);
        _headerText              = e.NameScope.Get<TextBlock>(TimeViewTheme.HeaderTextPart);

        _hourSelector   = e.NameScope.Get<DateTimePickerPanel>(TimeViewTheme.HourSelectorPart);
        _minuteSelector = e.NameScope.Get<DateTimePickerPanel>(TimeViewTheme.MinuteSelectorPart);
        _secondSelector = e.NameScope.Get<DateTimePickerPanel>(TimeViewTheme.SecondSelectorPart);
        _periodSelector = e.NameScope.Get<DateTimePickerPanel>(TimeViewTheme.PeriodSelectorPart);
        SetupPickerSelectorContainerHeight();

        _spacer3 = e.NameScope.Get<Rectangle>(TimeViewTheme.ThirdSpacerPart);
        InitPicker();

        if (_hourSelector is not null)
        {
            _hourSelector.SelectionChanged += HandleSelectionChanged;
            _hourSelector.CellHovered      += HandleSelectorCellHovered;
            _hourSelector.CellDbClicked    += HandleSelectorCellDbClicked;
        }

        if (_minuteSelector is not null)
        {
            _minuteSelector.SelectionChanged += HandleSelectionChanged;
            _minuteSelector.CellHovered      += HandleSelectorCellHovered;
            _minuteSelector.CellDbClicked    += HandleSelectorCellDbClicked;
        }

        if (_secondSelector is not null)
        {
            _secondSelector.SelectionChanged += HandleSelectionChanged;
            _secondSelector.CellHovered      += HandleSelectorCellHovered;
            _secondSelector.CellDbClicked    += HandleSelectorCellDbClicked;
        }

        if (_periodSelector is not null)
        {
            _periodSelector.SelectionChanged += HandleSelectionChanged;
            _periodSelector.CellHovered      += HandleSelectorCellHovered;
            _periodSelector.CellDbClicked    += HandleSelectorCellDbClicked;
        }
    }

    private void HandleSelectorCellDbClicked(object? sender, CellDbClickedEventArgs args)
    {
        if (args.IsSelected)
        {
            SelectedTime = CollectValue();
            TimeSelected?.Invoke(this, new TimeSelectedEventArgs(SelectedTime));
        }
    }

    private void HandleSelectorCellHovered(object? sender, CellHoverEventArgs args)
    {
        var selectedTime  = CollectValue(false);
        var hour          = selectedTime.Hours;
        var minute        = selectedTime.Minutes;
        var second        = selectedTime.Seconds;
        var period        = _periodSelector?.SelectedValue ?? default;
        var cellHoverInfo = args.CellHoverInfo;

        if (cellHoverInfo.HasValue)
        {
            var panelType = cellHoverInfo.Value.PanelType;
            var cellValue = cellHoverInfo.Value.CellValue;
            if (panelType == DateTimePickerPanelType.Hour)
            {
                hour = cellValue;
            }
            else if (panelType == DateTimePickerPanelType.Minute)
            {
                minute = cellValue;
            }
            else if (panelType == DateTimePickerPanelType.Second)
            {
                second = cellValue;
            }
            else if (panelType == DateTimePickerPanelType.TimePeriod)
            {
                period = cellValue;
            }

            if (ClockIdentifier == ClockIdentifierType.HourClock12)
            {
                if (period == 0 && hour == 12)
                {
                    // AM
                    hour = 0;
                }

                if (period == 1 && hour != 12)
                {
                    hour += 12;
                }
            }

            var hoverTime = new TimeSpan(hour, minute, second);
            HoverTimeChanged?.Invoke(this, new TimeSelectedEventArgs(hoverTime));
        }
    }

    private void HandleSelectionChanged(object? sender, EventArgs args)
    {
        var selectedValue = CollectValue();
        if (IsShowHeader)
        {
            if (_headerText is not null)
            {
                _headerText.Text =
                    DateTimeUtils.FormatTimeSpan(selectedValue, ClockIdentifier == ClockIdentifierType.HourClock12);
            }
        }

        TempTimeSelected?.Invoke(this, new TimeSelectedEventArgs(selectedValue));
    }

    private TimeSpan CollectValue(bool translate = true)
    {
        var hour   = _hourSelector!.SelectedValue;
        var minute = _minuteSelector!.SelectedValue;
        var second = _secondSelector!.SelectedValue;
        var period = _periodSelector!.SelectedValue;

        if (translate)
        {
            if (ClockIdentifier == ClockIdentifierType.HourClock12)
            {
                hour = period == 1 ? hour == 12 ? 12 : hour + 12 : period == 0 && hour == 12 ? 0 : hour;
            }
        }

        return new TimeSpan(hour, minute, second);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == MinuteIncrementProperty ||
            change.Property == SecondIncrementProperty ||
            change.Property == ClockIdentifierProperty)
        {
            InitPicker();
        }

        if (change.Property == SelectedTimeProperty)
        {
            if (this.IsAttachedToVisualTree() && SelectedTime is not null)
            {
                SyncTimeValueToPanel(SelectedTime.Value);
            }
        }

        if (change.Property == ItemHeightProperty ||
            change.Property == SelectorRowCountProperty)
        {
            SetupPickerSelectorContainerHeight();
        }
    }

    private void SyncTimeValueToPanel(TimeSpan time)
    {
        var clock12 = ClockIdentifier == ClockIdentifierType.HourClock12;
        var hour    = time.Hours;
        if (_hourSelector is not null)
        {
            _hourSelector.SelectedValue = !clock12 ? hour :
                hour > 12 ? hour - 12 :
                hour == 0 ? 12 : hour;
        }

        if (_minuteSelector is not null)
        {
            _minuteSelector.SelectedValue = time.Minutes;
        }

        if (_secondSelector is not null)
        {
            _secondSelector.SelectedValue = time.Seconds;
        }

        if (_periodSelector is not null)
        {
            _periodSelector.SelectedValue = hour >= 12 ? 1 : 0;
            ;
        }
    }

    private void InitPicker()
    {
        if (_pickerSelectorContainer == null)
        {
            return;
        }

        var selectedTime   = SelectedTime ?? TimeSpan.Zero;
        var clock12        = ClockIdentifier == ClockIdentifierType.HourClock12;
        var use24HourClock = ClockIdentifier == ClockIdentifierType.HourClock24;
        _hourSelector!.MaximumValue = clock12 ? 12 : 23;
        _hourSelector.MinimumValue  = clock12 ? 1 : 0;
        _hourSelector.ItemFormat    = "%h";

        _minuteSelector!.MaximumValue = 59;
        _minuteSelector.MinimumValue  = 0;
        _minuteSelector.Increment     = MinuteIncrement;
        _minuteSelector.ItemFormat    = "mm";

        _secondSelector!.MaximumValue = 59;
        _secondSelector.MinimumValue  = 0;
        _secondSelector.Increment     = SecondIncrement;
        _secondSelector.ItemFormat    = "ss";

        _periodSelector!.MaximumValue = 1;
        _periodSelector.MinimumValue  = 0;

        SyncTimeValueToPanel(selectedTime);

        _spacer3!.IsVisible    = !use24HourClock;
        _periodHost!.IsVisible = !use24HourClock;
    }

    private void SetupPickerSelectorContainerHeight()
    {
        if (_pickerSelectorContainer is null)
        {
            return;
        }

        _pickerSelectorContainer.Height = ItemHeight * SelectorRowCount;
    }
}