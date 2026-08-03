using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Calendar;

[LanguageProvider(LanguageCode.en_US, CalendarShowCase.LanguageId)]
internal partial class en_US
{
    public const string PageSubtitle = "Organize business content by date in a desktop calendar panel.";
    public const string PageDescription = "Calendar presents a month date grid or a year month grid with valid range, disabled dates, cell customization, and selection events.";
    public const string ComponentCategory = "Data Display";
    public const string ComponentStatusStable = "Stable";
    public const string ScenarioExamples = "Examples";

    public const string BasicTitle = "Basic";
    public const string BasicDescription = "A full-screen month calendar.";

    public const string CardTitle = "Card";
    public const string CardDescription = "Nested inside a container element for rendering in limited space.";

    public const string LunarCalendarTitle = "Lunar Calendar";
    public const string LunarCalendarDescription = "A full-screen calendar with lunar dates, solar terms, traditional festivals, and application-provided holiday annotations.";
    public const string LunarCalendarCardTitle = "Lunar Calendar Card";
    public const string LunarCalendarCardDescription = "A compact lunar calendar for limited spaces.";

    public const string SelectableCalendarTitle = "Selectable Calendar";
    public const string SelectableCalendarDescription = "A basic calendar component with Year/Month switch.";
    public const string SelectableCalendarSelectedMessage = "You selected date: {0:yyyy-MM-dd}";

    public const string NoticeCalendarTitle = "Notice Calendar";
    public const string NoticeCalendarDescription = "Render notice items in date cells and backlog numbers in month cells.";
    public const string NoticeCalendarWarningEventText = "This is warning event.";
    public const string NoticeCalendarUsualEventText = "This is usual event.";
    public const string NoticeCalendarErrorEventText = "This is error event.";
    public const string NoticeCalendarLongUsualEventText = "This is very long usual event......";
    public const string NoticeCalendarErrorEvent1Text = "This is error event 1.";
    public const string NoticeCalendarErrorEvent2Text = "This is error event 2.";
    public const string NoticeCalendarErrorEvent3Text = "This is error event 3.";
    public const string NoticeCalendarErrorEvent4Text = "This is error event 4.";
    public const string NoticeCalendarBacklogText = "Backlog number";

    public const string CrossDateEventsTitle = "Event Range";
    public const string CrossDateEventsDescription = "Declare RangeBars and let Calendar draw continuous event bars across dates.";
    public const string CrossDateEventsReleaseText = "Release window";
    public const string CrossDateEventsDesignReviewText = "Design review";
    public const string CrossDateEventsMaintenanceText = "Maintenance";
    public const string CrossDateEventsBugFixText = "Bug fix";

}
