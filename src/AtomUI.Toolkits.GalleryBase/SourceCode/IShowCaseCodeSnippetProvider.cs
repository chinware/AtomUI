namespace AtomUI.Toolkits.GalleryBase.SourceCode;

public interface IShowCaseCodeSnippetProvider
{
    bool TryGetSnippetGroup(ShowCaseCodeSnippetKey key, out ShowCaseCodeSnippetGroup group);
}
