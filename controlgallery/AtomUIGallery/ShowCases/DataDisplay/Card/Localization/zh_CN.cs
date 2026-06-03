using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Card;

[LanguageProvider(LanguageCode.zh_CN, CardShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础卡片";
    public const string BasicDescription = "包含标题、内容和右上角额外内容的基础卡片。支持默认和小号两种尺寸。";
    public const string NoBorderTitle = "无边框";
    public const string NoBorderDescription = "灰色背景上的无边框卡片。";
    public const string SimpleCardTitle = "简单卡片";
    public const string SimpleCardDescription = "只包含内容区域的简单卡片。";
    public const string CustomizedContentTitle = "自定义内容";
    public const string CustomizedContentDescription = "可以使用 Card.Meta 支持更灵活的内容。";
    public const string CardInColumnTitle = "栅格中的卡片";
    public const string CardInColumnDescription = "卡片通常在概览页面中配合栅格列布局使用。";
    public const string LoadingCardTitle = "加载中卡片";
    public const string LoadingCardDescription = "卡片内容获取过程中显示加载指示。";
    public const string GridCardTitle = "栅格卡片";
    public const string GridCardDescription = "栅格样式的卡片内容。";
    public const string InnerCardTitle = "内部卡片";
    public const string InnerCardDescription = "可放置在普通卡片内部，用于展示多级结构信息。";
    public const string WithTabsTitle = "带标签页";
    public const string WithTabsDescription = "可以承载更多内容。";
    public const string MoreContentConfigurationTitle = "支持更多内容配置";
    public const string MoreContentConfigurationDescription = "支持封面、头像、标题和描述的卡片。";
    public const string P2HeaderLargeSizeCard = "大尺寸卡片";
    public const string P2HeaderDefaultSizeCard = "默认尺寸卡片";
    public const string P2HeaderSmallSizeCard = "小尺寸卡片";
    public const string P2HeaderCardTitle = "卡片标题";
    public const string P2HeaderEuropeStreetBeat = "欧洲街拍";
    public const string P2HeaderCardTitle2 = "卡片标题";
    public const string P2HeaderTab1 = "标签页 1";
    public const string P2HeaderTab2 = "标签页 2";
    public const string P2HeaderArticle = "文章";
    public const string P2HeaderApp = "应用";
    public const string P2HeaderProject = "项目";
    public const string P2ContentMore = "更多";
    public const string P2TextCardContent = "卡片内容";
    public const string P2TextThisIsTheDescription = "这是描述内容";
    public const string P2ContentContent = "内容";
    public const string P2ContentContent1 = "内容 1";
    public const string P2ContentContent2 = "内容 2";
    public const string P2ContentArticleContent = "文章内容";
    public const string P2ContentAppContent = "应用内容";
    public const string P2ContentProjectContent = "项目内容";

    public const string P2ContentThisIsTheDescription = "这是描述内容";

    protected override Type GetResourceKindType() => typeof(CardShowCaseLangResourceKind);
}
