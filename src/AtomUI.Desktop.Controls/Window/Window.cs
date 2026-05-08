using System.Runtime.Versioning;
using AtomUI.Controls;
using AtomUI.Media;
using AtomUI.Native;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

using AvaloniaWindow = Avalonia.Controls.Window;

public class Window : AvaloniaWindow, 
                      IOperationSystemAware, 
                      IMediaBreakAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<object?> LogoProperty =
        WindowTitleBar.LogoProperty.AddOwner<Window>();
    
    public static readonly StyledProperty<IDataTemplate?> LogoTemplateProperty =
        WindowTitleBar.LogoTemplateProperty.AddOwner<Window>();
    
    public static readonly StyledProperty<bool> IsTitleBarVisibleProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsTitleBarVisible), defaultValue: true);
    
    public static readonly StyledProperty<object?> WindowFrameLayerProperty =
        AvaloniaProperty.Register<Window, object?>(nameof(WindowFrameLayer));
    
    public static readonly StyledProperty<IDataTemplate?> WindowFrameLayerTemplateProperty =
        AvaloniaProperty.Register<Window, IDataTemplate?>(nameof(WindowFrameLayerTemplate));
    
    public static readonly StyledProperty<double> WindowFrameLayerOpacityProperty =
        AvaloniaProperty.Register<Window, double>(nameof(WindowFrameLayerOpacity), 1.0);
    
    public static readonly StyledProperty<object?> ContentFrameLayerProperty =
        AvaloniaProperty.Register<Window, object?>(nameof(ContentFrameLayer));
    
    public static readonly StyledProperty<IDataTemplate?> ContentFrameLayerTemplateProperty =
        AvaloniaProperty.Register<Window, IDataTemplate?>(nameof(ContentFrameLayerTemplate));
    
    public static readonly StyledProperty<double> ContentFrameLayerOpacityProperty =
        AvaloniaProperty.Register<Window, double>(nameof(ContentFrameLayerOpacity), 1.0);
    
    public static readonly StyledProperty<IBrush?> ContentFrameBackgroundProperty =
        AvaloniaProperty.Register<Window, IBrush?>(nameof(ContentFrameBackground));
    
    public static readonly StyledProperty<object?> TitleBarFrameLayerProperty =
        AvaloniaProperty.Register<Window, object?>(nameof(TitleBarFrameLayer));
    
    public static readonly StyledProperty<IDataTemplate?> TitleBarFrameLayerTemplateProperty =
        AvaloniaProperty.Register<Window, IDataTemplate?>(nameof(TitleBarFrameLayerTemplate));
    
    public static readonly StyledProperty<double> TitleBarFrameLayerOpacityProperty =
        AvaloniaProperty.Register<Window, double>(nameof(TitleBarFrameLayerOpacity), 1.0);
    
    public static readonly StyledProperty<IBrush?> TitleBarFrameBackgroundProperty =
        AvaloniaProperty.Register<Window, IBrush?>(nameof(TitleBarFrameBackground));
    
    public static readonly StyledProperty<bool> IsFullScreenCaptionButtonVisibleProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsFullScreenCaptionButtonVisible));

    public static readonly StyledProperty<bool> IsPinCaptionButtonVisibleProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsPinCaptionButtonVisible));
    
    public static readonly StyledProperty<bool> IsCloseCaptionButtonVisibleProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsCloseCaptionButtonVisible), true);
    
    public static readonly StyledProperty<bool> IsMoveEnabledProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsMoveEnabled), defaultValue: true);
    
    public static readonly StyledProperty<MediaBreakPoint> MediaBreakPointProperty = 
        MediaBreakAwareControlProperty.MediaBreakPointProperty.AddOwner<Window>();
    
    public static readonly StyledProperty<OsType> OsTypeProperty =
        OperationSystemAwareControlProperty.OsTypeProperty.AddOwner<Window>();
    
    public static readonly StyledProperty<Version> OsVersionProperty =
        OperationSystemAwareControlProperty.OsVersionProperty.AddOwner<Window>();
    
    public bool IsTitleBarVisible
    {
        get => GetValue(IsTitleBarVisibleProperty);
        set => SetValue(IsTitleBarVisibleProperty, value);
    }
    
    [DependsOn(nameof(LogoTemplate))]
    public object? Logo
    {
        get => GetValue(LogoProperty);
        set => SetValue(LogoProperty, value);
    }
    
    public IDataTemplate? LogoTemplate
    {
        get => GetValue(LogoTemplateProperty);
        set => SetValue(LogoTemplateProperty, value);
    }
    
    [DependsOn(nameof(WindowFrameLayerTemplate))]
    public object? WindowFrameLayer
    {
        get => GetValue(WindowFrameLayerProperty);
        set => SetValue(WindowFrameLayerProperty, value);
    }
    
    public object? WindowFrameLayerTemplate
    {
        get => GetValue(WindowFrameLayerTemplateProperty);
        set => SetValue(WindowFrameLayerTemplateProperty, value);
    }
    
    public double WindowFrameLayerOpacity
    {
        get => GetValue(WindowFrameLayerOpacityProperty);
        set => SetValue(WindowFrameLayerOpacityProperty, value);
    }
    
    [DependsOn(nameof(ContentFrameLayerTemplate))]
    public object? ContentFrameLayer
    {
        get => GetValue(ContentFrameLayerProperty);
        set => SetValue(ContentFrameLayerProperty, value);
    }
    
    public object? ContentFrameLayerTemplate
    {
        get => GetValue(ContentFrameLayerTemplateProperty);
        set => SetValue(ContentFrameLayerTemplateProperty, value);
    }
    
    public double ContentFrameLayerOpacity
    {
        get => GetValue(ContentFrameLayerOpacityProperty);
        set => SetValue(ContentFrameLayerOpacityProperty, value);
    }
    
    public IBrush? ContentFrameBackground
    {
        get => GetValue(ContentFrameBackgroundProperty);
        set => SetValue(ContentFrameBackgroundProperty, value);
    }
    
    [DependsOn(nameof(TitleBarFrameLayerTemplate))]
    public object? TitleBarFrameLayer
    {
        get => GetValue(TitleBarFrameLayerProperty);
        set => SetValue(TitleBarFrameLayerProperty, value);
    }
    
    public object? TitleBarFrameLayerTemplate
    {
        get => GetValue(TitleBarFrameLayerTemplateProperty);
        set => SetValue(TitleBarFrameLayerTemplateProperty, value);
    }
    
    public double TitleBarFrameLayerOpacity
    {
        get => GetValue(TitleBarFrameLayerOpacityProperty);
        set => SetValue(TitleBarFrameLayerOpacityProperty, value);
    }
    
    public IBrush? TitleBarFrameBackground
    {
        get => GetValue(TitleBarFrameBackgroundProperty);
        set => SetValue(TitleBarFrameBackgroundProperty, value);
    }
    
    public bool IsFullScreenCaptionButtonVisible
    {
        get => GetValue(IsFullScreenCaptionButtonVisibleProperty);
        set => SetValue(IsFullScreenCaptionButtonVisibleProperty, value);
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
    
    public bool IsMoveEnabled
    {
        get => GetValue(IsMoveEnabledProperty);
        set => SetValue(IsMoveEnabledProperty, value);
    }
    
    public MediaBreakPoint MediaBreakPoint
    {
        get => GetValue(MediaBreakPointProperty);
        internal set => SetValue(MediaBreakPointProperty, value);
    }
    
    public OsType OsType => GetValue(OsTypeProperty);
    public Version OsVersion => GetValue(OsVersionProperty);
    
    #endregion
    
    #region 公共事件定义
    
    public event EventHandler<MediaBreakPointChangedEventArgs>? MediaBreakPointChanged;

    #endregion
    
    #region 内部属性定义
    internal static readonly DirectProperty<Window, Thickness> TitleBarOffsetMarginProperty = 
        AvaloniaProperty.RegisterDirect<Window, Thickness>(nameof (TitleBarOffsetMargin), 
            o => o.TitleBarOffsetMargin,
            (o, v) => o.TitleBarOffsetMargin = v);
    
    internal static readonly DirectProperty<Window, WindowTitleBar?> TitleBarProperty =
        AvaloniaProperty.RegisterDirect<Window, WindowTitleBar?>(
            nameof(TitleBar),
            o => o.TitleBar,
            (o, v) => o.TitleBar = v);
    
    internal static readonly DirectProperty<Window, bool> IsCsdEnabledProperty =
        AvaloniaProperty.RegisterDirect<Window, bool>(
            nameof(IsCsdEnabled),
            o => o.IsCsdEnabled,
            (o, v) => o.IsCsdEnabled = v);

    internal static readonly DirectProperty<Window, bool> IsCustomResizerVisibleProperty =
        AvaloniaProperty.RegisterDirect<Window, bool>(
            nameof(IsCustomResizerVisible),
            o => o.IsCustomResizerVisible,
            (o, v) => o.IsCustomResizerVisible = v);
    
    internal static readonly StyledProperty<double> TitleBarHeightProperty =
        AvaloniaProperty.Register<Window, double>(nameof(TitleBarHeight));
    
    internal static readonly StyledProperty<BoxShadows> FrameShadowProperty =
        AvaloniaProperty.Register<Window, BoxShadows>(nameof(FrameShadow));
    
    internal static readonly StyledProperty<Thickness> FrameShadowThicknessProperty =
        AvaloniaProperty.Register<Window, Thickness>(nameof(FrameShadowThickness));
    
    private Thickness _titleBarOffsetMargin;
    internal Thickness TitleBarOffsetMargin
    {
        get => _titleBarOffsetMargin;
        private set => SetAndRaise(TitleBarOffsetMarginProperty, ref _titleBarOffsetMargin, value);
    }
    
    private WindowTitleBar? _titleBar;

    internal WindowTitleBar? TitleBar
    {
        get => _titleBar;
        set => SetAndRaise(TitleBarProperty, ref _titleBar, value);
    }
    
    private bool _isCsdEnabled;

    internal bool IsCsdEnabled
    {
        get => _isCsdEnabled;
        set => SetAndRaise(IsCsdEnabledProperty, ref _isCsdEnabled, value);
    }

    private bool _isCustomResizerVisible;

    internal bool IsCustomResizerVisible
    {
        get => _isCustomResizerVisible;
        set => SetAndRaise(IsCustomResizerVisibleProperty, ref _isCustomResizerVisible, value);
    }
    
    internal double TitleBarHeight
    {
        get => GetValue(TitleBarHeightProperty);
        set => SetValue(TitleBarHeightProperty, value);
    }
    
    internal BoxShadows FrameShadow
    {
        get => GetValue(FrameShadowProperty);
        set => SetValue(FrameShadowProperty, value);
    }
    
    internal Thickness FrameShadowThickness
    {
        get => GetValue(FrameShadowThicknessProperty);
        set => SetValue(FrameShadowThicknessProperty, value);
    }
    #endregion
    
    protected override Type StyleKeyOverride { get; } = typeof(Window);
    private protected bool CloseByClickCloseCaptionButton;
    private Point? _lastMousePressedPoint;
    private PointerPressedEventArgs? _lastMousePressedEventArgs;
    private bool _isDragging;
    private bool _wasFullScreen;
    private FullscreenPopoverLayer? _fullscreenPopoverLayer;
    private WindowResizer? _windowResizer;

    // macOS 下 ConfigureMacOsWindow 的输入缓存，用于在 live resize 时短路，避免重复 P/Invoke
    private double? _macOsCachedTitleBarHeight;
    private double? _macOsCachedOffsetX;
    private double? _macOsCachedSpacing;
    private Size _macOsCachedClientSize;
    private bool _macOsCacheValid;
    
    static Window()
    {
        AffectsRender<Window>(TitleBarFrameBackgroundProperty, 
            ContentFrameBackgroundProperty);
        ConfigureOsType();
        FrameShadowProperty.Changed.AddClassHandler<Window>((window, args) => window.FrameShadowThickness = args.GetNewValue<BoxShadows>().Thickness());
    }

    public Window()
    {
        ConfigureCsdStatus();
        if (OperatingSystem.IsLinux())
        {
            this.AttachClickThroughShadow(
                new HashSet<AvaloniaProperty> { FrameShadowThicknessProperty },
                () => FrameShadowThickness);
        }
    }
    
    private static void ConfigureOsType()
    {
        if (OperatingSystem.IsWindows())
        {
            OsTypeProperty.OverrideDefaultValue<Window>(OsType.Windows);
        }
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
        {
            OsTypeProperty.OverrideDefaultValue<Window>(OsType.macOS);
        }
        else if (OperatingSystem.IsLinux())
        {
            OsTypeProperty.OverrideDefaultValue<Window>(OsType.Linux);
        }
        else
        {
            OsTypeProperty.OverrideDefaultValue<Window>(OsType.Unknown);
        }
        OsVersionProperty.OverrideDefaultValue<Window>(Environment.OSVersion.Version);
    }
    
    void IOperationSystemAware.SetOsType(OsType osType)
    {
        SetValue(OsTypeProperty, osType);
    }
    
    void IOperationSystemAware.SetOsVersion(Version version)
    {
        SetValue(OsVersionProperty, version);
    }
    
    internal void NotifyMediaBreakPointChanged(MediaBreakPoint breakPoint)
    {
        SetCurrentValue(MediaBreakPointProperty, breakPoint);
        MediaBreakPointChanged?.Invoke(this, new MediaBreakPointChangedEventArgs(breakPoint));
    }
    
    internal void NotifyCloseRequestByUser()
    {
        CloseByClickCloseCaptionButton = true;
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        HandleCreateTitleBar();

        var mediaQueryIndicator = e.NameScope.Find<WindowMediaQueryIndicator>("PART_MediaQueryIndicator");
        if (mediaQueryIndicator != null)
        {
            mediaQueryIndicator.OwnerWindow = this;
        }

        _windowResizer = e.NameScope.Find<WindowResizer>("PART_WindowResizer");
        if (_windowResizer != null)
        {
            _windowResizer.TargetWindow = this;
        }

        ConfigureCustomResizerVisible();

        if (!IsCsdEnabled)
        {
            _fullscreenPopoverLayer?.Detach();
            _fullscreenPopoverLayer = e.NameScope.Find<FullscreenPopoverLayer>("PART_FullscreenPopoverLayer");
            _fullscreenPopoverLayer?.Attach(this);
        }
    }

    private void HandleCreateTitleBar()
    {
        if (_titleBar != null)
        {
            _titleBar.MaximizeWindowRequested -= HandleTitleDoubleClicked;
            _titleBar.PointerPressed          -= HandleTitleBarPointerPressed;
            _titleBar.PointerReleased         -= HandleTitleBarPointerReleased;
            _titleBar.PointerMoved            -= HandleTitleBarPointerMoved;
            _titleBar.SizeChanged             -= HandleTitleBarSizeChanged;
        }
        var titleBar = NotifyCreateTitleBar(_titleBar);
        
        if (titleBar != null)
        {
            titleBar.MaximizeWindowRequested += HandleTitleDoubleClicked;
            titleBar.PointerPressed          += HandleTitleBarPointerPressed;
            titleBar.PointerReleased         += HandleTitleBarPointerReleased;
            titleBar.PointerMoved            += HandleTitleBarPointerMoved;
            titleBar.SizeChanged             += HandleTitleBarSizeChanged;
            NotifyConfigureTitleBar(titleBar);
        }
        
        TitleBar = titleBar;
    }
    
    private void HandleTitleDoubleClicked(object? sender, EventArgs e)
    {
        if (!CanResize)
        {
            return;
        }

        var windowState = WindowState;
        if (windowState == WindowState.FullScreen)
        {
            return;
        }

        if (windowState == WindowState.Normal && (OsType == OsType.macOS || CanMaximize))
        {
            WindowState = WindowState.Maximized;
        }
        else if (windowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
        }
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

        Point mousePosition = e.GetPosition(this);
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

    private void HandleTitleBarSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        SetCurrentValue(ExtendClientAreaTitleBarHeightHintProperty, e.NewSize.Height);
    }

    private void HandleTitleBarPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!IsMoveEnabled || e.InitialPressMouseButton != MouseButton.Left || !_isDragging)
        {
            return;
        }
   
        _lastMousePressedPoint     = null;
        _lastMousePressedEventArgs = null;
        _isDragging                = false;
    }
    
    protected virtual void NotifyConfigureTitleBar(WindowTitleBar titleBar)
    {
        titleBar[!WindowTitleBar.TitleProperty]        = this[!TitleProperty];
        titleBar[!WindowTitleBar.LogoProperty]         = this[!LogoProperty];
        titleBar[!WindowTitleBar.LogoTemplateProperty] = this[!LogoTemplateProperty];
    }

    protected virtual WindowTitleBar? NotifyCreateTitleBar(WindowTitleBar? oldTitleBar)
    {
        return new WindowTitleBar
        {
            Name = "PART_TitleBar"
        };
    }
    
    public static Window? GetMainWindow()
    {
        var lifetime = Application.Current?.ApplicationLifetime;
        if (lifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            return desktopLifetime.MainWindow as Window;
        }
        return null;
    }
    
    [SupportedOSPlatform("macos")]
    private void ConfigureMacOsWindow()
    {
        if (!ExtendClientAreaToDecorationsHint)
        {
            TitleBarOffsetMargin = default;
            return;
        }

        double? titleBarHeight = null;
        if (!MathUtils.AreClose(ExtendClientAreaTitleBarHeightHint, -1))
        {
            titleBarHeight = ExtendClientAreaTitleBarHeightHint;
        }

        double? offsetX = null;
        double? spacing = null;
        if (IsSet(MacStandardWindowButtons.OffsetXProperty))
        {
            offsetX = MacStandardWindowButtons.GetOffsetX(this);
        }

        if (IsSet(MacStandardWindowButtons.SpacingProperty))
        {
            spacing = MacStandardWindowButtons.GetSpacing(this);
        }

        var effectSpacing = spacing ?? 20;
        var currentClientSize = ClientSize;

        // live resize 期间，除 ClientSize 外的输入都不变化；即使高度变了，AppKit 的 autoresizing
        // 会把按钮维持在相对标题栏的同一视觉位置。当 4 个输入都和上次完全一致时，
        // 底层 SetStandardWindowButtonsLayout 的 GetFrame 短路也会命中，这里提前返回，
        // 避免一次 P/Invoke 往返。
        if (_macOsCacheValid &&
            Nullable.Equals(_macOsCachedTitleBarHeight, titleBarHeight) &&
            Nullable.Equals(_macOsCachedOffsetX, offsetX) &&
            Nullable.Equals(_macOsCachedSpacing, spacing) &&
            _macOsCachedClientSize == currentClientSize)
        {
            return;
        }

        MacStandardWindowButtons.SetStandardWindowButtonsLayout(this, titleBarHeight, offsetX, null, effectSpacing);
        var offset = this.GetRecommendedTitleBarContentLeftMargin(effectSpacing) ?? 0;
        TitleBarOffsetMargin = new Thickness(offset, 0, 0, 0);

        _macOsCachedTitleBarHeight = titleBarHeight;
        _macOsCachedOffsetX        = offsetX;
        _macOsCachedSpacing        = spacing;
        _macOsCachedClientSize     = currentClientSize;
        _macOsCacheValid           = true;
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        EnsureMinSizeForDecorations();
        if (OperatingSystem.IsMacOS())
        {
            ConfigureMacOsWindow();
        }

        if (OperatingSystem.IsWindows())
        {
            this.InitializeWinWindow();
        }
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        if (OperatingSystem.IsMacOS())
        {
            ConfigureMacOsWindow();
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == WindowStateProperty ||
            change.Property == ExtendClientAreaTitleBarHeightHintProperty)
        {
            if (OperatingSystem.IsMacOS())
            {
                // WindowState 切换（最大化/最小化/全屏）会让 macOS 重置按钮位置，
                // 此时即使输入参数相同也必须重新下发一次布局，否则缓存会把重置后的
                // 错误位置误判为“已是目标位置”而跳过修正。
                _macOsCacheValid = false;
                ConfigureMacOsWindow();
            }
        }
        if (change.Property == WindowStateProperty ||
            change.Property == FrameShadowThicknessProperty ||
            change.Property == CornerRadiusProperty)
        {
            EnsureMinSizeForDecorations();
        }
        if (OperatingSystem.IsWindows() && change.Property == WindowStateProperty)
        {
            UpdateWinDwmForWindowState();
        }
        if (change.Property == CanResizeProperty || change.Property == WindowStateProperty)
        {
            ConfigureCustomResizerVisible();
        }
    }

    [SupportedOSPlatform("windows")]
    private void UpdateWinDwmForWindowState()
    {
        if (WindowState == WindowState.FullScreen)
        {
            _wasFullScreen = true;
            return;
        }

        if (_wasFullScreen)
        {
            _wasFullScreen = false;
            Dispatcher.Post(this.ApplyWinDwmShadow, Avalonia.Threading.DispatcherPriority.Send);
        }
    }

    private void ConfigureCsdStatus()
    {
        if (OperatingSystem.IsMacOS())
        {
            IsCsdEnabled = false;
        }
        else if (OperatingSystem.IsLinux())
        {
            IsCsdEnabled = AvaloniaLocator.Current.GetService<X11PlatformOptions>()?.EnableDrawnDecorations == true;
        }
        else if (OperatingSystem.IsWindows())
        {
            IsCsdEnabled = false;
        }
    }

    private void ConfigureCustomResizerVisible()
    {
        if (OsType != OsType.Linux || IsCsdEnabled)
        {
            IsCustomResizerVisible = false;
        }
        else
        {
            IsCustomResizerVisible = CanResize && WindowState == WindowState.Normal;
        }
    }

    private void EnsureMinSizeForDecorations()
    {
        if (!OperatingSystem.IsLinux())
        {
            return;
        }

        if (WindowState is WindowState.Maximized or WindowState.FullScreen)
        {
            return;
        }

        var shadow       = FrameShadowThickness;
        var cornerRadius = CornerRadius;
        var maxCorner = Math.Max(
            Math.Max(cornerRadius.TopLeft, cornerRadius.TopRight),
            Math.Max(cornerRadius.BottomLeft, cornerRadius.BottomRight));

        const double frameBorder = 1;

        var horizontalDecoration = shadow.Left + shadow.Right + frameBorder * 2;
        var titleBarWidth        = _titleBar?.DesiredSize.Width ?? 0;
        var minDecorationWidth = Math.Max(
            horizontalDecoration + maxCorner * 2,
            horizontalDecoration + titleBarWidth);

        var verticalDecoration   = shadow.Top + shadow.Bottom + frameBorder * 2;
        var titleBarActualHeight = _titleBar?.DesiredSize.Height ?? TitleBarHeight;
        var minDecorationHeight = verticalDecoration
                                  + titleBarActualHeight
                                  + maxCorner * 2;

        if (MinWidth < minDecorationWidth)
        {
            MinWidth = minDecorationWidth;
        }

        if (MinHeight < minDecorationHeight)
        {
            MinHeight = minDecorationHeight;
        }
    }
}
