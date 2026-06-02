using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Transfer;

[LanguageProvider(LanguageCode.en_US, TransferShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic usage of Transfer involves providing the source data and target keys arrays, plus the rendering and some callback functions.";
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

    protected override Type GetResourceKindType() => typeof(TransferShowCaseLangResourceKind);
}
