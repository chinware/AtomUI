using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using AtomUI.Theme.Styling;

namespace AtomUI.Theme.Resources;

public class ControlSharedTokenResourceExtension : MarkupExtension
{
    private readonly object _resourceKey;

    public ControlSharedTokenResourceExtension(
        string? catalog,
        string controlId,
        SharedTokenKind kind)
    {
        _resourceKey = new ControlTokenIdentity(catalog, controlId).GetSharedTokenResourceKey(kind);
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new DynamicResourceExtension(_resourceKey);
    }
}
