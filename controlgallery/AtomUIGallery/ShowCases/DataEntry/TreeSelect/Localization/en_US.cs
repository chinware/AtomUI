using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TreeSelect;

[LanguageProvider(LanguageCode.en_US, TreeSelectShowCase.LanguageId)]
internal partial class en_US
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic usage.";
    public const string ScenarioBasic = "Basic";
    public const string ScenarioBehavior = "Behavior";
    public const string ScenarioAppearance = "Appearance";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string PageSubtitle = "Select values from hierarchical data in a compact input.";
    public const string PageDescription = "TreeSelect combines an input selector with tree navigation, making it suitable for hierarchical categories, organization nodes, permissions, and other nested option sets.";
    public const string ComponentCategory = "Data Entry";
    public const string ComponentStatusStable = "Stable";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyItemsSource = "Hierarchical options displayed by the selector.";
    public const string ApiPropertySelectedItem = "Currently selected tree item in single mode. Defaults to two-way binding and supports data validation.";
    public const string ApiPropertySelectedItems = "Currently selected tree items in multiple or checkable mode. Defaults to two-way binding, supports data validation, and refreshes on in-place collection changes.";
    public const string ApiPropertyIsMultiple = "Allows selecting multiple tree nodes.";
    public const string ApiPropertyIsTreeCheckable = "Shows checkboxes in the tree popup.";
    public const string ApiPropertyIsDefaultExpandAll = "Expands tree nodes by default when the popup opens.";
    public const string ApiPropertyIsAllowClear = "Displays a clear action when a value is selected.";
    public const string ApiPropertyIsFilterEnabled = "Allows filtering tree nodes from the input.";
    public const string ApiPropertyIsShowOverflowTip = "Shows a tooltip with the full selected node text when selected text or tags overflow.";
    public const string ApiPropertyOverflowTipDelay = "Delay in milliseconds before the overflow tooltip opens.";
    public const string ApiPropertyOverflowTipPlacement = "Placement of the overflow tooltip relative to the clipped selected node text or tag.";
    public const string ApiPropertyDataLoader = "Asynchronously loads child nodes when a tree item expands.";
    public const string ApiPropertyPlacement = "Controls where the popup is placed relative to the input.";
    public const string ApiPropertyMaxCount = "Maximum number of selected values before more options become disabled.";
    public const string ApiPropertyStyleVariant = "Input visual variant.";
    public const string ApiPropertyStatus = "Validation status style of the selector.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";
    public const string TokenNameMinPopupWidth = "Minimum popup width used by TreeSelect.";
    public const string BindingTitle = "Binding";
    public const string BindingDescription = "SelectedItem and SelectedItems are two-way bindable by default, including in-place collection changes.";
    public const string BindingSingleLabel = "Single selected item";
    public const string BindingMultipleLabel = "Multiple selected items";
    public const string BindingSetSingleButton = "Select your leaf";
    public const string BindingSetMultipleButton = "Select two nodes";
    public const string BindingClearButton = "Clear";
    public const string BindingViewModelValueLabel = "ViewModel value:";
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
    public const string SizeTypeTitle = "Sizes";
    public const string SizeTypeDescription = "TreeSelect supports large, middle, small, and Custom sizes with local height and font overrides.";
    public const string P2PlaceholderTextPleaseSelect = "Please select";
    public const string P2TextPlacement = "Placement:";
    public const string P2ContentTopleft = "Top Left";
    public const string P2ContentTopright = "Top Right";
    public const string P2ContentBottomleft = "Bottom Left";
    public const string P2ContentBottomright = "Bottom Right";
    public const string P2ContentLarge = "Large";
    public const string P2ContentDefault = "Default";
    public const string P2ContentSmall = "Small";
    public const string P2ContentCustom = "Custom";

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

}
