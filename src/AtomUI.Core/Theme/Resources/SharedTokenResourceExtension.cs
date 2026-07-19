using AtomUI.Theme.Resources;
using Avalonia;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.XamlIl.Runtime;

namespace AtomUI.Theme.Styling;

public class SharedTokenResourceExtension : TokenResourceExtension<SharedTokenKind>
{
    public SharedTokenResourceExtension()
    {
    }

    public SharedTokenResourceExtension(SharedTokenKind kind)
        : base(kind)
    {}

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var parentStack = serviceProvider?.GetService(
            typeof(IAvaloniaXamlIlParentStackProvider)) as IAvaloniaXamlIlParentStackProvider;
        if (parentStack is null)
        {
            return new DynamicResourceExtension(Kind);
        }

        Schema.ControlTokenIdentity? identity = null;
        ThemeContext? context = null;
        foreach (var parent in parentStack.Parents)
        {
            if (identity is null &&
                parent is AvaloniaObject avaloniaObject &&
                ControlTokenScope.TryGetIdentity(avaloniaObject, out var currentIdentity))
            {
                identity = currentIdentity;
            }

            if (context is null && parent is StyledElement styledElement)
            {
                context = styledElement.GetValue(ThemeScope.ContextProperty);
            }

            if (identity is not null && context is not null)
            {
                break;
            }
        }

        if (identity is null)
        {
            return new DynamicResourceExtension(Kind);
        }

        object resourceKey = context is null
            ? ControlSharedTokenResourceKey.Unbound(identity.Value, Kind)
            : context.Snapshot.Registry.GetControlSharedResourceKey(identity.Value, Kind);
        return new DynamicResourceExtension(resourceKey);
    }
}
