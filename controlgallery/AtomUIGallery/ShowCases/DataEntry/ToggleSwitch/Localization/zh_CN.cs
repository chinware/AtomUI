using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ToggleSwitch;

[LanguageProvider(LanguageCode.zh_CN, ToggleSwitchShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的用法。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "Switch 的禁用状态。";
    public const string TextAndIconTitle = "文本和图标";
    public const string TextAndIconDescription = "带文本和图标。";
    public const string TwoSizesTitle = "两种尺寸";
    public const string TwoSizesDescription = "size=Small 表示小尺寸开关。";
    public const string LoadingTitle = "加载中";
    public const string LoadingDescription = "标记开关的等待状态。";

    protected override Type GetResourceKindType() => typeof(ToggleSwitchShowCaseLangResourceKind);
}
