using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Pagination;

[LanguageProvider(LanguageCode.en_US, PaginationShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ComponentCategory = "Navigation";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "Navigate long lists with page numbers, sizes, totals, and quick jumps.";
    public const string PageDescription = "Pagination divides large data sets into predictable pages. It supports alignment, page size selection, quick jump input, total information, mini size, and simple read-only or editable modes.";
    public const string InfoNamespaceLabel = "Namespace:";
    public const string InfoPackageLabel = "Package:";
    public const string InfoBaseClassLabel = "Base class:";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyTotal = "Total number of records across all pages.";
    public const string ApiPropertyCurrentPage = "Current one-based page index.";
    public const string ApiPropertyPageSize = "Number of records represented by each page.";
    public const string ApiPropertyPageCount = "Computed number of pages from total and page size.";
    public const string ApiPropertyIsHideOnSinglePage = "Hides pagination when total records fit on a single page.";
    public const string ApiPropertyAlign = "Aligns the pagination content to the start, center, or end.";
    public const string ApiPropertySizeType = "Controls normal or small pagination density.";
    public const string ApiPropertyIsMotionEnabled = "Enables or disables motion transitions.";
    public const string ApiPropertyCurrentPageChanged = "Raised when the current page changes.";
    public const string ApiPropertyIsShowSizeChanger = "Shows the page size selector.";
    public const string ApiPropertyIsShowQuickJumper = "Shows the quick jump input.";
    public const string ApiPropertyIsShowTotalInfo = "Shows total record information.";
    public const string ApiPropertyTotalInfoTemplate = "Template used to format total information text.";
    public const string ApiPropertyIsReadOnly = "Controls whether SimplePagination uses read-only display or editable jump input.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameItemBg = "Background color of pagination items.";
    public const string TokenNameItemSize = "Default pagination item size.";
    public const string TokenNameItemActiveBg = "Background color of the active pagination item.";
    public const string TokenNameItemSizeSM = "Small pagination item size.";
    public const string TokenNameItemLinkBg = "Background color for pagination item links.";
    public const string TokenNameItemActiveBgDisabled = "Background color for disabled active pagination items.";
    public const string TokenNameItemActiveColorDisabled = "Text color for disabled active pagination items.";
    public const string TokenNameItemInputBg = "Background color of pagination input controls.";
    public const string TokenNameInputOutlineOffset = "Outline offset used by pagination inputs.";
    public const string TokenNamePaginationLayoutSpacing = "Horizontal spacing in normal pagination layout.";
    public const string TokenNamePaginationLayoutMiniSpacing = "Horizontal spacing in mini pagination layout.";
    public const string TokenNamePaginationQuickJumperInputWidth = "Width of the normal quick jumper input.";
    public const string TokenNamePaginationMiniQuickJumperInputWidth = "Width of the mini quick jumper input.";
    public const string TokenNamePaginationItemPaddingInline = "Horizontal padding of pagination items.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic pagination.";
    public const string AlignTitle = "Align";
    public const string AlignDescription = "Support three alignment modes: left alignment, center alignment, right alignment.";
    public const string MoreTitle = "More";
    public const string MoreDescription = "More pages.";
    public const string MiniSizeTitle = "Mini size";
    public const string MiniSizeDescription = "Mini size pagination.";
    public const string TotalNumberTitle = "Total number";
    public const string TotalNumberDescription = "You can show the total number of data by setting showTotal.";
    public const string SimpleModeTitle = "Simple mode";
    public const string SimpleModeDescription = "Simple mode.";

    protected override Type GetResourceKindType() => typeof(PaginationShowCaseLangResourceKind);
}
