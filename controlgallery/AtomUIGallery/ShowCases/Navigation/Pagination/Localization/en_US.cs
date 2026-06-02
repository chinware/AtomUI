using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Pagination;

[LanguageProvider(LanguageCode.en_US, PaginationShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
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
