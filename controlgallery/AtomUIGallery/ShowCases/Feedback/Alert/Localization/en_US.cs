using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Alert;

[LanguageProvider(LanguageCode.en_US, AlertShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The simplest usage for short messages.";
    public const string MoreTypesTitle = "More types";
    public const string MoreTypesDescription = "There are 4 types of Alert: success, info, warning, error.";
    public const string ClosableTitle = "Closable";
    public const string ClosableDescription = "To show close button.";
    public const string DescriptionTitle = "Description";
    public const string DescriptionDescription = "Additional description for alert message.";
    public const string CustomActionTitle = "Custom action";
    public const string CustomActionDescription = "Custom action.";
    public const string LoopBannerTitle = "Loop Banner";
    public const string LoopBannerDescription = "Show a loop banner.";

    protected override Type GetResourceKindType() => typeof(AlertShowCaseLangResourceKind);
}
