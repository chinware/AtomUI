using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ButtonSpinner;

[LanguageProvider(LanguageCode.en_US, ButtonSpinnerShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic button spinner.";
    public const string ThreeSizesTitle = "Three sizes of Input";
    public const string ThreeSizesDescription = "There are three sizes of an button spinner: large (40px), default (32px) and small (24px).";
    public const string VariantsTitle = "Variants";
    public const string VariantsDescription = "Variants of Input.";
    public const string DisabledTitle = "Disabled";
    public const string DisabledDescription = "Variants of disabled Input.";
    public const string PrePostTabTitle = "Pre / Post tab";
    public const string PrePostTabDescription = "Using pre and post tabs example.";
    public const string PrefixSuffixTitle = "prefix and suffix";
    public const string PrefixSuffixDescription = "Add a prefix or suffix icons inside input.";
    public const string StatusTitle = "Status";
    public const string StatusDescription = "Add status to Input with status, which could be error or warning.";
    public const string P2Text床前明月光 = "床前明月光";

    protected override Type GetResourceKindType() => typeof(ButtonSpinnerShowCaseLangResourceKind);
}
