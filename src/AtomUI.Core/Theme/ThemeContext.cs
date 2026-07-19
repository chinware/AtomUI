using AtomUI.Theme.Compilation;
using AtomUI.Theme.Resources;

namespace AtomUI.Theme;

internal sealed class ThemeContext
{
    private ThemeSnapshot _snapshot;
    private ThemeAppearance _appearance;

    internal ThemeContext(
        ThemeManager manager,
        ThemeSnapshot snapshot,
        long registrationId)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(snapshot);

        Manager         = manager;
        RegistrationId  = registrationId;
        _snapshot       = snapshot;
        _appearance     = snapshot.Appearance;
        ResourceProvider = new ThemeTokenResourceProvider(this);
    }

    internal ThemeManager Manager { get; }
    internal long RegistrationId { get; }
    internal ThemeSnapshot Snapshot => Volatile.Read(ref _snapshot);
    internal ThemeAppearance Appearance => _appearance;
    internal ThemeTokenResourceProvider ResourceProvider { get; }

    internal event EventHandler? Published;

    internal void Commit(ThemeSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        Volatile.Write(ref _snapshot, snapshot);
        _appearance = snapshot.Appearance;
    }

    internal void Publish(bool notifyResources = true)
    {
        if (notifyResources)
        {
            ResourceProvider.PublishSnapshotChanged();
        }
        Published?.Invoke(this, EventArgs.Empty);
    }
}
