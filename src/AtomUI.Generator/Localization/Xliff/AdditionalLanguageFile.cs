using System.Collections.Immutable;
using AtomUI.Generator.Localization.Catalog;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Generator.Localization.Xliff;

internal enum LanguageFileSourceKind
{
    ModuleBuiltIn,
    StaticLanguagePack,
    ApplicationOverride
}

internal enum LanguageFileContractValidation
{
    Verified,
    Deferred
}

internal enum LanguageInputActivationState
{
    Active,
    Dormant
}

internal sealed class AdditionalLanguageFile
{
    internal AdditionalLanguageFile(
        string path,
        SourceText text,
        AtomUI.Build.Tasks.LocalizationBuild.XliffDocumentModel document)
    {
        Path = path;
        Text = text;
        Document = document;
    }

    internal string Path { get; }

    internal SourceText Text { get; }

    internal AtomUI.Build.Tasks.LocalizationBuild.XliffDocumentModel Document { get; }
}

internal sealed class AdditionalLanguageFileParseResult
{
    internal AdditionalLanguageFileParseResult(
        string path,
        SourceText text,
        AdditionalLanguageFile? file,
        ImmutableArray<AtomUI.Build.Tasks.LocalizationBuild.XliffParseError> errors)
    {
        Path = path;
        Text = text;
        File = file;
        Errors = errors;
    }

    internal string Path { get; }

    internal SourceText Text { get; }

    internal AdditionalLanguageFile? File { get; }

    internal ImmutableArray<AtomUI.Build.Tasks.LocalizationBuild.XliffParseError> Errors { get; }
}

internal sealed class LanguageFileInput
{
    internal LanguageFileInput(
        AdditionalLanguageFile file,
        string moduleId,
        LanguageFileSourceKind sourceKind,
        string sourceIdentity,
        LanguageFileContractValidation contractValidation,
        string? sourceFingerprint)
    {
        File = file;
        ModuleId = moduleId;
        SourceKind = sourceKind;
        SourceIdentity = sourceIdentity;
        ContractValidation = contractValidation;
        SourceFingerprint = sourceFingerprint;
    }

    internal AdditionalLanguageFile File { get; }

    internal string Path => File.Path;

    internal SourceText Text => File.Text;

    internal AtomUI.Build.Tasks.LocalizationBuild.XliffDocumentModel Document => File.Document;

    internal string ModuleId { get; }

    internal LanguageFileSourceKind SourceKind { get; }

    internal string SourceIdentity { get; }

    internal LanguageFileContractValidation ContractValidation { get; }

    internal string? SourceFingerprint { get; }
}

internal sealed class LanguageFileInputResult
{
    internal LanguageFileInputResult(
        LanguageFileInput? input,
        ImmutableArray<Diagnostic> diagnostics)
    {
        Input = input;
        Diagnostics = diagnostics;
    }

    internal LanguageFileInput? Input { get; }

    internal ImmutableArray<Diagnostic> Diagnostics { get; }
}

internal sealed class LanguageInputResolution
{
    internal LanguageInputResolution(
        LanguageFileInput input,
        LanguageInputActivationState activationState,
        LanguageCatalogInfo? catalog)
    {
        Input = input;
        ActivationState = activationState;
        Catalog = catalog;
    }

    internal LanguageFileInput Input { get; }

    internal LanguageInputActivationState ActivationState { get; }

    internal LanguageCatalogInfo? Catalog { get; }
}
