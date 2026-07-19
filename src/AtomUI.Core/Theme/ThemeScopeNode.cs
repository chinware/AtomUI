using AtomUI.Theme.Configuration;

namespace AtomUI.Theme;

internal readonly record struct ThemeScopeStamp(
    long RegistrationId,
    long ParentRegistrationId,
    long ConfigRevision);

internal sealed class ThemeScopeNode
{
    internal ThemeScopeNode(
        long registrationId,
        long parentRegistrationId,
        ThemeConfigProvider provider,
        ThemeContext context,
        ThemeConfig config,
        ThemeConfig lastValidConfig)
    {
        RegistrationId       = registrationId;
        ParentRegistrationId = parentRegistrationId;
        Provider             = provider;
        Context              = context;
        Config               = config;
        LastValidConfig      = lastValidConfig;
        Children             = new SortedSet<long>();
    }

    internal long RegistrationId { get; }
    internal long ParentRegistrationId { get; set; }
    internal long ConfigRevision { get; set; }
    internal ThemeConfigProvider Provider { get; }
    internal ThemeContext Context { get; }
    internal ThemeConfig Config { get; set; }
    internal ThemeConfig LastValidConfig { get; set; }
    internal SortedSet<long> Children { get; }

    internal ThemeScopeStamp Stamp => new(
        RegistrationId,
        ParentRegistrationId,
        ConfigRevision);
}

internal sealed record ThemeScopeNodeCapture(
    ThemeScopeStamp Stamp,
    ThemeContext Context,
    ThemeConfig Config,
    ThemeConfig LastValidConfig);

internal sealed class ThemeScopeCapture
{
    internal ThemeScopeCapture(
        long topologyRevision,
        long rootRegistrationId,
        IReadOnlyList<ThemeScopeNodeCapture> nodes)
    {
        TopologyRevision   = topologyRevision;
        RootRegistrationId = rootRegistrationId;
        Nodes              = nodes;
    }

    internal long TopologyRevision { get; }
    internal long RootRegistrationId { get; }
    internal IReadOnlyList<ThemeScopeNodeCapture> Nodes { get; }
}
