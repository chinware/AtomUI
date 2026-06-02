using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Rate;

[LanguageProvider(LanguageCode.en_US, RateShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The simplest usage.";
    public const string HalfStarTitle = "Half star";
    public const string HalfStarDescription = "Support select half star.";
    public const string ShowCopywritingTitle = "Show copywriting";
    public const string ShowCopywritingDescription = "Add copywriting in rate components.";
    public const string ReadOnlyTitle = "Read only";
    public const string ReadOnlyDescription = "Read only, can't use mouse to interact.";
    public const string ClearStarTitle = "Clear star";
    public const string ClearStarDescription = "Support set allow to clear star when click again.";
    public const string OtherCharacterTitle = "Other Character";
    public const string OtherCharacterDescription = "Replace the default star to other character like alphabet, digit, iconfont or even Chinese word.";

    protected override Type GetResourceKindType() => typeof(RateShowCaseLangResourceKind);
}
