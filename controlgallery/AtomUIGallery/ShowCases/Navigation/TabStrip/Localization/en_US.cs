using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TabStrip;

[LanguageProvider(LanguageCode.en_US, TabStripShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string TabStripBasicTitle = "Basic";
    public const string TabStripBasicDescription = "Default activate first tab.";
    public const string TabStripItemsSourceTitle = "Items from ItemSource datasource";
    public const string TabStripItemsSourceDescription = "Add TabStripItem based on data source and item template.";
    public const string TabStripDisabledTitle = "Disabled";
    public const string TabStripDisabledDescription = "Disabled a tab.";
    public const string TabStripCenteredTitle = "Centered";
    public const string TabStripCenteredDescription = "Centered tabs.";
    public const string TabStripIconTitle = "Icon";
    public const string TabStripIconDescription = "The Tab with Icon.";
    public const string TabStripSlideTitle = "Slide";
    public const string TabStripSlideDescription = "In order to fit in more tabs, they can slide left and right (or up and down).";
    public const string TabStripCardTypeTitle = "Card type tab";
    public const string TabStripCardTypeDescription = "Another type of Tabs, which doesn't support vertical mode.";
    public const string TabStripClosableTitle = "Closable Tab";
    public const string TabStripClosableDescription = "We support closable tab settings.";
    public const string TabStripPositionTitle = "Position";
    public const string TabStripPositionDescription = "Tab's position: left, right, top or bottom. Will auto switch to top in mobile.";
    public const string TabStripCardShapePositionTitle = "Card Shape Position";
    public const string TabStripCardShapePositionDescription = "Tab's position: left, right, top or bottom. Will auto switch to top in mobile.";
    public const string TabStripSizeTitle = "Size";
    public const string TabStripSizeDescription = "Large size tabs are usually used in page header, and small size could be used in Modal.";
    public const string TabStripAddCloseTitle = "Add and close tab";
    public const string TabStripAddCloseDescription = "Hide default plus icon, and bind event for customized trigger.";
    public const string P2TextTabPosition = "Tab position:";
    public const string P2ContentTop = "Top";
    public const string P2ContentBottom = "Bottom";
    public const string P2ContentLeft = "Left";
    public const string P2ContentRight = "Right";
    public const string P2ContentSmall = "Small";
    public const string P2ContentMiddle = "Middle";
    public const string P2ContentLarge = "Large";
    public const string P2ContentTabN1 = "Tab 1";
    public const string P2ContentTabN2 = "Tab 2";
    public const string P2ContentTabN3 = "Tab 3";
    public const string P2ContentTabN4 = "Tab 4";
    public const string P2ContentTabN5 = "Tab 5";
    public const string P2ContentTabN6 = "Tab 6";
    public const string P2ContentTabN7 = "Tab 7";
    public const string P2ContentTabN8 = "Tab 8";
    public const string P2ContentTabN9 = "Tab 9";
    public const string P2ContentTabN10 = "Tab 10";
    public const string P2ContentTabN11 = "Tab 11";
    public const string P2ContentTabN12 = "Tab 12";
    public const string P2ContentTabN13 = "Tab 13";
    public const string P2ContentTabN14 = "Tab 14";
    public const string P2ContentTabN15 = "Tab 15";
    public const string P2ContentTabN16 = "Tab 16";
    public const string P2ContentTabN17 = "Tab 17";
    public const string P2ContentTabN18 = "Tab 18";
    public const string P2ContentTabN19 = "Tab 19";
    public const string P2ContentTabN20 = "Tab 20";
    public const string P2ContentTabN21 = "Tab 21";
    public const string P2ContentTabN22 = "Tab 22";
    public const string P2ContentTabN23 = "Tab 23";
    public const string P2ContentNewTabFormat = "new tab {0}";
    public const string P2TextTabContent = "Tab Content";

    protected override Type GetResourceKindType() => typeof(TabStripShowCaseLangResourceKind);
}
