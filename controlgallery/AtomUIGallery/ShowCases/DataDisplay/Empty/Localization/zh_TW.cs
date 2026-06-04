using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Empty;

[LanguageProvider(LanguageCode.zh_TW, EmptyShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最簡單的用法。";
    public const string SizeTitle = "尺寸";
    public const string SizeDescription = "AtomUI 支持小號、默認和大號三種尺寸。";
    public const string CustomizeTitle = "自定義";
    public const string CustomizeDescription = "自定義圖片來源、圖片尺寸、描述和額外內容。";
    public const string NoDescriptionTitle = "無描述";
    public const string NoDescriptionDescription = "不帶描述的最簡單用法。";
    public const string P2DescriptionCustomizeDescription = "Customize Description";
    public const string P2ContentCreateNow = "Create Now";

    protected override Type GetResourceKindType() => typeof(EmptyShowCaseLangResourceKind);
}

