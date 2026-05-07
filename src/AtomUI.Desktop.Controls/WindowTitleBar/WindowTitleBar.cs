using System.Reactive.Disposables;
using Avalonia.Threading;
using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

[PseudoClasses(StdPseudoClass.Active)]
[PseudoClasses(StdPseudoClass.Normal, StdPseudoClass.Minimized, StdPseudoClass.Maximized, StdPseudoClass.Fullscreen)]
public class WindowTitleBar : TemplatedControl, 
                              IMotionAwareControl, 
                              IOperationSystemAware
{
    #region 公共属性定义

    public static readonly StyledProperty<object?> LogoProperty =
        AvaloniaProperty.Register<WindowTitleBar, object?>(nameof(Logo));
    
    public static readonly StyledProperty<IDataTemplate?> LogoTemplateProperty =
        AvaloniaProperty.Register<WindowTitleBar, IDataTemplate?>(nameof(LogoTemplate));
    
    public static readonly StyledProperty<object?> TitleProperty =
        AvaloniaProperty.Register<WindowTitleBar, object?>(nameof(Title));
    
    public static readonly StyledProperty<IDataTemplate?> TitleTemplateProperty = 
        AvaloniaProperty.Register<WindowTitleBar, IDataTemplate?>(nameof (TitleTemplate));
    
    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AvaloniaProperty.Register<WindowTitleBar, object?>(nameof(LeftAddOn));

    public static readonly StyledProperty<IDataTemplate?> LeftAddOnTemplateProperty =
        AvaloniaProperty.Register<WindowTitleBar, IDataTemplate?>(nameof(LeftAddOnTemplate));
    
    public static readonly StyledProperty<object?> RightAddOnProperty =
        AvaloniaProperty.Register<WindowTitleBar, object?>(nameof(RightAddOn));

    public static readonly StyledProperty<IDataTemplate?> RightAddOnTemplateProperty =
        AvaloniaProperty.Register<WindowTitleBar, IDataTemplate?>(nameof(RightAddOnTemplate));
    
    public static readonly StyledProperty<bool> IsWindowActiveProperty = 
        AvaloniaProperty.Register<WindowTitleBar, bool>(nameof(IsWindowActive));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<WindowTitleBar>();
    
    public static readonly StyledProperty<OsType> OsTypeProperty =
        OperationSystemAwareControlProperty.OsTypeProperty.AddOwner<WindowTitleBar>();
    
    public static readonly StyledProperty<Version> OsVersionProperty =
        OperationSystemAwareControlProperty.OsVersionProperty.AddOwner<WindowTitleBar>();

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
    
    [DependsOn(nameof(TitleTemplate))]
    public object? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public IDataTemplate? TitleTemplate
    {
        get => GetValue(TitleTemplateProperty);
        set => SetValue(TitleTemplateProperty, value);
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

    public bool IsWindowActive
    {
        get => GetValue(IsWindowActiveProperty);
        set => SetValue(IsWindowActiveProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public OsType OsType => GetValue(OsTypeProperty);
    public Version OsVersion => GetValue(OsVersionProperty);
    #endregion

    #region 公共属性定义

    public event EventHandler? MaximizeWindowRequested;

    #endregion

    private CaptionButtonGroup? _captionButtonGroup;
    private CompositeDisposable? _disposables;
    private Window? _window;

    static WindowTitleBar()
    {
        FontSizeProperty.OverrideDefaultValue<WindowTitleBar>(13);
        FontWeightProperty.OverrideDefaultValue<WindowTitleBar>(FontWeight.Bold);
    }

    public WindowTitleBar()
    {
        this.ConfigureOsType();
        this.RegisterTokenResourceScope(WindowTitleBarToken.ScopeProvider);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _captionButtonGroup?.Detach();
        _captionButtonGroup = e.NameScope.Find<CaptionButtonGroup>("PART_CaptionButtonGroup");
        if (_window != null)
        {
            _captionButtonGroup?.Attach(_window);
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _disposables?.Dispose();
        _disposables = null;
        _window      = this.FindLogicalAncestorOfType<Window>();
        if (_window != null)
        {
            // OnApplyTemplate 可能在 OnAttachedToVisualTree 之前调用，
            // 此时 TopLevel 尚不可用导致 Attach 未执行，需要在这里补上
            _captionButtonGroup?.Attach(_window);

            _disposables = new CompositeDisposable(6)
            {
                _window.GetObservable(Window.WindowStateProperty).Subscribe(x =>
                {
                    PseudoClasses.Set(StdPseudoClass.Minimized, x == WindowState.Minimized);
                    PseudoClasses.Set(StdPseudoClass.Normal, x == WindowState.Normal);
                    PseudoClasses.Set(StdPseudoClass.Maximized, x == WindowState.Maximized);
                    PseudoClasses.Set(StdPseudoClass.Fullscreen, x == WindowState.FullScreen);
                }),
                _window.GetObservable(WindowBase.IsActiveProperty).Subscribe(isActive =>
                {
                    PseudoClasses.Set(StdPseudoClass.Active, isActive);
                    IsWindowActive = isActive;
                })
            };
        }
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        _disposables?.Dispose();
        _disposables = null;
        _captionButtonGroup?.Detach();
        _captionButtonGroup = null;
        _window             = null;
    }
    
    private bool _doubleClickPending;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.ClickCount == 2 && e.Properties.IsLeftButtonPressed)
        {
            // 不在 PointerPressed 里直接修改 WindowState：在 Windows 上，maximized → normal
            // 的切换由 DWM 同步执行 resize，而 Avalonia 在 PointerPressed 阶段对 source 做了
            // 隐式 pointer capture，resize 会破坏当前 pointer 事件链，导致 PointerReleased /
            // 后续 hover 失效（必须再点一次才会恢复）。把动作延后到 PointerReleased，
            // 跟点击最大化按钮的路径一致，可彻底规避此问题。
            _doubleClickPending = true;
            e.Handled           = true;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_doubleClickPending && e.InitialPressMouseButton == MouseButton.Left)
        {
            _doubleClickPending = false;
            MaximizeWindowRequested?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            _doubleClickPending = false;
        }
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        _doubleClickPending = false;
    }

    void IOperationSystemAware.SetOsType(OsType osType)
    {
        SetValue(OsTypeProperty, osType);
    }
    
    void IOperationSystemAware.SetOsVersion(Version version)
    {
        SetValue(OsVersionProperty, version);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Dispatcher.Post(this.EnableTransitions);
    }
}
