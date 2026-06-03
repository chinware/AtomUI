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
    public const string P2PlaceholderTextPleaseSelect = "Please select";
    public const string P2TextPlacement = "Placement:";
    public const string P2ContentTopleft = "Top Left";
    public const string P2ContentTopright = "Top Right";
    public const string P2ContentBottomleft = "Bottom Left";
    public const string P2ContentBottomright = "Bottom Right";

    public const string P2OnContentShowIcon = "Show icon";

    public const string P2OffContentShowIcon = "Show icon";

    public const string P2OnContentTreeLine = "Tree line";

    public const string P2OffContentTreeLine = "Tree line";

    public const string P2OnContentShowLeafIcon = "Show leaf icon";

    public const string P2OffContentShowLeafIcon = "Show leaf icon";
    public const string P2AddOnPrefix = "Prefix";
    public const string P2HeaderParent1 = "Parent 1";
    public const string P2HeaderParent10 = "Parent 1-0";
    public const string P2HeaderParent11 = "Parent 1-1";
    public const string P2HeaderLeaf1 = "Leaf 1";
    public const string P2HeaderLeaf2 = "Leaf 2";
    public const string P2HeaderLeaf3 = "Leaf 3";
    public const string P2HeaderLeaf4 = "Leaf 4";
    public const string P2HeaderLeaf5 = "Leaf 5";
    public const string P2HeaderLeaf6 = "Leaf 6";
    public const string P2HeaderLeaf11 = "Leaf 11";
    public const string P2HeaderMyLeaf = "My leaf";
    public const string P2HeaderYourLeaf = "Your leaf";
    public const string P2HeaderSss = "Node SSS";
    public const string P2HeaderNode1 = "Node 1";
    public const string P2HeaderNode2 = "Node 2";
    public const string P2HeaderChildNode = "Child Node";
    public const string P2HeaderChildNode1 = "Child Node 1";
    public const string P2HeaderChildNode2 = "Child Node 2";
    public const string P2HeaderChildNode3 = "Child Node 3";
    public const string P2HeaderChildNode4 = "Child Node 4";
    public const string P2HeaderChildNode5 = "Child Node 5";
    public const string P2HeaderChildNode6 = "Child Node 6";
    public const string P2HeaderChildNode7 = "Child Node 7";
    public const string P2HeaderExpandToLoad = "Expand to load";
    public const string P2HeaderTreeNode = "Tree Node";

    protected override Type GetResourceKindType() => typeof(TreeSelectShowCaseLangResourceKind);
}
