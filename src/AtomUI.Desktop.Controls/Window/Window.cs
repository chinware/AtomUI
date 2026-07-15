using System.Runtime.Versioning;
using System.Reactive.Disposables;
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
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

using AvaloniaWindow = Avalonia.Controls.Window;

public partial class Window : AvaloniaWindow,
                              IOperationSystemAware,
                              IMediaBreakAwareControl
{
    private const double WindowsCsdMinimumHeightInTitleBars = 2;

    #region 公共属性定义
    public static readonly StyledProperty<object?> LogoProperty =
        WindowTitleBar.LogoProperty.AddOwner<Window>();
    
    public static readonly StyledProperty<IDataTemplate?> LogoTemplateProperty =
        WindowTitleBar.LogoTemplateProperty.AddOwner<Window>();

    public static readonly StyledProperty<WindowTitleBarLogoVisibility> LogoVisibilityProperty =
        WindowTitleBar.LogoVisibilityProperty.AddOwner<Window>();
    
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

    public WindowTitleBarLogoVisibility LogoVisibility
    {
        get => GetValue(LogoVisibilityProperty);
        set => SetValue(LogoVisibilityProperty, value);
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

    internal static readonly DirectProperty<Window, bool> IsDrawnTitleBarOverlayVisibleProperty =
        AvaloniaProperty.RegisterDirect<Window, bool>(
            nameof(IsDrawnTitleBarOverlayVisible),
            o => o.IsDrawnTitleBarOverlayVisible);

    internal static readonly DirectProperty<Window, bool> IsEffectiveFullscreenLogoVisibleProperty =
        AvaloniaProperty.RegisterDirect<Window, bool>(
            nameof(IsEffectiveFullscreenLogoVisible),
            o => o.IsEffectiveFullscreenLogoVisible);
    
    internal static readonly StyledProperty<double> TitleBarHeightProperty =
        AvaloniaProperty.Register<Window, double>(nameof(TitleBarHeight));
    
    internal static readonly StyledProperty<BoxShadows> FrameShadowProperty =
        AvaloniaProperty.Register<Window, BoxShadows>(nameof(FrameShadow));
    
    internal static readonly StyledProperty<Thickness> FrameShadowThicknessProperty =
        AvaloniaProperty.Register<Window, Thickness>(nameof(FrameShadowThickness));

    private static readonly IDataTemplate s_windowIconLogoTemplate =
        new FuncDataTemplate<WindowIcon>((icon, _) => CreateWindowIconLogo(icon));
    
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

    internal void RaiseRoutedEventFromOverlay(Interactive source, RoutedEventArgs args)
    {
        if (args.RoutedEvent is null)
        {
            return;
        }

        using var route = BuildEventRoute(args.RoutedEvent);
        route.RaiseEvent(source, args);
    }

    private bool _isCustomResizerVisible;

    internal bool IsCustomResizerVisible
    {
        get => _isCustomResizerVisible;
        set => SetAndRaise(IsCustomResizerVisibleProperty, ref _isCustomResizerVisible, value);
    }

    private bool _isDrawnTitleBarOverlayVisible = true;

    internal bool IsDrawnTitleBarOverlayVisible
    {
        get => _isDrawnTitleBarOverlayVisible;
        private set => SetAndRaise(
            IsDrawnTitleBarOverlayVisibleProperty,
            ref _isDrawnTitleBarOverlayVisible,
            value);
    }

    private bool _isEffectiveFullscreenLogoVisible;

    internal bool IsEffectiveFullscreenLogoVisible
    {
        get => _isEffectiveFullscreenLogoVisible;
        private set => SetAndRaise(IsEffectiveFullscreenLogoVisibleProperty, ref _isEffectiveFullscreenLogoVisible, value);
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
    private readonly IWindowChromeManager? _platformChromeManager;
    private FullscreenPopoverLayer? _fullscreenPopoverLayer;
    private WindowResizer? _windowResizer;
    private MediaBreakPointIndicator? _mediaBreakPointIndicator;
    private int _drawnTitleBarOverlaySuppressionCount;

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
        FrameShadowProperty.Changed.AddClassHandler<Window>((window, args) =>
            window.HandleFrameShadowPropertyChanged(args.GetNewValue<BoxShadows>()));
    }

    public Window()
    {
        ConfigureCsdStatus();
        _platformChromeManager = WindowChromeManager.Attach(this);
    }

    public override void Show()
    {
        var restoreStartupLocation = _platformChromeManager?.PrepareInitialShowState();
        try
        {
            base.Show();
        }
        finally
        {
            restoreStartupLocation?.Invoke();
        }
    }

    internal void PreparePlatformChromeInitialShowLayout()
    {
        EnsureInitialized();
        ApplyStyling();
    }

    internal void SetPlatformChromeClientSize(Size clientSize)
    {
        ClientSize = clientSize;
    }

    internal void ConfigureManagedResizeGrip(Thickness gripThickness)
    {
        if (_windowResizer is null)
        {
            return;
        }

        var shadow = FrameShadowThickness;
        _windowResizer.GripThickness = gripThickness;
        _windowResizer.Margin = new Thickness(
            Math.Max(0, shadow.Left - gripThickness.Left),
            Math.Max(0, shadow.Top - gripThickness.Top),
            Math.Max(0, shadow.Right - gripThickness.Right),
            Math.Max(0, shadow.Bottom - gripThickness.Bottom));
    }

    internal IDisposable SuppressDrawnTitleBarOverlay()
    {
        _drawnTitleBarOverlaySuppressionCount++;
        IsDrawnTitleBarOverlayVisible = false;
        return Disposable.Create(this, static window => window.ReleaseDrawnTitleBarOverlaySuppression());
    }

    private void ReleaseDrawnTitleBarOverlaySuppression()
    {
        if (_drawnTitleBarOverlaySuppressionCount == 0)
        {
            return;
        }

        _drawnTitleBarOverlaySuppressionCount--;
        if (_drawnTitleBarOverlaySuppressionCount == 0)
        {
            IsDrawnTitleBarOverlayVisible = true;
        }
    }

    private void HandleFrameShadowPropertyChanged(BoxShadows frameShadow)
    {
        if (_platformChromeManager is not null)
        {
            _platformChromeManager.HandleFrameShadowChanged(frameShadow);
            return;
        }

        FrameShadowThickness = frameShadow.Thickness();
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
    
    private bool _mediaQueryReady;

    internal void NotifyMediaBreakPointChanged(MediaBreakPoint breakPoint)
    {
        if (MediaBreakPoint == breakPoint)
        {
            return;
        }
        SetCurrentValue(MediaBreakPointProperty, breakPoint);
        if (!_mediaQueryReady)
        {
            return;
        }
        MediaBreakPointChanged?.Invoke(this, new MediaBreakPointChangedEventArgs(breakPoint));
    }

    private void HandleFirstLayoutUpdatedForMediaQuery(object? sender, EventArgs e)
    {
        LayoutUpdated -= HandleFirstLayoutUpdatedForMediaQuery;
        _mediaQueryReady = true;
        MediaBreakPointChanged?.Invoke(this, new MediaBreakPointChangedEventArgs(MediaBreakPoint));
    }

    private void HandleIndicatorMediaBreakPointChanged(object? sender, MediaBreakPointChangedEventArgs args)
    {
        NotifyMediaBreakPointChanged(args.MediaBreakPoint);
    }
    
    internal void NotifyCloseRequestByUser()
    {
        CloseByClickCloseCaptionButton = true;
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        HandleCreateTitleBar();
        EnsureWindowsCsdMinimumHeight();

        if (_mediaBreakPointIndicator != null)
        {
            _mediaBreakPointIndicator.MediaBreakPointChanged -= HandleIndicatorMediaBreakPointChanged;
        }

        _mediaBreakPointIndicator =
            e.NameScope.Find<MediaBreakPointIndicator>(MediaBreakPointIndicator.MediaQueryIndicatorName);
        if (_mediaBreakPointIndicator != null)
        {
            _mediaBreakPointIndicator.MediaBreakPointChanged += HandleIndicatorMediaBreakPointChanged;
            NotifyMediaBreakPointChanged(_mediaBreakPointIndicator.MediaBreakPoint);
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
            _titleBar.PointerCaptureLost      -= HandleTitleBarPointerCaptureLost;
            _titleBar.SizeChanged             -= HandleTitleBarSizeChanged;
        }
        var titleBar = NotifyCreateTitleBar(_titleBar);
        
        if (titleBar != null)
        {
            titleBar.MaximizeWindowRequested += HandleTitleDoubleClicked;
            titleBar.PointerPressed          += HandleTitleBarPointerPressed;
            titleBar.PointerReleased         += HandleTitleBarPointerReleased;
            titleBar.PointerMoved            += HandleTitleBarPointerMoved;
            titleBar.PointerCaptureLost      += HandleTitleBarPointerCaptureLost;
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
        if (!IsMoveEnabled ||
            WindowState == WindowState.FullScreen ||
            !e.Properties.IsLeftButtonPressed)
        {
            ResetTitleBarMoveDragState();
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
                if (_lastMousePressedEventArgs is not { } pointerPressedEventArgs)
                {
                    ResetTitleBarMoveDragState();
                    return;
                }

                ResetTitleBarMoveDragState();
                BeginMoveDrag(pointerPressedEventArgs);
            }
        }
    }

    private void HandleTitleBarSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        EnsureWindowsCsdMinimumHeight(e.NewSize.Height);
        if (_platformChromeManager is not null)
        {
            _platformChromeManager.ConfigureTitleBarHeightHint(e.NewSize.Height);
        }
        else
        {
            SetCurrentValue(ExtendClientAreaTitleBarHeightHintProperty, e.NewSize.Height);
        }
    }

    private void HandleTitleBarPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Left)
        {
            ResetTitleBarMoveDragState();
        }
    }

    private void HandleTitleBarPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        ResetTitleBarMoveDragState();
    }

    private void ResetTitleBarMoveDragState()
    {
        _lastMousePressedPoint     = null;
        _lastMousePressedEventArgs = null;
    }

    internal static double CalculateWindowsCsdMinimumHeight(double titleBarHeight)
    {
        return double.IsFinite(titleBarHeight) && titleBarHeight > 0
            ? titleBarHeight * WindowsCsdMinimumHeightInTitleBars
            : 0;
    }

    private void EnsureWindowsCsdMinimumHeight(double measuredTitleBarHeight = 0)
    {
        if (!OperatingSystem.IsWindows() || !IsCsdEnabled)
        {
            return;
        }

        var titleBarHeight = Math.Max(measuredTitleBarHeight, TitleBarHeight);
        var minimumHeight  = CalculateWindowsCsdMinimumHeight(titleBarHeight);
        if (minimumHeight > 0 && MinHeight < minimumHeight)
        {
            SetCurrentValue(MinHeightProperty, minimumHeight);
        }
    }
    
    protected virtual void NotifyConfigureTitleBar(WindowTitleBar titleBar)
    {
        titleBar[!WindowTitleBar.TitleProperty]          = this[!TitleProperty];
        titleBar[!WindowTitleBar.LogoProperty]           = this[!LogoProperty];
        titleBar[!WindowTitleBar.LogoTemplateProperty]   = this[!LogoTemplateProperty];
        titleBar[!WindowTitleBar.LogoVisibilityProperty] = this[!LogoVisibilityProperty];
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
            _macOsCacheValid     = false;
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
        var offset = this.GetRecommendedTitleBarContentLeftMargin(effectSpacing);
        if (offset is not { } titleBarOffset)
        {
            // The title bar can measure before the native NSWindow standard buttons exist.
            // Do not cache that no-op pass, otherwise OnOpened may skip the first real correction.
            _macOsCacheValid = false;
            return;
        }

        TitleBarOffsetMargin = new Thickness(titleBarOffset, 0, 0, 0);

        _macOsCachedTitleBarHeight = titleBarHeight;
        _macOsCachedOffsetX        = offsetX;
        _macOsCachedSpacing        = spacing;
        _macOsCachedClientSize     = currentClientSize;
        _macOsCacheValid           = true;
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        _platformChromeManager?.UpdateFrameGeometry();
        ApplyDefaultLogoIfNeeded();
        if (OperatingSystem.IsMacOS())
        {
            _macOsCacheValid = false;
            ConfigureMacOsWindow();
        }

        if (!_mediaQueryReady)
        {
            LayoutUpdated += HandleFirstLayoutUpdatedForMediaQuery;
        }
    }

    private void ApplyDefaultLogoIfNeeded()
    {
        if (Logo != null || LogoTemplate != null)
        {
            return;
        }

        if (TryApplyWindowIconLogo(Icon))
        {
            return;
        }

        var mainWindow = GetMainWindow();
        if (mainWindow == null || ReferenceEquals(mainWindow, this))
        {
            return;
        }

        if (mainWindow.LogoTemplate != null)
        {
            SetCurrentValue(LogoTemplateProperty, mainWindow.LogoTemplate);
        }

        if (mainWindow.Logo != null)
        {
            SetCurrentValue(LogoProperty, mainWindow.Logo);
        }
        else if (mainWindow.LogoTemplate == null)
        {
            TryApplyWindowIconLogo(mainWindow.Icon);
        }
    }

    private bool TryApplyWindowIconLogo(WindowIcon? icon)
    {
        if (icon == null)
        {
            return false;
        }

        SetCurrentValue(LogoTemplateProperty, s_windowIconLogoTemplate);
        SetCurrentValue(LogoProperty, icon);
        return true;
    }

    private static Control? CreateWindowIconLogo(WindowIcon? icon)
    {
        if (icon == null)
        {
            return null;
        }

        try
        {
            using var stream = new MemoryStream();
            icon.Save(stream);
            stream.Position = 0;
            return new Image
            {
                Source  = new Bitmap(stream),
                Stretch = Stretch.Uniform
            };
        }
        catch
        {
            return null;
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
            change.Property == ExtendClientAreaTitleBarHeightHintProperty ||
            change.Property == TitleProperty)
        {
            if (OperatingSystem.IsMacOS())
            {
                // WindowState / Title 变化都可能让 AppKit 重置 standard button frame。
                // 即使 AtomUI 的布局输入没有变化，原生按钮当前位置也可能已经偏离目标，
                // 所以必须先让缓存失效再重新下发布局。
                _macOsCacheValid = false;
                ConfigureMacOsWindow();
                if (change.Property == TitleProperty)
                {
                    Dispatcher.Post(() =>
                    {
                        if (!OperatingSystem.IsMacOS())
                        {
                            return;
                        }
                        _macOsCacheValid = false;
                        ConfigureMacOsWindow();
                    }, Avalonia.Threading.DispatcherPriority.Loaded);
                }
            }
        }
        _platformChromeManager?.HandlePropertyChanged(change.Property);
        if (change.Property == IsExtendedIntoWindowDecorationsProperty)
        {
            RefreshPlatformCsdStatus();
        }
        if (change.Property == CanResizeProperty || change.Property == WindowStateProperty)
        {
            ConfigureCustomResizerVisible();
        }
        if (change.Property == MinHeightProperty ||
            change.Property == TitleBarHeightProperty ||
            change.Property == IsCsdEnabledProperty)
        {
            EnsureWindowsCsdMinimumHeight();
        }
        if (change.Property == IsMoveEnabledProperty ||
            change.Property == WindowStateProperty)
        {
            ResetTitleBarMoveDragState();
        }
        if (change.Property == LogoProperty ||
            change.Property == LogoTemplateProperty ||
            change.Property == LogoVisibilityProperty ||
            change.Property == TitleProperty)
        {
            UpdateEffectiveFullscreenLogoVisible();
        }
    }

    private void UpdateEffectiveFullscreenLogoVisible()
    {
        var hasLogo = Logo is not null || LogoTemplate is not null;
        IsEffectiveFullscreenLogoVisible = LogoVisibility switch
        {
            WindowTitleBarLogoVisibility.Always => hasLogo,
            WindowTitleBarLogoVisibility.Never => false,
            _ => hasLogo && HasTitleContent(Title)
        };
    }

    private static bool HasTitleContent(object? title)
    {
        return title switch
        {
            null => false,
            string text => !string.IsNullOrWhiteSpace(text),
            _ => true
        };
    }

    private void ConfigureCsdStatus()
    {
        if (OperatingSystem.IsMacOS())
        {
            IsCsdEnabled = false;
        }
        else if (OperatingSystem.IsLinux())
        {
            IsCsdEnabled = PlatformImpl?.NeedsManagedDecorations == true;
        }
        else if (OperatingSystem.IsWindows())
        {
            IsCsdEnabled = true;
        }
    }

    internal void RefreshPlatformCsdStatus()
    {
        ConfigureCsdStatus();
        ConfigureCustomResizerVisible();
    }

    private void ConfigureCustomResizerVisible()
    {
        if (OsType != OsType.Linux ||
            IsCsdEnabled && _platformChromeManager is not WaylandWindowChromeManager)
        {
            IsCustomResizerVisible = false;
        }
        else
        {
            IsCustomResizerVisible = CanResize && WindowState == WindowState.Normal;
        }
    }
}
