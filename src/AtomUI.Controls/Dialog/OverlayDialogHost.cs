using System.Diagnostics;
using AtomUI.Controls.DialogPositioning;
using AtomUI.Controls.Primitives;
using AtomUI.Controls.Themes;
using AtomUI.IconPkg;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
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
    
    public static readonly StyledProperty<Icon?> TitleIconProperty =
        AvaloniaProperty.Register<OverlayDialogHost, Icon?>(nameof (TitleIcon));
    
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
    
    public static readonly StyledProperty<Transform?> TransformProperty =
        AvaloniaProperty.Register<OverlayDialogHost, Transform?>(nameof (Transform));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<OverlayDialogHost>();
    
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public Icon? TitleIcon
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
    
    internal static readonly DirectProperty<OverlayDialogHost, bool> IsDraggingProperty =
        AvaloniaProperty.RegisterDirect<OverlayDialogHost, bool>(nameof(IsDragging),
            o => o.IsDragging,
            (o, v) => o.IsDragging = v);
    
    private bool _isDragging;

    internal bool IsDragging
    {
        get => _isDragging;
        set => SetAndRaise(IsDraggingProperty, ref _isDragging, value);
    }
    
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
    // 用于最大化
    private Point _originPosition;
    private OverlayDialogHeader? _header;
    private OverlayDialogResizer? _resizer;
    private readonly List<Action> _disposeActions = new();
    private Dialog _dialog;
    
    // 拖动
    private Point? _lastPoint;

    static OverlayDialogHost()
    {
        KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue<OverlayDialogHost>(KeyboardNavigationMode.Cycle);
        AffectsMeasure<OverlayDialogHost>(WindowStateProperty);
    }

    public OverlayDialogHost(DialogLayer dialogLayer, Dialog dialog)
    {
        _dialog                    = dialog;
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
        _dialogLayer.SizeChanged +=  HandleDialogLayerSizeChanged;

        if (IsModal)
        {
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
        }
        _dialogLayer.SizeChanged -= HandleDialogLayerSizeChanged;
        foreach (var disposeAction in _disposeActions)
        {
            disposeAction.Invoke();
        }
        _disposeActions.Clear();
    }

    private void HandleDialogLayerSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        ConfigureMaskSize(e.NewSize);
        _needsUpdate = true;
        UpdatePosition();
        
        var offsetX = Canvas.GetLeft(this);
        var offsetY = Canvas.GetTop(this);

        var bounds     = _dialogLayer.Bounds;
        var maxOffsetX = bounds.Width - DesiredSize.Width;
        var maxOffsetY =  bounds.Height - DesiredSize.Height;
        
        var deltaX = maxOffsetX - offsetX;
        if (deltaX < 0)
        {
            offsetX += deltaX;
        }
        
        _dialog.SetCurrentValue(Dialog.HorizontalOffsetProperty, new Dimension(Math.Min(Math.Max(offsetX, 0), maxOffsetX)));
        _dialog.SetCurrentValue(Dialog.VerticalOffsetProperty, new Dimension(Math.Min(Math.Max(offsetY, 0), maxOffsetY)));
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
        _needsUpdate           = true;
        UpdatePosition();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);
        if (WindowState == OverlayDialogState.Maximized)
        {
            return _dialogLayer.Bounds.Size;
        }
        return size;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_dialogSize != finalSize)
        {
            _dialogSize  = finalSize;
            _dialog.NotifyDialogHostMeasured(_dialogSize);
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
                    if (index != -1)
                    {
                        _dialogMask ??= new OverlayDialogMask();
                        _dialogLayer.Children.Insert(index, _dialogMask);
                        ConfigureMaskSize(_dialogLayer.Bounds.Size);
                    }
                }
            }
        }
        else if (change.Property == WindowStateProperty)
        {
            HandleWindowStateChanged(change.GetOldValue<OverlayDialogState>(), change.GetNewValue<OverlayDialogState>());
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
        Canvas.SetLeft(this, _lastRequestedPosition.X);
        Canvas.SetTop(this, _lastRequestedPosition.Y);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _resizer = e.NameScope.Find<OverlayDialogResizer>(OverlayDialogThemeConstants.ResizerPart);
        if (_resizer != null)
        {
            _resizer.TargetDialog = this;
        }
        _header = e.NameScope.Find<OverlayDialogHeader>(OverlayDialogThemeConstants.HeaderPart);
        ConfigureHeaderHandlers();
        if (_header != null)
        {
            _header.SetCurrentValue(OverlayDialogHeader.IsDialogMaximizedProperty, WindowState == OverlayDialogState.Maximized);
        }
    }

    private void ConfigureHeaderHandlers()
    {
        if (_header != null)
        {
            _header.DoubleTapped     += HandleHeaderDoubleClicked;
            _header.MaximizeRequest  += HandleHeaderMaximizeRequest;
            _header.NormalizeRequest += HandleHeaderNormalizeRequest;
            _header.CloseRequest     += HandleHeaderCloseRequest;
            _header.PointerPressed  += HandleHeaderPointerPressed;
            _header.PointerReleased += HandleHeaderPointerReleased;
            _header.PointerMoved    += HandleHeaderPointerMoved;
            _disposeActions.Add(() =>
            {
                _header.DoubleTapped     -= HandleHeaderDoubleClicked;
                _header.MaximizeRequest  -= HandleHeaderMaximizeRequest;
                _header.NormalizeRequest -= HandleHeaderNormalizeRequest;
                _header.CloseRequest     -= HandleHeaderCloseRequest;
                _header.PointerPressed  -= HandleHeaderPointerPressed;
                _header.PointerReleased -= HandleHeaderPointerReleased;
                _header.PointerMoved    -= HandleHeaderPointerMoved;
            });
        }
    }
    
    private void HandleHeaderDoubleClicked(object? sender, RoutedEventArgs e)
    {
        if (!IsMaximizable)
        {
            return;
        }

        WindowState = WindowState == OverlayDialogState.Normal ? OverlayDialogState.Maximized : OverlayDialogState.Normal;
    }
    
    private void HandleHeaderNormalizeRequest(object? sender, EventArgs e)
    {
        SetCurrentValue(WindowStateProperty, OverlayDialogState.Normal);
    }

    private void HandleHeaderMaximizeRequest(object? sender, EventArgs e)
    {
        SetCurrentValue(WindowStateProperty, OverlayDialogState.Maximized);
    }
    
    private void HandleHeaderCloseRequest(object? sender, EventArgs e)
    {
        if (Parent is Dialog dialog)
        {
            dialog.Close();
        }
    }

    private void HandleHeaderPointerPressed(object? sender, PointerEventArgs e)
    {
        if (IsDragMovable && e.Properties.IsLeftButtonPressed)
        {
            e.Handled  = true;
            _lastPoint = e.GetPosition(this);
            e.PreventGestureRecognition();
        }
    }

    private void HandleHeaderPointerReleased(object? sender, PointerEventArgs e)
    {
        if (_lastPoint.HasValue)
        {
            e.Handled = true;
            _lastPoint = null;
            SetCurrentValue(IsDraggingProperty, false);
        }
    }

    private void HandleHeaderPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_lastPoint.HasValue && e.Properties.IsLeftButtonPressed)
        {
            var delta             = e.GetPosition(this) - _lastPoint.Value;
            var manhattanDistance = Math.Abs(delta.X) + Math.Abs(delta.Y);
            if (manhattanDistance > Constants.DragThreshold)
            {
                if (!IsDragging)
                {
                    SetCurrentValue(IsDraggingProperty, true);
                }

                HandleDragging(e.GetPosition(this), delta);
            }
        }
    }
    
    private void HandleDragging(Point position, Point delta)
    {
        var offsetX = Canvas.GetLeft(this);
        var offsetY = Canvas.GetTop(this);

        offsetX += delta.X;
        offsetY += delta.Y;

        var bounds = _dialogLayer.Bounds;
        var maxOffsetX = bounds.Width - DesiredSize.Width;
        var maxOffsetY =  bounds.Height - DesiredSize.Height;
        
        _dialog.SetCurrentValue(Dialog.HorizontalOffsetProperty, new Dimension(Math.Min(Math.Max(offsetX, 0), maxOffsetX)));
        _dialog.SetCurrentValue(Dialog.VerticalOffsetProperty, new Dimension(Math.Min(Math.Max(offsetY, 0), maxOffsetY)));
    }
    
    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        if (_lastPoint.HasValue)
        {
            _lastPoint = null;
            IsDragging = false;
        }

        base.OnPointerCaptureLost(e);
    }

    private void HandleWindowStateChanged(OverlayDialogState oldState, OverlayDialogState newState)
    {
        if (oldState == OverlayDialogState.Normal)
        {
            _originPosition = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
        }

        if (newState == OverlayDialogState.Maximized)
        {
            _dialog.SetCurrentValue(Dialog.HorizontalOffsetProperty,
                new Dimension(0));
            _dialog.SetCurrentValue(Dialog.VerticalOffsetProperty,
                new Dimension(0));
        }
        else if (newState == OverlayDialogState.Normal)
        {
            _dialog.SetCurrentValue(Dialog.HorizontalOffsetProperty, new Dimension(_originPosition.X));
            _dialog.SetCurrentValue(Dialog.VerticalOffsetProperty, new Dimension(_originPosition.Y));
        }
        if (_header != null)
        {
            _header.SetCurrentValue(OverlayDialogHeader.IsDialogMaximizedProperty, WindowState == OverlayDialogState.Maximized);
        }
    }
}