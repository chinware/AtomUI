using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Theme.Palette;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[PseudoClasses(":open")]
public class ToolTip : ContentControl,
                       IMotionAwareControl,
                       IArrowAwareShadowMaskInfoProvider
{
    #region 附加属性定义

    public static readonly AttachedProperty<object?> TipProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, object?>("Tip");
    
    public static readonly AttachedProperty<double> TipHostWidthProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, double>("TipHostWidth", double.NaN);

    public static readonly AttachedProperty<PresetColorType?> PresetColorProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, PresetColorType?>("PresetColor");
    
    public static readonly AttachedProperty<Color?> ColorProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, Color?>("Color");
    
    public static readonly AttachedProperty<bool> IsUseOverlayHostProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("IsUseOverlayHost");
    
    public static readonly AttachedProperty<bool> IsShowArrowProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("IsShowArrow", true);
    
    public static readonly AttachedProperty<bool> IsPointAtCenterProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("IsPointAtCenter");
    
    public static readonly AttachedProperty<bool> IsOpenProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("IsOpen");

    public static readonly AttachedProperty<PlacementMode> PlacementProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, PlacementMode>("Placement",
            defaultValue: PlacementMode.Top);

    public static readonly AttachedProperty<double> HorizontalOffsetProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, double>("HorizontalOffset");

    public static readonly AttachedProperty<double> VerticalOffsetProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, double>("VerticalOffset");
    
    public static readonly AttachedProperty<int> ShowDelayProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, int>("ShowDelay", 400);

    public static readonly AttachedProperty<int> BetweenShowDelayProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, int>("BetweenShowDelay", 100);

    public static readonly AttachedProperty<bool> ShowOnDisabledProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("ShowOnDisabled", defaultValue: false,
            inherits: true);

    public static readonly AttachedProperty<bool> ServiceEnabledProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("ServiceEnabled", defaultValue: true, inherits: true);
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ToolTip>();
    
    public static readonly AttachedProperty<double> MarginToAnchorProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, double>("MarginToAnchor", 4);
    
    public static readonly AttachedProperty<bool> IsCustomShowAndHideProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("IsCustomShowAndHide");
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    #endregion

    #region 路由事件定义

    public static readonly RoutedEvent<CancelRoutedEventArgs> ToolTipOpeningEvent =
        RoutedEvent.Register<ToolTip, CancelRoutedEventArgs>("ToolTipOpening", RoutingStrategies.Direct);

    public static readonly RoutedEvent ToolTipClosingEvent =
        RoutedEvent.Register<ToolTip, RoutedEventArgs>("ToolTipClosing", RoutingStrategies.Direct);

    #endregion

    #region 内部属性定义

    internal static readonly AttachedProperty<ToolTip?> ToolTipProperty =
        AvaloniaProperty.RegisterAttached<ToolTip, Control, ToolTip?>("ToolTip");
    
    internal static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<ToolTip>();
    
    internal static readonly StyledProperty<bool> ShouldUseOverlayPopupProperty =
        Flyout.ShouldUseOverlayPopupProperty.AddOwner<ToolTip>();
    
    internal TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }
    
    internal bool ShouldUseOverlayPopup
    {
        get => GetValue(ShouldUseOverlayPopupProperty);
        set => SetValue(ShouldUseOverlayPopupProperty, value);
    }
    
    #endregion

    private Popup? _popup;
    private CompositeDisposable? _subscriptions;
    private ArrowDecoratedBox? _arrowDecoratedBox;
    private ContentPresenter? _contentPresenter;
    internal Control? AdornedControl { get; private set; }
    internal event EventHandler? Closed;

    static ToolTip()
    {
        IsOpenProperty.Changed.Subscribe(IsOpenChanged);
    }

    #region 附加属性访问器

    public static object? GetTip(Control element)
    {
        return element.GetValue(TipProperty);
    }

    public static void SetTip(Control element, object? value)
    {
        element.SetValue(TipProperty, value);
    }

    public static double GetTipHostWidth(Control element)
    {
        return element.GetValue(TipHostWidthProperty);
    }

    public static void SetTipHostWidth(Control element, double value)
    {
        element.SetValue(TipHostWidthProperty, value);
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

    public static void SetColor(Control element, Color? color)
    {
        element.SetValue(ColorProperty, color);
    }
        
    public static bool GetIsUseOverlayHost(Control element)
    {
        return element.GetValue(IsUseOverlayHostProperty);
    }

    public static void SetIsUseOverlayHost(Control element, bool value)
    {
        element.SetValue(IsUseOverlayHostProperty, value);
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
    
    public static bool GetIsOpen(Control element)
    {
        return element.GetValue(IsOpenProperty);
    }

    public static void SetIsOpen(Control element, bool value)
    {
        element.SetValue(IsOpenProperty, value);
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

    public static double GetMarginToAnchor(Control element)
    {
        return element.GetValue(MarginToAnchorProperty);
    }

    public static void SetMarginToAnchor(Control element, double margin)
    {
        element.SetValue(MarginToAnchorProperty, margin);
    }
    
    public static bool GetIsCustomShowAndHide(Control element)
    {
        return element.GetValue(IsCustomShowAndHideProperty);
    }

    public static void SetIsCustomShowAndHide(Control element, bool flag)
    {
        element.SetValue(IsCustomShowAndHideProperty, flag);
    }

    #endregion

    #region 路由事件访问器

    public static void AddToolTipOpeningHandler(Control element, EventHandler<CancelRoutedEventArgs> handler) =>
        element.AddHandler(ToolTipOpeningEvent, handler);

    public static void RemoveToolTipOpeningHandler(Control element, EventHandler<CancelRoutedEventArgs> handler) =>
        element.RemoveHandler(ToolTipOpeningEvent, handler);

    public static void AddToolTipClosingHandler(Control element, EventHandler<RoutedEventArgs> handler) =>
        element.AddHandler(ToolTipClosingEvent, handler);

    public static void RemoveToolTipClosingHandler(Control element, EventHandler<RoutedEventArgs> handler) =>
        element.RemoveHandler(ToolTipClosingEvent, handler);

    #endregion

    private static void IsOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var control = (Control)e.Sender;
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
                toolTip = tip as ToolTip ?? new ToolTip() { Content = tip };
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

    private void Open(Control control)
    {
        Close();
        
        if (_popup is null)
        {
            _popup = new Popup();
            _popup.Child = this;
            _popup.TakesFocusFromNativeControl = false;
            _popup.WindowManagerAddShadowHint = false;

            _popup.Opened                       += HandlePopupOpened;
            _popup.Closed                       += HandlePopupClosed;
            _popup.PositionFlipped              += HandlePositionFlipped;
        }

        _subscriptions = new CompositeDisposable([
            Bind(ShouldUseOverlayPopupProperty, control.GetBindingObservable(IsUseOverlayHostProperty)),
            _popup.Bind(Popup.ShouldUseOverlayLayerProperty, this.GetObservable(ShouldUseOverlayPopupProperty)),
            _popup.Bind(Popup.MotionDurationProperty, this.GetObservable(MotionDurationProperty)),
            _popup.Bind(Popup.IsMotionEnabledProperty, this.GetObservable(IsMotionEnabledProperty)),
            _popup.Bind(Popup.HorizontalOffsetProperty, control.GetBindingObservable(HorizontalOffsetProperty)),
            _popup.Bind(Popup.VerticalOffsetProperty, control.GetBindingObservable(VerticalOffsetProperty)),
            _popup.Bind(Popup.RequestedPlacementProperty, control.GetBindingObservable(PlacementProperty, v => (PlacementMode?)v)),
            _popup.Bind(Popup.MarginToAnchorProperty, control.GetBindingObservable(MarginToAnchorProperty)),
            _popup.Bind(Popup.IsPointAtCenterProperty, control.GetBindingObservable(IsPointAtCenterProperty)),
        ]);

        _popup.PlacementTarget = control;
        _popup.SetPopupParent(control);
        
        if (_arrowDecoratedBox is not null)
        {
            SetupArrowDecoratedBox(control);
        }
        else
        {
            TemplateApplied += DeferSetupArrowDecoratedBox;
        }

        ConfigureMotion(_popup, GetPlacement(control));
        _popup.IsOpen = true;
    }

    private void ConfigureMotion(Popup popup, PlacementMode placement)
    {
        (popup.OpenMotion, popup.CloseMotion) = PopupUtils.CreateMotionForPlacement(placement);
    }

    private void Close()
    {
        if (AdornedControl is { } adornedControl
            && GetIsOpen(adornedControl))
        {
            var args = new RoutedEventArgs(ToolTipClosingEvent);
            adornedControl.RaiseEvent(args);
        }
        
        if (_popup != null)
        {
            _popup.IsOpen = false;
        }
        else
        {
            _subscriptions?.Dispose();
        }
    }

    private void HandlePopupClosed(object? sender, EventArgs e)
    {
        if (AdornedControl is { } adornedControl
            && GetIsOpen(adornedControl))
        {
            adornedControl.SetCurrentValue(IsOpenProperty, false);
        }

        UpdatePseudoClasses(false);
        Closed?.Invoke(this, EventArgs.Empty);
        if (sender is Popup popup)
        {
            popup.SetPopupParent(null);
            popup.PlacementTarget = null;
            _subscriptions?.Dispose();
            _subscriptions = null;
        }
    }

    private void HandlePositionFlipped(object? sender, PopupFlippedEventArgs args)
    {
        if (sender is Popup popup && popup.PlacementTarget != null)
        {
            SetupArrowPosition(GetPlacement(popup.PlacementTarget), args.Flipped);
        }
    }

    private void HandlePopupOpened(object? sender, EventArgs e)
    {
        UpdatePseudoClasses(true);
    }

    private void UpdatePseudoClasses(bool newValue)
    {
        PseudoClasses.Set(":open", newValue);
    }

    CornerRadius IShadowMaskInfoProvider.GetMaskCornerRadius()
    {
        Debug.Assert(_arrowDecoratedBox != null);
        return _arrowDecoratedBox.GetMaskCornerRadius();
    }
    
    Rect IShadowMaskInfoProvider.GetMaskBounds()
    {
        Debug.Assert(_arrowDecoratedBox != null);
        return _arrowDecoratedBox.GetMaskBounds();
    }

    IBrush? IShadowMaskInfoProvider.GetMaskBackground()
    {
        return Background;
    }
    
    ArrowPosition IArrowAwareShadowMaskInfoProvider.GetArrowPosition()
    {
        Debug.Assert(_arrowDecoratedBox != null);
        return _arrowDecoratedBox.ArrowPosition;
    }
    
    bool IArrowAwareShadowMaskInfoProvider.IsShowArrow()
    {
        Debug.Assert(_arrowDecoratedBox != null);
        return _arrowDecoratedBox.IsShowArrow;
    }

    void IArrowAwareShadowMaskInfoProvider.SetArrowOpacity(double opacity)
    {
        Debug.Assert(_arrowDecoratedBox != null);
        _arrowDecoratedBox.ArrowOpacity = opacity;
    }

    Rect IArrowAwareShadowMaskInfoProvider.GetArrowIndicatorBounds()
    {
        Debug.Assert(_arrowDecoratedBox != null);
        return _arrowDecoratedBox.ArrowIndicatorBounds;
    }
    
    Rect IArrowAwareShadowMaskInfoProvider.GetArrowIndicatorLayoutBounds()
    {
        Debug.Assert(_arrowDecoratedBox != null);
        return _arrowDecoratedBox.ArrowIndicatorLayoutBounds;
    }
    
    AbstractArrowDecoratedBox IArrowAwareShadowMaskInfoProvider.GetArrowDecoratedBox()
    {
        Debug.Assert(_arrowDecoratedBox != null);
        return _arrowDecoratedBox;
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _arrowDecoratedBox = e.NameScope.Find<ArrowDecoratedBox>(ArrowDecoratedBox.ArrowDecoratorPart);
        _contentPresenter  = e.NameScope.Find<ContentPresenter>("PART_ContentPresenter");
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
            if (_contentPresenter != null)
            {
                _contentPresenter.Width = GetTipHostWidth(control);
            }
            
            _arrowDecoratedBox.Bind(ArrowDecoratedBox.IsShowArrowProperty,
                control.GetBindingObservable(IsShowArrowProperty, flag =>
                {
                    // 有些条件下是不能开启箭头指针的
                    if (flag && _popup is not null)
                    {
                        return PopupUtils.CanEnabledArrow(GetPlacement(control));
                    }

                    return flag;
                }));
            if (_popup is not null)
            {
                SetupArrowPosition(GetPlacement(control), false);
            }
        }
    }
    
    private void SetupArrowPosition(PlacementMode placement, bool isFlipped, PopupAnchor? anchor = null, PopupGravity? gravity = null)
    {
        var arrowPosition = PopupUtils.CalculateArrowPosition(placement, anchor, gravity);
        if (_arrowDecoratedBox is not null && arrowPosition is not null)
        {
            _arrowDecoratedBox.ArrowPosition = isFlipped ? ArrowPositionUtils.FlipArrowPosition(arrowPosition.Value) : arrowPosition.Value;
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
                Background = new SolidColorBrush(presetColor.Color());
                InvalidateVisual();
            }
            else if (color is not null)
            {
                Background = new SolidColorBrush(color.Value);
                InvalidateVisual();
            }
        }
    }
}
