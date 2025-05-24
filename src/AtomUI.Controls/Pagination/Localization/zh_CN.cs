using AtomUI.Theme;
using AtomUI.Utils;

namespace AtomUI.Controls.PaginationLang;

[LanguageProvider(LanguageCode.zh_CN, PaginationToken.ID)]
internal class zh_CN : AbstractLanguageProvider
{
    public const string JumpToText = "跳至";
    public const string PageText = "页";
    public const string TotalInfoFormat = "共 ${Total} 项";
}