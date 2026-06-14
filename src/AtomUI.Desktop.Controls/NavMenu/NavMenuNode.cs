using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls;

public interface INavMenuNode : ITreeNode<INavMenuNode>
{
    IDataTemplate? HeaderTemplate { get; }
    void UpdateParentNode(INavMenuNode? parentNode) => throw new NotImplementedException();
}

public class NavMenuNode : AvaloniaObject, INavMenuNode, IResourceHost, IThemeVariantHost
{
    public static readonly DirectProperty<NavMenuNode, object?> HeaderProperty =
        AvaloniaProperty.RegisterDirect<NavMenuNode, object?>(
            nameof(Header),
            o => o.Header,
            (o, v) => o.Header = v);
    
    public static readonly DirectProperty<NavMenuNode, IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.RegisterDirect<NavMenuNode, IDataTemplate?>(
            nameof(HeaderTemplate),
            o => o.HeaderTemplate,
            (o, v) => o.HeaderTemplate = v);
    
    public static readonly DirectProperty<NavMenuNode, EntityKey?> ItemKeyProperty =
        AvaloniaProperty.RegisterDirect<NavMenuNode, EntityKey?>(
            nameof(ItemKey),
            o => o.ItemKey,
            (o, v) => o.ItemKey = v);
    
    public static readonly DirectProperty<NavMenuNode, PathIcon?> IconProperty =
        AvaloniaProperty.RegisterDirect<NavMenuNode, PathIcon?>(
            nameof(Icon),
            o => o.Icon,
            (o, v) => o.Icon = v);
    
    public static readonly DirectProperty<NavMenuNode, bool> IsEnabledProperty =
        AvaloniaProperty.RegisterDirect<NavMenuNode, bool>(
            nameof(IsEnabled),
            o => o.IsEnabled,
            (o, v) => o.IsEnabled = v);
        
    private object? _header;

    public object? Header
    {
        get => _header;
        set => SetAndRaise(HeaderProperty, ref _header, value);
    }
    
    private IDataTemplate? _headerTemplate;

    public IDataTemplate? HeaderTemplate
    {
        get => _headerTemplate;
        set => SetAndRaise(HeaderTemplateProperty, ref _headerTemplate, value);
    }
    
    private EntityKey? _itemKey;

    public EntityKey? ItemKey
    {
        get => _itemKey;
        set => SetAndRaise(ItemKeyProperty, ref _itemKey, value);
    }
    
    private PathIcon? _icon;

    public PathIcon? Icon
    {
        get => _icon;
        set => SetAndRaise(IconProperty, ref _icon, value);
    }
    
    private bool _isEnabled = true;

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetAndRaise(IsEnabledProperty, ref _isEnabled, value);
    }
    
    public ITreeNode<INavMenuNode>? ParentNode { get; private set; }
    
    private readonly AvaloniaList<INavMenuNode> _children = [];
    private IResourceHost? _resourceHost;
    private int _resourceHostAttachmentCount;
    
    [Content]
    public IList<INavMenuNode> Children
    {
        get => _children;
        init => _children.AddRange(value);
    }

    IEnumerable<INavMenuNode> ITreeNode<INavMenuNode>.Children => Children;
    
    public NavMenuNode()
    {
        _children.CollectionChanged += HandleCollectionChanged;
    }

    #region 资源宿主定义

    public event EventHandler<ResourcesChangedEventArgs>? ResourcesChanged;
    public event EventHandler? ActualThemeVariantChanged;

    public bool HasResources => true;

    public ThemeVariant ActualThemeVariant =>
        (_resourceHost as IThemeVariantHost)?.ActualThemeVariant ??
        Application.Current?.ActualThemeVariant ??
        ThemeVariant.Default;

    public bool TryGetResource(object key, ThemeVariant? theme, out object? value)
    {
        if (_resourceHost?.TryFindResource(key, theme, out value) == true)
        {
            return true;
        }

        if (Application.Current?.TryGetResource(key, theme, out value) == true)
        {
            return true;
        }

        value = null;
        return false;
    }

    void IResourceHost.NotifyHostedResourcesChanged(ResourcesChangedEventArgs e)
    {
        ResourcesChanged?.Invoke(this, e);
    }

    internal IDisposable AttachResourceHost(IResourceHost resourceHost)
    {
        if (ReferenceEquals(_resourceHost, resourceHost))
        {
            _resourceHostAttachmentCount++;
        }
        else
        {
            DetachCurrentResourceHost();
            _resourceHost                = resourceHost;
            _resourceHostAttachmentCount = 1;
            RegisterResourceHost(resourceHost);
        }

        return Disposable.Create((Node: this, ResourceHost: resourceHost), state =>
        {
            state.Node.DetachResourceHost(state.ResourceHost);
        });
    }

    private void DetachResourceHost(IResourceHost resourceHost)
    {
        if (!ReferenceEquals(_resourceHost, resourceHost))
        {
            return;
        }

        _resourceHostAttachmentCount--;
        if (_resourceHostAttachmentCount > 0)
        {
            return;
        }

        DetachCurrentResourceHost();
        RaiseResourcesChanged();
        ActualThemeVariantChanged?.Invoke(this, EventArgs.Empty);
    }

    private void DetachCurrentResourceHost()
    {
        if (_resourceHost is null)
        {
            _resourceHostAttachmentCount = 0;
            return;
        }

        _resourceHost.ResourcesChanged -= HandleResourceHostResourcesChanged;

        if (_resourceHost is IThemeVariantHost themeVariantHost)
        {
            themeVariantHost.ActualThemeVariantChanged -= HandleResourceHostActualThemeVariantChanged;
        }

        _resourceHost                = null;
        _resourceHostAttachmentCount = 0;
    }

    private void RegisterResourceHost(IResourceHost resourceHost)
    {
        resourceHost.ResourcesChanged += HandleResourceHostResourcesChanged;

        if (resourceHost is IThemeVariantHost themeVariantHost)
        {
            themeVariantHost.ActualThemeVariantChanged += HandleResourceHostActualThemeVariantChanged;
        }

        RaiseResourcesChanged();
        ActualThemeVariantChanged?.Invoke(this, EventArgs.Empty);
    }

    private void HandleResourceHostResourcesChanged(object? sender, ResourcesChangedEventArgs e)
    {
        ResourcesChanged?.Invoke(this, e);
    }

    private void HandleResourceHostActualThemeVariantChanged(object? sender, EventArgs e)
    {
        ActualThemeVariantChanged?.Invoke(this, e);
    }

    private void RaiseResourcesChanged()
    {
        ResourcesChanged?.Invoke(this, ResourcesChangedEventArgs.Create());
    }

    #endregion
    
    public void UpdateParentNode(INavMenuNode? parentNode)
    {
        ParentNode = parentNode;
    }
    
    private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            if (e.NewItems != null)
            {
                foreach (var child in e.NewItems)
                {
                    if (child is INavMenuNode menuItemNode)
                    {
                        menuItemNode.UpdateParentNode(this);
                    }
                }
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            if (e.OldItems != null)
            {
                foreach (var child in e.OldItems)
                {
                    if (child is INavMenuNode menuItemNode)
                    {
                        menuItemNode.UpdateParentNode(null);
                    }
                }
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            foreach (var child in Children)
            {
                child.UpdateParentNode(this);
            }
        }
    }
}
