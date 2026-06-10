using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Collapse;

[LanguageProvider(LanguageCode.en_US, CollapseShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string ScenarioBasic = "Basic";
    public const string ScenarioAppearance = "Appearance";
    public const string ScenarioBehavior = "Behavior";
    public const string CollapseTitle = "Collapse";
    public const string CollapseDescription = "By default, any number of panels can be expanded at a time. The first panel is expanded in this example.";
    public const string SizeTitle = "Size";
    public const string SizeDescription = "Ant Design supports a default collapse size as well as a large and small size. If a large or small collapse is desired, set the size property to either large or small respectively. Omit the size property for a collapse with the default size.";
    public const string AccordionTitle = "Accordion";
    public const string AccordionDescription = "In accordion mode, only one panel can be expanded at a time.";
    public const string NestedPanelTitle = "Nested panel";
    public const string NestedPanelDescription = "Collapse is nested inside the Collapse.";
    public const string BorderlessTitle = "Borderless";
    public const string BorderlessDescription = "A borderless style of Collapse.";
    public const string NoArrowTitle = "No Arrow";
    public const string NoArrowDescription = "You can hide the arrow icon by passing IsShowExpandIcon={False} to CollapsePanel component.";
    public const string ExpandIconLocationTitle = "Expand icon location";
    public const string ExpandIconLocationDescription = "The expand icon can be placed in front or in the back.";
    public const string GhostCollapseTitle = "Ghost Collapse";
    public const string GhostCollapseDescription = "Making collapse's background to transparent.";
    public const string CollapsibleTitle = "Collapsible";
    public const string CollapsibleDescription = "Specify the trigger area of collapsible by collapsible.";
    public const string CustomPaddingTitle = "Custom Header And Content Padding";
    public const string CustomPaddingDescription = "Please set custom header margins and content spacing.";
    public const string P2HeaderThisIsPanelHeaderN1 = "This is panel header 1";
    public const string P2HeaderThisIsPanelHeaderN2 = "This is panel header 2";
    public const string P2HeaderThisIsPanelHeaderN3 = "This is panel header 3";
    public const string P2TitleDefaultSize = "Default Size";
    public const string P2HeaderThisIsDefaultSizePanelHeader = "This is default size panel header";
    public const string P2TitleSmallSize = "Small Size";
    public const string P2HeaderThisIsSmallSizePanelHeader = "This is small size panel header";
    public const string P2TitleLargeSize = "Large Size";
    public const string P2HeaderThisIsLargeSizePanelHeader = "This is large size panel header";
    public const string P2HeaderThisPanelCanOnlyBeCollapsedByClicking = "This panel can only be collapsed by clicking text";
    public const string P2HeaderThisPanelCanOnlyBeCollapsedByClicking2 = "This panel can only be collapsed by clicking icon";
    public const string P2HeaderThisPanelCanTBeCollapsed = "This panel can't be collapsed";
    public const string P2TextADogIsATypeOfDomesticatedAnimal = "A dog is a type of domesticated animal. Known for its loyalty and faithfulness, it can be found as a welcome guest in many households across the world.";
    public const string P2TextExpandIconPosition = "Expand Icon Position:";
    public const string P2ContentStart = "Start";
    public const string P2ContentEnd = "End";

    protected override Type GetResourceKindType() => typeof(CollapseShowCaseLangResourceKind);
}
