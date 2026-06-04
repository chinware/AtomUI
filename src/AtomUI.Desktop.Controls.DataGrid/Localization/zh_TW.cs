using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.DataGridLocalization;

[LanguageProvider(LanguageCode.zh_TW, DataGridToken.ID)]
internal class zh_TW : LanguageProvider
{
    public const string SelectAllFilterItems = "選擇所有";
    public const string AscendTooltip = "點擊升序";
    public const string DescendTooltip = "點擊降序";
    public const string CancelTooltip = "取消排序";
    public const string DeleteConfirmText = "確認刪除？";
    public const string CancelConfirmText = "確認取消？";
    public const string Operating = "正在操作中，請稍後";
    protected override Type GetResourceKindType() => typeof(DataGridLangResourceKind);
}