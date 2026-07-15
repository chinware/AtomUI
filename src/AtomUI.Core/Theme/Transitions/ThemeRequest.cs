using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme.Transitions;

internal enum ThemeTransitionReason
{
    Startup,
    UserRequest,
    ApplicationThemeVariantChanged,
    PropertyChanged
}

internal sealed record ThemeRequest(
    string ThemeId,
    IReadOnlyList<ThemeAlgorithm> Algorithms,
    ThemeTransitionReason Reason,
    IReadOnlyDictionary<string, string>? RuntimeOverrides = null);
