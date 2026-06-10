using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Transfer;

[LanguageProvider(LanguageCode.en_US, TransferShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic usage of Transfer involves providing the source data and target keys arrays, plus the rendering and some callback functions.";
    public const string ScenarioBasic = "Basic";
    public const string ScenarioAdvanced = "Advanced";
    public const string ScenarioTreeStatus = "Tree & Status";
    public const string OneWayTitle = "One Way";
    public const string OneWayDescription = "Use oneWay to make Transfer the one way style.";
    public const string SearchTitle = "Search";
    public const string SearchDescription = "Transfer with a search box.";
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

    protected override Type GetResourceKindType() => typeof(TransferShowCaseLangResourceKind);
}
