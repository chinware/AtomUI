using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Spin;

[LanguageProvider(LanguageCode.en_US, SpinShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicUsageTitle = "Basic Usage";
    public const string BasicUsageDescription = "A simple loading status.";
    public const string SizeTitle = "Size";
    public const string SizeDescription = "A small SpinIndicator is used for loading text, default sized SpinIndicator for loading a card-level block, and large SpinIndicator used for loading a page.";
    public const string CustomIndicatorTitle = "Custom spinning indicator";
    public const string CustomIndicatorDescription = "Use custom loading indicator.";
    public const string CustomizedDescriptionTitle = "Customized description";
    public const string CustomizedDescriptionDescription = "Customized description";
    public const string EmbeddedModeTitle = "Embedded mode";
    public const string EmbeddedModeDescription = "Embedding content into Spin will set it into loading state.";
    public const string P2DescriptionFurtherDetailsAboutTheContextOfThisAlert = "Further details about the context of this alert.";
    public const string P2TextLoadingState = "Loading state：";
    public const string P2TipLoading = "Loading...";

    public const string P2MessageAlertMessageTitle = "Alert message title";

    protected override Type GetResourceKindType() => typeof(SpinShowCaseLangResourceKind);
}
