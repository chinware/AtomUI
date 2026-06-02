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

    protected override Type GetResourceKindType() => typeof(CardShowCaseLangResourceKind);
}
