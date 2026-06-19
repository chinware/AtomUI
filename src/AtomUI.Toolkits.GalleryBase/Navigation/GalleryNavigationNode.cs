using AtomUI.Controls;

namespace AtomUI.Toolkits.GalleryBase.Navigation;

public sealed class GalleryNavigationNode
{
    public EntityKey Key { get; }

    public object Header { get; }

    public object? Icon { get; }

    public bool IsRoute { get; }

    public IReadOnlyList<GalleryNavigationNode> Children { get; }

    internal GalleryNavigationNode(EntityKey key,
                                   object header,
                                   object? icon,
                                   bool isRoute,
                                   IReadOnlyList<GalleryNavigationNode> children)
    {
        Key      = key;
        Header   = header;
        Icon     = icon;
        IsRoute  = isRoute;
        Children = children;
    }
}
