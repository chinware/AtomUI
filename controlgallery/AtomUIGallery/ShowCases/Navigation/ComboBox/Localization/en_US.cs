using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ComboBox;

[LanguageProvider(LanguageCode.en_US, ComboBoxShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic button spinner.";
    public const string ItemsSourceTitle = "Generate ComboBoxItem by ItemsSource";
    public const string ItemsSourceDescription = "Generate structure based on ItemsSource and template.";
    public const string DisabledTitle = "Disabled";
    public const string DisabledDescription = "Disabled button spinner.";
    public const string ThreeSizesTitle = "Three sizes of Input";
    public const string ThreeSizesDescription = "There are three sizes of an ComboBox: large (40px), default (32px) and small (24px).";
    public const string VariantsTitle = "Variants";
    public const string VariantsDescription = "Variants of Input.";
    public const string PrePostTabTitle = "Pre / Post tab";
    public const string PrePostTabDescription = "Using pre and post tabs example.";
    public const string PrefixSuffixTitle = "prefix and suffix";
    public const string PrefixSuffixDescription = "Add a prefix or suffix icons inside input.";
    public const string StatusTitle = "Status";
    public const string StatusDescription = "Add status to Input with status, which could be error or warning.";

    protected override Type GetResourceKindType() => typeof(ComboBoxShowCaseLangResourceKind);
}
