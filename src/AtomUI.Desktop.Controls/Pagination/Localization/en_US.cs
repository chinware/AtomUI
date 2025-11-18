using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.PaginationLang;

[LanguageProvider(LanguageCode.en_US, PaginationToken.ID)]
internal class en_US : LanguageProvider
{
    public const string JumpToText = "Go to";
    public const string PageText = "Page";
    public const string TotalInfoFormat = "Total ${Total} items";
}