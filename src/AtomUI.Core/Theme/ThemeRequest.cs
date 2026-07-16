namespace AtomUI.Theme;

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
