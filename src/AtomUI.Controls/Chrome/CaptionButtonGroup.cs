using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.Normal, StdPseudoClass.Minimized, StdPseudoClass.Maximized, StdPseudoClass.Fullscreen)]
public class CaptionButtonGroup : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsFullScreenEnabledProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsFullScreenEnabled), true);

    public static readonly StyledProperty<bool> IsMaximizeEnabledProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsMaximizeEnabled), defaultValue: true);

    public static readonly StyledProperty<bool> IsMinimizeEnabledProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsMinimizeEnabled), defaultValue: true);

    public static readonly StyledProperty<bool> IsPinEnabledProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsPinEnabled), true);
    
    public static readonly StyledProperty<bool> IsWindowActiveProperty = 
        TitleBar.IsWindowActiveProperty.AddOwner<CaptionButtonGroup>();

    public bool IsFullScreenEnabled
    {
        get => GetValue(IsFullScreenEnabledProperty);
        set => SetValue(IsFullScreenEnabledProperty, value);
    }

    public bool IsMaximizeEnabled
    {
        get => GetValue(IsMaximizeEnabledProperty);
        set => SetValue(IsMaximizeEnabledProperty, value);
    }

    public bool IsMinimizeEnabled
    {
        get => GetValue(IsMinimizeEnabledProperty);
        set => SetValue(IsMinimizeEnabledProperty, value);
    }

    public bool IsPinEnabled
    {
        get => GetValue(IsPinEnabledProperty);
        set => SetValue(IsPinEnabledProperty, value);
    }
    
    public bool IsWindowActive
    {
        get => GetValue(IsWindowActiveProperty);
        set => SetValue(IsWindowActiveProperty, value);
    }

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
    
    protected Window? HostWindow { get; private set; }

    #endregion
    
    private WindowState? _originWindowState;
    private CaptionButton? _fullScreenButton;
    private CaptionButton? _pinButton;
    private CaptionButton? _minimizeButton;
    private CaptionButton? _maximizeButton;
    private CaptionButton? _closeButton;
    
    private IDisposable? _disposables;
    private readonly List<Action> _disposeActions = new();

    static CaptionButtonGroup()
    {
        AffectsArrange<CaptionButtonGroup>(IsWindowMaximizedProperty,
            IsWindowFullScreenProperty,
            IsMaximizeEnabledProperty,
            IsPinEnabledProperty,
            IsMinimizeEnabledProperty);
    }

    public virtual void Attach(Window hostWindow)
    {
        if (_disposables != null)
        {
            return;
        }
           
        HostWindow = hostWindow;
        _disposables = new CompositeDisposable
        {
            HostWindow.GetObservable(Window.WindowStateProperty)
                      .Subscribe(x =>
                      {
                          PseudoClasses.Set(StdPseudoClass.Minimized, x == WindowState.Minimized);
                          PseudoClasses.Set(StdPseudoClass.Normal, x == WindowState.Normal);
                          PseudoClasses.Set(StdPseudoClass.Maximized, IsWindowMaximized);
                          PseudoClasses.Set(StdPseudoClass.Fullscreen, x == WindowState.FullScreen);
                      })
        };
        IsWindowPinned     = HostWindow.Topmost;
        IsWindowMaximized  = HostWindow.WindowState == WindowState.Maximized;
        IsWindowFullScreen = HostWindow.WindowState == WindowState.FullScreen;
    }

    public virtual void Detach()
    {
        if (_disposables == null)
        {
            return;
        }
        _disposables.Dispose();
        foreach (var disposeAction in _disposeActions)
        {
            disposeAction.Invoke();
        }
        _disposables = null;
        HostWindow   = null;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        foreach (var disposeAction in _disposeActions)
        {
            disposeAction.Invoke();
        }
        _disposeActions.Clear();
        
        _closeButton      = e.NameScope.Find<CaptionButton>(CaptionButtonGroupThemeConstants.CloseButtonPart);
        _maximizeButton   = e.NameScope.Find<CaptionButton>(CaptionButtonGroupThemeConstants.MaximizeButtonPart);
        _minimizeButton   = e.NameScope.Find<CaptionButton>(CaptionButtonGroupThemeConstants.MinimizeButtonPart);
        _pinButton        = e.NameScope.Find<CaptionButton>(CaptionButtonGroupThemeConstants.PinButtonPart);
        _fullScreenButton = e.NameScope.Find<CaptionButton>(CaptionButtonGroupThemeConstants.FullScreenButtonPart);

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
    
    private void HandleFullScreenButtonClicked(object? sender, RoutedEventArgs args)
    {
        if (HostWindow == null)
        {
            return;
        }

        if (!_isWindowFullScreen)
        {
            _originWindowState = HostWindow.WindowState;
        }
        else
        {
            HostWindow.WindowState = WindowState.FullScreen;
        }
        HostWindow.WindowState = _isWindowFullScreen
            ? _originWindowState ?? WindowState.Normal
            : WindowState.FullScreen;
        IsWindowFullScreen = HostWindow.WindowState == WindowState.FullScreen;
        IsWindowMaximized  = HostWindow.WindowState == WindowState.Maximized;
    }

    private void HandleMaximizeButtonClicked(object? sender, RoutedEventArgs args)
    {
        if (HostWindow == null)
        {
            return;
        }
        var windowState = HostWindow.WindowState;
        if (!HostWindow.IsMaximizeEnabled || windowState == WindowState.FullScreen)
        {
            return;
        }
        HostWindow.WindowState = windowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
        IsWindowFullScreen = HostWindow.WindowState == WindowState.FullScreen;
        IsWindowMaximized  = HostWindow.WindowState == WindowState.Maximized;
    }
    
    private void HandleCloseButtonClicked(object? sender, RoutedEventArgs e)
    {
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
        if (HostWindow == null || !HostWindow.IsPinEnabled)
        {
            return;
        }
        HostWindow.Topmost = !HostWindow.Topmost;
        IsWindowPinned = HostWindow.Topmost;
    }
}