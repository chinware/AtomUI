using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Card;

[LanguageProvider(LanguageCode.zh_TW, CardShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎卡片";
    public const string BasicDescription = "包含標題、內容和右上角額外內容的基礎卡片。支持默認和小號兩種尺寸。";
    public const string ScenarioBasic = "基礎";
    public const string ScenarioLayout = "佈局";
    public const string ScenarioAdvanced = "高級";
    public const string NoBorderTitle = "無邊框";
    public const string NoBorderDescription = "灰色背景上的無邊框卡片。";
    public const string SimpleCardTitle = "簡單卡片";
    public const string SimpleCardDescription = "只包含內容區域的簡單卡片。";
    public const string CustomizedContentTitle = "自定義內容";
    public const string CustomizedContentDescription = "可以使用 Card.Meta 支持更靈活的內容。";
    public const string CardInColumnTitle = "柵格中的卡片";
    public const string CardInColumnDescription = "卡片通常在概覽頁面中配合柵格列佈局使用。";
    public const string LoadingCardTitle = "加載中卡片";
    public const string LoadingCardDescription = "卡片內容獲取過程中顯示加載指示。";
    public const string GridCardTitle = "柵格卡片";
    public const string GridCardDescription = "柵格樣式的卡片內容。";
    public const string InnerCardTitle = "內部卡片";
    public const string InnerCardDescription = "可放置在普通卡片內部，用於展示多級結構信息。";
    public const string WithTabsTitle = "帶標籤頁";
    public const string WithTabsDescription = "可以承載更多內容。";
    public const string MoreContentConfigurationTitle = "支持更多內容配置";
    public const string MoreContentConfigurationDescription = "支持封面、頭像、標題和描述的卡片。";
    public const string P2HeaderLargeSizeCard = "大尺寸卡片";
    public const string P2HeaderDefaultSizeCard = "默認尺寸卡片";
    public const string P2HeaderSmallSizeCard = "小尺寸卡片";
    public const string P2HeaderCardTitle = "卡片標題";
    public const string P2HeaderEuropeStreetBeat = "歐洲街拍";
    public const string P2HeaderCardTitle2 = "卡片標題";
    public const string P2HeaderTab1 = "標籤頁 1";
    public const string P2HeaderTab2 = "標籤頁 2";
    public const string P2HeaderArticle = "文章";
    public const string P2HeaderApp = "應用";
    public const string P2HeaderProject = "項目";
    public const string P2ContentMore = "更多";
    public const string P2TextCardContent = "卡片內容";
    public const string P2TextThisIsTheDescription = "這是描述內容";
    public const string P2ContentContent = "內容";
    public const string P2ContentContent1 = "內容 1";
    public const string P2ContentContent2 = "內容 2";
    public const string P2ContentArticleContent = "文章內容";
    public const string P2ContentAppContent = "應用內容";
    public const string P2ContentProjectContent = "項目內容";

    public const string P2ContentThisIsTheDescription = "這是描述內容";

    protected override Type GetResourceKindType() => typeof(CardShowCaseLangResourceKind);
}

