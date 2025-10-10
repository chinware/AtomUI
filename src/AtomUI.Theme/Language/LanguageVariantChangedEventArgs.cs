namespace AtomUI.Theme.Language;

public class LanguageVariantChangedEventArgs : EventArgs
{
    public LanguageVariant? OldLanguage { get; }
    public LanguageVariant NewLanguage { get; }

    public LanguageVariantChangedEventArgs(LanguageVariant newLanguage, LanguageVariant? oldLanguage)
    {
        NewLanguage = newLanguage;
        OldLanguage = oldLanguage;
    }
}
