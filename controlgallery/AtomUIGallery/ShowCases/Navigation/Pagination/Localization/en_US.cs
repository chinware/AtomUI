using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Pagination;

[LanguageProvider(LanguageCode.en_US, PaginationShowCase.LanguageId)]
internal partial class en_US
{
    public const string ScenarioExamples = "Examples";
    public const string ComponentCategory = "Navigation";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "Navigate long lists with page numbers, sizes, totals, and quick jumps.";
    public const string PageDescription = "Pagination divides large data sets into predictable pages. It supports alignment, page size selection, quick jump input, total information, mini size, and simple read-only or editable modes.";
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic pagination.";
    public const string BindingTitle = "Controlled binding";
    public const string BindingDescription = "CurrentPage and PageSize can be bound without an explicit Mode=TwoWay; page clicks and size changes write back to the view model.";
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
    public const string P2TextCurrentPage = "CurrentPage:";
    public const string P2TextPageSize = "PageSize:";
    public const string P2ContentSetPage = "Set page 5 / 20";
    public const string P2ContentReset = "Reset";

}
