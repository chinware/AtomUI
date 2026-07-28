using System.Runtime.Versioning;
using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Media;
using AtomUI.Native;
using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Metadata;
using Avalonia.Styling;

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

    public static readonly StyledProperty<WindowTitleBarTitleAlignment> TitleAlignmentProperty =
        WindowTitleBar.TitleAlignmentProperty.AddOwner<Window>();

    public static readonly StyledProperty<object?> LeftAddOnProperty =
        WindowTitleBar.LeftAddOnProperty.AddOwner<Window>();

    public static readonly StyledProperty<IDataTemplate?> LeftAddOnTemplateProperty =
        WindowTitleBar.LeftAddOnTemplateProperty.AddOwner<Window>();

    public static readonly StyledProperty<object?> RightAddOnProperty =
        WindowTitleBar.RightAddOnProperty.AddOwner<Window>();

    public static readonly StyledProperty<IDataTemplate?> RightAddOnTemplateProperty =
        WindowTitleBar.RightAddOnTemplateProperty.AddOwner<Window>();
    
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

    public WindowTitleBarTitleAlignment TitleAlignment
    {
        get => GetValue(TitleAlignmentProperty);
        set => SetValue(TitleAlignmentProperty, value);
    }

    [DependsOn(nameof(LeftAddOnTemplate))]
    public object? LeftAddOn
    {
        get => GetValue(LeftAddOnProperty);
        set => SetValue(LeftAddOnProperty, value);
    }

    public IDataTemplate? LeftAddOnTemplate
    {
        get => GetValue(LeftAddOnTemplateProperty);
        set => SetValue(LeftAddOnTemplateProperty, value);
    }

    [DependsOn(nameof(RightAddOnTemplate))]
    public object? RightAddOn
    {
        get => GetValue(RightAddOnProperty);
        set => SetValue(RightAddOnProperty, value);
    }

    public IDataTemplate? RightAddOnTemplate
    {
        get => GetValue(RightAddOnTemplateProperty);
        set => SetValue(RightAddOnTemplateProperty, value);
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
    internal static readonly DirectProperty<Window, Thickness> NativeChromeInsetsProperty =
        AvaloniaProperty.RegisterDirect<Window, Thickness>(
            nameof(NativeChromeInsets),
            o => o.NativeChromeInsets,
            (o, v) => o.NativeChromeInsets = v);
    
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

    internal static readonly DirectProperty<Window, bool> IsDrawnChromeOverlayVisibleProperty =
        AvaloniaProperty.RegisterDirect<Window, bool>(
            nameof(IsDrawnChromeOverlayVisible),
            o => o.IsDrawnChromeOverlayVisible);

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

    internal static readonly DirectProperty<Window, Thickness> VisibleFrameBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<Window, Thickness>(
            nameof(VisibleFrameBorderThickness),
            o => o.VisibleFrameBorderThickness,
            (o, v) => o.VisibleFrameBorderThickness = v);

    private static readonly IDataTemplate s_windowIconLogoTemplate =
        new FuncDataTemplate<WindowIcon>((icon, _) => CreateWindowIconLogo(icon));
    
    private Thickness _nativeChromeInsets;

    internal Thickness NativeChromeInsets
    {
        get => _nativeChromeInsets;
        private set => SetAndRaise(NativeChromeInsetsProperty, ref _nativeChromeInsets, value);
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

    private bool _isDrawnChromeOverlayVisible = true;

    internal bool IsDrawnChromeOverlayVisible
    {
        get => _isDrawnChromeOverlayVisible;
        private set => SetAndRaise(
            IsDrawnChromeOverlayVisibleProperty,
            ref _isDrawnChromeOverlayVisible,
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

    /// <summary>
    /// The visible native or drawn frame border around the client surface.
    /// This is separate from <see cref="FrameShadowThickness"/>, which only
    /// describes AtomUI's shadow/transparent expansion area.
    /// </summary>
    internal Thickness VisibleFrameBorderThickness
    {
        get => _visibleFrameBorderThickness;
        set => SetAndRaise(
            VisibleFrameBorderThicknessProperty,
            ref _visibleFrameBorderThickness,
            value);
    }
    #endregion

    private Thickness _visibleFrameBorderThickness;
    
    protected override Type StyleKeyOverride { get; } = typeof(Window);
    private protected bool CloseByClickCloseCaptionButton;
    private Point? _lastMousePressedPoint;
    private PointerPressedEventArgs? _lastMousePressedEventArgs;
    private readonly IWindowChromeManager? _platformChromeManager;
    private FullscreenPopoverLayer? _fullscreenPopoverLayer;
    private WindowResizer? _windowResizer;
    private MediaBreakPointIndicator? _mediaBreakPointIndicator;
    private CompositeDisposable? _titleBarAddOnBindings;
    private IDisposable? _windowsCsdFrameThemeSubscription;
    private ThemeContextLease? _themeContextLease;

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
        var newThemeContextLease = PrepareThemeContextLease(this);
        Action? restoreInitialShowState = null;
        try
        {
            using var initialSurfaceThemeValues = CreateInitialSurfaceThemeValues();
            restoreInitialShowState = _platformChromeManager?.PrepareInitialShowState();
            ApplyCurrentWindowsCsdFrameTheme();
            base.Show();
        }
        catch
        {
            ReleaseFailedThemeContextLease(newThemeContextLease);
            throw;
        }
        finally
        {
            restoreInitialShowState?.Invoke();
        }
    }

    public new void Show(AvaloniaWindow owner)
    {
        var newThemeContextLease = PrepareThemeContextLease(owner);
        Action? restoreInitialShowState = null;
        try
        {
            using var initialSurfaceThemeValues = CreateInitialSurfaceThemeValues();
            restoreInitialShowState = _platformChromeManager?.PrepareInitialShowState();
            ApplyCurrentWindowsCsdFrameTheme();
            base.Show(owner);
        }
        catch
        {
            ReleaseFailedThemeContextLease(newThemeContextLease);
            throw;
        }
        finally
        {
            restoreInitialShowState?.Invoke();
        }
    }

    public new Task ShowDialog(AvaloniaWindow owner)
    {
        var newThemeContextLease = PrepareThemeContextLease(owner);
        Action? restoreInitialShowState = null;
        try
        {
            using var initialSurfaceThemeValues = CreateInitialSurfaceThemeValues();
            restoreInitialShowState = _platformChromeManager?.PrepareInitialShowState();
            ApplyCurrentWindowsCsdFrameTheme();
            return base.ShowDialog(owner);
        }
        catch
        {
            ReleaseFailedThemeContextLease(newThemeContextLease);
            throw;
        }
        finally
        {
            restoreInitialShowState?.Invoke();
        }
    }

    public new Task<TResult> ShowDialog<TResult>(AvaloniaWindow owner)
    {
        var newThemeContextLease = PrepareThemeContextLease(owner);
        Action? restoreInitialShowState = null;
        try
        {
            using var initialSurfaceThemeValues = CreateInitialSurfaceThemeValues();
            restoreInitialShowState = _platformChromeManager?.PrepareInitialShowState();
            ApplyCurrentWindowsCsdFrameTheme();
            return base.ShowDialog<TResult>(owner);
        }
        catch
        {
            ReleaseFailedThemeContextLease(newThemeContextLease);
            throw;
        }
        finally
        {
            restoreInitialShowState?.Invoke();
        }
    }

    private CompositeDisposable CreateInitialSurfaceThemeValues()
    {
        var values = new CompositeDisposable();
        if (!this.TryFindResource(WindowTokenKind.DefaultBackground, out var resource))
        {
            return values;
        }

        IBrush? background = resource switch
        {
            IBrush brush => brush,
            Color color  => new SolidColorBrush(color),
            _            => null
        };
        if (background is null)
        {
            return values;
        }

        // WindowOpenedEvent precedes ApplyStyling. Prime only the synchronous show interval;
        // local values still win, and WindowTheme owns the surface after these frames are removed.
        try
        {
            if (SetValue(BackgroundProperty, background, BindingPriority.Template) is { } backgroundValue)
            {
                values.Add(backgroundValue);
            }
            if (SetValue(
                    TransparencyBackgroundFallbackProperty,
                    background,
                    BindingPriority.Template) is { } fallbackValue)
            {
                values.Add(fallbackValue);
            }
            return values;
        }
        catch
        {
            values.Dispose();
            throw;
        }
    }

    private ThemeContextLease? PrepareThemeContextLease(StyledElement owner)
    {
        var context = ThemeScope.ResolveContext(owner);
        if (context is null || _themeContextLease?.IsOwnedBy(context) == true)
        {
            return null;
        }

        var lease = ThemeContextLease.Attach(this, context);
        _themeContextLease = lease;
        return lease;
    }

    private void ReleaseFailedThemeContextLease(ThemeContextLease? lease)
    {
        if (lease is null || !ReferenceEquals(_themeContextLease, lease))
        {
            return;
        }

        _themeContextLease = null;
        lease.Dispose();
        ResetThemeContextState();
    }

    private void ResetThemeContextState()
    {
        SetValue(RequestedThemeVariantProperty, null);
        SetValue(ThemeScope.ContextProperty, null);
    }

    internal void PreparePlatformChromeInitialShowLayout()
    {
        EnsureInitialized();
        ApplyStyling();
    }

    internal void PreparePlatformChromeInitialShowHandle()
    {
        EnsureInitialized();
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
        _titleBarAddOnBindings?.Dispose();
        _titleBarAddOnBindings = null;
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

        if (windowState == WindowState.Normal && CanMaximize)
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
        titleBar[!WindowTitleBar.TitleAlignmentProperty] = this[!TitleAlignmentProperty];
        ConfigureTitleBarAddOnBindings(titleBar);
        titleBar[!WindowTitleBar.NativeChromeInsetsProperty] = this[!NativeChromeInsetsProperty];
        titleBar[!WindowTitleBar.IsCsdEnabledProperty] = this[!IsCsdEnabledProperty];
        titleBar[!WindowTitleBar.HostWindowStateProperty] = this[!WindowStateProperty];
    }

    private void ConfigureTitleBarAddOnBindings(WindowTitleBar titleBar)
    {
        _titleBarAddOnBindings?.Dispose();
        _titleBarAddOnBindings = new CompositeDisposable
        {
            titleBar.Bind(
                WindowTitleBar.LeftAddOnProperty,
                this.GetObservable(LeftAddOnProperty),
                BindingPriority.Template),
            titleBar.Bind(
                WindowTitleBar.LeftAddOnTemplateProperty,
                this.GetObservable(LeftAddOnTemplateProperty),
                BindingPriority.Template),
            titleBar.Bind(
                WindowTitleBar.RightAddOnProperty,
                this.GetObservable(RightAddOnProperty),
                BindingPriority.Template),
            titleBar.Bind(
                WindowTitleBar.RightAddOnTemplateProperty,
                this.GetObservable(RightAddOnTemplateProperty),
                BindingPriority.Template)
        };
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
            NativeChromeInsets = default;
            _macOsCacheValid   = false;
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
            NativeChromeInsets = default;
            _macOsCacheValid = false;
            return;
        }

        NativeChromeInsets = new Thickness(titleBarOffset, 0, 0, 0);

        _macOsCachedTitleBarHeight = titleBarHeight;
        _macOsCachedOffsetX        = offsetX;
        _macOsCachedSpacing        = spacing;
        _macOsCachedClientSize     = currentClientSize;
        _macOsCacheValid           = true;
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        EnsureWindowsCsdFrameThemeSubscription();
        ApplyCurrentWindowsCsdFrameTheme();
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

    protected override void OnClosed(EventArgs e)
    {
        _themeContextLease?.Dispose();
        _themeContextLease = null;
        ResetThemeContextState();
        _windowsCsdFrameThemeSubscription?.Dispose();
        _windowsCsdFrameThemeSubscription = null;
        base.OnClosed(e);
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
            change.Property == TitleProperty ||
            change.Property == WindowDecorationsProperty)
        {
            if (OperatingSystem.IsMacOS())
            {
                // WindowState / Title / WindowDecorations 变化都可能让 AppKit 重置 standard button frame。
                // 即使 AtomUI 的布局输入没有变化，原生按钮当前位置也可能已经偏离目标，
                // 所以必须先让缓存失效再重新下发布局。
                _macOsCacheValid = false;
                ConfigureMacOsWindow();
                if (change.Property == TitleProperty ||
                    change.Property == WindowDecorationsProperty)
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
        if (change.Property == ExtendClientAreaTitleBarHeightHintProperty ||
            change.Property == IsCsdEnabledProperty ||
            change.Property == RequestedThemeVariantProperty)
        {
            ApplyCurrentWindowsCsdFrameTheme();
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
        ApplyCurrentWindowsCsdFrameTheme();
        ConfigureCustomResizerVisible();
    }

    private void EnsureWindowsCsdFrameThemeSubscription()
    {
        if (!OperatingSystem.IsWindows() || _windowsCsdFrameThemeSubscription is not null)
        {
            return;
        }

        var themeManager = Application.Current?.GetThemeManager();
        if (themeManager is null)
        {
            return;
        }

        EventHandler<ThemeChangedEventArgs> handler = (_, args) =>
            ApplyWindowsCsdFrameTheme(args.State.Appearance == ThemeAppearance.Dark);
        themeManager.ThemeChanged += handler;
        _windowsCsdFrameThemeSubscription = Disposable.Create(() =>
            themeManager.ThemeChanged -= handler);
    }

    private void ApplyCurrentWindowsCsdFrameTheme()
    {
        if (TryResolveCurrentWindowDarkMode() is { } isDarkMode)
        {
            ApplyWindowsCsdFrameTheme(isDarkMode);
        }
    }

    private bool? TryResolveCurrentWindowDarkMode()
    {
        if (GetValue(ThemeScope.ContextProperty) is { } context)
        {
            return context.Appearance == ThemeAppearance.Dark;
        }

        if (RequestedThemeVariant == ThemeVariant.Dark)
        {
            return true;
        }

        if (RequestedThemeVariant == ThemeVariant.Light)
        {
            return false;
        }

        return Application.Current?.GetThemeManager()?.CurrentTheme?.Appearance switch
        {
            ThemeAppearance.Dark  => true,
            ThemeAppearance.Light => false,
            _                     => null
        };
    }

    private void ApplyWindowsCsdFrameTheme(bool isDarkMode)
    {
        if (!OperatingSystem.IsWindows() || !IsCsdEnabled)
        {
            return;
        }

        this.SetWindowsCsdFrameDarkMode(isDarkMode);
    }

    private void ConfigureCustomResizerVisible()
    {
        IsCustomResizerVisible = _platformChromeManager?.UsesCustomResizer == true &&
                                 CanResize &&
                                 WindowState == WindowState.Normal;
    }
}
