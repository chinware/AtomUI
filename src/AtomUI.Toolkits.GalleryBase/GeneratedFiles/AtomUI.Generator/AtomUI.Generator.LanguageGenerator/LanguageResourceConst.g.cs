using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUI.Toolkits.GalleryBase.Localization
{
    public enum GalleryShowCaseHeaderLangResourceKind
    {
        BaseClassLabel,
        NamespaceLabel,
        PackageLabel
    }

    public class GalleryShowCaseHeaderLangResourceExtension : LanguageResourceExtension<GalleryShowCaseHeaderLangResourceKind>
    {
        public GalleryShowCaseHeaderLangResourceExtension()
        {
        }

        public GalleryShowCaseHeaderLangResourceExtension(GalleryShowCaseHeaderLangResourceKind kind) : base(kind)
        {
        }
    }
}