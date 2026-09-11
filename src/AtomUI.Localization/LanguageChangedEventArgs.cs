namespace AtomUI.Localization;

public sealed class LanguageChangedEventArgs : EventArgs
{
    public LanguageChangedEventArgs(LanguageChangeResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.Status != LanguageChangeStatus.Committed)
        {
            throw new ArgumentException(
                "LanguageChangedEventArgs requires a committed language change result.",
                nameof(result));
        }

        Result = result;
    }

    public LanguageChangeResult Result { get; }
}
