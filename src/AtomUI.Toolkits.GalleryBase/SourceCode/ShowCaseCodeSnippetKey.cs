namespace AtomUI.Toolkits.GalleryBase.SourceCode;

public readonly record struct ShowCaseCodeSnippetKey(
    string ViewTypeName,
    string PanelKey,
    int ItemIndex,
    string? SourceKey = null);
