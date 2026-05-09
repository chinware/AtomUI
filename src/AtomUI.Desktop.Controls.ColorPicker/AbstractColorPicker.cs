using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Media;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

using AvaloniaButton = Avalonia.Controls.Button;
using AtomUIFlyout = AtomUI.Desktop.Controls.Flyout;

public enum ColorPickerValueSyncMode
{
    Immediate,
    OnCompleted
}

public abstract class AbstractColorPicker : AvaloniaButton, 
                                            ISizeTypeAware,
                                            IMotionAwareControl,
                                            ICompactSpaceAware,
                                            IFormItemAware,
                                            IInputControlStatusAware,
                                            IInputControlStyleVariantAware
{
    #region 公共属性定义
    public static readonly StyledProperty<ColorFormat> FormatProperty =
        AbstractColorPickerView.FormatProperty.AddOwner<AbstractColorPicker>();

    public static readonly StyledProperty<FlyoutTriggerType> TriggerTypeProperty =
        FlyoutStateHelper.TriggerTypeProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<bool> IsShowArrowProperty =
        ArrowDecoratedBox.IsShowArrowProperty.AddOwner<AbstractColorPicker>();

    public static readonly StyledProperty<bool> IsPointAtCenterProperty =
        AtomUIFlyout.IsPointAtCenterProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        Popup.PlacementProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<PopupGravity> PlacementGravityProperty =
        Popup.PlacementGravityProperty.AddOwner<AbstractColorPicker>();

    public static readonly StyledProperty<double> MarginToAnchorProperty =
        Popup.MarginToAnchorProperty.AddOwner<AbstractColorPicker>();

    public static readonly StyledProperty<int> MouseEnterDelayProperty =
        FlyoutStateHelper.MouseEnterDelayProperty.AddOwner<AbstractColorPicker>();

    public static readonly StyledProperty<int> MouseLeaveDelayProperty =
        FlyoutStateHelper.MouseLeaveDelayProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<bool> IsAlphaEnabledProperty =
        AbstractColorPickerView.IsAlphaEnabledProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<bool> IsFormatEnabledProperty =
        AbstractColorPickerView.IsFormatEnabledProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<bool> IsShowTextProperty =
        AvaloniaProperty.Register<AbstractColorPicker, bool>(nameof(IsShowText));
    
    public static readonly StyledProperty<bool> IsClearEnabledProperty =
        AbstractColorPickerView.IsClearEnabledProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<string> EmptyColorTextProperty =
        AvaloniaProperty.Register<AbstractColorPicker, string>(nameof(EmptyColorText), string.Empty);
    
    public static readonly StyledProperty<bool> IsPaletteGroupEnabledProperty =
        AvaloniaProperty.Register<AbstractColorPicker, bool>(nameof(IsPaletteGroupEnabled));

    public static readonly StyledProperty<List<ColorPickerPalette>?> PaletteGroupProperty =
        ColorPickerPaletteGroup.PaletteGroupProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<InputControlStyleVariant> StyleVariantProperty =
        InputControlStyleVariantProperty.StyleVariantProperty.AddOwner<AbstractColorPicker>();

    public static readonly StyledProperty<InputControlStatus> StatusProperty =
        InputControlStatusProperty.StatusProperty.AddOwner<AbstractColorPicker>();

    public ColorFormat Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }
    
    public FlyoutTriggerType TriggerType
    {
        get => GetValue(TriggerTypeProperty);
        set => SetValue(TriggerTypeProperty, value);
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
    
    public PlacementMode Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }
    
    public PopupGravity PlacementGravity
    {
        get => GetValue(PlacementGravityProperty);
        set => SetValue(PlacementGravityProperty, value);
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
    
    public bool IsAlphaEnabled
    {
        get => GetValue(IsAlphaEnabledProperty);
        set => SetValue(IsAlphaEnabledProperty, value);
    }
    
    public bool IsFormatEnabled
    {
        get => GetValue(IsFormatEnabledProperty);
        set => SetValue(IsFormatEnabledProperty, value);
    }
    
    public bool IsShowText
    {
        get => GetValue(IsShowTextProperty);
        set => SetValue(IsShowTextProperty, value);
    }
    
    public bool IsClearEnabled
    {
        get => GetValue(IsClearEnabledProperty);
        set => SetValue(IsClearEnabledProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public string EmptyColorText
    {
        get => GetValue(EmptyColorTextProperty);
        set => SetValue(EmptyColorTextProperty, value);
    }

    public bool IsPaletteGroupEnabled
    {
        get => GetValue(IsPaletteGroupEnabledProperty);
        set => SetValue(IsPaletteGroupEnabledProperty, value);
    }

    public List<ColorPickerPalette>? PaletteGroup
    {
        get => GetValue(PaletteGroupProperty);
        set => SetValue(PaletteGroupProperty, value);
    }
    
    public InputControlStyleVariant StyleVariant
    {
        get => GetValue(StyleVariantProperty);
        set => SetValue(StyleVariantProperty, value);
    }

    public InputControlStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
    
    #endregion

    #region 内部属性定义
    
    internal static readonly DirectProperty<AbstractColorPicker, IBrush?> ColorBlockBackgroundProperty =
        AvaloniaProperty.RegisterDirect<AbstractColorPicker, IBrush?>(
            nameof(ColorBlockBackground),
            o => o.ColorBlockBackground,
            (o, v) => o.ColorBlockBackground = v);
    
    internal static readonly DirectProperty<AbstractColorPicker, CornerRadius> EffectiveCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<AbstractColorPicker, CornerRadius>(nameof(EffectiveCornerRadius),
            o => o.EffectiveCornerRadius,
            (o, v) => o.EffectiveCornerRadius = v);
    
    internal static readonly StyledProperty<SpaceItemPosition?> CompactSpaceItemPositionProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceItemPositionProperty.AddOwner<AbstractColorPicker>();
    
    internal static readonly StyledProperty<Orientation> CompactSpaceOrientationProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceOrientationProperty.AddOwner<AbstractColorPicker>();
    
    internal static readonly StyledProperty<bool> IsUsedInCompactSpaceProperty = 
        CompactSpaceAwareControlProperty.IsUsedInCompactSpaceProperty.AddOwner<AbstractColorPicker>();
    
    private IBrush? _colorBlockBackground;

    internal IBrush? ColorBlockBackground
    {
        get => _colorBlockBackground;
        set => SetAndRaise(ColorBlockBackgroundProperty, ref _colorBlockBackground, value);
    }
        
    private CornerRadius _effectiveCornerRadius;

    internal CornerRadius EffectiveCornerRadius
    {
        get => _effectiveCornerRadius;
        set => SetAndRaise(EffectiveCornerRadiusProperty, ref _effectiveCornerRadius, value);
    }
    
    internal SpaceItemPosition? CompactSpaceItemPosition
    {
        get => GetValue(CompactSpaceItemPositionProperty);
        set => SetValue(CompactSpaceItemPositionProperty, value);
    }
    
    internal Orientation CompactSpaceOrientation
    {
        get => GetValue(CompactSpaceOrientationProperty);
        set => SetValue(CompactSpaceOrientationProperty, value);
    }
    
    internal bool IsUsedInCompactSpace
    {
        get => GetValue(IsUsedInCompactSpaceProperty);
        set => SetValue(IsUsedInCompactSpaceProperty, value);
    }
    
    #endregion
    
    private readonly FlyoutStateHelper _flyoutStateHelper;
    private protected Flyout? PickerFlyout;
    private protected bool IsFlyoutOpen;
    
    static AbstractColorPicker()
    {
        AffectsMeasure<AbstractColorPicker>(IsShowTextProperty, 
            FormatProperty,
            ColorBlockBackgroundProperty);
    }
    
    public AbstractColorPicker()
    {
        this.RegisterTokenResourceScope(ColorPickerToken.ScopeProvider);
        _flyoutStateHelper = new FlyoutStateHelper();
        _flyoutStateHelper.FlyoutAboutToShow        += HandleFlyoutAboutToShow;
        _flyoutStateHelper.FlyoutAboutToClose       += HandleFlyoutAboutToClose;
        _flyoutStateHelper.FlyoutOpened             += HandleFlyoutOpened;
        _flyoutStateHelper.FlyoutClosed             += HandleFlyoutClosed;
        _flyoutStateHelper[!FlyoutStateHelper.TriggerTypeProperty]     = this[!TriggerTypeProperty];
        _flyoutStateHelper[!FlyoutStateHelper.MouseEnterDelayProperty] = this[!MouseEnterDelayProperty];
        _flyoutStateHelper[!FlyoutStateHelper.MouseLeaveDelayProperty] = this[!MouseLeaveDelayProperty];
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _flyoutStateHelper.NotifyAttachedToVisualTree();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _flyoutStateHelper.NotifyDetachedFromVisualTree();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsShowTextProperty || change.Property == FormatProperty)
        {
            GenerateValueText();
        }

        else if (change.Property == CornerRadiusProperty ||
                 change.Property == CompactSpaceItemPositionProperty ||
                 change.Property == CompactSpaceOrientationProperty)
        {
            ConfigureCornerRadius();
        }
    }

    protected abstract void GenerateValueText();
    protected abstract void GenerateColorBlockBackground();

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (PickerFlyout is null)
        {
            PickerFlyout                    =  CreatePickerFlyout();
            PickerFlyout.Opened            += HandlePickerFlyoutOpened;
            PickerFlyout.Closed            += HandlePickerFlyoutClosed;
            if (PickerFlyout is AbstractColorPickerFlyout colorPickerFlyout)
            {
                colorPickerFlyout.PresenterCreated += HandlePickerFlyoutPresenterCreated;
            }
            _flyoutStateHelper.Flyout       =  PickerFlyout;
        }
        _flyoutStateHelper.AnchorTarget = this;
        SetupFlyoutProperties();
    }

    protected virtual void SetupFlyoutProperties()
    {
        if (PickerFlyout is not null)
        {
            PickerFlyout[!PopupFlyoutBase.PlacementProperty]    = this[!PlacementProperty];
            PickerFlyout[!AtomUIFlyout.IsShowArrowProperty]     = this[!IsShowArrowProperty];
            PickerFlyout[!AtomUIFlyout.IsPointAtCenterProperty] = this[!IsPointAtCenterProperty];
            PickerFlyout[!AtomUIFlyout.MarginToAnchorProperty]  = this[!MarginToAnchorProperty];
            PickerFlyout[!IsMotionEnabledProperty]              = this[!IsMotionEnabledProperty];
        }
    }
    
    protected abstract Flyout CreatePickerFlyout();
    
    protected virtual void NotifyFlyoutPresenterCreated(Control flyoutPresenter)
    {
    }
    
    protected virtual void NotifyFlyoutClosed()
    {
    }
    
    private void HandleFlyoutClosed(object? sender, EventArgs args)
    {
        NotifyFlyoutClosed();
    }
    
    private void HandleFlyoutOpened(object? sender, EventArgs args)
    {
        NotifyFlyoutOpened();
    }

    private void HandlePickerFlyoutOpened(object? sender, EventArgs args)
    {
        IsFlyoutOpen = true;
        UpdatePseudoClasses();
    }

    private void HandlePickerFlyoutClosed(object? sender, EventArgs args)
    {
        IsFlyoutOpen = false;
        UpdatePseudoClasses();
    }

    private void HandlePickerFlyoutPresenterCreated(object? sender, Control presenter)
    {
        NotifyFlyoutPresenterCreated(presenter);
    }

    protected virtual void NotifyFlyoutOpened()
    {
    }
    
    private void HandleFlyoutAboutToClose(object? sender, EventArgs args)
    {
        NotifyFlyoutAboutToClose();
    }

    protected virtual void NotifyFlyoutAboutToClose()
    {
    }
    
    private void HandleFlyoutAboutToShow(object? sender, EventArgs args)
    {
        NotifyFlyoutAboutToShow();
    }

    protected virtual void NotifyFlyoutAboutToShow()
    {
    }
    
    public void ClosePickerFlyout()
    {
        _flyoutStateHelper.HideFlyout(true);
    }
    
    protected virtual bool FlyoutOpenPredicate(RawPointerEventArgs args)
    {
        if (!IsEnabled)
        {
            return false;
        }
        var pos = this.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
        if (!pos.HasValue)
        {
            return false;
        }

        var region = new Rect(pos.Value, Bounds.Size);
        return region.Contains(args.Position);
    }
    
    protected virtual void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.FlyoutOpen, IsFlyoutOpen);
    }

    public static string FormatColor(Color color, ColorFormat format)
    {
        if (format == ColorFormat.Hex)
        {
            return ColorToHexConverter.ToHexString(color, AlphaComponentPosition.Leading, false, true);
        }
        if (format == ColorFormat.Rgba)
        {
            return $"rgba({(int)color.R}, {(int)color.G}, {(int)color.B}, {color.GetAlphaF():0.00})";
        }

        var hsvColor = color.ToHsv();
        return $"hsva({hsvColor.H:0}, {hsvColor.S * 100:0}%, {hsvColor.V * 100:0}%,  {hsvColor.A:0.00})";
    }
    
    private void ConfigureCornerRadius()
    {
        EffectiveCornerRadius = CompactSpace.CalculateEffectiveCornerRadius(
            CornerRadius, 
            IsUsedInCompactSpace, 
            CompactSpaceItemPosition,
            CompactSpaceOrientation);
    }
    
    void ICompactSpaceAware.NotifyPositionChange(SpaceItemPosition? position)
    {
        IsUsedInCompactSpace     = position != null;
        CompactSpaceItemPosition = position;
    }
    
    void ICompactSpaceAware.NotifyOrientationChange(Orientation orientation)
    {
        CompactSpaceOrientation = orientation;
    }

    double ICompactSpaceAware.GetBorderThickness()
    {
        return CompactSpaceOrientation ==  Orientation.Horizontal ? BorderThickness.Left : BorderThickness.Top;
    }
    
    #region 实现 FormItem 接口
    
    private EventHandler? _formValueChanged;
    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    void IFormItemAware.SetFormValue(object? value) => NotifySetFormValue(value);

    object? IFormItemAware.GetFormValue() => NotifyGetFormValue();
    void IFormItemAware.ClearFormValue() => NotifyClearFormValue();
    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status) => NotifyValidateStatus(status);
    
    protected virtual void NotifyFormValueChanged(object? value)
    {
        _formValueChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void NotifySetFormValue(object? value)
    {
    }

    protected virtual object? NotifyGetFormValue()
    {
        return null;
    }

    protected virtual void NotifyClearFormValue()
    {
    }

    protected virtual void NotifyValidateStatus(FormValidateStatus status)
    {
        if (status == FormValidateStatus.Error)
        {
            SetCurrentValue(StatusProperty, InputControlStatus.Error);
        }
        else if (status == FormValidateStatus.Warning)
        {
            SetCurrentValue(StatusProperty, InputControlStatus.Warning);
        }
        else
        {
            SetCurrentValue(StatusProperty, InputControlStatus.Default);
        }
    }
    #endregion

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Dispatcher.Post(this.EnableTransitions);
    }
}
