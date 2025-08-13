using System.Diagnostics;
using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls.Primitives;

[PseudoClasses(FlyoutOpenPC)]
public abstract class InfoPickerInput : TemplatedControl,
                                        IMotionAwareControl,
                                        IControlSharedTokenResourcesHost
{
    internal const string FlyoutOpenPC = ":flyout-open";
    internal const string ChoosingPC = ":choosing";

    #region 公共属性定义

    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AvaloniaProperty.Register<InfoPickerInput, object?>(nameof(LeftAddOn));

    public static readonly StyledProperty<object?> RightAddOnProperty =
        AvaloniaProperty.Register<InfoPickerInput, object?>(nameof(RightAddOn));

    public static readonly StyledProperty<object?> InnerLeftContentProperty
        = AvaloniaProperty.Register<InfoPickerInput, object?>(nameof(InnerLeftContent));

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
        AddOnDecoratedBox.StyleVariantProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
        AddOnDecoratedBox.StatusProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<string?> WatermarkProperty =
        AvaloniaProperty.Register<InfoPickerInput, string?>(nameof(Watermark));
    
    public static readonly StyledProperty<Icon?> InfoIconProperty =
        AvaloniaProperty.Register<InfoPickerInput, Icon?>(nameof(InfoIcon));

    public static readonly StyledProperty<PlacementMode> PickerPlacementProperty =
        AvaloniaProperty.Register<InfoPickerInput, PlacementMode>(nameof(PickerPlacement),
            PlacementMode.BottomEdgeAlignedLeft);

    public static readonly StyledProperty<bool> IsShowArrowProperty =
        ArrowDecoratedBox.IsShowArrowProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<bool> IsPointAtCenterProperty =
        Flyout.IsPointAtCenterProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<double> MarginToAnchorProperty =
        Popup.MarginToAnchorProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<int> MouseEnterDelayProperty =
        FlyoutStateHelper.MouseEnterDelayProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<int> MouseLeaveDelayProperty =
        FlyoutStateHelper.MouseLeaveDelayProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        TextBox.IsReadOnlyProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<IBrush?> InputTextBrushProperty =
        AvaloniaProperty.Register<InfoPickerInput, IBrush?>(nameof(InputTextBrush));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<InfoPickerInput>();

    public object? LeftAddOn
    {
        get => GetValue(LeftAddOnProperty);
        set => SetValue(LeftAddOnProperty, value);
    }

    public object? RightAddOn
    {
        get => GetValue(RightAddOnProperty);
        set => SetValue(RightAddOnProperty, value);
    }

    public object? InnerLeftContent
    {
        get => GetValue(InnerLeftContentProperty);
        set => SetValue(InnerLeftContentProperty, value);
    }

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public AddOnDecoratedVariant StyleVariant
    {
        get => GetValue(StyleVariantProperty);
        set => SetValue(StyleVariantProperty, value);
    }

    public AddOnDecoratedStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public string? Watermark
    {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }
    
    public Icon? InfoIcon
    {
        get => GetValue(InfoIconProperty);
        set => SetValue(InfoIconProperty, value);
    }

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

    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public IBrush? InputTextBrush
    {
        get => GetValue(InputTextBrushProperty);
        set => SetValue(InputTextBrushProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<InfoPickerInput, string?>(nameof(Text));

    protected string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    internal static readonly DirectProperty<InfoPickerInput, double> PreferredInputWidthProperty
        = AvaloniaProperty.RegisterDirect<InfoPickerInput, double>(nameof(PreferredInputWidth),
            o => o.PreferredInputWidth,
            (o, v) => o.PreferredInputWidth = v);

    private double _preferredInputWidth = double.NaN;

    internal double PreferredInputWidth
    {
        get => _preferredInputWidth;
        set => SetAndRaise(PreferredInputWidthProperty, ref _preferredInputWidth, value);
    }

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => InfoPickerInputToken.ID;
    Control IMotionAwareControl.PropertyBindTarget => this;

    #endregion

    private protected AddOnDecoratedBox? _decoratedBox;
    private protected PickerClearUpButton? _pickerClearUpButton;
    private protected readonly FlyoutStateHelper _flyoutStateHelper;
    private protected Flyout? _pickerFlyout;
    protected bool _currentValidSelected;
    private IDisposable? _clearUpButtonDetectDisposable;
    protected AddOnDecoratedInnerBox? _pickerInnerBox;
    protected TextBox? _infoInputBox;

    private protected bool _isFlyoutOpen;
    private protected bool _isChoosing;

    static InfoPickerInput()
    {
        AffectsMeasure<InfoPickerInput>(PreferredInputWidthProperty, SizeTypeProperty);
    }

    public InfoPickerInput()
    {
        this.RegisterResources();
        _flyoutStateHelper = new FlyoutStateHelper
        {
            TriggerType = FlyoutTriggerType.Click
        };
        _flyoutStateHelper.FlyoutAboutToShow        += HandleFlyoutAboutToShow;
        _flyoutStateHelper.FlyoutAboutToClose       += HandleFlyoutAboutToClose;
        _flyoutStateHelper.FlyoutOpened             += HandleFlyoutOpened;
        _flyoutStateHelper.FlyoutClosed             += HandleFlyoutClosed;
        _flyoutStateHelper.OpenFlyoutPredicate      =  FlyoutOpenPredicate;
        _flyoutStateHelper.ClickHideFlyoutPredicate =  ClickHideFlyoutPredicate;
    }

    public virtual void Clear()
    {
        _infoInputBox?.Clear();
    }

    public void ClosePickerFlyout()
    {
        _flyoutStateHelper.HideFlyout(true);
    }

    private void HandleFlyoutAboutToShow(object? sender, EventArgs args)
    {
        _currentValidSelected = false;
        NotifyFlyoutAboutToShow();
    }

    protected virtual void NotifyFlyoutAboutToShow()
    {
    }

    private void HandleFlyoutAboutToClose(object? sender, EventArgs args)
    {
        NotifyFlyoutAboutToClose(_currentValidSelected);
    }

    protected virtual void NotifyFlyoutAboutToClose(bool selectedIsValid)
    {
    }

    private void HandleFlyoutOpened(object? sender, EventArgs args)
    {
        NotifyFlyoutOpened();
    }

    protected virtual void NotifyFlyoutOpened()
    {
    }

    private void HandleFlyoutClosed(object? sender, EventArgs args)
    {
        NotifyFlyoutClosed();
    }

    protected virtual void NotifyFlyoutClosed()
    {
    }

    protected virtual bool ClickHideFlyoutPredicate(IPopupHostProvider hostProvider, RawPointerEventArgs args)
    {
        if (hostProvider.PopupHost != args.Root)
        {
            if (!IsPointerInInfoInputBox(args.Position))
            {
                return true;
            }
        }

        return false;
    }

    protected virtual bool FlyoutOpenPredicate(Point position)
    {
        if (!IsEnabled)
        {
            return false;
        }

        return IsPointerInInfoInputBox(position);
    }

    private bool IsPointerInInfoInputBox(Point position)
    {
        if (_pickerInnerBox is not null)
        {
            var pos = _pickerInnerBox.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
            if (!pos.HasValue)
            {
                return false;
            }

            var targetWidth  = _pickerInnerBox.Bounds.Width;
            var targetHeight = _pickerInnerBox.Bounds.Height;
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

            if (_pickerClearUpButton is Control rightContent)
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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_pickerFlyout is null)
        {
            _pickerFlyout = CreatePickerFlyout();
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
            _pickerFlyout.PresenterCreated += (sender, args) => { NotifyFlyoutPresenterCreated(args.Presenter); };
            _flyoutStateHelper.Flyout      =  _pickerFlyout;
        }

        _decoratedBox        = e.NameScope.Get<AddOnDecoratedBox>(InfoPickerInputTheme.DecoratedBoxPart);
        _pickerInnerBox      = e.NameScope.Get<AddOnDecoratedInnerBox>(InfoPickerInputTheme.PickerInnerPart);
        _infoInputBox        = e.NameScope.Get<TextBox>(InfoPickerInputTheme.InfoInputBoxPart);
        _pickerClearUpButton = e.NameScope.Get<PickerClearUpButton>(InfoPickerInputTheme.ClearUpButtonPart);

        if (_pickerClearUpButton is not null)
        {
            _pickerClearUpButton.ClearRequest += (sender, args) => { NotifyClearButtonClicked(); };
        }

        _flyoutStateHelper.AnchorTarget = _pickerInnerBox;
        SetupFlyoutProperties();
    }

    protected virtual void NotifyClearButtonClicked()
    {
        Clear();
    }

    protected virtual void NotifyFlyoutPresenterCreated(Control flyoutPresenter)
    {
    }

    protected virtual void SetupFlyoutProperties()
    {
        if (_pickerFlyout is not null)
        {
            BindUtils.RelayBind(this, PickerPlacementProperty, _pickerFlyout, PopupFlyoutBase.PlacementProperty);
            BindUtils.RelayBind(this, IsShowArrowProperty, _pickerFlyout);
            BindUtils.RelayBind(this, IsPointAtCenterProperty, _pickerFlyout);
            BindUtils.RelayBind(this, MarginToAnchorProperty, _pickerFlyout);
            BindUtils.RelayBind(this, IsMotionEnabledProperty, _pickerFlyout, Flyout.IsMotionEnabledProperty);
        }
    }

    protected abstract Flyout CreatePickerFlyout();

    protected virtual void UpdatePseudoClasses()
    {
        PseudoClasses.Set(FlyoutOpenPC, _isFlyoutOpen);
        PseudoClasses.Set(ChoosingPC, _isChoosing);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        BindUtils.RelayBind(this, MouseEnterDelayProperty, _flyoutStateHelper,
            FlyoutStateHelper.MouseEnterDelayProperty);
        BindUtils.RelayBind(this, MouseLeaveDelayProperty, _flyoutStateHelper,
            FlyoutStateHelper.MouseLeaveDelayProperty);
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

    private void DetectClearUpButtonState(RawInputEventArgs args)
    {
        if (IsEnabled)
        {
            if (args is RawPointerEventArgs pointerEventArgs)
            {
                if (_pickerInnerBox is not null && _pickerClearUpButton != null)
                {
                    var topLevel = TopLevel.GetTopLevel(this);
                    Debug.Assert(topLevel is not null);
                    if (topLevel != pointerEventArgs.Root)
                    {
                        _pickerClearUpButton!.IsInClearMode = false;
                    }
                    else
                    {
                        var pos      = _pickerInnerBox.TranslatePoint(new Point(0, 0), topLevel);
                        if (!pos.HasValue)
                        {
                            return;
                        }
                        var bounds = new Rect(pos.Value, _pickerInnerBox.Bounds.Size);
                        if (bounds.Contains(pointerEventArgs.Position))
                        {
                            _pickerClearUpButton.IsInClearMode = ShowClearButtonPredicate();
                        }
                        else
                        {
                            _pickerClearUpButton.IsInClearMode = false;
                        }
                    }
                }
            }
        }
    }

    protected virtual bool ShowClearButtonPredicate()
    {
        return false;
    }
}