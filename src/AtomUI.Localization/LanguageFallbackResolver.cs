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

        var requestedValue = requestedLanguage.Value;
        var structuralValue = RemoveExtensionsAndPrivateUse(requestedValue);
        if (structuralValue.Length > 0)
        {
            AddCandidate(structuralValue, seen, candidates);
            while (true)
            {
                if (TryGetChineseScriptFallback(structuralValue, out var scriptFallback))
                {
                    AddCandidate(scriptFallback, seen, candidates);
                }

                var separator = structuralValue.LastIndexOf('-');
                if (separator < 0)
                {
                    break;
                }

                structuralValue = structuralValue.Substring(0, separator);
                AddCandidate(structuralValue, seen, candidates);
            }
        }

        if (seen.Add(LanguageTags.EnUS))
        {
            candidates.Add(LanguageTags.EnUS);
        }

        return new ReadOnlyCollection<LanguageTag>(candidates.ToArray());
    }

    internal static bool HasEnglishPrimaryLanguage(LanguageTag language)
    {
        if (language == default)
        {
            throw new ArgumentException("A valid language is required.", nameof(language));
        }

        var value = language.Value;
        var separator = value.IndexOf('-');
        var primaryLanguage = separator < 0 ? value : value.Substring(0, separator);
        return string.Equals(primaryLanguage, "en", StringComparison.Ordinal);
    }

    private static string RemoveExtensionsAndPrivateUse(string value)
    {
        if (value.StartsWith("x-", StringComparison.Ordinal))
        {
            return string.Empty;
        }

        var subtagStart = 0;
        var isPrimaryLanguage = true;
        while (subtagStart < value.Length)
        {
            var separator = value.IndexOf('-', subtagStart);
            var subtagLength = (separator < 0 ? value.Length : separator) - subtagStart;
            if (!isPrimaryLanguage && subtagLength == 1)
            {
                return value.Substring(0, subtagStart - 1);
            }

            if (separator < 0)
            {
                break;
            }

            isPrimaryLanguage = false;
            subtagStart = separator + 1;
        }

        return value;
    }

    private static bool TryGetChineseScriptFallback(string value, out string fallback)
    {
        fallback = string.Empty;
        if (!value.StartsWith("zh-", StringComparison.Ordinal) || value.IndexOf('-', 3) >= 0)
        {
            return false;
        }

        var region = value.Substring(3);
        switch (region)
        {
            case "CN":
            case "SG":
                fallback = "zh-Hans";
                return true;
            case "HK":
            case "MO":
            case "TW":
                fallback = "zh-Hant";
                return true;
            default:
                return false;
        }
    }

    private static void AddCandidate(
        string value,
        ISet<LanguageTag> seen,
        ICollection<LanguageTag> candidates)
    {
        var candidate = LanguageTag.Parse(value);
        if (seen.Add(candidate))
        {
            candidates.Add(candidate);
        }
    }
}
