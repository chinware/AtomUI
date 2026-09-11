namespace AtomUI.Localization;

public enum LanguageChangeStatus : byte
{
    Committed = 0,
    NoOp = 1
}

public sealed record LanguageChangeResult
{
    private LanguageChangeResult(
        LanguageChangeStatus status,
        LanguageState oldState,
        LanguageState newState)
    {
        Status = status;
        OldState = oldState;
        NewState = newState;
    }

    public LanguageChangeStatus Status { get; }

    public LanguageState OldState { get; }

    public LanguageState NewState { get; }

    public static LanguageChangeResult Committed(LanguageState oldState, LanguageState newState)
    {
        ArgumentNullException.ThrowIfNull(oldState);
        ArgumentNullException.ThrowIfNull(newState);
        return new LanguageChangeResult(LanguageChangeStatus.Committed, oldState, newState);
    }

    public static LanguageChangeResult NoOp(LanguageState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        return new LanguageChangeResult(LanguageChangeStatus.NoOp, state, state);
    }
}
