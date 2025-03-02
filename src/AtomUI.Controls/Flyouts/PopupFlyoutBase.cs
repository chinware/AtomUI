using System.ComponentModel;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Layout;
using Avalonia.Logging;

namespace AtomUI.Controls;

public class FlyoutPresenterCreatedEventArgs : EventArgs
{
    public Control Presenter { get; }

    public FlyoutPresenterCreatedEventArgs(Control presenter)
    {
        Presenter = presenter;
    }
}

/// <summary>
/// 最基本得弹窗 Flyout，在这里不处理那种带箭头得
/// </summary>
public abstract class PopupFlyoutBase : FlyoutBase, 
                                        IPopupHostProvider
{
    #region 公共属性定义

    /// <summary>
    /// 距离 anchor 的边距，根据垂直和水平进行设置
    /// 但是对某些组合无效，比如跟随鼠标的情况
    /// 还有些 anchor 和 gravity 的组合也没有用
    /// </summary>
    public static readonly StyledProperty<double> MarginToAnchorProperty =
        Popup.MarginToAnchorProperty.AddOwner<PopupFlyoutBase>();

    /// <inheritdoc cref="Popup.PlacementProperty" />
    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        Avalonia.Controls.Primitives.Popup.PlacementProperty.AddOwner<PopupFlyoutBase>();

    /// <inheritdoc cref="Popup.HorizontalOffsetProperty" />
    public static readonly StyledProperty<double> HorizontalOffsetProperty =
        Avalonia.Controls.Primitives.Popup.HorizontalOffsetProperty.AddOwner<PopupFlyoutBase>();

    /// <inheritdoc cref="Popup.VerticalOffsetProperty" />
    public static readonly StyledProperty<double> VerticalOffsetProperty =
        Avalonia.Controls.Primitives.Popup.VerticalOffsetProperty.AddOwner<PopupFlyoutBase>();

    /// <inheritdoc cref="Popup.PlacementAnchorProperty" />
    public static readonly StyledProperty<PopupAnchor> PlacementAnchorProperty =
        Avalonia.Controls.Primitives.Popup.PlacementAnchorProperty.AddOwner<PopupFlyoutBase>();

    /// <inheritdoc cref="Popup.PlacementAnchorProperty" />
    public static readonly StyledProperty<PopupGravity> PlacementGravityProperty =
        Avalonia.Controls.Primitives.Popup.PlacementGravityProperty.AddOwner<PopupFlyoutBase>();

    /// <summary>
    /// Defines the <see cref="ShowMode" /> property
    /// </summary>
    public static readonly StyledProperty<FlyoutShowMode> ShowModeProperty =
        AvaloniaProperty.Register<PopupFlyoutBase, FlyoutShowMode>(nameof(ShowMode));

    /// <summary>
    /// Defines the <see cref="OverlayInputPassThroughElement" /> property
    /// </summary>
    public static readonly StyledProperty<IInputElement?> OverlayInputPassThroughElementProperty =
        Avalonia.Controls.Primitives.Popup.OverlayInputPassThroughElementProperty.AddOwner<PopupFlyoutBase>();

    /// <summary>
    /// Defines the <see cref="PlacementConstraintAdjustment" /> property
    /// </summary>
    public static readonly StyledProperty<PopupPositionerConstraintAdjustment> PlacementConstraintAdjustmentProperty =
        Avalonia.Controls.Primitives.Popup.PlacementConstraintAdjustmentProperty.AddOwner<PopupFlyoutBase>();

    public double MarginToAnchor
    {
        get => GetValue(MarginToAnchorProperty);
        set => SetValue(MarginToAnchorProperty, value);
    }

    /// <inheritdoc cref="Popup.Placement" />
    public PlacementMode Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    /// <inheritdoc cref="Popup.PlacementGravity" />
    public PopupGravity PlacementGravity
    {
        get => GetValue(PlacementGravityProperty);
        set => SetValue(PlacementGravityProperty, value);
    }

    /// <inheritdoc cref="Popup.PlacementAnchor" />
    public PopupAnchor PlacementAnchor
    {
        get => GetValue(PlacementAnchorProperty);
        set => SetValue(PlacementAnchorProperty, value);
    }

    /// <inheritdoc cref="Popup.HorizontalOffset" />
    public double HorizontalOffset
    {
        get => GetValue(HorizontalOffsetProperty);
        set => SetValue(HorizontalOffsetProperty, value);
    }

    /// <inheritdoc cref="Popup.VerticalOffset" />
    public double VerticalOffset
    {
        get => GetValue(VerticalOffsetProperty);
        set => SetValue(VerticalOffsetProperty, value);
    }

    /// <summary>
    /// Gets or sets the desired ShowMode
    /// </summary>
    public FlyoutShowMode ShowMode
    {
        get => GetValue(ShowModeProperty);
        set => SetValue(ShowModeProperty, value);
    }

    /// <summary>
    /// Gets or sets an element that should receive pointer input events even when underneath
    /// the flyout's overlay.
    /// </summary>
    public IInputElement? OverlayInputPassThroughElement
    {
        get => GetValue(OverlayInputPassThroughElementProperty);
        set => SetValue(OverlayInputPassThroughElementProperty, value);
    }

    /// <inheritdoc cref="Popup.PlacementConstraintAdjustment" />
    public PopupPositionerConstraintAdjustment PlacementConstraintAdjustment
    {
        get => GetValue(PlacementConstraintAdjustmentProperty);
        set => SetValue(PlacementConstraintAdjustmentProperty, value);
    }
    
    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsDetectMouseClickEnabledProperty =
        AvaloniaProperty.Register<PopupFlyoutBase, bool>(nameof(IsDetectMouseClickEnabled), true);

    internal static readonly DirectProperty<PopupFlyoutBase, bool> IsMotionEnabledProperty
        = AvaloniaProperty.RegisterDirect<PopupFlyoutBase, bool>(nameof(IsMotionEnabled),
            o => o.IsMotionEnabled,
            (o, v) => o.IsMotionEnabled = v);
    
    internal bool IsDetectMouseClickEnabled
    {
        get => GetValue(IsDetectMouseClickEnabledProperty);
        set => SetValue(IsDetectMouseClickEnabledProperty, value);
    }
    
    private bool _isMotionEnabled;

    internal bool IsMotionEnabled
    {
        get => _isMotionEnabled;
        set => SetAndRaise(IsMotionEnabledProperty, ref _isMotionEnabled, value);
    }

    #endregion

    #region 公共事件定义

    event Action<IPopupHost?>? IPopupHostProvider.PopupHostChanged
    {
        add => _popupHostChangedHandler += value;
        remove => _popupHostChangedHandler -= value;
    }

    public event EventHandler<CancelEventArgs>? Closing;
    public event EventHandler? Opening;
    public event EventHandler<FlyoutPresenterCreatedEventArgs>? PresenterCreated;

    #endregion

    IPopupHost? IPopupHostProvider.PopupHost => Popup.Host;
    protected Popup Popup => _popupLazy.Value;

    private readonly Lazy<Popup> _popupLazy;
    private Rect? _enlargedPopupRect;
    private PixelRect? _enlargePopupRectScreenPixelRect;
    private IDisposable? _transientDisposable;
    private Action<IPopupHost?>? _popupHostChangedHandler;

    static PopupFlyoutBase()
    {
        Control.ContextFlyoutProperty.Changed.Subscribe(OnContextFlyoutPropertyChanged);
    }

    public PopupFlyoutBase()
    {
        _popupLazy = new Lazy<Popup>(() => CreatePopup());
    }

    protected internal virtual void NotifyPopupCreated(Popup popup)
    {
        BindUtils.RelayBind(this, MarginToAnchorProperty, popup);
        BindUtils.RelayBind(this, IsDetectMouseClickEnabledProperty, popup, Popup.IsDetectMouseClickEnabledProperty);
        BindUtils.RelayBind(this, IsMotionEnabledProperty, popup, Popup.IsMotionEnabledProperty);
    }

    protected internal virtual void NotifyPositionPopup(bool showAtPointer)
    {
        Size sz;
        // Popup.Child can't be null here, it was set in ShowAtCore.
        if (Popup.Child!.DesiredSize == default)
        {
            // Popup may not have been shown yet. Measure content
            sz = LayoutHelper.MeasureChild(Popup.Child, Size.Infinity, new Thickness());
        }
        else
        {
            sz = Popup.Child.DesiredSize;
        }

        Popup.VerticalOffset   = VerticalOffset;
        Popup.HorizontalOffset = HorizontalOffset;

        Popup.PlacementAnchor  = PlacementAnchor;
        Popup.PlacementGravity = PlacementGravity;

        if (showAtPointer)
        {
            Popup.Placement = PlacementMode.Pointer;
        }
        else
        {
            Popup.Placement                     = Placement;
            Popup.PlacementConstraintAdjustment = PlacementConstraintAdjustment;
        }
    }

    /// <summary>
    /// Shows the Flyout at the given Control
    /// </summary>
    /// <param name="placementTarget">The control to show the Flyout at</param>
    public sealed override void ShowAt(Control placementTarget)
    {
        ShowAtCore(placementTarget);
    }

    /// <summary>
    /// Shows the Flyout for the given control at the current pointer location, as in a ContextFlyout
    /// </summary>
    /// <param name="placementTarget">The target control</param>
    /// <param name="showAtPointer">True to show at pointer</param>
    public void ShowAt(Control placementTarget, bool showAtPointer)
    {
        ShowAtCore(placementTarget, showAtPointer);
    }

    /// <summary>
    /// Hides the Flyout
    /// </summary>
    public sealed override void Hide()
    {
        HideCore();
    }

    /// <returns>True, if action was handled</returns>
    protected virtual bool HideCore(bool canCancel = true)
    {
        if (!IsOpen)
        {
            return false;
        }

        if (canCancel)
        {
            if (CancelClosing())
            {
                return false;
            }
        }

        IsOpen       = false;
        Popup.IsOpen = false;

        HandlePopupClosed();

        return true;
    }

    protected void HandlePopupClosed()
    {
        Popup.SetLogicalParent(null);

        // Ensure this isn't active
        _transientDisposable?.Dispose();
        _transientDisposable             = null;
        _enlargedPopupRect               = null;
        _enlargePopupRectScreenPixelRect = null;

        if (Target != null)
        {
            Target.DetachedFromVisualTree -= HandlePlacementTargetDetachedFromVisualTree;
            Target.KeyUp                  -= HandlePlacementTargetOrPopupKeyUp;
        }

        OnClosed();

        Target = null;
    }

    /// <returns>True, if action was handled</returns>
    protected virtual bool ShowAtCore(Control placementTarget, bool showAtPointer = false)
    {
        if (!PrepareShowPopup(placementTarget, showAtPointer))
        {
            return false;
        }

        IsOpen = Popup.IsOpen = true;
        HandlePopupOpened(placementTarget);

        return true;
    }

    protected bool PrepareShowPopup(Control placementTarget, bool showAtPointer = false)
    {
        if (placementTarget == null)
        {
            throw new ArgumentNullException(nameof(placementTarget));
        }

        if (IsOpen)
        {
            if (placementTarget == Target)
            {
                return false;
            } // Close before opening a new one

            _ = HideCore(false);
        }

        if (Popup.Parent != null && Popup.Parent != placementTarget)
        {
            Popup.SetLogicalParent(null);
        }

        if (Popup.Parent == null || Popup.PlacementTarget != placementTarget)
        {
            Popup.PlacementTarget = Target = placementTarget;
            Popup.SetLogicalParent(placementTarget);
            Popup.SetTemplatedParent(placementTarget.TemplatedParent);
        }

        if (Popup.Child == null)
        {
            Popup.Child = CreatePresenter();
            PresenterCreated?.Invoke(this, new FlyoutPresenterCreatedEventArgs(Popup.Child));
        }

        Popup.OverlayInputPassThroughElement = OverlayInputPassThroughElement;

        if (CancelOpening())
        {
            return false;
        }

        NotifyPositionPopup(showAtPointer);
        return true;
    }

    protected void HandlePopupOpened(Control placementTarget)
    {
        OnOpened();

        placementTarget.DetachedFromVisualTree += HandlePlacementTargetDetachedFromVisualTree;
        placementTarget.KeyUp                  += HandlePlacementTargetOrPopupKeyUp;

        if (ShowMode == FlyoutShowMode.Standard)
        {
            // Try and focus content inside Flyout
            if (Popup.Child!.Focusable)
            {
                Popup.Child.Focus();
            }
            else
            {
                var nextFocus = KeyboardNavigationHandler.GetNext(Popup.Child, NavigationDirection.Next);
                nextFocus?.Focus();
            }
        }
        else if (ShowMode == FlyoutShowMode.TransientWithDismissOnPointerMoveAway)
        {
            var inputManager = AvaloniaLocator.Current.GetService<IInputManager>();
            _transientDisposable = inputManager?.Process.Subscribe(HandleTransientDismiss);
        }
    }

    private void HandlePlacementTargetDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        _ = HideCore(false);
    }

    private void HandleTransientDismiss(RawInputEventArgs args)
    {
        if (args is RawPointerEventArgs pArgs && pArgs.Type == RawPointerEventType.Move)
        {
            // In ShowMode = TransientWithDismissOnPointerMoveAway, the Flyout is kept
            // shown as long as the pointer is within a certain px distance from the
            // flyout itself. I'm not sure what WinUI uses, but I'm defaulting to 
            // 100px, which seems about right
            // enlargedPopupRect is the Flyout bounds enlarged 100px
            // For windowed popups, enlargedPopupRect is in screen coordinates,
            // for overlay popups, its in OverlayLayer coordinates

            if (_enlargedPopupRect == null && _enlargePopupRectScreenPixelRect == null)
            {
                // Only do this once when the Flyout opens & cache the result
                if (Popup.Host is PopupRoot root)
                {
                    // Get the popup root bounds and convert to screen coordinates

                    var tmp = root.Bounds.Inflate(100);
                    _enlargePopupRectScreenPixelRect =
                        new PixelRect(root.PointToScreen(tmp.TopLeft), root.PointToScreen(tmp.BottomRight));
                }
                else if (Popup.Host is OverlayPopupHost host)
                {
                    // Overlay popups are in OverlayLayer coordinates, just use that
                    _enlargedPopupRect = host.Bounds.Inflate(100);
                }

                return;
            }

            if (Popup.Host is PopupRoot && pArgs.Root is Visual eventRoot)
            {
                // As long as the pointer stays within the enlargedPopupRect
                // the flyout stays open. If it leaves, close it
                // Despite working in screen coordinates, leaving the TopLevel
                // window will not close this (as pointer events stop), which 
                // does match UWP
                var pt = eventRoot.PointToScreen(pArgs.Position);
                if (!_enlargePopupRectScreenPixelRect?.Contains(pt) ?? false)
                {
                    HideCore(false);
                }
            }
            else if (Popup.Host is OverlayPopupHost)
            {
                // Same as above here, but just different coordinate space
                // so we don't need to translate
                if (!_enlargedPopupRect?.Contains(pArgs.Position) ?? false)
                {
                    HideCore(false);
                }
            }
        }
    }

    protected virtual void OnOpening(CancelEventArgs args)
    {
        Opening?.Invoke(this, args);
    }

    protected virtual void OnClosing(CancelEventArgs args)
    {
        Closing?.Invoke(this, args);
    }

    /// <summary>
    /// Used to create the content the Flyout displays
    /// </summary>
    /// <returns></returns>
    protected abstract Control CreatePresenter();

    private Popup CreatePopup()
    {
        var popup = new Popup
        {
            WindowManagerAddShadowHint = false,
            IsLightDismissEnabled      = false,
            //Note: This is required to prevent Button.Flyout from opening the flyout again after dismiss.
            OverlayDismissEventPassThrough = false
        };

        popup.Opened += OnPopupOpened;
        popup.Closed += OnPopupClosed;
        popup.AddClosingEventHandler(OnPopupClosing);

        popup.KeyUp += HandlePlacementTargetOrPopupKeyUp;
        NotifyPopupCreated(popup);
        return popup;
    }

    private void OnPopupOpened(object? sender, EventArgs e)
    {
        IsOpen = true;

        _popupHostChangedHandler?.Invoke(Popup.Host);
    }

    private void OnPopupClosing(object? sender, CancelEventArgs e)
    {
        if (IsOpen)
        {
            e.Cancel = CancelClosing();
        }
    }

    private void OnPopupClosed(object? sender, EventArgs e)
    {
        HideCore(false);

        _popupHostChangedHandler?.Invoke(null);
    }

    // This method is handling both popup logical tree and target logical tree.
    private void HandlePlacementTargetOrPopupKeyUp(object? sender, KeyEventArgs e)
    {
        if (!e.Handled
            && IsOpen
            && Target?.ContextFlyout == this)
        {
            var keymap = Application.Current!.PlatformSettings?.HotkeyConfiguration;

            if (keymap?.OpenContextMenu.Any(k => k.Matches(e)) == true)
            {
                e.Handled = HideCore();
            }
        }
    }

    private static void OnContextFlyoutPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Sender is Control c)
        {
            if (args.OldValue is FlyoutBase)
            {
                c.ContextRequested -= OnControlContextRequested;
            }

            if (args.NewValue is FlyoutBase)
            {
                c.ContextRequested += OnControlContextRequested;
            }
        }
    }

    private static void OnControlContextRequested(object? sender, ContextRequestedEventArgs e)
    {
        if (!e.Handled
            && sender is Control control
            && control.ContextFlyout is { } flyout)
        {
            if (control.ContextMenu != null)
            {
                Logger.TryGet(LogEventLevel.Verbose, "FlyoutBase")
                      ?.Log(control, "ContextMenu and ContextFlyout are both set, defaulting to ContextMenu");
                return;
            }

            if (flyout is PopupFlyoutBase popupFlyout)
            {
                // We do not support absolute popup positioning yet, so we ignore "point" at this moment.
                var triggeredByPointerInput = e.TryGetPosition(null, out _);
                e.Handled = popupFlyout.ShowAtCore(control, triggeredByPointerInput);
            }
            else
            {
                flyout.ShowAt(control);
                e.Handled = true;
            }
        }
    }

    private bool CancelClosing()
    {
        var eventArgs = new CancelEventArgs();
        OnClosing(eventArgs);
        return eventArgs.Cancel;
    }

    private bool CancelOpening()
    {
        var eventArgs = new CancelEventArgs();
        OnOpening(eventArgs);
        return eventArgs.Cancel;
    }

    internal static void SetPresenterClasses(Control? presenter, Classes classes)
    {
        if (presenter is null)
        {
            return;
        }

        //Remove any classes no longer in use, ignoring pseudo classes
        for (var i = presenter.Classes.Count - 1; i >= 0; i--)
        {
            if (!classes.Contains(presenter.Classes[i]) &&
                !presenter.Classes[i].Contains(':'))
            {
                presenter.Classes.RemoveAt(i);
            }
        }

        //Add new classes
        presenter.Classes.AddRange(classes);
    }

    /// <summary>
    /// 有时候需要清空 Popup 的 Child控件，强制重新创建
    /// </summary>
    internal void ClearUpPopupChild()
    {
        Popup.Child = null;
    }
}