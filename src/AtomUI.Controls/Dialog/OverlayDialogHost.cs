using System.Diagnostics;
using AtomUI.Controls.DialogPositioning;
using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public sealed class OverlayDialogHost : ContentControl,
                                        IInputRoot,
                                        IDisposable,
                                        IDialogHost,
                                        IMotionAwareControl,
                                        IManagedDialogPositionerDialog
{
    #region 公共属性定义
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<OverlayDialogHost, string?>(nameof (Title));
    
    public static readonly StyledProperty<bool> IsResizableProperty =
        Dialog.IsResizableProperty.AddOwner<OverlayDialogHost>();
        
    public static readonly StyledProperty<bool> IsClosableProperty =
        Dialog.IsClosableProperty.AddOwner<OverlayDialogHost>();
    
    public static readonly StyledProperty<bool> IsDragMovableProperty =
        Dialog.IsDragMovableProperty.AddOwner<OverlayDialogHost>();
    
    public static readonly StyledProperty<bool> IsModalProperty =
        Dialog.IsModalProperty.AddOwner<OverlayDialogHost>();
    
    public static readonly StyledProperty<DialogHorizontalPlacement> HorizontalPlacementProperty =
        Dialog.HorizontalPlacementProperty.AddOwner<OverlayDialogHost>();
    
    public static readonly StyledProperty<DialogVerticalPlacement> VerticalPlacementProperty =
        Dialog.VerticalPlacementProperty.AddOwner<OverlayDialogHost>();
    
    public static readonly StyledProperty<Transform?> TransformProperty =
        AvaloniaProperty.Register<OverlayDialogHost, Transform?>(nameof (Transform));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<OverlayDialogHost>();
    
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public bool IsResizable
    {
        get => GetValue(IsResizableProperty);
        set => SetValue(IsResizableProperty, value);
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
    
    public DialogHorizontalPlacement HorizontalPlacement
    {
        get => GetValue(HorizontalPlacementProperty);
        set => SetValue(HorizontalPlacementProperty, value);
    }
    
    public DialogVerticalPlacement VerticalPlacement
    {
        get => GetValue(VerticalPlacementProperty);
        set => SetValue(VerticalPlacementProperty, value);
    }
    
    public Transform? Transform
    {
        get => GetValue(TransformProperty);
        set => SetValue(TransformProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion
    
    #region 内部属性定义
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    internal bool IsHidden => _dialogLayer.Children.Contains(this);
    
    #endregion

    private IInputRoot? InputRoot => TopLevel.GetTopLevel(this);
    private readonly DialogLayer _dialogLayer;
    private readonly ManagedDialogPositioner _positioner;
    private readonly IKeyboardNavigationHandler? _keyboardNavigationHandler;
    private Point _lastRequestedPosition;
    private DialogPositionRequest? _dialogPositionRequest;
    private Size _dialogSize;
    private bool _needsUpdate;
    private OverlayDialogMask? _dialogMask;

    static OverlayDialogHost()
    {
        KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue<OverlayDialogHost>(KeyboardNavigationMode.Cycle);
    }

    public OverlayDialogHost(DialogLayer dialogLayer)
    {
        _dialogLayer               = dialogLayer;
        _positioner                = new ManagedDialogPositioner(this);
        _keyboardNavigationHandler = AvaloniaLocator.Current.GetService<IKeyboardNavigationHandler>();
        _keyboardNavigationHandler?.SetOwner(this);
    }
    
    IKeyboardNavigationHandler? IInputRoot.KeyboardNavigationHandler => _keyboardNavigationHandler;
    
    IFocusManager? IInputRoot.FocusManager => InputRoot?.FocusManager;
    
    IPlatformSettings? IInputRoot.PlatformSettings => InputRoot?.PlatformSettings;
    
    IInputElement? IInputRoot.PointerOverElement
    {
        get => InputRoot?.PointerOverElement;
        set
        {
            if (InputRoot is { } inputRoot)
            {
                inputRoot.PointerOverElement = value;
            }
        }
    }

    bool IInputRoot.ShowAccessKeys
    {
        get => InputRoot?.ShowAccessKeys ?? false;
        set
        {
            if (InputRoot is { } inputRoot)
            {
                inputRoot.ShowAccessKeys = value;
            }
        }
    }
    
    public void SetChild(Control? control)
    {
        Content = control;
    }
    
    public void Dispose() => Hide();
    
    public Visual? HostedVisualTreeRoot => null;
    
    // 多个 overlay 中始终置顶也可以有的
    bool IDialogHost.Topmost { get; set; }
    
    public void Show()
    {
        if (IsModal)
        {
            _dialogLayer.SizeChanged +=  HandleDialogLayerSizeChanged;
            _dialogMask              ??= new OverlayDialogMask();
            _dialogLayer.Children.Add(_dialogMask);
            ConfigureMaskSize(_dialogLayer.Bounds.Size);
        }
        _dialogLayer.Children.Add(this);
        if (Content is Visual visual && !visual.IsAttachedToVisualTree())
        {
            // We need to force a measure pass so any descendants are built, for focus to work.
            UpdateLayout();
        }
    }
    
    public void Hide()
    {
        _dialogLayer.Children.Remove(this);
        if (IsModal)
        {
            Debug.Assert(_dialogMask != null);
            _dialogLayer.Children.Remove(_dialogMask);
            _dialogLayer.SizeChanged -= HandleDialogLayerSizeChanged;
        }
    }

    private void HandleDialogLayerSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        ConfigureMaskSize(e.NewSize);
    }

    private void ConfigureMaskSize(Size size)
    {
        if (IsModal && _dialogMask != null)
        {
            _dialogMask.Width  = size.Width;
            _dialogMask.Height = size.Height;
        }
    }
    
    public void TakeFocus()
    {
        // Nothing to do here: overlay popups are implemented inside the window.
    }
    
    void IDialogHost.ConfigurePosition(DialogPositionRequest positionRequest)
    {
        _dialogPositionRequest = positionRequest;
        _needsUpdate          = true;
        UpdatePosition();
    }
    
    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_dialogSize != finalSize)
        {
            _dialogSize  = finalSize;
            _needsUpdate = true;
            UpdatePosition();
        }
        return base.ArrangeOverride(finalSize);
    }

    private void UpdatePosition()
    {
        if (_needsUpdate && _dialogPositionRequest is not null)
        {
            _needsUpdate = false;
            _positioner.Update(_dialogPositionRequest, _dialogSize);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsModalProperty)
        {
            if (!IsHidden)
            {
                if (change.OldValue is bool oldValue && oldValue && _dialogMask != null)
                {
                    _dialogLayer.Children.Remove(_dialogMask);
                }

                if (change.NewValue is bool newValue && newValue)
                {
                    // 找到自己
                    var index = _dialogLayer.Children.IndexOf(this);
                    _dialogMask ??= new OverlayDialogMask();
                    _dialogLayer.Children.Insert(index, _dialogMask);
                    ConfigureMaskSize(_dialogLayer.Bounds.Size);
                }
            }
        }
    }
    
    IReadOnlyList<ManagedDialogPositionerScreenInfo> IManagedDialogPositionerDialog.Screens
    {
        get
        {
            var rc       = new Rect(default, _dialogLayer.AvailableSize);
            var topLevel = TopLevel.GetTopLevel(this);
            if(topLevel != null)
            {
                var padding = topLevel.InsetsManager?.SafeAreaPadding ?? default;
                rc = rc.Deflate(padding);
            }
    
            return [new ManagedDialogPositionerScreenInfo(rc, rc)];
        }
    }
    
    double IManagedDialogPositionerDialog.Scaling => 1;
    
    Rect IManagedDialogPositionerDialog.ParentClientAreaScreenGeometry => new Rect(default, _dialogLayer.Bounds.Size);
    
    void IManagedDialogPositionerDialog.MoveAndResize(Point devicePoint, Size virtualSize)
    {
        _lastRequestedPosition = devicePoint;
        Dispatcher.UIThread.Post(() =>
        {
            Canvas.SetLeft(this, _lastRequestedPosition.X);
            Canvas.SetTop(this, _lastRequestedPosition.Y);
        });
    }
}