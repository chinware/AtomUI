using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TabControl;

[LanguageProvider(LanguageCode.en_US, TabControlShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string TabControlBasicTitle = "Basic";
    public const string TabControlBasicDescription = "Default activate first tab.";
    public const string TabControlItemsSourceTitle = "Items from ItemSource datasource";
    public const string TabControlItemsSourceDescription = "Add TabItem based on data source and item template.";
    public const string TabControlDisabledTitle = "Disabled";
    public const string TabControlDisabledDescription = "Disabled a tab.";
    public const string TabControlCenteredTitle = "Centered";
    public const string TabControlCenteredDescription = "Centered tabs.";
    public const string TabControlIconTitle = "Icon";
    public const string TabControlIconDescription = "The Tab with Icon.";
    public const string TabControlSlideTitle = "Slide";
    public const string TabControlSlideDescription = "In order to fit in more tabs, they can slide left and right (or up and down).";
    public const string TabControlCardTypeTitle = "Card type tab";
    public const string TabControlCardTypeDescription = "Another type of Tabs, which doesn't support vertical mode.";
    public const string TabControlClosableTitle = "Closable Tab";
    public const string TabControlClosableDescription = "We support closable tab settings.";
    public const string TabControlPositionTitle = "Position";
    public const string TabControlPositionDescription = "Tab's position: left, right, top or bottom. Will auto switch to top in mobile.";
    public const string TabControlCardShapePositionTitle = "Card Shape Position";
    public const string TabControlCardShapePositionDescription = "Tab's position: left, right, top or bottom. Will auto switch to top in mobile.";
    public const string TabControlSizeTitle = "Size";
    public const string TabControlSizeDescription = "Large size tabs are usually used in page header, and small size could be used in Modal.";
    public const string TabControlAddCloseTitle = "Add and close tab";
    public const string TabControlAddCloseDescription = "Hide default plus icon, and bind event for customized trigger.";
    public const string TabControlExtraContentTitle = "Extra content";
    public const string TabControlExtraContentDescription = "You can add extra actions to the right or left or even both side of Tabs.";
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

    protected override Type GetResourceKindType() => typeof(TabControlShowCaseLangResourceKind);
}
