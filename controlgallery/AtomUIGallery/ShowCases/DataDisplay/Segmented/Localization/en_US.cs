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
    public const string ThreeSizesTitle = "Three sizes of Segmented";
    public const string ThreeSizesDescription = "There are three sizes of a Segmented: large (40px), default (32px) and small (24px).";
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

    protected override Type GetResourceKindType() => typeof(SegmentedShowCaseLangResourceKind);
}
