using AtomUI.Theme.Language;
using AtomUI.Toolkits.GalleryBase.Controls;

namespace AtomUI.Toolkits.GalleryBase.Localization;

[LanguageProvider(LanguageCode.zh_TW, GalleryShowCaseHeader.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string NamespaceLabel = "命名空間";
    public const string PackageLabel = "套件";
    public const string BaseClassLabel = "基底類別";

    protected override Type GetResourceKindType() => typeof(GalleryShowCaseHeaderLangResourceKind);
}
