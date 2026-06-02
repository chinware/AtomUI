using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Cascader;

[LanguageProvider(LanguageCode.en_US, CascaderShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Cascade selection box for selecting province/city/district.";
    public const string DefaultValueTitle = "Default value";
    public const string DefaultValueDescription = "Specifies default value by an array.";
    public const string HoverTitle = "Hover";
    public const string HoverDescription = "Hover to expand sub menu, click to select option.";
    public const string DisabledOptionTitle = "Disabled option";
    public const string DisabledOptionDescription = "Disable option by specifying the disabled property in options.";
    public const string ChangeOnSelectTitle = "Change on select";
    public const string ChangeOnSelectDescription = "Allows the selection of only parent options.";
    public const string MultipleTitle = "Multiple";
    public const string MultipleDescription = "Select multiple options. Disable the checkbox by adding the disableCheckbox property and selecting a specific item. The style of the disable can be modified by the className.";
    public const string ShowCheckedStrategyTitle = "ShowCheckedStrategy";
    public const string ShowCheckedStrategyDescription = "Shows a selected item in a box using showCheckedStrategy.";
    public const string SearchTitle = "Search";
    public const string SearchDescription = "Search and select options directly.";
    public const string LoadOptionsLazilyTitle = "Load Options Lazily";
    public const string LoadOptionsLazilyDescription = "Load options lazily with loadData.";
    public const string PrefixAndSuffixTitle = "Prefix and Suffix";
    public const string PrefixAndSuffixDescription = "Use prefix to customize the prefix content, use suffixIcon to customize the selection box suffix icon, and use expandIcon to customize the current item expand icon.";
    public const string VariantsTitle = "Variants";
    public const string VariantsDescription = "Variants of Cascader, there are four variants: outlined filled borderless and underlined.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "You can manually specify the position of the popup via placement.";
    public const string StatusTitle = "Status";
    public const string StatusDescription = "Add status to Cascader with status, which could be error or warning.";
    public const string SizeTitle = "Size";
    public const string SizeDescription = "Cascade selection box of different sizes.";
    public const string BasicCascaderViewTitle = "Basic CascaderView";
    public const string BasicCascaderViewDescription = "The most basic usage.";
    public const string GenerateByTemplateTitle = "Generate by template";
    public const string GenerateByTemplateDescription = "You can use the Template mechanism to generate tree nodes";
    public const string SearchableTitle = "Searchable";
    public const string SearchableDescription = "Searchable CascaderView.";
    public const string SearchableItemsSourceTitle = "Searchable";
    public const string SearchableItemsSourceDescription = "Searchable CascaderView with ItemsSource.";
    public const string DefaultExpandedTitle = "Default expanded";
    public const string DefaultExpandedDescription = "You can set the default expansion path..";

    protected override Type GetResourceKindType() => typeof(CascaderShowCaseLangResourceKind);
}
