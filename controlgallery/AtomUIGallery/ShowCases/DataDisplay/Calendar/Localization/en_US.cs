using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Calendar;

[LanguageProvider(LanguageCode.en_US, CalendarShowCase.LanguageId)]
internal partial class en_US
{
    public const string PageSubtitle = "Organize business content by date in a desktop calendar panel.";
    public const string PageDescription = "Calendar presents a month date grid or a year month grid following Ant Design 6 semantics, with valid range, disabled dates, cell customization, and selection events.";
    public const string ComponentCategory = "Data Display";
    public const string ComponentStatusStable = "Stable";
    public const string ScenarioExamples = "Examples";

    public const string BasicTitle = "Basic";
    public const string BasicDescription = "A full-screen month calendar.";

    public const string MiniTitle = "Mini";
    public const string MiniDescription = "A compact calendar for narrow containers (Fullscreen = false).";

    public const string YearModeTitle = "Year mode";
    public const string YearModeDescription = "A year panel showing a month grid (Mode = Year).";

    public const string ShowWeekTitle = "Week numbers";
    public const string ShowWeekDescription = "Show an extra week-number column (ShowWeek = true).";

    public const string RangeTitle = "Valid range and disabled dates";
    public const string RangeDescription = "Constrain selectable dates with ValidRange, and disable specific dates with DisabledDate.";

    public const string CellTemplateTitle = "Custom cell content";
    public const string CellTemplateDescription = "Render business content inside each cell with CellTemplate.";

    public const string FullCellTemplateTitle = "Custom full cell";
    public const string FullCellTemplateDescription = "Replace the whole cell inner content with FullCellTemplate.";

    public const string HeaderTemplateTitle = "Custom header";
    public const string HeaderTemplateDescription = "Replace the default header with HeaderTemplate.";

    public const string EventsTitle = "Selection events";
    public const string EventsDescription = "Observe ValueChanged, Selected, and PanelChanged and their sources.";
    public const string EventsLogHint = "Interact with the calendar to see events.";
}
