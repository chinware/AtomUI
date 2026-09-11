using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Generator.Localization.Xliff;

internal static class AdditionalLanguageFileParser
{
    internal static AdditionalLanguageFileParseResult Parse(
        AdditionalText additionalText,
        CancellationToken cancellationToken)
    {
        var text = additionalText.GetText(cancellationToken);
        if (text is null)
        {
            var emptyText = SourceText.From(string.Empty);
            return new AdditionalLanguageFileParseResult(
                additionalText.Path,
                emptyText,
                null,
                [new AtomUI.Build.Tasks.LocalizationBuild.XliffParseError("the file cannot be read", 1, 1)]);
        }

        var parseResult = AtomUI.Build.Tasks.LocalizationBuild.Xliff21Parser.Parse(text.ToString());
        if (parseResult.Errors.Count > 0)
        {
            return new AdditionalLanguageFileParseResult(
                additionalText.Path,
                text,
                null,
                parseResult.Errors.ToImmutableArray());
        }

        return new AdditionalLanguageFileParseResult(
            additionalText.Path,
            text,
            new AdditionalLanguageFile(additionalText.Path, text, parseResult.Document!),
            ImmutableArray<AtomUI.Build.Tasks.LocalizationBuild.XliffParseError>.Empty);
    }
}
