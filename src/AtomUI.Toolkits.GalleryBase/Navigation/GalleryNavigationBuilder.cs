using AtomUI.Controls;
using AtomUI.Toolkits.GalleryBase.Configuration;

namespace AtomUI.Toolkits.GalleryBase.Navigation;

public sealed class GalleryNavigationBuilder
{
    private readonly GalleryNavigationBuilderState _state = new();
    private readonly List<GalleryNavigationNodeBuilder> _nodes = new();

    public EntityKey DefaultRoute { get; set; }

    public IList<EntityKey> DefaultOpenKeys { get; } = new List<EntityKey>();

    public GalleryNavigationBuilder AddPage(EntityKey key, object header, object? icon = null)
    {
        AddNode(_nodes, key, header, icon, isRoute: true);
        return this;
    }

    public GalleryNavigationGroupBuilder AddGroup(EntityKey key, object header, object? icon = null)
    {
        var node = AddNode(_nodes, key, header, icon, isRoute: false);
        return new GalleryNavigationGroupBuilder(_state, node.Children);
    }

    internal IReadOnlyList<GalleryNavigationNode> BuildNodes()
    {
        return Array.AsReadOnly(_nodes.Select(BuildNode).ToArray());
    }

    private GalleryNavigationNodeBuilder AddNode(IList<GalleryNavigationNodeBuilder> nodes,
                                                EntityKey key,
                                                object header,
                                                object? icon,
                                                bool isRoute)
    {
        return _state.AddNode(nodes, key, header, icon, isRoute);
    }

    private static GalleryNavigationNode BuildNode(GalleryNavigationNodeBuilder node)
    {
        return new GalleryNavigationNode(
            node.Key,
            node.Header,
            node.Icon,
            node.IsRoute,
            Array.AsReadOnly(node.Children.Select(BuildNode).ToArray()));
    }
}

public sealed class GalleryNavigationGroupBuilder
{
    private readonly GalleryNavigationBuilderState      _state;
    private readonly IList<GalleryNavigationNodeBuilder> _nodes;

    internal GalleryNavigationGroupBuilder(GalleryNavigationBuilderState state,
                                           IList<GalleryNavigationNodeBuilder> nodes)
    {
        _state = state;
        _nodes = nodes;
    }

    public GalleryNavigationGroupBuilder AddPage(EntityKey key, object header, object? icon = null)
    {
        _state.AddNode(_nodes, key, header, icon, isRoute: true);
        return this;
    }

    public GalleryNavigationGroupBuilder AddGroup(EntityKey key, object header, object? icon = null)
    {
        var node = _state.AddNode(_nodes, key, header, icon, isRoute: false);
        return new GalleryNavigationGroupBuilder(_state, node.Children);
    }
}

internal sealed class GalleryNavigationBuilderState
{
    private readonly HashSet<EntityKey> _keys = new();

    public GalleryNavigationNodeBuilder AddNode(IList<GalleryNavigationNodeBuilder> nodes,
                                                EntityKey key,
                                                object header,
                                                object? icon,
                                                bool isRoute)
    {
        if (string.IsNullOrWhiteSpace(key.Value))
        {
            throw new GalleryConfigurationException("Gallery navigation node Key must not be empty.");
        }

        if (!_keys.Add(key))
        {
            throw new GalleryConfigurationException(
                $"Gallery navigation node key '{key}' is already registered.");
        }

        if (header is null)
        {
            throw new GalleryConfigurationException(
                $"Gallery navigation node '{key}' Header must not be null.");
        }

        var node = new GalleryNavigationNodeBuilder(key, header, icon, isRoute);
        nodes.Add(node);
        return node;
    }
}

internal sealed class GalleryNavigationNodeBuilder
{
    public EntityKey Key { get; }

    public object Header { get; }

    public object? Icon { get; }

    public bool IsRoute { get; }

    public IList<GalleryNavigationNodeBuilder> Children { get; } = new List<GalleryNavigationNodeBuilder>();

    public GalleryNavigationNodeBuilder(EntityKey key, object header, object? icon, bool isRoute)
    {
        Key     = key;
        Header  = header;
        Icon    = icon;
        IsRoute = isRoute;
    }
}
