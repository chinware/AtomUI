using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.GroupBox;

[LanguageProvider(LanguageCode.en_US, GroupBoxShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic usage of the GroupBox control.";
    public const string HeaderPositionTitle = "Header title Position";
    public const string HeaderPositionDescription = "GroupBox Header supports three position types: Left, Center and Right.";
    public const string HeaderStyleTitle = "Header title style";
    public const string HeaderStyleDescription = "GroupBox Header supports customizing some properties of color and font.";
    public const string HeaderIconTitle = "Header Icon";
    public const string HeaderIconDescription = "GroupBox Header supports specifying Icon.";
    public const string P2HeaderTitleTitleInfo = "Title Info";
    public const string P2TextContentOfGroupBox = "Content of group box";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string PageSubtitle = "Group related content under a titled frame.";
    public const string PageDescription =
        "GroupBox wraps related content with a bordered container and a configurable header title, position, icon and typography.";
    public const string ComponentCategory = "Data Display";
    public const string ComponentStatusStable = "Stable";
    public const string InfoNamespaceLabel = "Namespace";
    public const string InfoPackageLabel = "Package";
    public const string InfoBaseClassLabel = "Base class";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyHeaderTitle = "Text displayed in the group header.";
    public const string ApiPropertyHeaderTitleColor = "Brush used for the header title text.";
    public const string ApiPropertyHeaderIcon = "Optional icon displayed before the header title.";
    public const string ApiPropertyHeaderTitlePosition = "Controls whether the header title is aligned left, center or right.";
    public const string ApiPropertyHeaderFontSize = "Font size of the header title.";
    public const string ApiPropertyHeaderFontStyle = "Font style of the header title.";
    public const string ApiPropertyHeaderFontWeight = "Font weight of the header title.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameContentPadding = "Padding inside the content area.";
    public const string TokenNameHeaderContainerMargin = "Outer margin of the header container.";
    public const string TokenNameHeaderContentPadding = "Padding inside the header content.";
    public const string TokenNameHeaderIconMargin = "Margin around the header icon.";
    public const string TokenNameOrientationMarginPercent = "Header side offset ratio for title positioning.";
    public const string TokenNameTextPaddingInline = "Inline text padding measured in em.";
    public const string TokenNameVerticalMarginInline = "Horizontal margin for the vertical separator.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";

    protected override Type GetResourceKindType() => typeof(GroupBoxShowCaseLangResourceKind);
}
