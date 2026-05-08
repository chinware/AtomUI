using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using Avalonia.Media.Transformation;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

using AtomUIPopup = Popup;

internal class OverlayDialogHost : ContentControl,
                                   IDialogHost,
                                   IMotionAwareControl
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<OverlayDialogHost, string?>(nameof(Title));

    public static readonly StyledProperty<PathIcon?> TitleIconProperty =
        AvaloniaProperty.Register<OverlayDialogHost, PathIcon?>(nameof(TitleIcon));

    public static readonly StyledProperty<bool> IsResizableProperty =
        Dialog.IsResizableProperty.AddOwner<OverlayDialogHost>();

    public static readonly StyledProperty<bool> IsClosableProperty =
        Dialog.IsClosableProperty.AddOwner<OverlayDialogHost>();

    public static readonly StyledProperty<bool> IsMaximizableProperty =
        Dialog.IsMaximizableProperty.AddOwner<OverlayDialogHost>();

    public static readonly StyledProperty<bool> IsDragMovableProperty =
        Dialog.IsDragMovableProperty.AddOwner<OverlayDialogHost>();

    public static readonly StyledProperty<bool> IsModalProperty =
        Dialog.IsModalProperty.AddOwner<OverlayDialogHost>();

    public static readonly StyledProperty<OverlayDialogState> WindowStateProperty =
        AvaloniaProperty.Register<OverlayDialogHost, OverlayDialogState>(nameof(WindowState));

    public static readonly StyledProperty<DialogStandardButtons> StandardButtonsProperty =
        DialogButtonBox.StandardButtonsProperty.AddOwner<OverlayDialogHost>();

    public static readonly StyledProperty<DialogStandardButton> DefaultStandardButtonProperty =
        DialogButtonBox.DefaultStandardButtonProperty.AddOwner<OverlayDialogHost>();

    public static readonly StyledProperty<DialogStandardButton> EscapeStandardButtonProperty =
        DialogButtonBox.EscapeStandardButtonProperty.AddOwner<OverlayDialogHost>();

    public static readonly StyledProperty<bool> IsFooterVisibleProperty =
        Dialog.IsFooterVisibleProperty.AddOwner<OverlayDialogHost>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<OverlayDialogHost>();

    public static readonly StyledProperty<bool> IsLoadingProperty =
        Dialog.IsLoadingProperty.AddOwner<OverlayDialogHost>();

    public static readonly StyledProperty<bool> IsConfirmLoadingProperty =
        Dialog.IsConfirmLoadingProperty.AddOwner<OverlayDialogHost>();

    internal static readonly DirectProperty<OverlayDialogHost, bool> IsEffectiveFooterVisibleProperty =
        AvaloniaProperty.RegisterDirect<OverlayDialogHost, bool>(
            nameof(IsEffectiveFooterVisible),
            o => o.IsEffectiveFooterVisible,
            (o, v) => o.IsEffectiveFooterVisible = v);

    internal static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.Register<OverlayDialogHost, TimeSpan>(nameof(AnimationDuration));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public PathIcon? TitleIcon
    {
        get => GetValue(TitleIconProperty);
        set => SetValue(TitleIconProperty, value);
    }

    public bool IsResizable
    {
        get => GetValue(IsResizableProperty);
        set => SetValue(IsResizableProperty, value);
    }

    public bool IsMaximizable
    {
        get => GetValue(IsMaximizableProperty);
        set => SetValue(IsMaximizableProperty, value);
    }

    public bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    public bool IsDragMovable
    {
        get => GetValue(IsDragMovableProperty);
        set => SetValue(IsDragMovableProperty, value);
    }

    public bool IsModal
    {
        get => GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }

    public OverlayDialogState WindowState
    {
        get => GetValue(WindowStateProperty);
        set => SetValue(WindowStateProperty, value);
    }

    public DialogStandardButtons StandardButtons
    {
        get => GetValue(StandardButtonsProperty);
        set => SetValue(StandardButtonsProperty, value);
    }

    public DialogStandardButton DefaultStandardButton
    {
        get => GetValue(DefaultStandardButtonProperty);
        set => SetValue(DefaultStandardButtonProperty, value);
    }

    public DialogStandardButton EscapeStandardButton
    {
        get => GetValue(EscapeStandardButtonProperty);
        set => SetValue(EscapeStandardButtonProperty, value);
    }

    public bool IsFooterVisible
    {
        get => GetValue(IsFooterVisibleProperty);
        set => SetValue(IsFooterVisibleProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public bool IsConfirmLoading
    {
        get => GetValue(IsConfirmLoadingProperty);
        set => SetValue(IsConfirmLoadingProperty, value);
    }

    private bool _isEffectiveFooterVisible;

    internal bool IsEffectiveFooterVisible
    {
        get => _isEffectiveFooterVisible;
        set => SetAndRaise(IsEffectiveFooterVisibleProperty, ref _isEffectiveFooterVisible, value);
    }

    internal TimeSpan AnimationDuration
    {
        get => GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }

    bool IDialogHost.Topmost { get; set; }
    public AvaloniaList<DialogButton> CustomButtons { get; } = new();

    private readonly AtomUIPopup _popup;
    private readonly Dialog _dialog;
    private readonly OverlayDialogMask _dialogMask;
    private DialogButtonBox? _buttonBox;
    private OverlayDialogHeader? _header;
    private OverlayDialogResizer? _resizer;
    private CompositeDisposable? _confirmLoadingBindings;
    private Rect _ownerBounds;
    private Point _popupOffset;
    private Size _hostSize;
    private Size? _restoreSize;
    private bool _isCloseRequested;
    private Action? _closeCallback;
    private OverlayPopupHost? _animatedOverlayHost;
    private Panel? _animatedOverlayLayer;

    private const double MinVisibleX = 100;
    private const double MinVisibleY = 40;

    public OverlayDialogHost(Control placementTarget, Dialog dialog, IAvaloniaDependencyResolver? dependencyResolver)
    {
        _popup = new AtomUIPopup
        {
            PlacementTarget               = placementTarget,
            Placement                     = PlacementMode.Custom,
            PlacementConstraintAdjustment = PopupPositionerConstraintAdjustment.None,
            DependencyResolver            = dependencyResolver,
            ShouldUseOverlayLayer         = true,
            CustomPopupPlacementCallback  = HandlePlacePopup,
        };
        TokenResourceBinder.CreateTokenBinding(_popup, AtomUIPopup.OverlayHostShadowProperty, SharedTokenKind.BoxShadows);
        _popup.SetPopupParent(placementTarget);
        _popup.Closed                   += HandlePopupClosed;
        _dialogMask                     =  new OverlayDialogMask();
        _dialog                         =  dialog;
        CustomButtons.CollectionChanged += HandleCustomButtonsChanged;
    }

    public void BindDialog(Dialog dialog, CompositeDisposable disposables)
    {
        this[!TitleProperty]                 = dialog[!Dialog.TitleProperty];
        this[!TitleIconProperty]             = dialog[!Dialog.TitleIconProperty];
        this[!IsResizableProperty]           = dialog[!Dialog.IsResizableProperty];
        this[!IsClosableProperty]            = dialog[!Dialog.IsClosableProperty];
        this[!IsMaximizableProperty]         = dialog[!Dialog.IsMaximizableProperty];
        this[!IsDragMovableProperty]         = dialog[!Dialog.IsDragMovableProperty];
        this[!IsModalProperty]               = dialog[!Dialog.IsModalProperty];
        this[!IsMotionEnabledProperty]       = dialog[!Dialog.IsMotionEnabledProperty];
        this[!IsLoadingProperty]             = dialog[!Dialog.IsLoadingProperty];
        this[!IsConfirmLoadingProperty]      = dialog[!Dialog.IsConfirmLoadingProperty];
        this[!IsFooterVisibleProperty]       = dialog[!Dialog.IsFooterVisibleProperty];
        this[!StandardButtonsProperty]       = dialog[!Dialog.StandardButtonsProperty];
        this[!DefaultStandardButtonProperty] = dialog[!Dialog.DefaultStandardButtonProperty];
        this[!EscapeStandardButtonProperty]  = dialog[!Dialog.EscapeStandardButtonProperty];
    }

    private const double CollapsedScale = 0.75;

    public void Show()
    {
        ConfigurePopupChild();

        _popup.IsOpen = true;
        BringToFront();

        if (!IsMotionEnabled)
        {
            // 非动画路径：仍需把 mask 挂到 OverlayLayer，否则 modal 遮罩出不来。
            Dispatcher.UIThread.Post(AttachMaskToOverlayLayer);
            return;
        }

        // 防 flash：Popup 打开是同步的，OverlayPopupHost 此时已可 FindAncestorOfType 找到。
        // 立即清 transitions + Opacity=0，首帧就渲染为完全透明。offset 还算不出
        // （layout 没跑），留到下一 tick 补完初始态 transform。
        var initialHost = this.FindAncestorOfType<OverlayPopupHost>();
        if (initialHost is not null)
        {
            initialHost.Transitions = null;
            initialHost.Opacity     = 0.0;
        }

        // OverlayPopupHost 的 layout 要等到下一 tick 才稳定，offset 需要它的屏幕坐标。
        Dispatcher.UIThread.Post(() =>
        {
            if (_isCloseRequested)
            {
                return;
            }

            AttachMaskToOverlayLayer();
            var overlayHost = initialHost ?? this.FindAncestorOfType<OverlayPopupHost>();
            if (overlayHost is null)
            {
                return;
            }

            _animatedOverlayHost = overlayHost;

            // 清 transitions 让起始态瞬时生效，装回 transitions 后下一 Post 设目标态才会
            // 被 transitions 抓到这一次"起始→目标"变化去插值。若不清空，起始态本身也会触发
            // 一次被 Post2 立刻打断的动画，实际看起来像没动画。
            var offset = CalculateOffsetFromPlacement();
            overlayHost.Transitions     = null;
            overlayHost.Opacity         = 0.0;
            overlayHost.RenderTransform = BuildCollapsedTransform(offset);
            EnsureTransitionsOn(overlayHost);

            if (IsModal)
            {
                ResetMaskOpacityWithoutTransition(0.0);
            }

            Dispatcher.UIThread.Post(() =>
            {
                if (_isCloseRequested)
                {
                    return;
                }
                overlayHost.Opacity         = 1.0;
                overlayHost.RenderTransform = BuildIdentityTransform();

                if (IsModal)
                {
                    _dialogMask.Opacity = 1.0;
                }
            });
        });
    }

    public void Close(Action? callback = null)
    {
        _closeCallback = callback;
        if (_isCloseRequested)
        {
            return;
        }

        _isCloseRequested = true;
        if (!_popup.IsOpen)
        {
            CleanupPopup();
            return;
        }

        if (IsMotionEnabled && _animatedOverlayHost is { } overlayHost)
        {
            var offset = CalculateOffsetFromPlacement();
            overlayHost.Opacity         = 0.0;
            overlayHost.RenderTransform = BuildCollapsedTransform(offset);

            if (IsModal)
            {
                _dialogMask.Opacity = 0.0;
            }

            DispatcherTimer.RunOnce(() =>
            {
                if (_popup.IsOpen)
                {
                    _popup.IsOpen = false;
                }
                else
                {
                    CleanupPopup();
                }
            }, AnimationDuration);
        }
        else
        {
            _popup.IsOpen = false;
        }
    }

    private static void EnsureTransitionsOn(OverlayPopupHost host)
    {
        if (host.Transitions is { Count: > 0 })
        {
            return;
        }
        host.Transitions =
        [
            TransitionUtils.CreateTransition<DoubleTransition>(
                Visual.OpacityProperty,
                SharedTokenKind.MotionDurationMid,
                new CircularEaseOut()),
            TransitionUtils.CreateTransition<TransformOperationsTransition>(
                Visual.RenderTransformProperty,
                SharedTokenKind.MotionDurationMid,
                new CircularEaseOut())
        ];
    }

    private void ResetMaskOpacityWithoutTransition(double value)
    {
        // mask 的 Opacity transition 由主题装配；这里瞬时归位，套"清空 → 赋值 → 装回"。
        var maskTransitions = _dialogMask.Transitions;
        _dialogMask.Transitions = null;
        _dialogMask.Opacity     = value;
        _dialogMask.Transitions = maskTransitions;
    }

    private static TransformOperations BuildCollapsedTransform(Point offset)
    {
        var builder = new TransformOperations.Builder(2);
        builder.AppendScale(CollapsedScale, CollapsedScale);
        builder.AppendTranslate(-offset.X, -offset.Y);
        return builder.Build();
    }

    private static TransformOperations BuildIdentityTransform()
    {
        var builder = new TransformOperations.Builder(2);
        builder.AppendScale(1.0, 1.0);
        builder.AppendTranslate(0, 0);
        return builder.Build();
    }

    private void AttachMaskToOverlayLayer()
    {
        if (!IsModal)
        {
            return;
        }

        var overlayHost = this.FindAncestorOfType<OverlayPopupHost>();
        if (overlayHost?.GetPopupOverlayLayer() is not Panel overlayLayer)
        {
            return;
        }

        _animatedOverlayLayer = overlayLayer;

        // mask 与 popupHost 是兄弟节点，Z 序在 popupHost 之前（作为底层被覆盖）。
        var hostIndex = overlayLayer.Children.IndexOf(overlayHost);
        if (hostIndex < 0)
        {
            return;
        }

        if (_dialogMask.Parent is Panel oldParent)
        {
            oldParent.Children.Remove(_dialogMask);
        }

        overlayLayer.Children.Insert(hostIndex, _dialogMask);
        SyncMaskBounds();
    }

    private void SyncMaskBounds()
    {
        _dialogMask.Width  = _ownerBounds.Width;
        _dialogMask.Height = _ownerBounds.Height;
    }

    private Point CalculateOffsetFromPlacement()
    {
        var offset = new Point();
        if (_dialog.PlacementTarget is not { } target)
        {
            return offset;
        }

        // Popup 是独立的 TopLevel 可视树，无法靠 TranslatePoint 在 PlacementTarget 与 host
        // 之间做坐标换算；改用屏幕像素点再按 RenderScaling 回到 DIP。
        var targetRoot = target.GetVisualRoot();
        var hostRoot   = this.GetVisualRoot();
        if (targetRoot is null || hostRoot is null)
        {
            return offset;
        }

        var sourceScreen = target.PointToScreen(default);
        var hostScreen   = this.PointToScreen(default);
        var scaling      = (hostRoot as TopLevel)?.RenderScaling
                           ?? (targetRoot as TopLevel)?.RenderScaling
                           ?? 1.0;

        var dx = (hostScreen.X - sourceScreen.X) / scaling;
        var dy = (hostScreen.Y - sourceScreen.Y) / scaling;
        return new Point(dx, dy);
    }

    private void HandlePopupClosed(object? sender, EventArgs e)
    {
        CleanupPopup();
    }

    private void CleanupPopup()
    {
        _popup.Closed          -= HandlePopupClosed;
        _confirmLoadingBindings?.Dispose();
        _confirmLoadingBindings = null;
        _popup.Child           = null;
        _popup.PlacementTarget = null;
        _popup.SetPopupParent(null);
        if (_animatedOverlayLayer is { } overlayLayer)
        {
            overlayLayer.Children.Remove(_dialogMask);
        }
        _animatedOverlayHost  = null;
        _animatedOverlayLayer = null;
        _isCloseRequested     = false;
        _closeCallback?.Invoke();
        _closeCallback = null;
    }

    public void AttachPlacement(Control placementTarget)
    {
        var topLevel = TopLevel.GetTopLevel(placementTarget);
        if (topLevel != null)
        {
            _ownerBounds = new Rect(default, topLevel.ClientSize);
            ConfigureModalRootSize();
        }
    }

    public void UpdateSizing()
    {
        Width     = _dialog.HostWidth;
        MinWidth  = _dialog.HostMinWidth;
        MaxWidth  = _dialog.HostMaxWidth;
        Height    = _dialog.HostHeight;
        MinHeight = _dialog.HostMinHeight;
        MaxHeight = _dialog.HostMaxHeight;
    }

    public void UpdatePlacement()
    {
        if (_hostSize == default)
        {
            return;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel != null)
        {
            _ownerBounds = new Rect(default, topLevel.ClientSize);
            ConfigureModalRootSize();
        }

        // 拆掉 _popupRoot Canvas 后，this 直接作为 _popup.Child，不再有 Canvas 座标层；
        // modal 和非 modal 都走 _popupOffset + popup 原生 placement 定位。
        var offset = _dialog.CalculatePlacementOffset(_hostSize, _ownerBounds.Size);
        if (IsModal)
        {
            offset = ConstrainOffset(offset, _hostSize);
        }
        _popupOffset = offset;
        _popup.HandlePositionChange();
    }

    private void HandlePlacePopup(CustomPopupPlacement placement)
    {
        placement.AnchorRectangle = new Rect(default, new Size(1, 1));
        placement.Anchor          = PopupAnchor.TopLeft;
        placement.Gravity         = PopupGravity.BottomRight;
        placement.Offset          = IsModal
            ? _popupOffset
            : ConstrainOffset(_popupOffset, placement.PopupSize);
    }

    private Point ConstrainOffset(Point offset, Size popupSize)
    {
        var ownerSize = _ownerBounds.Size;
        var minVisX   = Math.Min(MinVisibleX, popupSize.Width);
        var minVisY   = Math.Min(MinVisibleY, popupSize.Height);
        
        // 上边界：硬约束
        offset = offset.WithY(Math.Max(0, offset.Y));

        // 左边界：保留 minVisX 可见
        offset = offset.WithX(Math.Max(-(popupSize.Width - minVisX), offset.X));

        // 右边界：保留 minVisX 可见
        if (offset.X > ownerSize.Width - minVisX)
        {
            offset = offset.WithX(ownerSize.Width - minVisX);
        }

        // 下边界：保留 minVisY（标题栏）可见
        if (offset.Y > ownerSize.Height - minVisY)
        {
            offset = offset.WithY(ownerSize.Height - minVisY);
        }

        return offset;
    }

    private void ConfigurePopupChild()
    {
        // Popup 只装 dialog；mask 走 OverlayLayer 做 popupHost 的兄弟节点（见 AttachMaskToOverlayLayer），
        // 避免被 OverlayPopupHost 的 scale/translate 动画带着一起缩放。
        _popup.Child = this;
    }

    private void ConfigureModalRootSize()
    {
        if (!IsModal)
        {
            return;
        }

        SyncMaskBounds();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);
        return new Size(Math.Max(size.Width, MinWidth), Math.Max(size.Height, MinHeight));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        _hostSize = size;
        UpdatePlacement();

        if (WindowState == OverlayDialogState.Maximized)
        {
            Width  = _ownerBounds.Width;
            Height = _ownerBounds.Height;
        }

        return size;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        DetachTemplateHandlers();

        _buttonBox = e.NameScope.Find<DialogButtonBox>("PART_ButtonBox");
        _header    = e.NameScope.Find<OverlayDialogHeader>("PART_Header");
        _resizer   = e.NameScope.Find<OverlayDialogResizer>("PART_Resizer");

        if (_buttonBox != null)
        {
            _buttonBox.CustomButtons.AddRange(CustomButtons);
            _buttonBox.Clicked             += HandleButtonBoxClicked;
            _buttonBox.ButtonsSynchronized += HandleButtonsSynchronized;
        }

        if (_header != null)
        {
            _header.CloseRequest     += HandleHeaderCloseRequest;
            _header.MaximizeRequest  += HandleHeaderMaximizeRequest;
            _header.NormalizeRequest += HandleHeaderNormalizeRequest;
            _header.PointerPressed   += HandleHeaderPointerPressed;
            _header.PointerReleased  += HandleHeaderPointerReleased;
            _header.PointerMoved     += HandleHeaderPointerMoved;
        }

        if (_resizer != null)
        {
            _resizer.TargetDialog  =  this;
            _resizer.ResizeRequest += HandleResizeRequest;
        }

        ConfigureEffectiveFooterVisible();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == StandardButtonsProperty ||
            change.Property == IsLoadingProperty ||
            change.Property == IsFooterVisibleProperty)
        {
            ConfigureEffectiveFooterVisible();
        }
    }

    private Point? _dragStart;

    private void HandleHeaderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BringToFront();
        var topLevel = TopLevel.GetTopLevel(this);
        if (!IsDragMovable ||
            WindowState == OverlayDialogState.Maximized ||
            !e.Properties.IsLeftButtonPressed ||
            topLevel is null)
        {
            return;
        }

        _dragStart = e.GetPosition(topLevel);
        e.Pointer.Capture(_header);
    }

    private void BringToFront()
    {
        if (this.FindAncestorOfType<OverlayPopupHost>() is { } popupHost)
        {
            if (popupHost.GetPopupOverlayLayer() is Panel popupOverlayLayer)
            {
                var children = popupOverlayLayer.Children;
                var index    = children.IndexOf(popupHost);
                if (index >= 0 && index < children.Count - 1)
                {
                    children.RemoveAt(index);
                    children.Add(popupHost);
                }
            }
        }
    }

    private void HandleHeaderPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragStart is null || !e.Properties.IsLeftButtonPressed)
        {
            return;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return;
        }

        var position = e.GetPosition(topLevel);
        var delta    = position - _dragStart.Value;
        _dragStart = position;
        _dialog.SetCurrentValue(Dialog.OffsetXProperty, _dialog.OffsetX + delta.X);
        _dialog.SetCurrentValue(Dialog.OffsetYProperty, _dialog.OffsetY + delta.Y);
    }

    private void HandleHeaderPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _dragStart = null;
        e.Pointer.Capture(null);
    }

    private void HandleHeaderCloseRequest(object? sender, EventArgs e)
    {
        if (IsClosable)
        {
            _dialog.NotifyDialogHostCloseRequest();
        }
    }

    private void HandleHeaderMaximizeRequest(object? sender, EventArgs e)
    {
        _restoreSize = Bounds.Size;
        WindowState  = OverlayDialogState.Maximized;
        Width        = _ownerBounds.Width;
        Height       = _ownerBounds.Height;
        _dialog.SetCurrentValue(Dialog.OffsetXProperty, 0);
        _dialog.SetCurrentValue(Dialog.OffsetYProperty, 0);
    }

    private void HandleHeaderNormalizeRequest(object? sender, EventArgs e)
    {
        WindowState = OverlayDialogState.Normal;
        if (_restoreSize is { } restoreSize)
        {
            Width  = restoreSize.Width;
            Height = restoreSize.Height;
        }
    }

    private void HandleResizeRequest(object? sender, OverlayDialogResizeEventArgs args)
    {
        if (WindowState == OverlayDialogState.Maximized)
        {
            return;
        }

        var location  = args.Location;
        var deltaX    = args.DeltaOffsetX;
        var deltaY    = args.DeltaOffsetY;
        var newWidth  = Bounds.Width;
        var newHeight = Bounds.Height;

        if (location.HasFlag(ResizeHandleLocation.East))
        {
            newWidth = Math.Max(MinWidth, Bounds.Width + deltaX);
        }
        else if (location.HasFlag(ResizeHandleLocation.West))
        {
            var candidate   = Math.Max(MinWidth, Bounds.Width - deltaX);
            var actualDelta = candidate - Bounds.Width;
            newWidth = candidate;
            _dialog.SetCurrentValue(Dialog.OffsetXProperty, _dialog.OffsetX - actualDelta);
        }

        if (location.HasFlag(ResizeHandleLocation.South))
        {
            newHeight = Math.Max(MinHeight, Bounds.Height + deltaY);
        }
        else if (location.HasFlag(ResizeHandleLocation.North))
        {
            var candidate   = Math.Max(MinHeight, Bounds.Height - deltaY);
            var actualDelta = candidate - Bounds.Height;
            newHeight = candidate;
            _dialog.SetCurrentValue(Dialog.OffsetYProperty, _dialog.OffsetY - actualDelta);
        }

        Width  = newWidth;
        Height = newHeight;
    }

    private void HandleCustomButtonsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_buttonBox != null)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _buttonBox.CustomButtons.AddRange(e.NewItems!.OfType<DialogButton>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _buttonBox.CustomButtons.RemoveAll(e.OldItems!.OfType<DialogButton>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException();
            }
        }

        ConfigureEffectiveFooterVisible();
    }

    private void ConfigureEffectiveFooterVisible()
    {
        SetCurrentValue(IsEffectiveFooterVisibleProperty,
            IsFooterVisible && !IsLoading && (StandardButtons.Count > 0 || CustomButtons.Count > 0));
    }

    private void HandleButtonBoxClicked(object? sender, DialogButtonClickedEventArgs args)
    {
        _dialog.NotifyDialogButtonBoxClicked(args.SourceButton);
    }

    private void HandleButtonsSynchronized(object? sender, DialogBoxButtonSyncEventArgs args)
    {
        _dialog.NotifyDialogButtonSynchronized(args.Buttons);
        _confirmLoadingBindings?.Dispose();
        _confirmLoadingBindings = new CompositeDisposable(args.Buttons.Count);
        foreach (var button in args.Buttons)
        {
            if (button.Role == DialogButtonRole.AcceptRole ||
                button.Role == DialogButtonRole.YesRole ||
                button.Role == DialogButtonRole.ApplyRole)
            {
                _confirmLoadingBindings.Add(button.Bind(Button.IsLoadingProperty,
                    this.GetObservable(IsConfirmLoadingProperty)));
            }
        }
    }

    private void DetachTemplateHandlers()
    {
        _confirmLoadingBindings?.Dispose();
        _confirmLoadingBindings = null;

        if (_buttonBox != null)
        {
            _buttonBox.CustomButtons.Clear();
            _buttonBox.Clicked             -= HandleButtonBoxClicked;
            _buttonBox.ButtonsSynchronized -= HandleButtonsSynchronized;
        }

        if (_header != null)
        {
            _header.CloseRequest     -= HandleHeaderCloseRequest;
            _header.MaximizeRequest  -= HandleHeaderMaximizeRequest;
            _header.NormalizeRequest -= HandleHeaderNormalizeRequest;
            _header.PointerPressed   -= HandleHeaderPointerPressed;
            _header.PointerReleased  -= HandleHeaderPointerReleased;
            _header.PointerMoved     -= HandleHeaderPointerMoved;
        }

        if (_resizer != null)
        {
            _resizer.ResizeRequest -= HandleResizeRequest;
        }
    }
}