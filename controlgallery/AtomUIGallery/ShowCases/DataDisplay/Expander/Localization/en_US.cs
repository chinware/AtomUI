using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Expander;

[LanguageProvider(LanguageCode.en_US, ExpanderShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string ExpanderTitle = "Expander";
    public const string ExpanderDescription = "By default, The simplest usage is to expand downward.";
    public const string SizeTitle = "Size";
    public const string SizeDescription = "Ant Design supports a default Expander size as well as a large and small size. If a large or small Expander is desired, set the size property to either large or small respectively. Omit the size property for a Expander with the default size.";
    public const string ExpandingDirectionTitle = "Expanding Direction";
    public const string ExpandingDirectionDescription = "The content area supports expansion in four directions.";
    public const string NestedPanelTitle = "Nested panel";
    public const string NestedPanelDescription = "Expander is nested inside the Collapse.";
    public const string BorderlessTitle = "Borderless";
    public const string BorderlessDescription = "A borderless style of Expander.";
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

    protected override Type GetResourceKindType() => typeof(ExpanderShowCaseLangResourceKind);
}
