using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Transfer;

[LanguageProvider(LanguageCode.en_US, TransferShowCase.LanguageId)]
internal partial class en_US
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic usage of Transfer involves providing the source data and target keys arrays, plus the rendering and some callback functions.";
    public const string ScenarioBasic = "Basic";
    public const string ScenarioAdvanced = "Advanced";
    public const string ScenarioTreeStatus = "Tree & Status";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string PageSubtitle = "Move items between two collections with optional search, paging, and tree views.";
    public const string PageDescription = "Transfer presents candidate items on the left and selected items on the right. It supports one-way movement, filtering, pagination, custom item templates, and tree-backed source data.";
    public const string ComponentCategory = "Data Entry";
    public const string ComponentStatusStable = "Stable";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyItemsSource = "Data collection displayed by the transfer lists.";
    public const string ApiPropertyTargetKeys = "Keys of items currently placed in the target list. The property binds TwoWay by default and tracks collection mutations.";
    public const string ApiPropertySelectedKeys = "Keys currently selected in the source and target panels. The property binds TwoWay by default and tracks collection mutations.";
    public const string ApiPropertySourceTitle = "Title displayed above the source list.";
    public const string ApiPropertyTargetTitle = "Title displayed above the target list.";
    public const string ApiPropertyIsOneWay = "Allows moving items only from source to target.";
    public const string ApiPropertyIsFilterEnabled = "Shows a search box for filtering transfer items.";
    public const string ApiPropertyFilterPlaceholderText = "Placeholder text shown in the filter input.";
    public const string ApiPropertyFilterValueSelector = "Selects the text used by the default filter.";
    public const string ApiPropertyListWidth = "Width of each transfer list.";
    public const string ApiPropertyListHeight = "Height of each transfer list.";
    public const string ApiPropertyPageSize = "Number of items shown per transfer page.";
    public const string ApiPropertyStatus = "Validation status style of the transfer surface.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";
    public const string TokenNameListWidth = "Default width of a transfer list.";
    public const string TokenNameListWidthLG = "Width of a large transfer list.";
    public const string TokenNameListHeight = "Default height of a transfer list.";
    public const string TokenNameItemHeight = "Height of each transfer item row.";
    public const string TokenNameItemPadding = "Vertical padding of each transfer item row.";
    public const string TokenNameHeaderHeight = "Height of the transfer list header.";
    public const string TokenNameHeaderPadding = "Padding inside the transfer list header.";
    public const string TokenNamePaginationMargin = "Outer margin of the transfer pagination area.";
    public const string TokenNameDataGridSelectionHeaderMargin = "Margin used by the data-grid transfer selection header.";
    public const string OneWayTitle = "One Way";
    public const string OneWayDescription = "Use oneWay to make Transfer the one way style.";
    public const string SearchTitle = "Search";
    public const string SearchDescription = "Transfer with a search box.";
    public const string ControlledKeysTitle = "Controlled keys";
    public const string ControlledKeysDescription = "Bind TargetKeys and SelectedKeys to ObservableCollection values. External collection mutations and Transfer interactions stay synchronized.";
    public const string AdvancedTitle = "Advanced";
    public const string AdvancedDescription = "Advanced Usage of Transfer. You can customize the labels of the transfer buttons, the width and height of the columns, and what should be displayed in the footer.";
    public const string PaginationTitle = "Pagination";
    public const string PaginationDescription = "Store a large amount of items with pagination.";
    public const string TreeTransferTitle = "Tree Transfer";
    public const string TreeTransferDescription = "Customize the render list with a Tree component.";
    public const string StatusTitle = "Status";
    public const string StatusDescription = "Add status to Transfer with status, which could be error or warning.";
    public const string P2SourceTitle = "Source";
    public const string P2TargetTitle = "Target";
    public const string P2TextText = "-";
    public const string P2HeaderName = "Name";
    public const string P2HeaderTag = "Tag";
    public const string P2HeaderDescription = "Description";
    public const string P2ContentLeftButtonReload = "Left button reload";
    public const string P2ContentRightButtonReload = "Right button reload";
    public const string P2ContentAddTargetKey = "Add key 3 to target";
    public const string P2ContentClearTargetKeys = "Clear target keys";
    public const string P2ContentSelectSourceKey = "Select key 4";
    public const string P2TargetKeysCountLabel = "Target keys:";
    public const string P2SelectedKeysCountLabel = "Selected keys:";

    public const string P2OnContentDisable = "Disable";

    public const string P2OffContentEnable = "Enable";

    public const string P2FilterPlaceholderTextSearchHere = "Search here";

    public const string P2ToSourceButtonTextToLeft = "To left";

    public const string P2ToTargetButtonTextToRight = "To right";

    public const string P2OnContentOnyWay = "One way";

    public const string P2OffContentOnyWay = "One way";
    public const string P2ItemContentFormat = "content{0}";
    public const string P2ItemDescriptionFormat = "description of content{0}";
    public const string P2TagCat = "cat";
    public const string P2TagDog = "dog";
    public const string P2TagBird = "bird";

}
