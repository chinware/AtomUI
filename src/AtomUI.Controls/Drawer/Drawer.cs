using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class Drawer : Control,
                      IMotionAwareControl,
                      IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<Drawer, object?>(nameof(Content));

    public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty =
        AvaloniaProperty.Register<Drawer, IDataTemplate?>(nameof(ContentTemplate));

    public static readonly StyledProperty<bool> IsOpenProperty = 
        AvaloniaProperty.Register<Drawer, bool>(nameof(IsOpen), false, false, BindingMode.TwoWay);

    public static readonly StyledProperty<DrawerPlacement> PlacementProperty = 
        AvaloniaProperty.Register<Drawer, DrawerPlacement>(nameof(Placement), DrawerPlacement.Right);

    public static readonly StyledProperty<Visual?> OpenOnProperty =
        AvaloniaProperty.Register<Drawer, Visual?>(nameof(OpenOn));

    public static readonly StyledProperty<bool> IsShowMaskProperty = 
        AvaloniaProperty.Register<Drawer, bool>(nameof(IsShowMask), true);

    public static readonly StyledProperty<bool> IsShowCloseButtonProperty = 
        AvaloniaProperty.Register<Drawer, bool>(nameof(IsShowCloseButton), true);

    public static readonly StyledProperty<bool> CloseWhenClickOnMaskProperty = 
        AvaloniaProperty.Register<Drawer, bool>(nameof(CloseWhenClickOnMask), true);

    public static readonly StyledProperty<string> TitleProperty = 
        AvaloniaProperty.Register<Drawer, string>(nameof(Title));

    public static readonly StyledProperty<object?> FooterProperty =
        AvaloniaProperty.Register<Drawer, object?>(nameof(Footer));

    public static readonly StyledProperty<IDataTemplate?> FooterTemplateProperty =
        AvaloniaProperty.Register<Drawer, IDataTemplate?>(nameof(FooterTemplate));

    public static readonly StyledProperty<object?> ExtraProperty =
        AvaloniaProperty.Register<Drawer, object?>(nameof(Extra));

    public static readonly StyledProperty<IDataTemplate?> ExtraTemplateProperty =
        AvaloniaProperty.Register<Drawer, IDataTemplate?>(nameof(ExtraTemplate));

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<Drawer>();

    public static readonly StyledProperty<double> DialogSizeProperty =
        AvaloniaProperty.Register<Drawer, double>(nameof(DialogSize));

    public static readonly StyledProperty<double> PushOffsetPercentProperty =
        AvaloniaProperty.Register<Drawer, double>(nameof(PushOffsetPercent));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Drawer>();
    
    [Content]
    [DependsOn(nameof(ContentTemplate))]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public IDataTemplate? ContentTemplate
    {
        get => GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public DrawerPlacement Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    public Visual? OpenOn
    {
        get => GetValue(OpenOnProperty);
        set => SetValue(OpenOnProperty, value);
    }

    public bool IsShowMask
    {
        get => GetValue(IsShowMaskProperty);
        set => SetValue(IsShowMaskProperty, value);
    }

    public bool IsShowCloseButton
    {
        get => GetValue(IsShowCloseButtonProperty);
        set => SetValue(IsShowCloseButtonProperty, value);
    }

    public bool CloseWhenClickOnMask
    {
        get => GetValue(CloseWhenClickOnMaskProperty);
        set => SetValue(CloseWhenClickOnMaskProperty, value);
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    [DependsOn(nameof(FooterTemplate))]
    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    public IDataTemplate? FooterTemplate
    {
        get => GetValue(FooterTemplateProperty);
        set => SetValue(FooterTemplateProperty, value);
    }

    [DependsOn(nameof(ExtraTemplate))]
    public object? Extra
    {
        get => GetValue(ExtraProperty);
        set => SetValue(ExtraProperty, value);
    }

    public IDataTemplate? ExtraTemplate
    {
        get => GetValue(ExtraTemplateProperty);
        set => SetValue(ExtraTemplateProperty, value);
    }

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public double DialogSize
    {
        get => GetValue(DialogSizeProperty);
        set => SetValue(DialogSizeProperty, value);
    }

    public double PushOffsetPercent
    {
        get => GetValue(PushOffsetPercentProperty);
        set => SetValue(PushOffsetPercentProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    #region 公共事件定义

    public event EventHandler? Opened;
    public event EventHandler? Closed;

    #endregion

    #region 内部属性定义

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => DrawerToken.ID;

    #endregion

    private DrawerContainer? _container;
    private CompositeDisposable? _relayBindingDisposables;
    private CompositeDisposable? _containerDisposables;
    
    static Drawer()
    {
        SizeTypeProperty.OverrideDefaultValue<Drawer>(SizeType.Small);
    }

    public Drawer()
    {
        this.RegisterResources();
        this.ConfigureMotionBindingStyle();
    }

    public static Drawer? GetDrawer(Visual element)
    {
        var container = element.FindAncestorOfType<DrawerContainer>();
        if (container?.Drawer != null && container.Drawer.TryGetTarget(out var drawer))
        {
            return drawer;
        }

        return null;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var parentDrawer = FindParentDrawer();
        _relayBindingDisposables = new CompositeDisposable();
        if (parentDrawer != null)
        {
            _relayBindingDisposables.Add(BindUtils.RelayBind(parentDrawer, OpenOnProperty, this, OpenOnProperty, BindingMode.Default,
                BindingPriority.Template));
            _relayBindingDisposables.Add(BindUtils.RelayBind(parentDrawer, IsMotionEnabledProperty, this, IsMotionEnabledProperty,
                BindingMode.Default, BindingPriority.Template));
        }
        else
        {
            _relayBindingDisposables.Add(Bind(OpenOnProperty, new Binding()
            {
                Priority = BindingPriority.Template,
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
                {
                    AncestorType = typeof(TopLevel),
                }
            }));
        }
        _relayBindingDisposables.Add(TokenResourceBinder.CreateTokenBinding(this, PushOffsetPercentProperty,
            DrawerTokenKey.PushOffsetPercent));
        SetupDialogSizeTypeBindings();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _relayBindingDisposables?.Dispose();
        _containerDisposables?.Dispose();
    }

    private void SetupDialogSizeTypeBindings()
    {
        if (SizeType == SizeType.Large)
        {
            _relayBindingDisposables?.Add(
                TokenResourceBinder.CreateTokenBinding(this, DialogSizeProperty, DrawerTokenKey.LargeSize));
        }
        else if (SizeType == SizeType.Middle)
        {
            _relayBindingDisposables?.Add(
                TokenResourceBinder.CreateTokenBinding(this, DialogSizeProperty, DrawerTokenKey.MiddleSize));
        }
        else
        {
            _relayBindingDisposables?.Add(
                TokenResourceBinder.CreateTokenBinding(this, DialogSizeProperty, DrawerTokenKey.SmallSize));
        }
    }

    private Drawer? FindParentDrawer()
    {
        Drawer? target  = null;
        var     current = Parent;
        while (current != null && current.GetType() != typeof(ScopeAwareAdornerLayer))
        {
            if (current is DrawerContainer container)
            {
                if (container.Drawer != null && container.Drawer.TryGetTarget(out var drawer))
                {
                    target = drawer;
                }
            }

            if (target != null)
            {
                break;
            }

            current = current.Parent;
        }

        return target;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsOpenProperty)
            {
                HandleIsOpenChanged();
            }
            else if (change.Property == SizeTypeProperty)
            {
                SetupDialogSizeTypeBindings();
            }
        }

        if (change.Property == OpenOnProperty)
        {
            if (OpenOn != null)
            {
                ScopeAwareAdornerLayer.SetAdornedElement(this, OpenOn);
            }
        }
    }

    private void HandleIsOpenChanged()
    {
        if (IsOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    private void Open()
    {
        var layer = ScopeAwareAdornerLayer.GetLayer(this);
        Debug.Assert(layer != null);
        NotifyBeforeOpen(layer);
        CreateDrawerContainer();
        Debug.Assert(_container != null);
        _container.Open(layer);
    }

    private void Close()
    {
        var layer = ScopeAwareAdornerLayer.GetLayer(this);
        Debug.Assert(layer != null);
        NotifyBeforeClose(layer);
        Debug.Assert(_container != null);
        _container.Close(layer);
        _containerDisposables?.Dispose();
        _container = null;
    }

    private void CreateDrawerContainer()
    {
        _container = new DrawerContainer()
        {
            Drawer = new WeakReference<Drawer>(this)
        };
        _containerDisposables = new CompositeDisposable();
        _containerDisposables.Add(BindUtils.RelayBind(this, ContentProperty, _container, DrawerContainer.ContentProperty));
        _containerDisposables.Add(BindUtils.RelayBind(this, ContentTemplateProperty, _container, DrawerContainer.ContentTemplateProperty));
        _containerDisposables.Add(BindUtils.RelayBind(this, FooterProperty, _container, DrawerContainer.FooterProperty));
        _containerDisposables.Add(BindUtils.RelayBind(this, FooterTemplateProperty, _container, DrawerContainer.FooterTemplateProperty));
        _containerDisposables.Add(BindUtils.RelayBind(this, ExtraProperty, _container, DrawerContainer.ExtraProperty));
        _containerDisposables.Add(BindUtils.RelayBind(this, ExtraTemplateProperty, _container, DrawerContainer.ExtraTemplateProperty));
        _containerDisposables.Add(BindUtils.RelayBind(this, DialogSizeProperty, _container, DrawerContainer.DialogSizeProperty));
        _containerDisposables.Add(BindUtils.RelayBind(this, PlacementProperty, _container, DrawerContainer.PlacementProperty));
        _containerDisposables.Add(BindUtils.RelayBind(this, TitleProperty, _container, DrawerContainer.TitleProperty));
        _containerDisposables.Add(BindUtils.RelayBind(this, IsShowMaskProperty, _container, DrawerContainer.IsShowMaskProperty));
        _containerDisposables.Add(BindUtils.RelayBind(this, IsShowCloseButtonProperty, _container, DrawerContainer.IsShowCloseButtonProperty));
        _containerDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, _container, DrawerContainer.IsMotionEnabledProperty));
        _containerDisposables.Add(BindUtils.RelayBind(this, CloseWhenClickOnMaskProperty, _container,
            DrawerContainer.CloseWhenClickOnMaskProperty));
        _containerDisposables.Add(BindUtils.RelayBind(this, PushOffsetPercentProperty, _container,
            DrawerContainer.PushOffsetPercentProperty));
    }

    protected internal virtual void NotifyBeforeOpen(ScopeAwareAdornerLayer layer)
    {
        var current = Parent;
        while (current != null && current.GetType() != typeof(ScopeAwareAdornerLayer))
        {
            if (current is DrawerContainer container)
            {
                if (container.Drawer != null && container.Drawer.TryGetTarget(out var drawer))
                {
                    drawer.NotifyChildDrawerAboutToOpen(this);
                }
            }

            current = current.Parent;
        }
    }

    internal void NotifyChildDrawerAboutToOpen(Drawer childDrawer)
    {
        _container?.NotifyChildDrawerAboutToOpen(childDrawer);
    }

    internal void NotifyChildDrawerAboutToClose(Drawer childDrawer)
    {
        _container?.NotifyChildDrawerAboutToClose(childDrawer);
    }

    protected internal virtual void NotifyBeforeClose(ScopeAwareAdornerLayer layer)
    {
        var current = Parent;
        while (current != null && current.GetType() != typeof(ScopeAwareAdornerLayer))
        {
            if (current is DrawerContainer container)
            {
                if (container.Drawer != null && container.Drawer.TryGetTarget(out var drawer))
                {
                    drawer.NotifyChildDrawerAboutToClose(this);
                }
            }

            current = current.Parent;
        }
    }

    protected internal virtual void NotifyOpened()
    {
        Opened?.Invoke(this, EventArgs.Empty);
    }

    protected internal virtual void NotifyClosed()
    {
        Closed?.Invoke(this, EventArgs.Empty);
    }
}