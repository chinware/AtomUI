using System.Globalization;

namespace AtomUI.Localization;

public sealed record LanguageDefinition
{
    public LanguageDefinition(
        LanguageTag tag,
        CultureInfo formattingCulture,
        string nativeName,
        LanguageTextDirection textDirection)
    {
        if (tag == default)
        {
            throw new ArgumentException("A language definition requires a valid language tag.", nameof(tag));
        }

        ArgumentNullException.ThrowIfNull(formattingCulture);
        ArgumentNullException.ThrowIfNull(nativeName);

        if (string.IsNullOrWhiteSpace(nativeName))
        {
            throw new ArgumentException("A native language name cannot be empty or whitespace.", nameof(nativeName));
        }

        if (textDirection is not LanguageTextDirection.LeftToRight and
            not LanguageTextDirection.RightToLeft)
        {
            throw new ArgumentOutOfRangeException(nameof(textDirection), textDirection, "Unknown text direction.");
        }

        Tag = tag;
        FormattingCulture = CultureInfo.ReadOnly((CultureInfo)formattingCulture.Clone());
        NativeName = nativeName;
        TextDirection = textDirection;
    }

    public LanguageTag Tag { get; }

    public CultureInfo FormattingCulture { get; }

    public string NativeName { get; }

    public LanguageTextDirection TextDirection { get; }
}
