namespace AtomUI.Theme;

public sealed record ThemeState
{
    private readonly IReadOnlyList<string> _algorithms;

    public ThemeState(
        string themeId,
        IReadOnlyList<string> algorithms,
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
    public IReadOnlyList<string> Algorithms => _algorithms;
    public ThemeAppearance Appearance { get; }
    public ulong Fingerprint { get; }
    public long TransitionId { get; }
}
