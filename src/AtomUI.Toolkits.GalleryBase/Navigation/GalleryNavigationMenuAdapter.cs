using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls;
using AtomUI.Toolkits.GalleryBase.Localization;
using Avalonia.Controls;

namespace AtomUI.Toolkits.GalleryBase.Navigation;

public sealed class GalleryNavigationMenuAdapter
{
    public IReadOnlyList<NavMenuNode> BuildNodes(IEnumerable<GalleryNavigationNode> nodes)
    {
        return nodes.Select(BuildNode).ToArray();
    }

    public IList<TreeNodePath> BuildDefaultOpenPaths(IEnumerable<EntityKey> defaultOpenKeys)
    {
        return defaultOpenKeys.Select(key => new TreeNodePath(key.Value)).ToArray();
    }

    private static NavMenuNode BuildNode(GalleryNavigationNode node)
    {
        var navMenuNode = new NavMenuNode
        {
            Header  = ResolveHeader(node.Header),
            ItemKey = node.Key,
            Icon    = ResolveIcon(node.Icon)
        };

        foreach (var child in node.Children)
        {
            navMenuNode.Children.Add(BuildNode(child));
        }

        return navMenuNode;
    }

    private static object ResolveHeader(object header)
    {
        return header is IGalleryLocalizedText localizedText
            ? localizedText.Resolve()
            : header;
    }

    private static PathIcon? ResolveIcon(object? icon)
    {
        return icon switch
        {
            null              => null,
            PathIcon pathIcon => pathIcon,
            Func<PathIcon> factory => factory(),
            _                 => null
        };
    }
}
