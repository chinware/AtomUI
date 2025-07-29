// Referenced from https://github.com/kikipoulet/SukiUI project

using System.Runtime.InteropServices;
using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaWindow = Avalonia.Controls.Window;

public class Window : AvaloniaWindow, IDisposable
{
    #region 公共属性

    public static readonly StyledProperty<bool> IsTitleBarVisibleProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsTitleBarVisible), defaultValue: true);
    
    public static readonly StyledProperty<double> MaxWidthScreenRatioProperty =
        AvaloniaProperty.Register<Window, double>(nameof(MaxWidthScreenRatio), double.NaN);
    
    public static readonly StyledProperty<double> MaxHeightScreenRatioProperty =
        AvaloniaProperty.Register<Window, double>(nameof(MaxHeightScreenRatio), double.NaN);
    
    public static readonly StyledProperty<double> TitleFontSizeProperty =
        AvaloniaProperty.Register<Window, double>(nameof(TitleFontSize), defaultValue: 13);
    
    public static readonly StyledProperty<FontWeight> TitleFontWeightProperty =
        AvaloniaProperty.Register<Window, FontWeight>(nameof(TitleFontWeight), defaultValue: FontWeight.Bold);
    
    public static readonly StyledProperty<ContextMenu> TitleBarContextMenuProperty =
        AvaloniaProperty.Register<Window, ContextMenu>(nameof(TitleBarContextMenu));
    
    public static readonly StyledProperty<Control?> LogoProperty =
        TitleBar.LogoProperty.AddOwner<Window>();
    
    public static readonly StyledProperty<bool> IsFullScreenEnabledProperty =
        CaptionButtonGroup.IsFullScreenEnabledProperty.AddOwner<Window>();
    
    public static readonly StyledProperty<bool> IsMaximizeEnabledProperty =
        CaptionButtonGroup.IsMaximizeEnabledProperty.AddOwner<Window>();
    
    public static readonly StyledProperty<bool> IsMinimizeEnabledProperty =
        CaptionButtonGroup.IsMinimizeEnabledProperty.AddOwner<Window>();
    
    public static readonly StyledProperty<bool> IsPinEnabledProperty =
        CaptionButtonGroup.IsPinEnabledProperty.AddOwner<Window>();
    
    public static readonly StyledProperty<bool> IsMoveEnabledProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsMoveEnabled), defaultValue: true);
    
    public static readonly StyledProperty<BoxShadows> FrameShadowsProperty =
        AvaloniaProperty.Register<Window, BoxShadows>(nameof(FrameShadows));
    
    public double TitleFontSize
    {
        get => GetValue(TitleFontSizeProperty);
        set => SetValue(TitleFontSizeProperty, value);
    }
    
    public FontWeight TitleFontWeight
    {
        get => GetValue(TitleFontWeightProperty);
        set => SetValue(TitleFontWeightProperty, value);
    }
    
    public bool IsTitleBarVisible
    {
        get => GetValue(IsTitleBarVisibleProperty);
        set => SetValue(IsTitleBarVisibleProperty, value);
    }
    
    public double MaxWidthScreenRatio
    {
        get => GetValue(MaxWidthScreenRatioProperty);
        set => SetValue(MaxWidthScreenRatioProperty, value);
    }
    
    public double MaxHeightScreenRatio
    {
        get => GetValue(MaxHeightScreenRatioProperty);
        set => SetValue(MaxHeightScreenRatioProperty, value);
    }
    
    public ContextMenu TitleBarContextMenu
    {
        get => GetValue(TitleBarContextMenuProperty);
        set => SetValue(TitleBarContextMenuProperty, value);
    }
    
    public Control? Logo
    {
        get => GetValue(LogoProperty);
        set => SetValue(LogoProperty, value);
    }
    
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
    
    public bool IsMoveEnabled
    {
        get => GetValue(IsMoveEnabledProperty);
        set => SetValue(IsMoveEnabledProperty, value);
    }

    public bool IsFrameShadowsEnabled { get; init; } = true;
    
    public BoxShadows FrameShadows
    {
        get => GetValue(FrameShadowsProperty);
        set => SetValue(FrameShadowsProperty, value);
    }
    #endregion

    #region 内部属性定义

    public static readonly DirectProperty<Window, WindowState> PreviousVisibleWindowStateProperty =
        AvaloniaProperty.RegisterDirect<Window, WindowState>(
            nameof(PreviousVisibleWindowState),
            o => o.PreviousVisibleWindowState);
    private WindowState _previousVisibleWindowState = WindowState.Normal;

    public WindowState PreviousVisibleWindowState
    {
        get => _previousVisibleWindowState;
        private set => SetAndRaise(PreviousVisibleWindowStateProperty, ref _previousVisibleWindowState, value);
    }
    #endregion
    
    protected override Type StyleKeyOverride { get; } = typeof(Window);
    private WindowResizer? _windowResizer;
    private readonly List<Action> _disposeActions = new();
    private TitleBar? _titleBar;
    private bool _isDisposed;
    private Point? _lastMousePressedPoint;
    private PointerPressedEventArgs? _lastMousePressedEventArgs;
    private bool _isDragging;

    public Window()
    {
        ScalingChanged += HandleScalingChanged;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == MaxWidthScreenRatioProperty)
        {
            this.ConstrainMaxSizeToScreenRatio(MaxWidthScreenRatio, double.NaN);
        }
        else if (change.Property == MaxHeightScreenRatioProperty)
        {
            this.ConstrainMaxSizeToScreenRatio(double.NaN, MaxHeightScreenRatio);
        }
        if (change.Property == WindowStateProperty)
        {
            if (change.OldValue is not WindowState oldWindowState ||
                change.NewValue is not WindowState newWindowState)
            {
                return;
            }
            HandleWindowStateChanged(oldWindowState, newWindowState);
        }
    }
    
    private void HandleWindowStateChanged(WindowState oldState, WindowState newState)
    {
        if (oldState != WindowState.Minimized)
        {
            PreviousVisibleWindowState = oldState;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Margin = new Thickness(newState == WindowState.Maximized
                ? 7
                : 0);
        }

        this.ConstrainMaxSizeToScreenRatio(MaxWidthScreenRatio, MaxHeightScreenRatio);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _windowResizer = e.NameScope.Find<WindowResizer>(WindowThemeConstants.WindowResizerPart);
        if (_windowResizer != null)
        {
            _windowResizer.TargetWindow = this;
        }
        _titleBar = e.NameScope.Find<TitleBar>(WindowThemeConstants.TitleBarPart);
        if (_titleBar != null)
        {
            _titleBar.DoubleTapped    += HandleTitleDoubleClicked;
            _titleBar.PointerPressed  += HandleTitleBarPointerPressed;
            _titleBar.PointerReleased += HandleTitleBarPointerReleased;
            _titleBar.PointerMoved    += HandleTitleBarPointerMoved;
            _disposeActions.Add(() =>
            {
                _titleBar.DoubleTapped    -= HandleTitleDoubleClicked;
                _titleBar.PointerPressed  -= HandleTitleBarPointerPressed;
                _titleBar.PointerReleased -= HandleTitleBarPointerReleased;
                _titleBar.PointerMoved    -= HandleTitleBarPointerMoved;
            });
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        Dispose();
        base.OnClosed(e);
    }
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            return;
        }
        
        if (desktop.MainWindow is Window window && window != this)
        {
            Icon ??= window.Icon;
        }
    }

    private void HandleScalingChanged(object? sender, EventArgs e)
    {
        this.ConstrainMaxSizeToScreenRatio(MaxWidthScreenRatio, MaxHeightScreenRatio);
    }

    private void HandleTitleDoubleClicked(object? sender, RoutedEventArgs e)
    {
        var windowState = WindowState;
        if (!IsMaximizeEnabled || windowState == WindowState.FullScreen)
        {
            return;
        }
        WindowState = windowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void HandleTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!IsMoveEnabled || WindowState == WindowState.FullScreen)
        {
            return;
        }
        _lastMousePressedPoint     = e.GetPosition(this);
        _lastMousePressedEventArgs = e;
    }

    private void HandleTitleBarPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!IsMoveEnabled || WindowState == WindowState.FullScreen || !e.Properties.IsLeftButtonPressed)
        {
            return;
        }
        Point mousePosition       = e.GetPosition(this);
        if (_lastMousePressedPoint != null)
        {
            var   distanceFromInitial = (Vector)(mousePosition - _lastMousePressedPoint);
            if (distanceFromInitial.Length > Constants.DragThreshold)
            {
                _isDragging = true;
                if (_lastMousePressedEventArgs is not null)
                {
                    BeginMoveDrag(_lastMousePressedEventArgs);
                }
            }
        }
    }
    
    private void HandleTitleBarPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!IsMoveEnabled || e.InitialPressMouseButton != MouseButton.Left || !_isDragging)
        {
            return;
        }
        this.ConstrainMaxSizeToScreenRatio(MaxWidthScreenRatio, MaxHeightScreenRatio);
        _lastMousePressedPoint     = null;
        _lastMousePressedEventArgs = null;
        _isDragging                = false;
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }
        _isDisposed     =  true;
        
        ScalingChanged -= HandleScalingChanged;
        foreach (var disposeAction in _disposeActions)
        {
            disposeAction.Invoke();
        }
    }
}
