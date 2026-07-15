using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

[PseudoClasses(StdPseudoClass.Normal, StdPseudoClass.Minimized, StdPseudoClass.Maximized, StdPseudoClass.Fullscreen)]
internal class CaptionButtonGroup : TemplatedControl, IOperationSystemAware
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsFullScreenCaptionButtonVisibleProperty =
        Window.IsFullScreenCaptionButtonVisibleProperty.AddOwner<CaptionButtonGroup>();
    
    public static readonly StyledProperty<bool> IsMaximizeCaptionButtonVisibleProperty =
        AvaloniaProperty.Register<CaptionButtonGroup, bool>(nameof(IsMaximizeCaptionButtonVisible), defaultValue: true);

    public static readonly StyledProperty<bool> IsMinimizeCaptionButtonVisibleProperty =
        AvaloniaProperty.Register<CaptionButtonGroup, bool>(nameof(IsMinimizeCaptionButtonVisible), defaultValue: true);
    
    public static readonly StyledProperty<bool> IsPinCaptionButtonVisibleProperty =
        Window.IsPinCaptionButtonVisibleProperty.AddOwner<CaptionButtonGroup>();
    
    public static readonly StyledProperty<bool> IsCloseCaptionButtonVisibleProperty =
        Window.IsCloseCaptionButtonVisibleProperty.AddOwner<CaptionButtonGroup>();
    
    public static readonly StyledProperty<bool> IsWindowActiveProperty = 
        WindowTitleBar.IsWindowActiveProperty.AddOwner<CaptionButtonGroup>();
    
    public static readonly StyledProperty<OsType> OsTypeProperty =
        OperationSystemAwareControlProperty.OsTypeProperty.AddOwner<CaptionButtonGroup>();
    
    public static readonly StyledProperty<Version> OsVersionProperty =
        OperationSystemAwareControlProperty.OsVersionProperty.AddOwner<CaptionButtonGroup>();

    public bool IsFullScreenCaptionButtonVisible
    {
        get => GetValue(IsFullScreenCaptionButtonVisibleProperty);
        set => SetValue(IsFullScreenCaptionButtonVisibleProperty, value);
    }

    public bool IsMaximizeCaptionButtonVisible
    {
        get => GetValue(IsMaximizeCaptionButtonVisibleProperty);
        set => SetValue(IsMaximizeCaptionButtonVisibleProperty, value);
    }

    public bool IsMinimizeCaptionButtonVisible
    {
        get => GetValue(IsMinimizeCaptionButtonVisibleProperty);
        set => SetValue(IsMinimizeCaptionButtonVisibleProperty, value);
    }

    public bool IsPinCaptionButtonVisible
    {
        get => GetValue(IsPinCaptionButtonVisibleProperty);
        set => SetValue(IsPinCaptionButtonVisibleProperty, value);
    }
    
    public bool IsCloseCaptionButtonVisible
    {
        get => GetValue(IsCloseCaptionButtonVisibleProperty);
        set => SetValue(IsCloseCaptionButtonVisibleProperty, value);
    }
    
    public bool IsWindowActive
    {
        get => GetValue(IsWindowActiveProperty);
        set => SetValue(IsWindowActiveProperty, value);
    }
    
    public OsType OsType => GetValue(OsTypeProperty);
    public Version OsVersion => GetValue(OsVersionProperty);

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CaptionButtonGroup>();
    
    internal static readonly DirectProperty<CaptionButtonGroup, bool> IsWindowMaximizedProperty =
        AvaloniaProperty.RegisterDirect<CaptionButtonGroup, bool>(
            nameof(IsWindowMaximized),
            o => o.IsWindowMaximized,
            (o, v) => o.IsWindowMaximized = v);
    
    internal static readonly DirectProperty<CaptionButtonGroup, bool> IsWindowFullScreenProperty =
        AvaloniaProperty.RegisterDirect<CaptionButtonGroup, bool>(
            nameof(IsWindowFullScreen),
            o => o.IsWindowFullScreen,
            (o, v) => o.IsWindowFullScreen = v);
    
    internal static readonly DirectProperty<CaptionButtonGroup, bool> IsWindowPinnedProperty =
        AvaloniaProperty.RegisterDirect<CaptionButtonGroup, bool>(
            nameof(IsWindowPinned),
            o => o.IsWindowPinned,
            (o, v) => o.IsWindowPinned = v);

    internal static readonly DirectProperty<CaptionButtonGroup, bool> IsFullScreenButtonEffectivelyVisibleProperty =
        AvaloniaProperty.RegisterDirect<CaptionButtonGroup, bool>(
            nameof(IsFullScreenButtonEffectivelyVisible),
            o => o.IsFullScreenButtonEffectivelyVisible,
            (o, v) => o.IsFullScreenButtonEffectivelyVisible = v);

    internal static readonly DirectProperty<CaptionButtonGroup, bool> IsMinimizeButtonEffectivelyVisibleProperty =
        AvaloniaProperty.RegisterDirect<CaptionButtonGroup, bool>(
            nameof(IsMinimizeButtonEffectivelyVisible),
            o => o.IsMinimizeButtonEffectivelyVisible,
            (o, v) => o.IsMinimizeButtonEffectivelyVisible = v);

    internal static readonly DirectProperty<CaptionButtonGroup, bool> IsMaximizeButtonEffectivelyVisibleProperty =
        AvaloniaProperty.RegisterDirect<CaptionButtonGroup, bool>(
            nameof(IsMaximizeButtonEffectivelyVisible),
            o => o.IsMaximizeButtonEffectivelyVisible,
            (o, v) => o.IsMaximizeButtonEffectivelyVisible = v);

    internal static readonly DirectProperty<CaptionButtonGroup, bool> IsPinButtonEffectivelyVisibleProperty =
        AvaloniaProperty.RegisterDirect<CaptionButtonGroup, bool>(
            nameof(IsPinButtonEffectivelyVisible),
            o => o.IsPinButtonEffectivelyVisible,
            (o, v) => o.IsPinButtonEffectivelyVisible = v);
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    private bool _isWindowMaximized;

    internal bool IsWindowMaximized
    {
        get => _isWindowMaximized;
        set => SetAndRaise(IsWindowMaximizedProperty, ref _isWindowMaximized, value);
    }
    
    private bool _isWindowFullScreen;

    internal bool IsWindowFullScreen
    {
        get => _isWindowFullScreen;
        set => SetAndRaise(IsWindowFullScreenProperty, ref _isWindowFullScreen, value);
    }
    
    private bool _isWindowPinned;

    internal bool IsWindowPinned
    {
        get => _isWindowPinned;
        set => SetAndRaise(IsWindowPinnedProperty, ref _isWindowPinned, value);
    }

    private bool _isFullScreenButtonEffectivelyVisible;

    internal bool IsFullScreenButtonEffectivelyVisible
    {
        get => _isFullScreenButtonEffectivelyVisible;
        set => SetAndRaise(IsFullScreenButtonEffectivelyVisibleProperty, ref _isFullScreenButtonEffectivelyVisible, value);
    }

    private bool _isMinimizeButtonEffectivelyVisible = true;

    internal bool IsMinimizeButtonEffectivelyVisible
    {
        get => _isMinimizeButtonEffectivelyVisible;
        set => SetAndRaise(IsMinimizeButtonEffectivelyVisibleProperty, ref _isMinimizeButtonEffectivelyVisible, value);
    }

    private bool _isMaximizeButtonEffectivelyVisible = true;

    internal bool IsMaximizeButtonEffectivelyVisible
    {
        get => _isMaximizeButtonEffectivelyVisible;
        set => SetAndRaise(IsMaximizeButtonEffectivelyVisibleProperty, ref _isMaximizeButtonEffectivelyVisible, value);
    }

    private bool _isPinButtonEffectivelyVisible;

    internal bool IsPinButtonEffectivelyVisible
    {
        get => _isPinButtonEffectivelyVisible;
        set => SetAndRaise(IsPinButtonEffectivelyVisibleProperty, ref _isPinButtonEffectivelyVisible, value);
    }
    
    protected Window? HostWindow { get; private set; }

    #endregion

    #region 内部协作 API

    internal static bool IsPinSupportedForBackend(LinuxWindowingBackend backend)
    {
        return backend != LinuxWindowingBackend.Wayland;
    }

    #endregion
    
    private WindowState? _originWindowState;
    private WindowState? _lastWindowState;
    private CaptionButton? _fullScreenButton;
    private CaptionButton? _pinButton;
    private CaptionButton? _minimizeButton;
    private CaptionButton? _maximizeButton;
    private CaptionButton? _closeButton;
    
    private CompositeDisposable? _disposables;
    private readonly List<Action> _disposeActions = new();

    static CaptionButtonGroup()
    {
        IsFullScreenCaptionButtonVisibleProperty.Changed.AddClassHandler<CaptionButtonGroup>((group, _) => group.UpdateFullScreenButtonVisibility());
        IsWindowMaximizedProperty.Changed.AddClassHandler<CaptionButtonGroup>((group, _) => group.UpdateFullScreenButtonVisibility());
        IsMinimizeCaptionButtonVisibleProperty.Changed.AddClassHandler<CaptionButtonGroup>((group, _) => group.UpdateMinimizeButtonVisibility());
        IsMaximizeCaptionButtonVisibleProperty.Changed.AddClassHandler<CaptionButtonGroup>((group, _) => group.UpdateMaximizeButtonVisibility());
        IsPinCaptionButtonVisibleProperty.Changed.AddClassHandler<CaptionButtonGroup>((group, _) => group.UpdatePinButtonVisibility());
        IsWindowFullScreenProperty.Changed.AddClassHandler<CaptionButtonGroup>((group, _) =>
        {
            group.UpdateMinimizeButtonVisibility();
            group.UpdateMaximizeButtonVisibility();
        });
    }

    public CaptionButtonGroup()
    {
        this.ConfigureOsType();
        UpdateCaptionButtonVisibility();
    }

    public virtual void Attach(Window hostWindow)
    {
        if (_disposables != null)
        {
            return;
        }

        HostWindow = hostWindow;
        
        _disposables = new CompositeDisposable(8);
        hostWindow.Opened += HandleHostWindowOpened;
        _disposables.Add(Disposable.Create(() => hostWindow.Opened -= HandleHostWindowOpened));
        _disposables.Add(BindUtils.RelayBind(hostWindow, Window.IsFullScreenCaptionButtonVisibleProperty, this, IsFullScreenCaptionButtonVisibleProperty));
        _disposables.Add(BindUtils.RelayBind(hostWindow, Window.IsPinCaptionButtonVisibleProperty, this, IsPinCaptionButtonVisibleProperty));
        _disposables.Add(BindUtils.RelayBind(hostWindow, Window.CanMaximizeProperty, this, IsMaximizeCaptionButtonVisibleProperty));
        _disposables.Add(BindUtils.RelayBind(hostWindow, Window.CanMinimizeProperty, this, IsMinimizeCaptionButtonVisibleProperty));
        _disposables.Add(BindUtils.RelayBind(hostWindow, Window.IsCloseCaptionButtonVisibleProperty, this, IsCloseCaptionButtonVisibleProperty));
        _disposables.Add(HostWindow.GetObservable(Window.WindowStateProperty)
                                   .Subscribe(HandleWindowStateChanged));
        _disposables.Add(HostWindow.GetObservable(Window.TopmostProperty)
                                   .Subscribe(x =>
                                   {
                                       IsWindowPinned = HostWindow.Topmost;
                                   }));
        UpdatePinButtonVisibility();
    }

    public virtual void Detach()
    {
        if (_disposables == null)
        {
            return;
        }
        _disposables.Dispose();
        DisposeTemplateHandlers();
        _disposables    = null;
        HostWindow      = null;
        _lastWindowState = null;
        UpdatePinButtonVisibility();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        DisposeTemplateHandlers();
        
        _closeButton      = e.NameScope.Find<CaptionButton>("PART_CloseButton");
        _maximizeButton   = e.NameScope.Find<CaptionButton>("PART_MaximizeButton");
        _minimizeButton   = e.NameScope.Find<CaptionButton>("PART_MinimizeButton");
        _pinButton        = e.NameScope.Find<CaptionButton>("PART_PinButton");
        _fullScreenButton = e.NameScope.Find<CaptionButton>("PART_FullScreenButton");

        if (_closeButton != null)
        {
            _closeButton.Click += HandleCloseButtonClicked;
            _disposeActions.Add(() => _closeButton.Click -= HandleCloseButtonClicked);
        }

        if (_maximizeButton != null)
        {
            _maximizeButton.Click += HandleMaximizeButtonClicked;
            _disposeActions.Add(() => _maximizeButton.Click -= HandleMaximizeButtonClicked);
        }

        if (_fullScreenButton != null)
        {
            _fullScreenButton.Click += HandleFullScreenButtonClicked;
            _disposeActions.Add(() => _fullScreenButton.Click -= HandleFullScreenButtonClicked);
        }

        if (_minimizeButton != null)
        {
            _minimizeButton.Click += HandleMinimizeButtonClicked;
            _disposeActions.Add(() => _minimizeButton.Click -= HandleMinimizeButtonClicked);
        }

        if (_pinButton != null)
        {
            _pinButton.Click += HandlePinButtonClicked;
            _disposeActions.Add(() => _pinButton.Click -= HandlePinButtonClicked);
        }
    }

    private void UpdateCaptionButtonVisibility()
    {
        UpdateFullScreenButtonVisibility();
        UpdateMinimizeButtonVisibility();
        UpdateMaximizeButtonVisibility();
        UpdatePinButtonVisibility();
    }

    private void HandleWindowStateChanged(WindowState windowState)
    {
        var stateChanged = _lastWindowState.HasValue && _lastWindowState.Value != windowState;
        _lastWindowState = windowState;

        PseudoClasses.Set(StdPseudoClass.Minimized, windowState == WindowState.Minimized);
        PseudoClasses.Set(StdPseudoClass.Normal, windowState == WindowState.Normal);
        PseudoClasses.Set(StdPseudoClass.Maximized, windowState == WindowState.Maximized);
        PseudoClasses.Set(StdPseudoClass.Fullscreen, windowState == WindowState.FullScreen);
        IsWindowMaximized  = windowState == WindowState.Maximized;
        IsWindowFullScreen = windowState == WindowState.FullScreen;

        if (stateChanged)
        {
            InvalidateWindowsCaptionButtonPointerOverVisualStates();
        }
    }

    private void InvalidateWindowsCaptionButtonPointerOverVisualStates()
    {
        InvalidateWindowsCaptionButtonPointerOverVisualState(_fullScreenButton);
        InvalidateWindowsCaptionButtonPointerOverVisualState(_pinButton);
        InvalidateWindowsCaptionButtonPointerOverVisualState(_minimizeButton);
        InvalidateWindowsCaptionButtonPointerOverVisualState(_maximizeButton);
        InvalidateWindowsCaptionButtonPointerOverVisualState(_closeButton);
    }

    private static void InvalidateWindowsCaptionButtonPointerOverVisualState(CaptionButton? button)
    {
        if (button is WindowsCaptionButton windowsCaptionButton)
        {
            windowsCaptionButton.InvalidatePointerOverVisualState();
        }
    }

    private void HandleHostWindowOpened(object? sender, EventArgs args)
    {
        UpdatePinButtonVisibility();
    }

    private void UpdateFullScreenButtonVisibility()
    {
        IsFullScreenButtonEffectivelyVisible = IsFullScreenCaptionButtonVisible && !IsWindowMaximized;
    }

    private void UpdateMinimizeButtonVisibility()
    {
        IsMinimizeButtonEffectivelyVisible = IsMinimizeCaptionButtonVisible && !IsWindowFullScreen;
    }

    private void UpdateMaximizeButtonVisibility()
    {
        IsMaximizeButtonEffectivelyVisible = IsMaximizeCaptionButtonVisible && !IsWindowFullScreen;
    }

    private void UpdatePinButtonVisibility()
    {
        IsPinButtonEffectivelyVisible = IsPinCaptionButtonVisible && IsPinSupportedByCurrentBackend();
    }

    private bool IsPinSupportedByCurrentBackend()
    {
        if (!OperatingSystem.IsLinux())
        {
            return true;
        }

        var configuredPlatform = AvaloniaLocator.Current.GetService<AtomUIWindowingPlatformOptions>()?.Platform;
        var platformImpl       = HostWindow?.PlatformImpl;
        var backend = LinuxWindowChromeManager.ResolveBackend(
            configuredPlatform,
            platformImpl?.Handle?.HandleDescriptor,
            platformImpl?.GetType().Assembly.GetName().Name);
        return IsPinSupportedForBackend(backend);
    }

    private void DisposeTemplateHandlers()
    {
        foreach (var disposeAction in _disposeActions)
        {
            disposeAction.Invoke();
        }
        _disposeActions.Clear();
    }
    
    private void HandleFullScreenButtonClicked(object? sender, RoutedEventArgs args)
    {
        if (HostWindow == null)
        {
            return;
        }

        if (!IsWindowFullScreen)
        {
            _originWindowState = HostWindow.WindowState;
        }
        HostWindow.WindowState = IsWindowFullScreen
            ? _originWindowState ?? WindowState.Normal
            : WindowState.FullScreen;
    }

    private void HandleMaximizeButtonClicked(object? sender, RoutedEventArgs args)
    {
        if (HostWindow == null)
        {
            return;
        }
        var windowState = HostWindow.WindowState;
        if (!HostWindow.CanMaximize || windowState == WindowState.FullScreen)
        {
            return;
        }
        HostWindow.WindowState = windowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }
    
    private void HandleCloseButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (HostWindow != null)
        {
            HostWindow.NotifyCloseRequestByUser();
        }
        HostWindow?.Close();
    }
    
    private void HandleMinimizeButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (HostWindow == null)
        {
            return;
        }
        HostWindow.WindowState = WindowState.Minimized;
    }

    private void HandlePinButtonClicked(object? sender, RoutedEventArgs args)
    {
        if (HostWindow == null || !IsPinButtonEffectivelyVisible)
        {
            return;
        }
        HostWindow.Topmost = !HostWindow.Topmost;
        IsWindowPinned = HostWindow.Topmost;
    }
    
    void IOperationSystemAware.SetOsType(OsType osType)
    {
        SetValue(OsTypeProperty, osType);
    }
    
    void IOperationSystemAware.SetOsVersion(Version version)
    {
        SetValue(OsVersionProperty, version);
    }
}
