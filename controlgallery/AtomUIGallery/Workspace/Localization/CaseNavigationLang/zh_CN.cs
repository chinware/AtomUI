using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using AtomUIGallery.Workspace.Views;

namespace AtomUIGallery.Workspace.Localization.CaseNavigationLang;

[LanguageProvider(LanguageCode.zh_CN, CaseNavigation.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string General = "通用";
    public const string General_AboutUs = "关于我们";
    public const string General_Palette = "调色板";
    public const string Layout = "布局";
    public const string Navigation = "导航";
    public const string DataEntry = "数据录入";
    public const string DataDisplay = "数据展示";
    public const string Feedback = "反馈";

    protected override Type GetResourceKindType() => typeof(CaseNavigationLangResourceKind);
}
