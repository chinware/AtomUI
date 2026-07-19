using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AtomUI.Theme;

internal static class ThemeScope
{
    public static readonly AttachedProperty<ThemeContext?> ContextProperty =
        AvaloniaProperty.RegisterAttached<ThemeConfigProvider, StyledElement, ThemeContext?>(
            "Context",
            inherits: true);

    internal static ThemeContext? ResolveContext(
        StyledElement owner,
        ILogicalRoot? knownRoot = null)
    {
        ArgumentNullException.ThrowIfNull(owner);
        if (owner.GetValue(ContextProperty) is { } inheritedContext)
        {
            return inheritedContext;
        }

        var root = knownRoot ?? FindLogicalRoot(owner);
        if (root is not TopLevel topLevel)
        {
            return null;
        }

        if (topLevel.GetValue(ContextProperty) is { } rootContext)
        {
            return rootContext;
        }

        var manager = AvaloniaLocator.Current.GetService(typeof(ThemeManager)) as ThemeManager;
        if (manager is null)
        {
            return null;
        }

        rootContext = manager.RootContext;
        topLevel.SetValue(ContextProperty, rootContext);
        return rootContext;
    }

    private static ILogicalRoot? FindLogicalRoot(StyledElement owner)
    {
        ILogical? current = owner;
        while (current is not null)
        {
            if (current is ILogicalRoot root)
            {
                return root;
            }
            current = current.GetLogicalParent();
        }
        return null;
    }
}
