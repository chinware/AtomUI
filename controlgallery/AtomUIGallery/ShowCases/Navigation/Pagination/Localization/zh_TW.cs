using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Pagination;

[LanguageProvider(LanguageCode.zh_TW, PaginationShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎分頁";
    public const string BasicDescription = "基礎分頁。";
    public const string AlignTitle = "對齊方式";
    public const string AlignDescription = "支持左對齊、居中對齊和右對齊三種對齊方式。";
    public const string MoreTitle = "更多頁碼";
    public const string MoreDescription = "更多頁碼。";
    public const string MiniSizeTitle = "迷你尺寸";
    public const string MiniSizeDescription = "迷你尺寸分頁。";
    public const string TotalNumberTitle = "總數";
    public const string TotalNumberDescription = "可以通過設置 showTotal 展示數據總量。";
    public const string SimpleModeTitle = "簡潔模式";
    public const string SimpleModeDescription = "簡潔模式。";

    protected override Type GetResourceKindType() => typeof(PaginationShowCaseLangResourceKind);
}

