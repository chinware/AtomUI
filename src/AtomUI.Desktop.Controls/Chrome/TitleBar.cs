using System.Reactive.Disposables;
using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Themes;
using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

[PseudoClasses(StdPseudoClass.Active)]
[PseudoClasses(StdPseudoClass.Normal, StdPseudoClass.Minimized, StdPseudoClass.Maximized, StdPseudoClass.Fullscreen)]
internal class TitleBar : ContentControl, IControlSharedTokenResourcesHost, IMotionAwareControl, IOperationSystemAware
{
    #region 公共属性定义

    public static readonly StyledProperty<Control?> LogoProperty =
        AvaloniaProperty.Register<TitleBar, Control?>(nameof(Logo));

    public static readonly StyledProperty<double> TitleFontSizeProperty =
        AvaloniaProperty.Register<TitleBar, double>(nameof(TitleFontSize), defaultValue: 13);

    public static readonly StyledProperty<FontWeight> TitleFontWeightProperty =
        AvaloniaProperty.Register<TitleBar, FontWeight>(nameof(TitleFontWeight), defaultValue: FontWeight.Bold);

    public static readonly StyledProperty<object?> AddOnProperty =
        AvaloniaProperty.Register<TitleBar, object?>(nameof(AddOn));

    public static readonly StyledProperty<IDataTemplate?> AddOnTemplateProperty =
        AvaloniaProperty.Register<TitleBar, IDataTemplate?>(nameof(AddOnTemplate));
    
    public static readonly StyledProperty<bool> IsWindowActiveProperty = 
        AvaloniaProperty.Register<TitleBar, bool>(nameof(IsWindowActive));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<TitleBar>();
    
    public static readonly StyledProperty<OsType> OsTypeProperty =
        OperationSystemAwareControlProperty.OsTypeProperty.AddOwner<TitleBar>();
    
    public static readonly StyledProperty<Version> OsVersionProperty =
        OperationSystemAwareControlProperty.OsVersionProperty.AddOwner<TitleBar>();

    public Control? Logo
    {
        get => GetValue(LogoProperty);
        set => SetValue(LogoProperty, value);
    }

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

    [DependsOn(nameof(AddOnTemplate))]
    public object? AddOn
    {
        get => GetValue(AddOnProperty);
        set => SetValue(AddOnProperty, value);
    }

    public IDataTemplate? AddOnTemplate
    {
        get => GetValue(AddOnTemplateProperty);
        set => SetValue(AddOnTemplateProperty, value);
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

    #region 内部属性定义

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ChromeToken.ID;

    #endregion

    private CaptionButtonGroup? _captionButtonGroup;
    private CompositeDisposable? _disposables;

    public TitleBar()
    {
        this.ConfigureOsType();
        this.RegisterResources();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _captionButtonGroup?.Detach();
        _captionButtonGroup = e.NameScope.Find<CaptionButtonGroup>(TitleBarThemeConstants.CaptionButtonGroupPart);
        if (VisualRoot is Window window)
        {
            _captionButtonGroup?.Attach(window);
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (VisualRoot is Window window)
        {
            _disposables = new CompositeDisposable(6)
            {
                window.GetObservable(Window.WindowStateProperty).Subscribe(x =>
                      {
                          PseudoClasses.Set(StdPseudoClass.Minimized, x == WindowState.Minimized);
                          PseudoClasses.Set(StdPseudoClass.Normal, x == WindowState.Normal);
                          PseudoClasses.Set(StdPseudoClass.Maximized, x == WindowState.Maximized);
                          PseudoClasses.Set(StdPseudoClass.Fullscreen, x == WindowState.FullScreen);
                      }),
                window.GetObservable(WindowBase.IsActiveProperty).Subscribe(isActive =>
                {
                    PseudoClasses.Set(StdPseudoClass.Active, isActive);
                    IsWindowActive = isActive;
                })
            };
        }
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _disposables?.Dispose();
        _captionButtonGroup?.Detach();
        _captionButtonGroup = null;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);
            }
        }
    }

    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions = [
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty)
                ];
            }
        }
        else
        {
            Transitions = null;
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureTransitions(false);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Transitions = null;
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