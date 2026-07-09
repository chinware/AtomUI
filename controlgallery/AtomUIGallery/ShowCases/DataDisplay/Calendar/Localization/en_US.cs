using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Calendar;

[LanguageProvider(LanguageCode.en_US, CalendarShowCase.LanguageId)]
internal partial class en_US
{
    public const string PageSubtitle = "Select dates from a monthly calendar panel.";
    public const string PageDescription = "Calendar presents dates in month, year, or decade views and supports single, range, and multiple-range selection.";
    public const string ComponentCategory = "Data Display";
    public const string ComponentStatusStable = "Stable";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";

    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The simplest usage for Calendar.";

    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyFirstDayOfWeek = "Sets the day that starts each week.";
    public const string ApiPropertyIsTodayHighlighted = "Controls whether today is highlighted in the month view.";
    public const string ApiPropertyHeaderBackground = "Sets the calendar header background brush.";
    public const string ApiPropertyDisplayMode = "Controls whether the calendar shows month, year, or decade content.";
    public const string ApiPropertySelectionMode = "Controls whether users can select one date, one range, multiple ranges, or nothing.";
    public const string ApiPropertySelectedDate = "Gets or sets the primary selected date.";
    public const string ApiPropertyDisplayDate = "Gets or sets the date currently displayed by the calendar.";
    public const string ApiPropertyDisplayDateStart = "Sets the earliest date that can be displayed.";
    public const string ApiPropertyDisplayDateEnd = "Sets the latest date that can be displayed.";
    public const string ApiPropertyIsMotionEnabled = "Controls whether calendar transition motion is enabled.";

    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameCellHoverBg = "Cell hover background color.";
    public const string TokenNameCellActiveWithRangeBg = "Cell background color inside a selected range.";
    public const string TokenNameCellHoverWithRangeBg = "Cell hover background color inside a selected range.";
    public const string TokenNameCellBgDisabled = "Disabled cell background color.";
    public const string TokenNameCellRangeBorderColor = "Border color used by range selection cells.";
    public const string TokenNameCellHeight = "Calendar cell height.";
    public const string TokenNameCellWidth = "Calendar cell width.";
    public const string TokenNameCellLineHeight = "Calendar cell text line height.";
    public const string TokenNamePanelContentPadding = "Calendar panel content padding.";
    public const string TokenNameItemPanelMinWidth = "Calendar item panel minimum width.";
    public const string TokenNameItemPanelMinHeight = "Calendar item panel minimum height.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";

}
