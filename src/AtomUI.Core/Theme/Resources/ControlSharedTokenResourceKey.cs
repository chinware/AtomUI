using SchemaControlTokenIdentity = AtomUI.Theme.Schema.ControlTokenIdentity;

namespace AtomUI.Theme.Resources;

internal readonly record struct ControlSharedTokenResourceKey
{
    internal ControlSharedTokenResourceKey(int controlSlot, SharedTokenKind kind)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(controlSlot);
        ControlSlot = controlSlot;
        Kind        = kind;
        Identity    = default;
        IsBound     = true;
    }

    private ControlSharedTokenResourceKey(SchemaControlTokenIdentity identity, SharedTokenKind kind)
    {
        ControlSlot = -1;
        Kind        = kind;
        Identity    = identity;
        IsBound     = false;
    }

    internal int ControlSlot { get; }
    internal SharedTokenKind Kind { get; }
    internal SchemaControlTokenIdentity Identity { get; }
    internal bool IsBound { get; }

    internal static ControlSharedTokenResourceKey Unbound(
        SchemaControlTokenIdentity identity,
        SharedTokenKind kind)
    {
        return new ControlSharedTokenResourceKey(identity, kind);
    }
}
