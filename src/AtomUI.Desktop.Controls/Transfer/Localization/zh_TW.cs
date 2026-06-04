using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.TransferLang;

[LanguageProvider(LanguageCode.zh_TW, TransferToken.ID)]
internal class zh_TW : LanguageProvider
{
    public const string Item = "項";
    public const string Items = "項";
    public const string SelectAll = "全選所有";
    public const string DeSelectAll = "取消全選";
    public const string RemoveCurrentPage = "刪除當頁";
    public const string RemoveAll = "刪除所有";
    public const string InvertSelectCurrentPage = "反選當頁";
    public const string SelectCurrentPage = "選擇當頁";
    
    protected override Type GetResourceKindType() => typeof(TransferLangResourceKind);
}