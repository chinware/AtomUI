using System.Collections.ObjectModel;
using System.Globalization;

namespace AtomUI.Localization;

internal static class LanguageFallbackResolver
{
    internal static IReadOnlyList<LanguageTag> Resolve(
        LanguageTag requestedLanguage,
        CultureInfo formattingCulture)
    {
        if (requestedLanguage == default)
        {
            throw new ArgumentException("A valid requested language is required.", nameof(requestedLanguage));
        }
        ArgumentNullException.ThrowIfNull(formattingCulture);

        var candidates = new List<LanguageTag> { requestedLanguage };
        var seen = new HashSet<LanguageTag> { requestedLanguage };
        if (requestedLanguage == LanguageTags.EnUS)
        {
            return Array.AsReadOnly(candidates.ToArray());
        }

        var parent = formattingCulture.Parent;
        while (!string.IsNullOrEmpty(parent.Name))
        {
            var candidate = LanguageTag.FromCultureInfo(parent);
            if (seen.Add(candidate))
            {
                candidates.Add(candidate);
            }
            parent = parent.Parent;
        }

        if (seen.Add(LanguageTags.EnUS))
        {
            candidates.Add(LanguageTags.EnUS);
        }

        return new ReadOnlyCollection<LanguageTag>(candidates.ToArray());
    }
}
