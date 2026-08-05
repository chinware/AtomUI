using System.Globalization;

namespace AtomUI.Localization;

public sealed record LanguageState
{
    public LanguageState(
        LanguageTag currentLanguage,
        CultureInfo formattingCulture,
        LanguageTextDirection textDirection,
        long revision)
    {
        if (currentLanguage == default)
        {
            throw new ArgumentException("A language state requires a valid current language.", nameof(currentLanguage));
        }

        ArgumentNullException.ThrowIfNull(formattingCulture);

        if (textDirection is not LanguageTextDirection.LeftToRight and
            not LanguageTextDirection.RightToLeft)
        {
            throw new ArgumentOutOfRangeException(nameof(textDirection), textDirection, "Unknown text direction.");
        }

        if (revision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(revision), revision, "A language revision cannot be negative.");
        }

        CurrentLanguage = currentLanguage;
        FormattingCulture = CultureInfo.ReadOnly((CultureInfo)formattingCulture.Clone());
        TextDirection = textDirection;
        Revision = revision;
    }

    public LanguageTag CurrentLanguage { get; }

    public CultureInfo FormattingCulture { get; }

    public LanguageTextDirection TextDirection { get; }

    public long Revision { get; }
}
