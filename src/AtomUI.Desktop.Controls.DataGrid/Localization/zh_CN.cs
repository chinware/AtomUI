using AtomUI.Theme;
using AtomUI.Theme.Language;
using AtomUI.Utils;

namespace AtomUI.Controls.DataGridLocalization;

[LanguageProvider(LanguageCode.zh_CN, DataGridToken.ID)]
internal class zh_CN : LanguageProvider
{
    public const string SelectAllFilterItems = "选择所有";
    public const string AscendTooltip = "点击升序";
    public const string DescendTooltip = "点击降序";
    public const string CancelTooltip = "取消排序";
    public const string DeleteConfirmText = "确认删除？";
    public const string CancelConfirmText = "确认取消？";
    public const string Operating = "正在操作中，请稍后";
}