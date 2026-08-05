using System.Collections.Immutable;
using AtomUI.Generator.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Generator.Localization;

internal static class LanguageDataFileParser
{
    private const string SchemaHeader = "# atomui-language-tags-schema: 1";

    internal static LanguageDataParseResult Parse(
        AdditionalText additionalText,
        CancellationToken cancellationToken)
    {
        var text = additionalText.GetText(cancellationToken);
        if (text is null)
        {
            return InvalidFile(additionalText.Path, "the file cannot be read");
        }

        var entries = ImmutableArray.CreateBuilder<LanguageDataEntry>();
        var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
        var identifiers = new HashSet<string>(StringComparer.Ordinal);
        var tags = new HashSet<string>(StringComparer.Ordinal);
        var hasSchemaHeader = false;

        for (var lineIndex = 0; lineIndex < text.Lines.Count; lineIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var textLine = text.Lines[lineIndex];
            var line = textLine.ToString();
            var trimmed = line.Trim();
            if (trimmed.Length == 0)
            {
                continue;
            }

            if (string.Equals(trimmed, SchemaHeader, StringComparison.Ordinal))
            {
                hasSchemaHeader = true;
                continue;
            }

            if (trimmed.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            var location = CreateLocation(additionalText.Path, textLine);
            var columns = line.Split('\t');
            if (columns.Length != 5)
            {
                diagnostics.Add(CreateInvalidDiagnostic(
                    additionalText.Path,
                    lineIndex,
                    location,
                    "a record must contain exactly five tab-separated columns"));
                continue;
            }

            var identifier = columns[0].Trim();
            var sourceTag = columns[1].Trim();
            var cultureName = columns[2].Trim();
            var nativeName = columns[3].Trim();
            var direction = columns[4].Trim();

            if (!SyntaxFacts.IsValidIdentifier(identifier))
            {
                diagnostics.Add(CreateInvalidDiagnostic(
                    additionalText.Path,
                    lineIndex,
                    location,
                    $"'{identifier}' is not a valid C# identifier"));
                continue;
            }

            if (!LanguageDataTagNormalizer.TryNormalize(sourceTag, out var canonicalTag))
            {
                diagnostics.Add(CreateInvalidDiagnostic(
                    additionalText.Path,
                    lineIndex,
                    location,
                    $"'{sourceTag}' is not a valid BCP 47 language, script, or region tag"));
                continue;
            }

            if (!string.Equals(sourceTag, canonicalTag, StringComparison.Ordinal))
            {
                diagnostics.Add(CreateInvalidDiagnostic(
                    additionalText.Path,
                    lineIndex,
                    location,
                    $"'{sourceTag}' is not canonical BCP 47; use '{canonicalTag}'"));
                continue;
            }

            if (cultureName.Length == 0 || nativeName.Length == 0)
            {
                diagnostics.Add(CreateInvalidDiagnostic(
                    additionalText.Path,
                    lineIndex,
                    location,
                    "culture name and native name cannot be empty"));
                continue;
            }

            var isRightToLeft = false;
            if (direction == "RightToLeft")
            {
                isRightToLeft = true;
            }
            else if (direction != "LeftToRight")
            {
                diagnostics.Add(CreateInvalidDiagnostic(
                    additionalText.Path,
                    lineIndex,
                    location,
                    "text direction must be LeftToRight or RightToLeft"));
                continue;
            }

            if (!identifiers.Add(identifier))
            {
                diagnostics.Add(CreateDuplicateDiagnostic(
                    additionalText.Path,
                    lineIndex,
                    location,
                    $"identifier '{identifier}'"));
                continue;
            }

            if (!tags.Add(canonicalTag))
            {
                diagnostics.Add(CreateDuplicateDiagnostic(
                    additionalText.Path,
                    lineIndex,
                    location,
                    $"tag '{canonicalTag}'"));
                continue;
            }

            entries.Add(new LanguageDataEntry(
                identifier,
                canonicalTag,
                cultureName,
                nativeName,
                isRightToLeft,
                location));
        }

        if (!hasSchemaHeader)
        {
            diagnostics.Add(CreateInvalidDiagnostic(
                additionalText.Path,
                0,
                Location.Create(
                    additionalText.Path,
                    new TextSpan(0, 0),
                    new LinePositionSpan(LinePosition.Zero, LinePosition.Zero)),
                $"the required schema header '{SchemaHeader}' is missing"));
        }

        return new LanguageDataParseResult(
            entries.OrderBy(static entry => entry.Identifier, StringComparer.Ordinal).ToImmutableArray(),
            diagnostics.ToImmutable());
    }

    private static LanguageDataParseResult InvalidFile(string path, string reason)
    {
        var diagnostic = CreateInvalidDiagnostic(
            path,
            0,
            Location.Create(
                path,
                new TextSpan(0, 0),
                new LinePositionSpan(LinePosition.Zero, LinePosition.Zero)),
            reason);
        return new LanguageDataParseResult([], [diagnostic]);
    }

    private static Diagnostic CreateInvalidDiagnostic(
        string path,
        int zeroBasedLine,
        Location location,
        string reason)
    {
        return Diagnostic.Create(
            AtomUIDiagnosticDescriptors.LocalizationInvalidLanguageData,
            location,
            path,
            zeroBasedLine + 1,
            reason);
    }

    private static Diagnostic CreateDuplicateDiagnostic(
        string path,
        int zeroBasedLine,
        Location location,
        string duplicate)
    {
        return Diagnostic.Create(
            AtomUIDiagnosticDescriptors.LocalizationDuplicateLanguageData,
            location,
            path,
            zeroBasedLine + 1,
            duplicate);
    }

    private static Location CreateLocation(string path, TextLine line)
    {
        var linePosition = new LinePosition(line.LineNumber, 0);
        return Location.Create(
            path,
            line.Span,
            new LinePositionSpan(linePosition, new LinePosition(line.LineNumber, line.Span.Length)));
    }
}

internal static class LanguageDataTagNormalizer
{
    internal static bool TryNormalize(string value, out string canonicalValue)
    {
        canonicalValue = string.Empty;
        if (string.IsNullOrEmpty(value) || value[0] == '-' || value[value.Length - 1] == '-')
        {
            return false;
        }

        var subtags = value.Split('-');
        if (subtags.Any(static subtag => subtag.Length == 0 || !subtag.All(IsAsciiAlphaNumeric)))
        {
            return false;
        }

        if (subtags[0].Length is < 2 or > 8 || !subtags[0].All(IsAsciiAlpha))
        {
            return false;
        }

        var canonical = new List<string>(subtags.Length)
        {
            subtags[0].ToLowerInvariant()
        };
        var index = 1;

        if (index < subtags.Length && subtags[index].Length == 4 && subtags[index].All(IsAsciiAlpha))
        {
            var script = subtags[index].ToLowerInvariant();
            canonical.Add(char.ToUpperInvariant(script[0]) + script.Substring(1));
            index++;
        }

        if (index < subtags.Length && IsRegion(subtags[index]))
        {
            canonical.Add(subtags[index].ToUpperInvariant());
            index++;
        }

        if (index != subtags.Length)
        {
            return false;
        }

        canonicalValue = string.Join("-", canonical);
        return true;
    }

    private static bool IsRegion(string value)
    {
        return value.Length == 2 && value.All(IsAsciiAlpha) ||
               value.Length == 3 && value.All(IsAsciiDigit);
    }

    private static bool IsAsciiAlpha(char value)
    {
        return value is >= 'A' and <= 'Z' or >= 'a' and <= 'z';
    }

    private static bool IsAsciiDigit(char value)
    {
        return value is >= '0' and <= '9';
    }

    private static bool IsAsciiAlphaNumeric(char value)
    {
        return IsAsciiAlpha(value) || IsAsciiDigit(value);
    }
}
