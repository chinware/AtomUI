using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Pagination;

[LanguageProvider(LanguageCode.zh_TW, PaginationShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string ScenarioExamples = "示例";
    public const string ComponentCategory = "導航";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "用頁碼、頁大小、總數和快速跳轉瀏覽長列表。";
    public const string PageDescription = "Pagination 將大型數據集拆分為可預測的頁面，支持對齊方式、頁大小選擇、快速跳轉、總數信息、迷你尺寸以及簡潔只讀或可編輯模式。";
    public const string BasicTitle = "基礎分頁";
    public const string BasicDescription = "基礎分頁。";
    public const string BindingTitle = "受控繫結";
    public const string BindingDescription = "CurrentPage 和 PageSize 不需要顯式設置 Mode=TwoWay；點擊頁碼和修改頁大小都會寫回 ViewModel。";
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
    public const string P2TextCurrentPage = "CurrentPage：";
    public const string P2TextPageSize = "PageSize：";
    public const string P2ContentSetPage = "設置第 5 頁 / 20";
    public const string P2ContentReset = "重置";

}
