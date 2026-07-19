using AtomUI.Theme.Configuration;

namespace AtomUI.Theme;

public enum ThemeTransitionReason : byte
{
    Startup,
    UserRequest,
    FollowSystem,
    LocalConfigChanged,
    ScopeTopologyChanged
}

public sealed record ThemeRequest(
    string ThemeId,
    ThemeConfig? Config,
    ThemeTransitionReason Reason);
