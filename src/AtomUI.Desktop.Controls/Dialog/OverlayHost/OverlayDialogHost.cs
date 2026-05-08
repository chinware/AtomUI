using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;

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
    private DialogButtonBox? _buttonBox;
    private OverlayDialogHeader? _header;
    private OverlayDialogResizer? _resizer;
    private CompositeDisposable? _confirmLoadingBindings;
    private Rect _ownerBounds;
    private Point _popupOffset;
    private Size _hostSize;
    private Size? _restoreSize;

    private const double MinVisibleX = 100;
    private const double MinVisibleY = 40;

    public OverlayDialogHost(Control placementTarget, Dialog dialog, IAvaloniaDependencyResolver? dependencyResolver)
    {
        _popup = new AtomUIPopup
        {
            PlacementTarget                = placementTarget,
            Placement                      = PlacementMode.Custom,
            PlacementConstraintAdjustment  = PopupPositionerConstraintAdjustment.None,
            DependencyResolver             = dependencyResolver,
            ShouldUseOverlayLayer          = true,
            IsLightDismissEnabled          = dialog.IsLightDismissEnabled,
            CustomPopupPlacementCallback   = HandlePlacePopup,
        };
        TokenResourceBinder.CreateTokenBinding(_popup, AtomUIPopup.OverlayHostShadowProperty, SharedTokenKind.BoxShadows);
        _popup.SetPopupParent(placementTarget);
        _popup.Child = this;
        _dialog = dialog;
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

    public void Show()
    {
        _popup.IsOpen = true;
    }

    public void Close(Action? callback = null)
    {
        _popup.IsOpen          = false;
        _popup.Child           = null;
        _popup.PlacementTarget = null;
        _popup.SetPopupParent(null);
        callback?.Invoke();
    }

    public void AttachPlacement(Control placementTarget)
    {
        var topLevel = TopLevel.GetTopLevel(placementTarget);
        if (topLevel != null)
        {
            _ownerBounds = new Rect(default, topLevel.ClientSize);
        }
    }

    public void UpdateSizing()
    {
        Width     = _dialog.Width;
        MinWidth  = _dialog.MinWidth;
        MaxWidth  = _dialog.MaxWidth;
        Height    = _dialog.Height;
        MinHeight = _dialog.MinHeight;
        MaxHeight = _dialog.MaxHeight;
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
        }

        _popupOffset = _dialog.CalculatePlacementOffset(_hostSize, _ownerBounds.Size);
        _popup.HandlePositionChange();
    }

    private void HandlePlacePopup(CustomPopupPlacement placement)
    {
        var ownerSize = _ownerBounds.Size;
        var popupSize = placement.PopupSize;

        placement.AnchorRectangle = new Rect(default, new Size(1, 1));
        placement.Anchor          = PopupAnchor.TopLeft;
        placement.Gravity         = PopupGravity.BottomRight;

        var offset = _popupOffset;

        var minVisX = Math.Min(MinVisibleX, popupSize.Width);
        var minVisY = Math.Min(MinVisibleY, popupSize.Height);

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

        placement.Offset = offset;
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
            _resizer.TargetDialog    = this;
            _resizer.ResizeRequest  += HandleResizeRequest;
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
        if (!IsDragMovable || WindowState == OverlayDialogState.Maximized || !e.Properties.IsLeftButtonPressed)
        {
            return;
        }

        _dragStart = e.GetPosition(TopLevel.GetTopLevel(this));
        e.Pointer.Capture(_header);
    }

    private void HandleHeaderPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragStart is null || !e.Properties.IsLeftButtonPressed)
        {
            return;
        }

        var position = e.GetPosition(TopLevel.GetTopLevel(this));
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
                button[!Button.IsLoadingProperty] = this[!IsConfirmLoadingProperty];
            }
        }
    }

    private void DetachTemplateHandlers()
    {
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
