namespace AtomUI.Theme.Language;

public interface ILanguageManager
{
    LanguageVariant LanguageVariant { get; set; }

    event EventHandler<LanguageVariantChangedEventArgs>? LanguageVariantChanged;
}
