using System.Text.RegularExpressions;

namespace AtomUIGallery.Tests.ShowCases;

internal static class ShowCaseSnapshotMarkup
{
    public static string Normalize(string source)
    {
        return string.Join(
            "\n",
            StripDeferredLoadingMarkup(source)
                .Replace("\r\n", "\n")
                .Split('\n')
                .Select(line => line.Trim())
                .Where(line => line.Length > 0));
    }

    public static string StripDeferredLoadingMarkup(string source)
    {
        var normalized = source.Replace("\r\n", "\n");

        normalized = Regex.Replace(
            normalized,
            @"\s*<gallery:ShowCaseItem\.DeferredContentTemplate>\s*<DataTemplate(?:\s+x:DataType=""[^""]+"")?>\s*",
            "\n",
            RegexOptions.CultureInvariant);

        normalized = Regex.Replace(
            normalized,
            @"\s*</DataTemplate>\s*</gallery:ShowCaseItem\.DeferredContentTemplate>\s*",
            "\n",
            RegexOptions.CultureInvariant);

        normalized = Regex.Replace(
            normalized,
            @"\s*IsDeferredContentEnabled=""True""",
            string.Empty,
            RegexOptions.CultureInvariant);

        normalized = Regex.Replace(
            normalized,
            @"\s*DeferredPlaceholderHeight=""[^""]+""",
            string.Empty,
            RegexOptions.CultureInvariant);

        return normalized;
    }
}
