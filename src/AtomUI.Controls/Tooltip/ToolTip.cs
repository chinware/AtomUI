using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Theme;
using AtomUI.Theme.Palette;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.Open)]
public class ToolTip : ContentControl,
                       IControlSharedTokenResourcesHost,
                       IAnimationAwareControl,
                       IPopupHostProvider,
                       IShadowMaskInfoProvider,
                       ITokenResourceConsumer
{
    #region 公共属性定义

    public static readonly AttachedProperty<object?> TipProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, object?>("Tip");

    public static readonly AttachedProperty<bool> IsOpenProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("IsOpen");

    public static readonly AttachedProperty<PresetColorType?> PresetColorProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, PresetColorType?>("PresetColor");

    public static readonly AttachedProperty<Color?> ColorProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, Color?>("Color");

    public static readonly AttachedProperty<bool> IsShowArrowProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("IsShowArrow", true);

    public static readonly AttachedProperty<bool> IsPointAtCenterProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("IsPointAtCenter");

    public static readonly AttachedProperty<PlacementMode> PlacementProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, PlacementMode>(
            "Placement", PlacementMode.Top);

    public static readonly AttachedProperty<double> HorizontalOffsetProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, double>("HorizontalOffset");

    public static readonly AttachedProperty<double> VerticalOffsetProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, double>("VerticalOffset");

    /// <summary>
    /// 距离 anchor 的边距，根据垂直和水平进行设置
    /// </summary>
    public static readonly AttachedProperty<double> MarginToAnchorProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, double>("MarginToAnchor", 2);

    public static readonly AttachedProperty<int> ShowDelayProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, int>("ShowDelay", 400);

    public static readonly AttachedProperty<int> BetweenShowDelayProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, int>("BetweenShowDelay", 100);

    public static readonly AttachedProperty<bool> ShowOnDisabledProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("ShowOnDisabled", false, true);

    public static readonly AttachedProperty<bool> ServiceEnabledProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("ServiceEnabled", true, true);

    public static readonly AttachedProperty<CustomPopupPlacementCallback?> CustomPopupPlacementCallbackProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, CustomPopupPlacementCallback?>(
            "CustomPopupPlacementCallback");

    public static readonly AttachedProperty<bool> IsCustomHideProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("IsCustomHide");

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AnimationAwareControlProperty.IsMotionEnabledProperty.AddOwner<ToolTip>();

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AnimationAwareControlProperty.IsWaveAnimationEnabledProperty.AddOwner<ToolTip>();

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }

    #endregion

    #region 附加属性设置方法定义

    public static object? GetTip(Control element)
    {
        return element.GetValue(TipProperty);
    }

    public static void SetTip(Control element, object? value)
    {
        element.SetValue(TipProperty, value);
    }

    public static bool GetIsOpen(Control element)
    {
        return element.GetValue(IsOpenProperty);
    }

    public static void SetIsOpen(Control element, bool value)
    {
        element.SetValue(IsOpenProperty, value);
    }

    public static PresetColorType? GetPresetColor(Control element)
    {
        return element.GetValue(PresetColorProperty);
    }

    public static void SetPresetColor(Control element, PresetColorType color)
    {
        element.SetValue(PresetColorProperty, color);
    }

    public static Color? GetColor(Control element)
    {
        return element.GetValue(ColorProperty);
    }

    public static void SetColor(Control element, Color color)
    {
        element.SetValue(ColorProperty, color);
    }

    public static bool GetIsShowArrow(Control element)
    {
        return element.GetValue(IsShowArrowProperty);
    }

    public static void SetIsShowArrow(Control element, bool flag)
    {
        element.SetValue(IsShowArrowProperty, flag);
    }

    public static bool GetIsPointAtCenter(Control element)
    {
        return element.GetValue(IsPointAtCenterProperty);
    }

    public static void SetIsPointAtCenter(Control element, bool flag)
    {
        element.SetValue(IsPointAtCenterProperty, flag);
    }

    public static PlacementMode GetPlacement(Control element)
    {
        return element.GetValue(PlacementProperty);
    }

    public static void SetPlacement(Control element, PlacementMode value)
    {
        element.SetValue(PlacementProperty, value);
    }

    public static double GetHorizontalOffset(Control element)
    {
        return element.GetValue(HorizontalOffsetProperty);
    }

    public static void SetHorizontalOffset(Control element, double value)
    {
        element.SetValue(HorizontalOffsetProperty, value);
    }

    public static double GetVerticalOffset(Control element)
    {
        return element.GetValue(VerticalOffsetProperty);
    }

    public static void SetVerticalOffset(Control element, double value)
    {
        element.SetValue(VerticalOffsetProperty, value);
    }

    public static double GetMarginToAnchor(Control element)
    {
        return element.GetValue(MarginToAnchorProperty);
    }

    public static void SetMarginToAnchor(Control element, double margin)
    {
        element.SetValue(MarginToAnchorProperty, margin);
    }

    public static int GetShowDelay(Control element)
    {
        return element.GetValue(ShowDelayProperty);
    }

    public static void SetShowDelay(Control element, int value)
    {
        element.SetValue(ShowDelayProperty, value);
    }

    public static int GetBetweenShowDelay(Control element)
    {
        return element.GetValue(BetweenShowDelayProperty);
    }

    public static void SetBetweenShowDelay(Control element, int value)
    {
        element.SetValue(BetweenShowDelayProperty, value);
    }

    public static bool GetShowOnDisabled(Control element)
    {
        return element.GetValue(ShowOnDisabledProperty);
    }

    public static void SetShowOnDisabled(Control element, bool value)
    {
        element.SetValue(ShowOnDisabledProperty, value);
    }

    public static bool GetServiceEnabled(Control element)
    {
        return element.GetValue(ServiceEnabledProperty);
    }

    public static void SetServiceEnabled(Control element, bool value)
    {
        element.SetValue(ServiceEnabledProperty, value);
    }

    public static CustomPopupPlacementCallback? GetCustomPopupPlacementCallback(Control element)
    {
        return element.GetValue(CustomPopupPlacementCallbackProperty);
    }

    public static void SetCustomPopupPlacementCallback(Control element, CustomPopupPlacementCallback? value)
    {
        element.SetValue(CustomPopupPlacementCallbackProperty, value);
    }

    public static bool GetIsCustomHide(Control element)
    {
        return element.GetValue(IsCustomHideProperty);
    }

    public static void SetIsCustomHide(Control element, bool flag)
    {
        element.SetValue(IsCustomHideProperty, flag);
    }

    #endregion

    #region 公共事件定义

    public static readonly RoutedEvent<CancelRoutedEventArgs> ToolTipOpeningEvent =
        RoutedEvent.Register<ToolTip, CancelRoutedEventArgs>("ToolTipOpening", RoutingStrategies.Direct);

    public static readonly RoutedEvent ToolTipClosingEvent =
        RoutedEvent.Register<ToolTip, RoutedEventArgs>("ToolTipClosing", RoutingStrategies.Direct);

    event Action<IPopupHost?>? IPopupHostProvider.PopupHostChanged
    {
        add => _popupHostChangedHandler += value;
        remove => _popupHostChangedHandler -= value;
    }

    public static void AddToolTipOpeningHandler(Control element, EventHandler<CancelRoutedEventArgs> handler)
    {
        element.AddHandler(ToolTipOpeningEvent, handler);
    }

    public static void RemoveToolTipOpeningHandler(Control element, EventHandler<CancelRoutedEventArgs> handler)
    {
        element.RemoveHandler(ToolTipOpeningEvent, handler);
    }

    public static void AddToolTipClosingHandler(Control element, EventHandler<RoutedEventArgs> handler)
    {
        element.AddHandler(ToolTipClosingEvent, handler);
    }

    public static void RemoveToolTipClosingHandler(Control element, EventHandler<RoutedEventArgs> handler)
    {
        element.RemoveHandler(ToolTipClosingEvent, handler);
    }

    #endregion

    #region 内部属性定义

    internal static readonly AttachedProperty<ToolTip?> ToolTipProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, ToolTip?>("ToolTip");

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ToolTipToken.ID;
    Control IAnimationAwareControl.PropertyBindTarget => this;
    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;

    #endregion

    internal Control? AdornedControl { get; private set; }
    internal event EventHandler? Closed;
    internal IPopupHost? PopupHost => _popup?.Host;
    IPopupHost? IPopupHostProvider.PopupHost => _popup?.Host;

    private Popup? _popup;
    private Action<IPopupHost?>? _popupHostChangedHandler;
    private CompositeDisposable? _subscriptions;
    private ArrowDecoratedBox? _arrowDecoratedBox;
    private CompositeDisposable? _tokenBindingsDisposable;

    static ToolTip()
    {
        IsOpenProperty.Changed.Subscribe(HandleIsOpenChanged);
    }

    public ToolTip()
    {
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
    }

    public CornerRadius GetMaskCornerRadius()
    {
        Debug.Assert(_arrowDecoratedBox != null);
        return _arrowDecoratedBox.GetMaskCornerRadius();
    }

    public Rect GetMaskBounds()
    {
        Debug.Assert(_arrowDecoratedBox != null);
        return _arrowDecoratedBox.GetMaskBounds();
    }

    private void Open(Control control)
    {
        Close();

        if (_popup is null)
        {
            _popup = new Popup
            {
                WindowManagerAddShadowHint     = false,
                IsLightDismissEnabled          = false,
                OverlayDismissEventPassThrough = false
            };
            _popup.Child                       = this;
            _popup.TakesFocusFromNativeControl = false;

            _popup.Opened          += HandlePopupOpened;
            _popup.Closed          += HandlePopupClosed;
            _popup.PositionFlipped += HandlePopupPositionFlipped;
        }

        if (_popup is IPopupHostProvider popupHostProvider)
        {
            popupHostProvider.PopupHostChanged += HandlePopupHostChanged;
        }

        // 这里评估是 tooltip 会多次复用，所以这里把绑定管理起来，关闭的时候释放
        // 不知道对不对
        _subscriptions = new CompositeDisposable(new[]
        {
            _popup.Bind(Popup.HorizontalOffsetProperty, control.GetBindingObservable(HorizontalOffsetProperty)),
            _popup.Bind(Popup.VerticalOffsetProperty, control.GetBindingObservable(VerticalOffsetProperty)),
            _popup.Bind(Popup.PlacementProperty, control.GetBindingObservable(PlacementProperty)),
            _popup.Bind(Popup.MarginToAnchorProperty, control.GetBindingObservable(MarginToAnchorProperty)),
            _popup.Bind(Popup.CustomPopupPlacementCallbackProperty,
                control.GetBindingObservable(CustomPopupPlacementCallbackProperty)),
        });

        var placement        = GetPlacement(control);
        var anchorAndGravity = PopupUtils.GetAnchorAndGravity(placement);

        _popup.PlacementTarget  = control;
        _popup.Placement        = placement;
        _popup.PlacementAnchor  = anchorAndGravity.Item1;
        _popup.PlacementGravity = anchorAndGravity.Item2;
        _popup.SetPopupParent(control);

        if (_arrowDecoratedBox is not null)
        {
            SetupArrowDecoratedBox(control);
        }
        else
        {
            TemplateApplied += DeferSetupArrowDecoratedBox;
        }

        _popup.MotionAwareOpen();
    }

    private void DeferSetupArrowDecoratedBox(object? sender, TemplateAppliedEventArgs args)
    {
        TemplateApplied -= DeferSetupArrowDecoratedBox;
        Debug.Assert(_popup != null && _popup.PlacementTarget != null);
        SetupArrowDecoratedBox(_popup.PlacementTarget);
    }

    private void SetupArrowDecoratedBox(Control control)
    {
        if (_arrowDecoratedBox is not null)
        {
            SetToolTipColor(control);
            _subscriptions?.Add(_arrowDecoratedBox.Bind(ArrowDecoratedBox.IsShowArrowProperty,
                control.GetBindingObservable(IsShowArrowProperty, flag =>
                {
                    // 有些条件下是不能开启箭头指针的
                    if (flag && _popup is not null)
                    {
                        return PopupUtils.CanEnabledArrow(_popup.Placement, _popup.PlacementAnchor,
                            _popup.PlacementGravity);
                    }

                    return flag;
                })));
            if (_popup is not null)
            {
                SetupArrowPosition(_popup.Placement, _popup.PlacementAnchor,
                    _popup.PlacementGravity);
            }
        }
    }

    private void SetupArrowPosition(PlacementMode placement, PopupAnchor? anchor = null, PopupGravity? gravity = null)
    {
        var arrowPosition = PopupUtils.CalculateArrowPosition(placement, anchor, gravity);
        if (_arrowDecoratedBox is not null && arrowPosition is not null)
        {
            _arrowDecoratedBox.ArrowPosition = arrowPosition.Value;
        }
    }

    private void HandlePopupPositionFlipped(object? sender, PopupFlippedEventArgs e)
    {
        if (sender is Popup popup)
        {
            SetupArrowPosition(popup.Placement, popup.PlacementAnchor, popup.PlacementGravity);
        }
    }

    private void Close()
    {
        if (AdornedControl is { } adornedControl
            && GetIsOpen(adornedControl))
        {
            var args = new RoutedEventArgs(ToolTipClosingEvent);
            adornedControl.RaiseEvent(args);
        }

        if (_popup is not null)
        {
            _popup.MotionAwareClose(() =>
            {
                _popup.SetPopupParent(null);
                _popup.PlacementTarget = null;
                if (_popup is IPopupHostProvider popupHostProvider)
                {
                    popupHostProvider.PopupHostChanged -= HandlePopupHostChanged;
                }

                _subscriptions?.Dispose();
            });
        }
    }

    private void SetToolTipColor(Control control)
    {
        // Preset 优先级高
        if (_arrowDecoratedBox is not null)
        {
            var presetColorType = GetPresetColor(control);
            var color           = GetColor(control);
            if (presetColorType is not null)
            {
                var presetColor = PresetPrimaryColor.GetColor(presetColorType.Value);
                _arrowDecoratedBox.Background = new SolidColorBrush(presetColor.Color());
                InvalidateVisual();
            }
            else if (color is not null)
            {
                _arrowDecoratedBox.Background = new SolidColorBrush(color.Value);
                InvalidateVisual();
            }
        }
    }

    private static void HandleIsOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var control  = (Control)e.Sender;
        var newValue = (bool)e.NewValue!;

        if (newValue)
        {
            var args = new CancelRoutedEventArgs(ToolTipOpeningEvent);
            control.RaiseEvent(args);
            if (args.Cancel)
            {
                control.SetCurrentValue(IsOpenProperty, false);
                return;
            }

            var tip = GetTip(control);
            if (tip == null)
            {
                control.SetCurrentValue(IsOpenProperty, false);
                return;
            }

            var toolTip = control.GetValue(ToolTipProperty);
            if (toolTip == null || (tip != toolTip && tip != toolTip.Content))
            {
                toolTip?.Close();

                toolTip = tip as ToolTip ?? new ToolTip { Content = tip };
                control.SetValue(ToolTipProperty, toolTip);
            }

            toolTip.AdornedControl = control;
            toolTip.Open(control);
        }
        else if (control.GetValue(ToolTipProperty) is { } toolTip)
        {
            toolTip.AdornedControl = null;
            toolTip.Close();
        }
    }

    private void HandlePopupHostChanged(IPopupHost? host)
    {
        if (_popup is not null)
        {
            var control = _popup.PlacementTarget;
            if (control is not null)
            {
                // 如果指定这个，那么用户指定的 offset 失效
                var offset = CalculatePopupPositionDelta(control, _popup.Placement, _popup.PlacementAnchor,
                    _popup.PlacementGravity);
                control.SetValue(HorizontalOffsetProperty, offset.X);
                control.SetValue(VerticalOffsetProperty, offset.Y);
            }
        }
    }

    private void HandlePopupClosed(object? sender, EventArgs e)
    {
        // This condition is true, when Popup was closed by any other reason outside of ToolTipService/ToolTip, keeping IsOpen=true.
        if (AdornedControl is { } adornedControl
            && GetIsOpen(adornedControl))
        {
            adornedControl.SetCurrentValue(IsOpenProperty, false);
        }

        _popupHostChangedHandler?.Invoke(null);
        UpdatePseudoClasses(false);
        Closed?.Invoke(this, EventArgs.Empty);
    }

    private void HandlePopupOpened(object? sender, EventArgs e)
    {
        _popupHostChangedHandler?.Invoke(((Popup)sender!).Host);
        UpdatePseudoClasses(true);
    }

    private void UpdatePseudoClasses(bool newValue)
    {
        PseudoClasses.Set(StdPseudoClass.Open, newValue);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        this.RunThemeTokenBindingActions();
        _arrowDecoratedBox = e.NameScope.Find<ArrowDecoratedBox>(ToolTipTheme.ToolTipContainerPart);
    }

    private Point CalculatePopupPositionDelta(Control control,
                                              PlacementMode placement,
                                              PopupAnchor? anchor = null,
                                              PopupGravity? gravity = null)
    {
        var offsetX         = 0d;
        var offsetY         = 0d;
        var isShowArrow     = GetIsShowArrow(control);
        var isPointAtCenter = GetIsPointAtCenter(control);
        if (isShowArrow && isPointAtCenter && _arrowDecoratedBox is not null)
        {
            if (PopupUtils.CanEnabledArrow(placement, anchor, gravity))
            {
                var arrowVertexPoint = _arrowDecoratedBox.ArrowVertexPoint;

                var anchorSize = control.Bounds.Size;
                var centerX    = anchorSize.Width / 2;
                var centerY    = anchorSize.Height / 2;
                // 这里计算不需要全局坐标
                if (placement == PlacementMode.TopEdgeAlignedLeft ||
                    placement == PlacementMode.BottomEdgeAlignedLeft)
                {
                    offsetX += centerX - arrowVertexPoint.Item1;
                }
                else if (placement == PlacementMode.TopEdgeAlignedRight ||
                         placement == PlacementMode.BottomEdgeAlignedRight)
                {
                    offsetX -= centerX - arrowVertexPoint.Item2;
                }
                else if (placement == PlacementMode.RightEdgeAlignedTop ||
                         placement == PlacementMode.LeftEdgeAlignedTop)
                {
                    offsetY += centerY - arrowVertexPoint.Item1;
                }
                else if (placement == PlacementMode.RightEdgeAlignedBottom ||
                         placement == PlacementMode.LeftEdgeAlignedBottom)
                {
                    offsetY -= centerY - arrowVertexPoint.Item2;
                }
            }
        }

        return new Point(offsetX, offsetY);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingsDisposable = new CompositeDisposable();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
}