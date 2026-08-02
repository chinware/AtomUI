using AtomUI.Theme.Algorithms;

namespace AtomUI.Theme;

public sealed record ThemeState
{
    private readonly IReadOnlyList<ThemeAlgorithm> _algorithms;

    public ThemeState(
        string themeId,
        IReadOnlyList<ThemeAlgorithm> algorithms,
        ThemeAppearance appearance,
        ulong fingerprint,
        long transitionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(themeId);
        ArgumentNullException.ThrowIfNull(algorithms);

        ThemeId      = themeId;
        _algorithms  = Array.AsReadOnly(algorithms.ToArray());
        Appearance   = appearance;
        Fingerprint  = fingerprint;
        TransitionId = transitionId;
    }

    public string ThemeId { get; }
    public IReadOnlyList<ThemeAlgorithm> Algorithms => _algorithms;
    public ThemeAppearance Appearance { get; }
    public ulong Fingerprint { get; }
    public long TransitionId { get; }
}
