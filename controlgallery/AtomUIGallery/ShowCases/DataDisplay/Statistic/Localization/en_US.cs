using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Statistic;

[LanguageProvider(LanguageCode.en_US, StatisticShowCase.LanguageId)]
internal class en_US : LanguageProvider
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

    protected override Type GetResourceKindType() => typeof(StatisticShowCaseLangResourceKind);
}
