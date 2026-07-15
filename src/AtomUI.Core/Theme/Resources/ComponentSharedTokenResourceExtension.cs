using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using AtomUI.Theme.Styling;

namespace AtomUI.Theme.Resources;

public class ComponentSharedTokenResourceExtension : MarkupExtension
{
    private readonly object _resourceKey;

    public ComponentSharedTokenResourceExtension(
        string? catalog,
        string componentId,
        SharedTokenKind kind)
    {
        _resourceKey = new ComponentTokenIdentity(catalog, componentId).GetSharedTokenResourceKey(kind);
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new DynamicResourceExtension(_resourceKey);
    }
}
