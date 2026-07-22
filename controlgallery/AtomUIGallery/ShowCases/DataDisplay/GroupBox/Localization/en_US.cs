using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.GroupBox;

[LanguageProvider(LanguageCode.en_US, GroupBoxShowCase.LanguageId)]
internal partial class en_US
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic usage of the GroupBox control.";
    public const string AutoHeightTitle = "Auto height";
    public const string AutoHeightDescription = "GroupBox grows from the measured header, padding and content size when no height is set.";
    public const string HeaderPositionTitle = "Header title Position";
    public const string HeaderPositionDescription = "GroupBox Header supports three position types: Left, Center and Right.";
    public const string HeaderStyleTitle = "Header title style";
    public const string HeaderStyleDescription = "GroupBox Header supports customizing some properties of color and font.";
    public const string HeaderIconTitle = "Header Icon";
    public const string HeaderIconDescription = "GroupBox Header supports specifying Icon.";
    public const string P2HeaderTitleTitleInfo = "Title Info";
    public const string P2TextContentOfGroupBox = "Content of group box";
    public const string AutoHeightContentOverview = "The GroupBox below does not set Height, so its content area grows with the text rows.";
    public const string AutoHeightContentDetail = "When the content comes from StackPanel, Grid or explicitly sized controls, GroupBox uses the content DesiredSize to calculate its total height.";
    public const string AutoHeightContentFooter = "If the parent sets Height or MaxHeight, Avalonia layout constraints still decide clipping or scrolling.";
    public const string ScenarioExamples = "Examples";
    public const string PageSubtitle = "Group related content under a titled frame.";
    public const string PageDescription =
        "GroupBox wraps related content with a bordered container and a configurable header title, position, icon and typography.";
    public const string ComponentCategory = "Data Display";
    public const string ComponentStatusStable = "Stable";

}
