using System.Threading;
using AtomUI.Theme.Compilation;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Theme.Resources;

internal sealed class ThemeTokenResourceProvider : ResourceProvider
{
    private ThemeSnapshot _snapshot;

    internal ThemeTokenResourceProvider(ThemeSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        _snapshot = snapshot;
    }

    public ThemeSnapshot Snapshot => Volatile.Read(ref _snapshot);

    public override bool HasResources => true;

    public override bool TryGetResource(object key, ThemeVariant? theme, out object? value)
    {
        var snapshot = Snapshot;
        if (key is ControlSharedTokenResourceKey controlKey)
        {
            var identity = new ControlTokenIdentity(controlKey.Catalog, controlKey.ControlId);
            if (snapshot.Controls.TryGetValue(identity, out var control))
            {
                return control.TryGetSharedResource(controlKey.Kind, snapshot.SharedResources, out value);
            }

            value = null;
            return false;
        }

        return snapshot.Resources.TryGetValue(key, out value);
    }

    internal void PrepareSnapshot(ThemeSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
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
