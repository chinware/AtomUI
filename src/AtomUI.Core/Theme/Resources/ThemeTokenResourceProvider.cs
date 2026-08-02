using AtomUI.Theme.Compilation;
using AtomUI.Theme.Schema;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Theme.Resources;

internal sealed class ThemeTokenResourceProvider : ResourceProvider
{
    private ThemeSnapshot? _snapshot;
    private readonly ThemeContext? _context;

    internal ThemeTokenResourceProvider(ThemeSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        _snapshot = snapshot;
    }

    internal ThemeTokenResourceProvider(ThemeContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public ThemeSnapshot Snapshot => _context?.Snapshot ?? Volatile.Read(ref _snapshot)!;

    public override bool HasResources => true;

    public override bool TryGetResource(object key, ThemeVariant? theme, out object? value)
    {
        var snapshot = Snapshot;
        if (key is ControlTokenResourceKey controlKey)
        {
            var controlSlot = controlKey.ControlSlot;
            if (!controlKey.IsBound)
            {
                ControlTokenDescriptor? descriptor;
                if (controlKey.ControlType is { } controlType)
                {
                    if (!snapshot.Registry.TryGetControl(controlType, out descriptor))
                    {
                        throw new InvalidOperationException(
                            $"Control type '{controlType.FullName}' is not registered in the active theme schema.");
                    }
                }
                else if (!snapshot.Registry.TryGetControl(controlKey.Identity, out descriptor))
                {
                    throw new InvalidOperationException(
                        $"Control Token identity '{controlKey.Identity}' is not registered in the active theme schema.");
                }
                controlSlot = descriptor.Slot;
            }

            if ((uint)controlSlot >= (uint)snapshot.Controls.Count)
            {
                value = null;
                return false;
            }

            var control = snapshot.Controls[controlSlot];
            return control.TryGetSharedResource(
                snapshot.Registry.GetSharedResourceKey(controlKey.Kind),
                snapshot.GlobalResources,
                out value);
        }

        if (key is ControlOwnTokenResourceKey ownKey)
        {
            if (!snapshot.Registry.TryGetControl(ownKey.ControlType, out var descriptor))
            {
                throw new InvalidOperationException(
                    $"Control type '{ownKey.ControlType.FullName}' is not registered in the active theme schema.");
            }
            if (!snapshot.Registry.TryGetControlResourceSlot(ownKey.ResourceKey, out var ownControlSlot) ||
                ownControlSlot != descriptor.Slot)
            {
                throw new InvalidOperationException(
                    $"Own Token resource key '{ownKey.ResourceKey}' does not belong to Control type " +
                    $"'{ownKey.ControlType.FullName}'.");
            }

            return snapshot.Controls[descriptor.Slot]
                           .ControlResources
                           .TryGetValue(ownKey.ResourceKey, out value);
        }

        if (snapshot.GlobalResources.TryGetValue(key, out value))
        {
            return true;
        }
        if (snapshot.Registry.TryGetControlResourceSlot(key, out var resourceControlSlot) &&
            (uint)resourceControlSlot < (uint)snapshot.Controls.Count)
        {
            return snapshot.Controls[resourceControlSlot].ControlResources.TryGetValue(key, out value);
        }

        value = null;
        return false;
    }

    internal void PrepareSnapshot(ThemeSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        if (_context is not null)
        {
            throw new InvalidOperationException("A context-backed resource provider is updated through its ThemeContext.");
        }
        Volatile.Write(ref _snapshot, snapshot);
    }

    internal void PublishSnapshotChanged()
    {
        RaiseResourcesChanged();
    }

    public void ReplaceSnapshot(ThemeSnapshot snapshot)
    {
        PrepareSnapshot(snapshot);
        PublishSnapshotChanged();
    }
}
