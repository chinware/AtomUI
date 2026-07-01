using AtomUI.Theme.Language;
using AtomUI.Toolkits.GalleryBase.Controls;

namespace AtomUI.Toolkits.GalleryBase.Localization;

[LanguageProvider(LanguageCode.zh_CN, GalleryShowCaseHeader.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string NamespaceLabel = "命名空间";
    public const string PackageLabel = "包";
    public const string BaseClassLabel = "基类";

    protected override Type GetResourceKindType() => typeof(GalleryShowCaseHeaderLangResourceKind);
}
