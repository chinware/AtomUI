using System.Collections.Immutable;
using System.Globalization;
using AtomUI.Generator.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Generator.Localization.Xliff;

internal enum LanguageFileSourceKind
{
    ModuleBuiltIn,
    StaticLanguagePack,
    ApplicationOverride
}

internal sealed class AdditionalLanguageFile
{
    internal AdditionalLanguageFile(
        string path,
        string moduleId,
        LanguageFileSourceKind sourceKind,
        string sourceIdentity,
        int? contractVersion,
        string? sourceFingerprint,
        SourceText text,
        AtomUI.Localization.Build.XliffDocumentModel document)
    {
        Path = path;
        ModuleId = moduleId;
        SourceKind = sourceKind;
        SourceIdentity = sourceIdentity;
        ContractVersion = contractVersion;
        SourceFingerprint = sourceFingerprint;
        Text = text;
        Document = document;
    }

    internal string Path { get; }

    internal string ModuleId { get; }

    internal LanguageFileSourceKind SourceKind { get; }

    internal string SourceIdentity { get; }

    internal int? ContractVersion { get; }

    internal string? SourceFingerprint { get; }

    internal SourceText Text { get; }

    internal AtomUI.Localization.Build.XliffDocumentModel Document { get; }
}

internal sealed class AdditionalLanguageFileParseResult
{
    internal AdditionalLanguageFileParseResult(
        AdditionalLanguageFile? file,
        ImmutableArray<Diagnostic> diagnostics)
    {
        File = file;
        Diagnostics = diagnostics;
    }

    internal AdditionalLanguageFile? File { get; }

    internal ImmutableArray<Diagnostic> Diagnostics { get; }
}

internal static class AdditionalLanguageFileParser
{
    internal static AdditionalLanguageFileParseResult Parse(
        AdditionalText additionalText,
        AnalyzerConfigOptionsProvider optionsProvider,
        CancellationToken cancellationToken)
    {
        var text = additionalText.GetText(cancellationToken);
        if (text is null)
        {
            return Invalid(
                additionalText.Path,
                null,
                new AtomUI.Localization.Build.XliffParseError(
                    "the file cannot be read",
                    1,
                    1));
        }

        var parseResult = AtomUI.Localization.Build.Xliff21Parser.Parse(text.ToString());
        if (parseResult.Errors.Count > 0)
        {
            var diagnostics = parseResult.Errors
                                         .Select(error => CreateDiagnostic(additionalText.Path, text, error))
                                         .ToImmutableArray();
            return new AdditionalLanguageFileParseResult(null, diagnostics);
        }

        var fileOptions = optionsProvider.GetOptions(additionalText);
        var moduleId = LanguageGeneratorOptions.GetFileValue(
            fileOptions,
            LanguageGeneratorOptions.ModuleIdMetadata,
            LanguageGeneratorOptions.GetModuleId(optionsProvider, "Application"));
        var sourceIdentity = LanguageGeneratorOptions.GetFileValue(
            fileOptions,
            LanguageGeneratorOptions.SourceIdentityMetadata,
            moduleId);
        var sourceKindText = LanguageGeneratorOptions.GetFileValue(
            fileOptions,
            LanguageGeneratorOptions.SourceKindMetadata,
            nameof(LanguageFileSourceKind.ModuleBuiltIn));
        if (!Enum.TryParse(sourceKindText, ignoreCase: false, out LanguageFileSourceKind sourceKind))
        {
            return Invalid(
                additionalText.Path,
                text,
                new AtomUI.Localization.Build.XliffParseError(
                    $"AtomUILanguageSourceKind '{sourceKindText}' is not supported",
                    1,
                    1));
        }

        int? contractVersion = null;
        var contractVersionText = LanguageGeneratorOptions.GetFileValue(
            fileOptions,
            LanguageGeneratorOptions.ContractVersionMetadata,
            string.Empty);
        var contractVersionRequired = sourceKind is LanguageFileSourceKind.StaticLanguagePack or
                                      LanguageFileSourceKind.ApplicationOverride;
        if (contractVersionRequired || contractVersionText.Length > 0)
        {
            if (!int.TryParse(
                    contractVersionText,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var parsedContractVersion) ||
                parsedContractVersion <= 0)
            {
                return Invalid(
                    additionalText.Path,
                    text,
                    new AtomUI.Localization.Build.XliffParseError(
                        "AtomUILanguageContractVersion must be a positive integer when specified and is " +
                        "required for external language inputs",
                        1,
                        1));
            }
            contractVersion = parsedContractVersion;
        }

        var sourceFingerprint = LanguageGeneratorOptions.GetFileValue(
            fileOptions,
            LanguageGeneratorOptions.SourceFingerprintMetadata,
            string.Empty);
        if (sourceKind == LanguageFileSourceKind.StaticLanguagePack && sourceFingerprint.Length == 0)
        {
            return Invalid(
                additionalText.Path,
                text,
                new AtomUI.Localization.Build.XliffParseError(
                    "AtomUILanguageSourceFingerprint is required for static language packages",
                    1,
                    1));
        }

        if (sourceFingerprint.Length > 0)
        {
            if (!IsLowercaseSha256(sourceFingerprint))
            {
                return Invalid(
                    additionalText.Path,
                    text,
                    new AtomUI.Localization.Build.XliffParseError(
                        "AtomUILanguageSourceFingerprint must contain exactly 64 lowercase hexadecimal characters",
                        1,
                        1));
            }

            var actualFingerprint = AtomUI.Localization.Build.LanguageSourceFingerprint.Compute(
                parseResult.Document!);
            if (!string.Equals(sourceFingerprint, actualFingerprint, StringComparison.Ordinal))
            {
                return Invalid(
                    additionalText.Path,
                    text,
                    new AtomUI.Localization.Build.XliffParseError(
                        $"the declared source fingerprint '{sourceFingerprint}' does not match " +
                        $"the XLIFF source contract fingerprint '{actualFingerprint}'",
                        1,
                        1));
            }
        }

        return new AdditionalLanguageFileParseResult(
            new AdditionalLanguageFile(
                additionalText.Path,
                moduleId,
                sourceKind,
                sourceIdentity,
                contractVersion,
                sourceFingerprint.Length == 0 ? null : sourceFingerprint,
                text,
                parseResult.Document!),
            ImmutableArray<Diagnostic>.Empty);
    }

    private static bool IsLowercaseSha256(string value)
    {
        if (value.Length != 64)
        {
            return false;
        }

        foreach (var character in value)
        {
            if (character is not (>= '0' and <= '9') and not (>= 'a' and <= 'f'))
            {
                return false;
            }
        }
        return true;
    }

    private static AdditionalLanguageFileParseResult Invalid(
        string path,
        SourceText? text,
        AtomUI.Localization.Build.XliffParseError error)
    {
        var sourceText = text ?? SourceText.From(string.Empty);
        return new AdditionalLanguageFileParseResult(
            null,
            [CreateDiagnostic(path, sourceText, error)]);
    }

    private static Diagnostic CreateDiagnostic(
        string path,
        SourceText text,
        AtomUI.Localization.Build.XliffParseError error)
    {
        var location = CreateLocation(path, text, error.Line, error.Column);
        return Diagnostic.Create(
            AtomUIDiagnosticDescriptors.LocalizationInvalidXliff,
            location,
            path,
            error.Message);
    }

    private static Location CreateLocation(
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
