using AtomUI.Generator.Diagnostics;
using AtomUI.Generator.Localization.Xliff;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Generator.Localization;

internal static class LocalizationDiagnosticFactory
{
    internal static Diagnostic InvalidXliff(
        string path,
        SourceText text,
        AtomUI.Build.Tasks.LocalizationBuild.XliffParseError error)
    {
        return Diagnostic.Create(
            AtomUIDiagnosticDescriptors.LocalizationInvalidXliff,
            CreateLocation(path, text, error.Line, error.Column),
            path,
            error.Message);
    }

    internal static Diagnostic FromFileValidation(
        LanguageFileInput input,
        AtomUI.Build.Tasks.LocalizationBuild.LanguageFileValidationDiagnostic diagnostic)
    {
        var location = CreateLocation(input.Path, input.Text, diagnostic.Line, diagnostic.Column);
        if (diagnostic.Kind == AtomUI.Build.Tasks.LocalizationBuild.LanguageFileValidationDiagnosticKind.InvalidTranslation)
        {
            return Diagnostic.Create(
                AtomUIDiagnosticDescriptors.LocalizationInvalidTranslation,
                location,
                diagnostic.UnitKey ?? input.Document.File.Id,
                input.Document.TargetLanguage ?? input.Document.SourceLanguage,
                diagnostic.Message);
        }

        return Diagnostic.Create(
            AtomUIDiagnosticDescriptors.LocalizationCatalogXliffMismatch,
            location,
            input.Path,
            $"{input.ModuleId}:{input.Document.File.Id}",
            diagnostic.Message);
    }

    internal static Location CreateLocation(
        string path,
        SourceText text,
        int oneBasedLine,
        int oneBasedColumn)
    {
        if (text.Lines.Count == 0)
        {
            return Location.Create(
                path,
                new TextSpan(0, 0),
                new LinePositionSpan(new LinePosition(0, 0), new LinePosition(0, 0)));
        }

        var lineIndex = Math.Max(0, Math.Min(oneBasedLine - 1, text.Lines.Count - 1));
        var line = text.Lines[lineIndex];
        var column = Math.Max(0, Math.Min(oneBasedColumn - 1, line.Span.Length));
        var position = line.Start + column;
        var linePosition = new LinePosition(lineIndex, column);
        return Location.Create(
            path,
            new TextSpan(position, 0),
            new LinePositionSpan(linePosition, linePosition));
    }
}
