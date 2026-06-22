using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Segmented;

[LanguageProvider(LanguageCode.en_US, SegmentedShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic usage.";
    public const string BlockSegmentedTitle = "Block Segmented";
    public const string BlockSegmentedDescription = "block property will make the Segmented fit to its parent width.";
    public const string DisabledTitle = "Disabled";
    public const string DisabledDescription = "Disabled Segmented.";
    public const string ThreeSizesTitle = "Sizes of Segmented";
    public const string ThreeSizesDescription = "Segmented supports large (40px), default (32px), small (24px), and custom size.";
    public const string IconOnlyTitle = "With Icon only";
    public const string IconOnlyDescription = "Set icon without label for Segmented Item.";
    public const string WithIconTitle = "With Icon";
    public const string WithIconDescription = "Set icon for Segmented Item.";
    public const string P2ContentDaily = "Daily";
    public const string P2ContentWeekly = "Weekly";
    public const string P2ContentMonthly = "Monthly";
    public const string P2ContentQuarterly = "Quarterly";
    public const string P2ContentYearly = "Yearly";
    public const string P2ContentLongtextLongtextLongtextLongtext = "longtext-longtext-longtext-longtext";
    public const string P2ContentMap = "Map";
    public const string P2ContentTransit = "Transit";
    public const string P2ContentSatellite = "Satellite";
    public const string P2ContentList = "List";
    public const string P2ContentAva = "Ava";

    public const string P2ContentKanban = "Kanban";

    public const string P2ContentAtomUI = "AtomUI";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string PageSubtitle = "Group mutually exclusive options into a compact segmented selector.";
    public const string PageDescription =
        "Segmented presents a small set of related options with single selection, optional icons, responsive expansion and token-based sizing.";
    public const string ComponentCategory = "Data Display";
    public const string ComponentStatusStable = "Stable";
    public const string InfoNamespaceLabel = "Namespace";
    public const string InfoPackageLabel = "Package";
    public const string InfoBaseClassLabel = "Base class";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertySizeType = "Controls the size of the segmented track and items.";
    public const string ApiPropertyIsExpanding = "Stretches items to fill the available parent width.";
    public const string ApiPropertyIsMotionEnabled = "Enables animated movement of the selected thumb and item background transitions.";
    public const string ApiPropertySelectedIndex = "Index of the currently selected item.";
    public const string ApiPropertySelectedItem = "The currently selected item or bound item value.";
    public const string ApiEventSelectionChanged = "Raised when the selected item changes.";
    public const string ApiPropertyIcon = "Optional icon displayed before a segmented item label.";
    public const string ApiPropertyIsSelected = "Marks an item as the selected item.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameTrackPadding = "Padding around the segmented track.";
    public const string TokenNameTrackBg = "Background color of the segmented track.";
    public const string TokenNameItemColor = "Default text color of segmented items.";
    public const string TokenNameItemHoverColor = "Text color of items while hovered.";
    public const string TokenNameItemHoverBg = "Background color of items while hovered.";
    public const string TokenNameItemActiveBg = "Background color of items while pressed.";
    public const string TokenNameItemSelectedBg = "Background color of the selected item.";
    public const string TokenNameItemSelectedColor = "Text color of the selected item.";
    public const string TokenNameItemMinHeightLG = "Minimum item height in large size.";
    public const string TokenNameItemMinHeight = "Minimum item height in default size.";
    public const string TokenNameItemMinHeightSM = "Minimum item height in small size.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";

    protected override Type GetResourceKindType() => typeof(SegmentedShowCaseLangResourceKind);
}
