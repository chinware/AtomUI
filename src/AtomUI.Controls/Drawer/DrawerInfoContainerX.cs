using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

internal class DrawerInfoContainerX : HeaderedContentControl
{
    #region 内部属性定义
    
    internal static readonly DirectProperty<DrawerInfoContainerX, DrawerPlacement> PlacementProperty
        = AvaloniaProperty.RegisterDirect<DrawerInfoContainerX, DrawerPlacement>(nameof(Placement),
            o => o.Placement,
            (o, v) => o.Placement = v);
    
    internal static readonly DirectProperty<DrawerInfoContainerX, bool> IsShowCloseButtonProperty
        = AvaloniaProperty.RegisterDirect<DrawerInfoContainerX, bool>(nameof(IsShowCloseButton),
            o => o.IsShowCloseButton,
            (o, v) => o.IsShowCloseButton = v);
    
    internal static readonly DirectProperty<DrawerInfoContainerX, string> TitleProperty
        = AvaloniaProperty.RegisterDirect<DrawerInfoContainerX, string>(nameof(Title),
            o => o.Title,
            (o, v) => o.Title = v);
    
    internal static readonly DirectProperty<DrawerInfoContainerX, object?> FooterProperty
        = AvaloniaProperty.RegisterDirect<DrawerInfoContainerX, object?>(nameof(Footer),
            o => o.Footer,
            (o, v) => o.Footer = v);
    
    internal static readonly DirectProperty<DrawerInfoContainerX, IDataTemplate?> FooterTemplateProperty
        = AvaloniaProperty.RegisterDirect<DrawerInfoContainerX, IDataTemplate?>(nameof(FooterTemplate),
            o => o.FooterTemplate,
            (o, v) => o.FooterTemplate = v);
    
    internal static readonly DirectProperty<DrawerInfoContainerX, object?> ExtraProperty
        = AvaloniaProperty.RegisterDirect<DrawerInfoContainerX, object?>(nameof(Extra),
            o => o.Extra,
            (o, v) => o.Extra = v);
    
    internal static readonly DirectProperty<DrawerInfoContainerX, IDataTemplate?> ExtraTemplateProperty
        = AvaloniaProperty.RegisterDirect<DrawerInfoContainerX, IDataTemplate?>(nameof(ExtraTemplate),
            o => o.ExtraTemplate,
            (o, v) => o.ExtraTemplate = v);
    
    internal static readonly DirectProperty<DrawerInfoContainerX, SizeType> SizeTypeProperty
        = AvaloniaProperty.RegisterDirect<DrawerInfoContainerX, SizeType>(nameof(SizeType),
            o => o.SizeType,
            (o, v) => o.SizeType = v);

    internal static readonly DirectProperty<DrawerInfoContainerX, bool> HasFooterProperty
        = AvaloniaProperty.RegisterDirect<DrawerInfoContainerX, bool>(nameof(HasFooter),
            o => o.HasFooter,
            (o, v) => o.HasFooter = v);

    internal static readonly DirectProperty<DrawerInfoContainerX, bool> HasExtraProperty
        = AvaloniaProperty.RegisterDirect<DrawerInfoContainerX, bool>(nameof(HasExtra),
            o => o.HasExtra,
            (o, v) => o.HasExtra = v);
    
    internal static readonly DirectProperty<DrawerInfoContainerX, bool> IsMotionEnabledProperty
        = AvaloniaProperty.RegisterDirect<DrawerInfoContainerX, bool>(nameof(IsMotionEnabled),
            o => o.IsMotionEnabled,
            (o, v) => o.IsMotionEnabled = v);

    private DrawerPlacement _placement = DrawerPlacement.Right;

    internal DrawerPlacement Placement
    {
        get => _placement;
        set => SetAndRaise(PlacementProperty, ref _placement, value);
    }

    private bool _isShowCloseButton = true;

    internal bool IsShowCloseButton
    {
        get => _isShowCloseButton;
        set => SetAndRaise(IsShowCloseButtonProperty, ref _isShowCloseButton, value);
    }

    private string _title = string.Empty;

    internal string Title
    {
        get => _title;
        set => SetAndRaise(TitleProperty, ref _title, value);
    }

    private object? _footer;

    internal object? Footer
    {
        get => _footer;
        set => SetAndRaise(FooterProperty, ref _footer, value);
    }
    
    private IDataTemplate? _footerTemplate;

    internal IDataTemplate? FooterTemplate
    {
        get => _footerTemplate;
        set => SetAndRaise(FooterTemplateProperty, ref _footerTemplate, value);
    }
    
    private object? _extra;

    internal object? Extra
    {
        get => _extra;
        set => SetAndRaise(ExtraProperty, ref _extra, value);
    }
    
    private IDataTemplate? _extraTemplate;

    internal IDataTemplate? ExtraTemplate
    {
        get => _extraTemplate;
        set => SetAndRaise(ExtraTemplateProperty, ref _extraTemplate, value);
    }
    
    private SizeType _sizeType;

    internal SizeType SizeType
    {
        get => _sizeType;
        set => SetAndRaise(SizeTypeProperty, ref _sizeType, value);
    }

    private bool _hasFooter;

    internal bool HasFooter
    {
        get => _hasFooter;
        set => SetAndRaise(HasFooterProperty, ref _hasFooter, value);
    }
    
    private bool _hasExtra;

    internal bool HasExtra
    {
        get => _hasExtra;
        set => SetAndRaise(HasExtraProperty, ref _hasExtra, value);
    }
    
    private bool _isMotionEnabled;

    internal bool IsMotionEnabled
    {
        get => _isMotionEnabled;
        set => SetAndRaise(IsMotionEnabledProperty, ref _isMotionEnabled, value);
    }
    
    #endregion
    
    internal event EventHandler? CloseRequested;
    private IconButton? _closeButton;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == FooterProperty || change.Property == FooterTemplateProperty)
        {
            HasFooter = Footer != null || FooterTemplate != null;
        } 
        else if (change.Property == ExtraProperty || change.Property == ExtraTemplateProperty)
        {
            HasExtra = Extra != null || ExtraTemplate != null;
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            e.Handled = true;
        }
    }
    
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (e.InitialPressMouseButton == MouseButton.Left)
        {
            e.Handled = true;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _closeButton = e.NameScope.Find<IconButton>(DrawerInfoContainerTheme.CloseButtonPart);
        if (_closeButton != null)
        {
            _closeButton.Click += HandleCloseButtonClick;
        }
    }

    private void HandleCloseButtonClick(object? sender, RoutedEventArgs e)
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}