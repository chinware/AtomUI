using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Metadata;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public class Drawer : Control,
                      IMotionAwareControl,
                      ICustomizableSizeTypeAware
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

    public static readonly StyledProperty<Control?> OpenOnProperty =
        AvaloniaProperty.Register<Drawer, Control?>(nameof(OpenOn));

    public static readonly StyledProperty<bool> IsShowMaskProperty = 
        AvaloniaProperty.Register<Drawer, bool>(nameof(IsShowMask), true);

    public static readonly StyledProperty<bool> IsShowCloseButtonProperty = 
        AvaloniaProperty.Register<Drawer, bool>(nameof(IsShowCloseButton), true);

    public static readonly StyledProperty<bool> IsCloseOnMaskClickProperty = 
        AvaloniaProperty.Register<Drawer, bool>(nameof(IsCloseOnMaskClick), true);

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

    public static readonly StyledProperty<CustomizableSizeType> SizeTypeProperty =
        CustomizableSizeTypeControlProperty.SizeTypeProperty.AddOwner<Drawer>();

    public static readonly StyledProperty<Dimension> DialogSizeProperty =
        AvaloniaProperty.Register<Drawer, Dimension>(nameof(DialogSize));

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

    public Control? OpenOn
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

    public bool IsCloseOnMaskClick
    {
        get => GetValue(IsCloseOnMaskClickProperty);
        set => SetValue(IsCloseOnMaskClickProperty, value);
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

    public CustomizableSizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public Dimension DialogSize
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
    
    internal static readonly DirectProperty<Drawer, double> EffectiveDialogSizeProperty =
        AvaloniaProperty.RegisterDirect<Drawer, double>(nameof(EffectiveDialogSize),
            o => o.EffectiveDialogSize,
            (o, v) => o.EffectiveDialogSize = v);
    
    private double _effectiveDialogSize;

    internal double EffectiveDialogSize
    {
        get => _effectiveDialogSize;
        set => SetAndRaise(EffectiveDialogSizeProperty, ref _effectiveDialogSize, value);
    }

    #endregion

    private DrawerContainer? _container;
    private CompositeDisposable? _relayBindingDisposables;
    private IDisposable? _pushOffsetPercentBinding;
    private IDisposable? _dialogSizeBinding;
    private Control? _openOnSizeChangedTarget;
    private int _visualTreeVersion;
    
    static Drawer()
    {
        SizeTypeProperty.OverrideDefaultValue<Drawer>(CustomizableSizeType.Small);
    }

    public Drawer()
    {
        this.RegisterTokenResourceScope(DrawerToken.ScopeProvider);
        this.ConfigureMotionBindingStyle();
        ApplyPushOffsetPercentTokenBinding();
        ApplyDialogSizeTokenBinding();
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
        _visualTreeVersion++;
        base.OnAttachedToVisualTree(e);
        var parentDrawer = FindParentDrawer();
        _relayBindingDisposables?.Dispose();
        _relayBindingDisposables = new CompositeDisposable();
        if (parentDrawer != null)
        {
            _relayBindingDisposables.Add(BindUtils.RelayBind(parentDrawer, OpenOnProperty, this, OpenOnProperty, BindingMode.Default,
                BindingPriority.Template));
            _relayBindingDisposables.Add(BindUtils.RelayBind(parentDrawer, IsMotionEnabledProperty, this, IsMotionEnabledProperty,
                BindingMode.Default, BindingPriority.Template));
        }

        if (_pushOffsetPercentBinding == null)
        {
            ApplyPushOffsetPercentTokenBinding();
        }
        if (_dialogSizeBinding == null)
        {
            ApplyDialogSizeTokenBinding();
        }
        ConfigureEffectiveDialogSize();
        if (IsOpen)
        {
            Open();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (IsOpen)
        {
            var parentDrawers = FindParentDrawers();
            var detachVersion = ++_visualTreeVersion;
            Dispatcher.UIThread.Post(
                () => CompleteDeferredDetach(detachVersion, parentDrawers),
                DispatcherPriority.Background);
            base.OnDetachedFromVisualTree(e);
            return;
        }

        _visualTreeVersion++;
        CompleteDetachedFromVisualTree();
        base.OnDetachedFromVisualTree(e);
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

    private IReadOnlyList<Drawer>? FindParentDrawers()
    {
        List<Drawer>? drawers = null;
        var current = Parent;
        while (current != null && current.GetType() != typeof(ScopeAwareAdornerLayer))
        {
            if (current is DrawerContainer container &&
                container.Drawer != null &&
                container.Drawer.TryGetTarget(out var drawer))
            {
                drawers ??= new List<Drawer>();
                drawers.Add(drawer);
            }

            current = current.Parent;
        }

        return drawers;
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
        }

        if (change.Property == OpenOnProperty)
        {
            var target = ResolveOpenTarget();
            if (IsOpen && target != null)
            {
                ScopeAwareAdornerLayer.SetAdornedElement(this, target);
                ConfigureEffectiveDialogSize(target);
                ConfigureOpenOnSizeChangedSubscription(target);
            }
            else
            {
                DetachOpenOnSizeChanged();
                ConfigureEffectiveDialogSize(target);
            }
        }
        else if (change.Property == DialogSizeProperty ||
                 change.Property == PlacementProperty)
        {
            ConfigureEffectiveDialogSize(ResolveOpenTarget());
            ConfigureOpenOnSizeChangedSubscription(ResolveOpenTarget());
        }

        if (change.Property == SizeTypeProperty)
        {
            ApplyDialogSizeTokenBinding();
        }

        SyncDrawerContainerProperty(change.Property);
        if (change.Property == PlacementProperty && IsOpen && this.IsAttachedToVisualTree())
        {
            NotifyParentDrawersChildDrawerPlacementChanged();
        }
    }

    private void HandleOpenOnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        ConfigureEffectiveDialogSize(sender as Control ?? ResolveOpenTarget());
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
        var target = ResolveOpenTarget();
        if (target == null)
        {
            return;
        }
        ScopeAwareAdornerLayer.SetAdornedElement(this, target);
        var layer = ScopeAwareAdornerLayer.GetLayer(this);
        if (layer == null)
        {
            return;
        }
        ConfigureEffectiveDialogSize(target);
        ConfigureOpenOnSizeChangedSubscription(target);
        NotifyBeforeOpen(layer);
        CreateDrawerContainer();
        _container?.Open(layer, target);
    }

    private void Close()
    {
        var layer = ScopeAwareAdornerLayer.GetLayer(this);
        DetachOpenOnSizeChanged();
        if (layer == null || _container == null)
        {
            return;
        }
        NotifyBeforeClose(layer);
        _container.Close(layer);
    }

    private void CreateDrawerContainer()
    {
        if (_container == null)
        {
            _container = new DrawerContainer()
            {
                Drawer = new WeakReference<Drawer>(this)
            };
            _container[!DrawerContainer.DataContextProperty]          = this[!DataContextProperty];
            _container[!DrawerContainer.ContentProperty]              = this[!ContentProperty];
            _container[!DrawerContainer.ContentTemplateProperty]      = this[!ContentTemplateProperty];
            _container[!DrawerContainer.FooterProperty]               = this[!FooterProperty];
            _container[!DrawerContainer.FooterTemplateProperty]       = this[!FooterTemplateProperty];
            _container[!DrawerContainer.ExtraProperty]                = this[!ExtraProperty];
            _container[!DrawerContainer.ExtraTemplateProperty]        = this[!ExtraTemplateProperty];
            _container[!DrawerContainer.DialogSizeProperty]           = this[!EffectiveDialogSizeProperty];
            _container[!DrawerContainer.PlacementProperty]            = this[!PlacementProperty];
            _container[!DrawerContainer.TitleProperty]                = this[!TitleProperty];
            _container[!DrawerContainer.IsShowMaskProperty]           = this[!IsShowMaskProperty];
            _container[!DrawerContainer.IsShowCloseButtonProperty]    = this[!IsShowCloseButtonProperty];
            _container[!DrawerContainer.IsMotionEnabledProperty]      = this[!IsMotionEnabledProperty];
            _container[!DrawerContainer.IsCloseOnMaskClickProperty]   = this[!IsCloseOnMaskClickProperty];
            _container[!DrawerContainer.PushOffsetPercentProperty]    = this[!PushOffsetPercentProperty];
            SyncDrawerContainerProperties();
        }
    }

    private void SyncDrawerContainerProperties()
    {
        if (_container == null)
        {
            return;
        }

        _container.DataContext          = DataContext;
        _container.Content              = Content;
        _container.ContentTemplate      = ContentTemplate;
        _container.Footer               = Footer;
        _container.FooterTemplate       = FooterTemplate;
        _container.Extra                = Extra;
        _container.ExtraTemplate        = ExtraTemplate;
        _container.DialogSize           = EffectiveDialogSize;
        _container.Placement            = Placement;
        _container.Title                = Title;
        _container.IsShowMask           = IsShowMask;
        _container.IsShowCloseButton    = IsShowCloseButton;
        _container.IsMotionEnabled      = IsMotionEnabled;
        _container.IsCloseOnMaskClick   = IsCloseOnMaskClick;
        _container.PushOffsetPercent    = PushOffsetPercent;
    }

    private void SyncDrawerContainerProperty(AvaloniaProperty property)
    {
        if (_container == null)
        {
            return;
        }

        if (property == DataContextProperty)
        {
            _container.DataContext = DataContext;
        }
        else if (property == ContentProperty)
        {
            _container.Content = Content;
        }
        else if (property == ContentTemplateProperty)
        {
            _container.ContentTemplate = ContentTemplate;
        }
        else if (property == FooterProperty)
        {
            _container.Footer = Footer;
        }
        else if (property == FooterTemplateProperty)
        {
            _container.FooterTemplate = FooterTemplate;
        }
        else if (property == ExtraProperty)
        {
            _container.Extra = Extra;
        }
        else if (property == ExtraTemplateProperty)
        {
            _container.ExtraTemplate = ExtraTemplate;
        }
        else if (property == EffectiveDialogSizeProperty)
        {
            _container.DialogSize = EffectiveDialogSize;
        }
        else if (property == PlacementProperty)
        {
            _container.Placement = Placement;
        }
        else if (property == TitleProperty)
        {
            _container.Title = Title;
        }
        else if (property == IsShowMaskProperty)
        {
            _container.IsShowMask = IsShowMask;
        }
        else if (property == IsShowCloseButtonProperty)
        {
            _container.IsShowCloseButton = IsShowCloseButton;
        }
        else if (property == IsMotionEnabledProperty)
        {
            _container.IsMotionEnabled = IsMotionEnabled;
        }
        else if (property == IsCloseOnMaskClickProperty)
        {
            _container.IsCloseOnMaskClick = IsCloseOnMaskClick;
        }
        else if (property == PushOffsetPercentProperty)
        {
            _container.PushOffsetPercent = PushOffsetPercent;
        }
    }

    protected internal virtual void NotifyBeforeOpen(ScopeAwareAdornerLayer layer)
    {
        Drawer? firstParentDrawer = null;
        var current = Parent;
        while (current != null && current.GetType() != typeof(ScopeAwareAdornerLayer))
        {
            if (current is DrawerContainer container)
            {
                if (container.Drawer != null && container.Drawer.TryGetTarget(out var drawer))
                {
                    if (firstParentDrawer == null)
                    {
                        firstParentDrawer = drawer;
                        DataContext       =  firstParentDrawer.DataContext;
                    }
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

    internal void NotifyChildDrawerPlacementChanged(Drawer childDrawer)
    {
        _container?.NotifyChildDrawerPlacementChanged(childDrawer);
    }

    protected virtual void NotifyBeforeClose(ScopeAwareAdornerLayer layer)
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

    private void NotifyParentDrawersChildDrawerPlacementChanged()
    {
        var current = Parent;
        while (current != null && current.GetType() != typeof(ScopeAwareAdornerLayer))
        {
            if (current is DrawerContainer container)
            {
                if (container.Drawer != null && container.Drawer.TryGetTarget(out var drawer))
                {
                    drawer.NotifyChildDrawerPlacementChanged(this);
                }
            }

            current = current.Parent;
        }
    }

    private void NotifyParentDrawersChildDrawerDetached(IReadOnlyList<Drawer>? parentDrawers)
    {
        if (parentDrawers == null)
        {
            return;
        }

        foreach (var drawer in parentDrawers)
        {
            drawer.NotifyChildDrawerAboutToClose(this);
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
    
    private void ApplyDialogSizeTokenBinding()
    {
        _dialogSizeBinding?.Dispose();
        var tokenKind = SizeType switch
        {
            CustomizableSizeType.Small  => DrawerTokenKind.SmallSize,
            CustomizableSizeType.Middle => DrawerTokenKind.MiddleSize,
            CustomizableSizeType.Large  => DrawerTokenKind.LargeSize,
            _                           => DrawerTokenKind.SmallSize
        };
        _dialogSizeBinding = TokenResourceBinder.CreateTokenBinding(this, DialogSizeProperty, tokenKind);
    }

    private void ApplyPushOffsetPercentTokenBinding()
    {
        _pushOffsetPercentBinding?.Dispose();
        _pushOffsetPercentBinding = TokenResourceBinder.CreateTokenBinding(
            this,
            PushOffsetPercentProperty,
            DrawerTokenKind.PushOffsetPercent);
    }

    private Control? ResolveOpenTarget()
    {
        return OpenOn ?? TopLevel.GetTopLevel(this) as Control;
    }

    private void ConfigureOpenOnSizeChangedSubscription(Control? target)
    {
        if (!IsOpen || !DialogSize.IsPercentage || target == null)
        {
            DetachOpenOnSizeChanged();
            return;
        }

        if (ReferenceEquals(_openOnSizeChangedTarget, target))
        {
            return;
        }

        DetachOpenOnSizeChanged();
        _openOnSizeChangedTarget = target;
        target.SizeChanged += HandleOpenOnSizeChanged;
    }

    private void DetachOpenOnSizeChanged()
    {
        if (_openOnSizeChangedTarget == null)
        {
            return;
        }

        _openOnSizeChangedTarget.SizeChanged -= HandleOpenOnSizeChanged;
        _openOnSizeChangedTarget = null;
    }

    private void ReleaseDrawerContainer()
    {
        if (_container == null)
        {
            return;
        }

        _container.Release();
        _container = null;
    }

    private void CompleteDeferredDetach(int detachVersion, IReadOnlyList<Drawer>? parentDrawers)
    {
        if (detachVersion != _visualTreeVersion || this.IsAttachedToVisualTree())
        {
            return;
        }

        NotifyParentDrawersChildDrawerDetached(parentDrawers);
        CompleteDetachedFromVisualTree();
    }

    private void CompleteDetachedFromVisualTree()
    {
        DetachOpenOnSizeChanged();
        ReleaseDrawerContainer();
        _relayBindingDisposables?.Dispose();
        _relayBindingDisposables = null;
        _pushOffsetPercentBinding?.Dispose();
        _pushOffsetPercentBinding = null;
        _dialogSizeBinding?.Dispose();
        _dialogSizeBinding = null;
        ScopeAwareAdornerLayer.SetAdornedElement(this, null);
    }

    private void ConfigureEffectiveDialogSize(Control? target = null)
    {
        if (DialogSize.IsAbsolute)
        {
            SetCurrentValue(EffectiveDialogSizeProperty, DialogSize.Value);
        }
        else if (DialogSize.IsPercentage)
        {
            if (target != null)
            {
                var containerSize = target.Bounds.Size;
                if (Placement == DrawerPlacement.Top || Placement == DrawerPlacement.Bottom)
                {
                    SetCurrentValue(EffectiveDialogSizeProperty, DialogSize.Resolve(containerSize.Height));
                }
                else
                {
                    SetCurrentValue(EffectiveDialogSizeProperty, DialogSize.Resolve(containerSize.Width));
                }
            }
        }
    }
}
