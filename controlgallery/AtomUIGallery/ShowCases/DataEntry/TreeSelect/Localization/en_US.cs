using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TreeSelect;

[LanguageProvider(LanguageCode.en_US, TreeSelectShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic usage.";
    public const string MultipleSelectionTitle = "Multiple Selection";
    public const string MultipleSelectionDescription = "Multiple selection usage.";
    public const string GenerateFromTreeDataTitle = "Generate from tree data";
    public const string GenerateFromTreeDataDescription = "The tree structure can be populated using treeData property. This is a quick and easy way to provide the tree content.";
    public const string CheckableTitle = "Checkable";
    public const string CheckableDescription = "Multiple and checkable.";
    public const string AsynchronousLoadingTitle = "Asynchronous loading";
    public const string AsynchronousLoadingDescription = "Asynchronous loading tree node.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "You can manually specify the position of the popup via placement.";
    public const string ShowTreeLineTitle = "Show Tree Line";
    public const string ShowTreeLineDescription = "Use treeLine to show the line style.";
    public const string VariantsTitle = "Variants";
    public const string VariantsDescription = "Variants of TreeSelect, there are four variants: outlined filled borderless and underlined.";
    public const string StatusTitle = "Status";
    public const string StatusDescription = "Add status to TreeSelect with status, which could be error or warning.";
    public const string PrefixAndSuffixTitle = "Prefix and Suffix";
    public const string PrefixAndSuffixDescription = "Custom prefix and suffixIcon.";
    public const string MaxCountTitle = "Max Count";
    public const string MaxCountDescription = "You can set the maxCount prop to control the max number of items can be selected. When the limit is exceeded, the options will become disabled.";

    protected override Type GetResourceKindType() => typeof(TreeSelectShowCaseLangResourceKind);
}
