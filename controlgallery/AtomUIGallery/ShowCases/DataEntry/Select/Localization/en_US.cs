using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Select;

[LanguageProvider(LanguageCode.en_US, SelectShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic Usage.";
    public const string SearchFieldTitle = "Select with search field";
    public const string SearchFieldDescription = "Search the options while expanded.";
    public const string CustomSearchTitle = "Custom Search";
    public const string CustomSearchDescription = "Customize search using filterOption, Case-sensitive search for Header.";
    public const string MultipleSelectionTitle = "multiple selection";
    public const string MultipleSelectionDescription = "Multiple selection, selecting from existing items.";
    public const string TagsTitle = "Tags";
    public const string TagsDescription = "Allow user to select tags from list or input custom tag.";
    public const string SizesTitle = "Sizes";
    public const string SizesDescription = "The height of the input field for the select defaults to 32px. If size is set to large, the height will be 40px, and if set to small, 24px.";
    public const string CustomDropdownOptionsTitle = "Custom dropdown options";
    public const string CustomDropdownOptionsDescription = "Use optionRender to customize the rendering dropdown options";
    public const string OptionGroupTitle = "Option Group";
    public const string OptionGroupDescription = "Using OptGroup to group the options.";
    public const string VariantsTitle = "Variants";
    public const string VariantsDescription = "Variants of Select, there are four variants: outlined filled borderless and underlined.";
    public const string HideAlreadySelectedTitle = "Hide Already Selected";
    public const string HideAlreadySelectedDescription = "Hide already selected options in the dropdown.";
    public const string StatusTitle = "Status";
    public const string StatusDescription = "Add status to Select with status, which could be error or warning.";
    public const string MaxCountTitle = "Max Count";
    public const string MaxCountDescription = "You can set the maxCount prop to control the max number of items can be selected. When the limit is exceeded, the options will become disabled.";
    public const string ResponsiveMaxTagCountTitle = "Responsive maxTagCount";
    public const string ResponsiveMaxTagCountDescription = "Auto collapse to tag with responsive case. Not recommend use in large form case since responsive calculation has a perf cost.";
    public const string PrefixAndSuffixTitle = "Prefix and Suffix";
    public const string PrefixAndSuffixDescription = "Custom prefix and suffixIcon.";

    protected override Type GetResourceKindType() => typeof(SelectShowCaseLangResourceKind);
}
