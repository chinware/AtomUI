namespace AtomUI.Docs.LLMsGenerator.Reader;

public sealed class ControlDocModel
{
    public string ControlName { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public string SourceOverviewPath { get; init; } = string.Empty;

    public string SourceImplementationPath { get; init; } = string.Empty;

    public string? SourceTokenPath { get; init; }

    public string SourceChangelogPath { get; init; } = string.Empty;

    public string OutputIndexPath { get; init; } = string.Empty;

    public string OutputSemanticPath { get; init; } = string.Empty;

    public string PackageName { get; init; } = string.Empty;

    public string DotNetNamespace { get; init; } = string.Empty;

    public string AxamlNamespace { get; init; } = string.Empty;

    public string GalleryPath { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public IReadOnlyList<MarkdownTableRow> SemanticParts { get; init; } = [];

    public string SemanticPartsMarkdown { get; init; } = string.Empty;

    public string TemplatePartsMarkdown { get; init; } = string.Empty;

    public string AbstractAxamlStructureMarkdown { get; init; } = string.Empty;

    public string CompositionModelMarkdown { get; init; } = string.Empty;

    public string PseudoClassesMarkdown { get; init; } = string.Empty;

    public string SourceIndex { get; init; } = string.Empty;

    public bool HasTokenDoc { get; init; }

    public string TokenSourceDescription { get; init; } = string.Empty;

    public string OverviewSection { get; init; } = string.Empty;

    public string DesignLanguageSection { get; init; } = string.Empty;

    public string ApiSection { get; init; } = string.Empty;

    public string StateSection { get; init; } = string.Empty;

    public string ThemeSection { get; init; } = string.Empty;

    public string CompatibilitySection { get; init; } = string.Empty;

    public string VerificationSection { get; init; } = string.Empty;

    public string ImplementationLifecycleSection { get; init; } = string.Empty;

    public string ImplementationAotSection { get; init; } = string.Empty;

    public string ImplementationInvariantsSection { get; init; } = string.Empty;

    public string ImplementationTestsSection { get; init; } = string.Empty;

    public string GalleryExamplesMarkdown { get; init; } = string.Empty;

    public string SourceOverviewRelativePath { get; init; } = string.Empty;

    public string SourceImplementationRelativePath { get; init; } = string.Empty;

    public string? SourceTokenRelativePath { get; init; }

    public string SourceChangelogRelativePath { get; init; } = string.Empty;

    public bool HasExplicitMetadata { get; init; }

    public bool HasExplicitSemanticParts { get; init; }

    public bool HasExplicitExportSourceTable { get; init; }

    public bool HasExplicitTokenSourceDescription { get; init; }
}

public sealed record MarkdownTableRow(IReadOnlyList<string> Cells);
