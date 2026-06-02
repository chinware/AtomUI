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

    protected override Type GetResourceKindType() => typeof(StatisticShowCaseLangResourceKind);
}
