using Avalonia;
using SchemaControlTokenIdentity = AtomUI.Theme.Schema.ControlTokenIdentity;

namespace AtomUI.Theme.Resources;

public static class ControlTokenScope
{
    public static readonly AttachedProperty<SchemaControlTokenIdentity> IdentityProperty =
        AvaloniaProperty.RegisterAttached<ThemeConfigProvider, AvaloniaObject, SchemaControlTokenIdentity>(
            "Identity");

    public static SchemaControlTokenIdentity GetIdentity(AvaloniaObject value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value.GetValue(IdentityProperty);
    }

    public static void SetIdentity(AvaloniaObject value, SchemaControlTokenIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(value);
        value.SetValue(IdentityProperty, identity);
    }

    internal static bool TryGetIdentity(
        AvaloniaObject value,
        out SchemaControlTokenIdentity identity)
    {
        if (value.IsSet(IdentityProperty))
        {
            identity = value.GetValue(IdentityProperty);
            return true;
        }

        identity = default;
        return false;
    }
}
