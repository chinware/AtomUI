using System.Diagnostics;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class DrawerX : Control,
                       ISizeTypeAware,
                       IAnimationAwareControl,
                       IControlSharedTokenResourcesHost
{
    #region 公共属性定义
    
    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<DrawerX, object?>(nameof(Content));

    public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty =
        AvaloniaProperty.Register<DrawerX, IDataTemplate?>(nameof(ContentTemplate));

    public static readonly StyledProperty<bool> IsOpenProperty = AvaloniaProperty
        .Register<DrawerX, bool>(nameof(IsOpen), false, false, BindingMode.TwoWay);

    public static readonly StyledProperty<DrawerPlacement> PlacementProperty = AvaloniaProperty
        .Register<DrawerX, DrawerPlacement>(nameof(Placement), DrawerPlacement.Right);

    public static readonly StyledProperty<DrawerOpenMode> OpenModeProperty = AvaloniaProperty
        .Register<DrawerX, DrawerOpenMode>(nameof(OpenMode), DrawerOpenMode.Overlay);

    public static readonly StyledProperty<Visual?> OpenOnProperty = AvaloniaProperty
        .Register<DrawerX, Visual?>(nameof(OpenOn));

    public static readonly StyledProperty<bool> IsShowMaskProperty = AvaloniaProperty
        .Register<DrawerX, bool>(nameof(IsShowMask), true);
    
    public static readonly StyledProperty<bool> IsShowCloseButtonProperty = AvaloniaProperty
        .Register<DrawerX, bool>(nameof(IsShowCloseButton), true);

    public static readonly StyledProperty<bool> CloseWhenClickOnMaskProperty = AvaloniaProperty
        .Register<DrawerX, bool>(nameof(CloseWhenClickOnMask), true);

    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty
        .Register<DrawerX, string>(nameof(Title));

    public static readonly StyledProperty<object?> FooterProperty =
        AvaloniaProperty.Register<DrawerX, object?>(nameof(Footer));

    public static readonly StyledProperty<IDataTemplate?> FooterTemplateProperty =
        AvaloniaProperty.Register<DrawerX, IDataTemplate?>(nameof(FooterTemplate));

    public static readonly StyledProperty<object?> ExtraProperty =
        AvaloniaProperty.Register<DrawerX, object?>(nameof(Extra));

    public static readonly StyledProperty<IDataTemplate?> ExtraTemplateProperty =
        AvaloniaProperty.Register<DrawerX, IDataTemplate?>(nameof(ExtraTemplate));

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        AvaloniaProperty.Register<DrawerX, SizeType>(nameof(SizeType), SizeType.Small);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AvaloniaProperty.Register<DrawerX, bool>(nameof(IsMotionEnabled));

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AvaloniaProperty.Register<DrawerX, bool>(nameof(IsWaveAnimationEnabled), true);
    
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

    public DrawerOpenMode OpenMode
    {
        get => GetValue(OpenModeProperty);
        set => SetValue(OpenModeProperty, value);
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

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }

    #endregion

    #region 公共事件定义
    
    public event EventHandler? Opened;
    public event EventHandler? Closed;
    
    #endregion

    #region 内部属性定义

    Control IAnimationAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => DrawerToken.ID;

    #endregion

    private DrawerContainerX? _container;

    public DrawerX()
    {
        this.RegisterResources();
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        var parentDrawer = FindParentDrawer();
        if (parentDrawer != null)
        {
            BindUtils.RelayBind(parentDrawer, OpenOnProperty, this, OpenOnProperty);
        }
        else
        {
            Bind(OpenOnProperty, new Binding()
            {
                Priority = BindingPriority.Template,
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
                {
                    AncestorType = typeof(TopLevel),
                }
            });
        }
    }

    private DrawerX? FindParentDrawer()
    {
        DrawerX? target  = null;
        var      current = Parent;
        while (current != null && current.GetType() != typeof(DrawerLayer))
        {
            if (current is DrawerContainerX container)
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
        if (VisualRoot != null)
        {
            if (change.Property == IsOpenProperty)
            {
                HandleIsOpenChanged();
            }
        }
        if (change.Property == OpenOnProperty)
        {
            if (OpenOn != null)
            {
                DrawerLayer.SetAttachTargetElement(this, OpenOn);
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
        var layer = DrawerLayer.GetDrawerLayer(this);
        Debug.Assert(layer != null);
        NotifyBeforeOpen(layer);
        CreateDrawerContainer();
        Debug.Assert(_container != null);
        _container.Open(layer);
    }

    private void Close()
    {
        var layer = DrawerLayer.GetDrawerLayer(this);
        Debug.Assert(layer != null);
        NotifyBeforeClose(layer);
        Debug.Assert(_container != null);
        _container.Close(layer);
    }
    
    private void CreateDrawerContainer()
    {
        if (_container is null)
        {
            _container = new DrawerContainerX()
            {
                Drawer = new WeakReference<DrawerX>(this)
            };
            BindUtils.RelayBind(this, ContentProperty, _container, DrawerContainerX.ContentProperty);
            BindUtils.RelayBind(this, ContentTemplateProperty, _container, DrawerContainerX.ContentTemplateProperty);
            BindUtils.RelayBind(this, FooterProperty, _container, DrawerContainerX.FooterProperty);
            BindUtils.RelayBind(this, FooterTemplateProperty, _container, DrawerContainerX.FooterTemplateProperty);
            BindUtils.RelayBind(this, ExtraProperty, _container, DrawerContainerX.ExtraProperty);
            BindUtils.RelayBind(this, ExtraTemplateProperty, _container, DrawerContainerX.ExtraTemplateProperty);
            BindUtils.RelayBind(this, SizeTypeProperty, _container, DrawerContainerX.SizeTypeProperty);
            BindUtils.RelayBind(this, PlacementProperty, _container, DrawerContainerX.PlacementProperty);
            BindUtils.RelayBind(this, TitleProperty, _container, DrawerContainerX.TitleProperty);
            BindUtils.RelayBind(this, IsShowMaskProperty, _container, DrawerContainerX.IsShowMaskProperty);
            BindUtils.RelayBind(this, IsMotionEnabledProperty, _container, DrawerContainerX.IsMotionEnabledProperty);
            BindUtils.RelayBind(this, CloseWhenClickOnMaskProperty, _container, DrawerContainerX.CloseWhenClickOnMaskProperty);
        }
    }

    protected internal virtual void NotifyBeforeOpen(DrawerLayer layer)
    {
        var      current = Parent;
        while (current != null && current.GetType() != typeof(DrawerLayer))
        {
            if (current is DrawerContainerX container)
            {
                if (container.Drawer != null && container.Drawer.TryGetTarget(out var drawer))
                {
                    drawer.NotifyChildDrawerAboutToOpen(this);
                }
            }
            
            current = current.Parent;
        }
    }

    internal void NotifyChildDrawerAboutToOpen(DrawerX childDrawer)
    {
        Console.WriteLine(childDrawer);
        if (_container != null)
        {
            _container.Background = Brushes.Brown;
        }
    }

    protected internal virtual void NotifyBeforeClose(DrawerLayer layer)
    {
        
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