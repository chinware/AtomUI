using AtomUI.Theme;
using AtomUI.Utils;

namespace AtomUI.Controls.DataGridLang;

[LanguageProvider(LanguageCode.zh_CN, DataGridToken.ID)]
internal class zh_CN : AbstractLanguageProvider
{
    public const string SelectAllFilterItems = "选择所有";
    public const string AscendTooltip = "点击升序";
    public const string DescendTooltip = "点击降序";
    public const string CancelTooltip = "取消排序";
}