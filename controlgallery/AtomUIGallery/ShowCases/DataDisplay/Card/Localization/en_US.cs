using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Card;

[LanguageProvider(LanguageCode.en_US, CardShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "A basic card containing a title, content and an extra corner content. Supports two sizes: default and small.";
    public const string NoBorderTitle = "No border";
    public const string NoBorderDescription = "A borderless card on a gray background.";
    public const string SimpleCardTitle = "Simple card";
    public const string SimpleCardDescription = "A simple card only containing a content area.";
    public const string CustomizedContentTitle = "Customized content";
    public const string CustomizedContentDescription = "You can use Card.Meta to support more flexible content.";
    public const string CardInColumnTitle = "Card in column";
    public const string CardInColumnDescription = "Cards usually cooperate with grid column layout in overview page.";
    public const string LoadingCardTitle = "Loading card";
    public const string LoadingCardDescription = "Shows a loading indicator while the contents of the card is being fetched.";
    public const string GridCardTitle = "Grid card";
    public const string GridCardDescription = "Grid style card content..";
    public const string InnerCardTitle = "Inner card";
    public const string InnerCardDescription = "It can be placed inside the ordinary card to display the information of the multilevel structure.";
    public const string WithTabsTitle = "With tabs";
    public const string WithTabsDescription = "More content can be hosted..";
    public const string MoreContentConfigurationTitle = "Support more content configuration";
    public const string MoreContentConfigurationDescription = "A Card that supports cover, avatar, title and description.";
    public const string P2HeaderLargeSizeCard = "Large size card";
    public const string P2HeaderDefaultSizeCard = "Default size card";
    public const string P2HeaderSmallSizeCard = "Small size card";
    public const string P2HeaderCardTitle = "Card title";
    public const string P2HeaderEuropeStreetBeat = "Europe Street beat";
    public const string P2HeaderCardTitle2 = "Card Title";
    public const string P2HeaderTab1 = "Tab1";
    public const string P2HeaderTab2 = "Tab2";
    public const string P2HeaderArticle = "article";
    public const string P2HeaderApp = "app";
    public const string P2HeaderProject = "project";
    public const string P2ContentMore = "More";
    public const string P2TextCardContent = "Card content";
    public const string P2TextThisIsTheDescription = "This is the description";
    public const string P2ContentContent = "Content";
    public const string P2ContentContent1 = "content1";
    public const string P2ContentContent2 = "content2";
    public const string P2ContentArticleContent = "article content";
    public const string P2ContentAppContent = "app content";
    public const string P2ContentProjectContent = "project content";

    public const string P2ContentThisIsTheDescription = "This is the description";

    protected override Type GetResourceKindType() => typeof(CardShowCaseLangResourceKind);
}
