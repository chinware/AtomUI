using AtomUI.Toolkits.GalleryBase.SourceCode;
using AtomUIGallery.Generated;

namespace AtomUIGallery.SourceCode;

internal sealed class AtomUIGalleryShowCaseCodeSnippetProvider : IShowCaseCodeSnippetProvider
{
    public bool TryGetSnippetGroup(ShowCaseCodeSnippetKey key, out ShowCaseCodeSnippetGroup group)
    {
        return ShowCaseCodeSnippetCatalog.TryGetSnippetGroup(
            key.ViewTypeName,
            key.PanelKey,
            key.ItemIndex,
            key.SourceKey,
            out group);
    }
}
