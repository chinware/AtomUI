namespace AtomUI.Localization;

public interface ILanguageManager
{
    LanguageState Current { get; }

    IReadOnlyList<LanguageDefinition> SupportedLanguages { get; }

    event EventHandler<LanguageChangedEventArgs>? LanguageChanged;

    LanguageChangeResult ChangeLanguage(LanguageTag language);
}
