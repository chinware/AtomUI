using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Statistic;

[LanguageProvider(LanguageCode.en_US, StatisticShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Simplest Usage.";
    public const string UnitTitle = "Unit";
    public const string UnitDescription = "Add unit through prefix and suffix.";
    public const string InCardTitle = "In Card";
    public const string InCardDescription = "Display statistic data in Card.";
    public const string AnimatedNumberTitle = "Animated number";
    public const string AnimatedNumberDescription = "Animated number with StatisticCountUp.";
    public const string TimerTitle = "Timer";
    public const string TimerDescription = "Timer component.";
    public const string P2HeaderActiveUsers = "Active Users";
    public const string P2HeaderAccountBalanceCny = "Account Balance (CNY)";
    public const string P2HeaderFeedback = "Feedback";
    public const string P2HeaderUnmerged = "Unmerged";
    public const string P2HeaderActive = "Active";
    public const string P2HeaderIdle = "Idle";
    public const string P2HeaderMillionSeconds = "Million Seconds";
    public const string P2HeaderCountdown = "Countdown";
    public const string P2HeaderCountup = "Countup";
    public const string P2HeaderDayLevelCountdown = "Day Level (Countdown)";
    public const string P2HeaderDayLevelCountup = "Day Level (Countup)";
    public const string P2ContentRecharge = "Recharge";
    public const string P2DayLevelFormat = "d\\ \\d\\ h\\ \\h\\ m\\ \\m\\ s\\ \\s";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string PageSubtitle = "Display numeric facts, metrics and countdown values with clear visual emphasis.";
    public const string PageDescription =
        "Statistic presents important numbers with optional units, icons, loading states, animated values and timer-based countdown or countup displays.";
    public const string ComponentCategory = "Data Display";
    public const string ComponentStatusStable = "Stable";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyHeader = "Title or label displayed above the statistic value.";
    public const string ApiPropertyValue = "Value rendered by Statistic before formatting.";
    public const string ApiPropertyFormatter = "Custom formatter used to render Statistic values.";
    public const string ApiPropertyDecimalSeparator = "Decimal separator used for numeric formatting.";
    public const string ApiPropertyGroupSeparator = "Group separator used for numeric formatting.";
    public const string ApiPropertyPrecision = "Number of fractional digits used for numeric formatting.";
    public const string ApiPropertyIsLoading = "Shows a skeleton placeholder instead of the value content.";
    public const string ApiPropertyValuePrefixAddOn = "Optional content displayed before the value.";
    public const string ApiPropertyValueSuffixAddOn = "Optional content displayed after the value.";
    public const string ApiPropertyContentForeground = "Brush used by the statistic value and add-on content.";
    public const string ApiPropertyContentFontSize = "Font size used by the statistic value and add-on content.";
    public const string ApiPropertyTimerValue = "Target date/time used by TimerStatistic for countdown or countup.";
    public const string ApiPropertyFormat = "TimeSpan format string used by TimerStatistic output.";
    public const string ApiPropertyRefreshDuration = "Timer refresh interval.";
    public const string ApiEventCountdownFinished = "Raised when a TimerStatistic countdown reaches zero.";
    public const string ApiPropertyEndValue = "Target number animated by StatisticCountUp.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameTitleFontSize = "Font size used by the statistic title.";
    public const string TokenNameContentFontSize = "Font size used by the statistic value content.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";

    protected override Type GetResourceKindType() => typeof(StatisticShowCaseLangResourceKind);
}
