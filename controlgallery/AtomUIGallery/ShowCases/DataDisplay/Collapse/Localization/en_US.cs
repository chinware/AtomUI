using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Collapse;

[LanguageProvider(LanguageCode.en_US, CollapseShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
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

    protected override Type GetResourceKindType() => typeof(CollapseShowCaseLangResourceKind);
}
