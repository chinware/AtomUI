namespace AtomUI.Toolkits.GalleryBase.SourceCode;

public sealed record ShowCaseCodeSnippet(
    string TabTitle,
    string Language,
    string Text,
    string? SourceFilePath,
    int StartLine,
    int EndLine);
