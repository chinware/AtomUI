using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.PaginationLang;

[LanguageProvider(LanguageCode.zh_TW, PaginationToken.ID)]
internal class zh_TW : LanguageProvider
{
    public zh_TW()
        : base(LanguageCode.zh_TW, PaginationToken.ID)
    {
    }

    public const string JumpToText = "跳至";
    public const string PageText = "頁";
    public const string TotalInfoFormat = "共 ${Total} 項";
    
    protected override Type GetResourceKindType() => typeof(PaginationLangResourceKind);
}