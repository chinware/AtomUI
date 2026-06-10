using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ToggleSwitch;

[LanguageProvider(LanguageCode.zh_CN, ToggleSwitchShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
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
    public const string P2ContentToggleDisabled = "切换禁用";
    public const string P2ContentToggleLoading = "切换加载";

    public const string P2OnContentOn = "开";

    public const string P2OffContentOff = "关";

    public const string P2OnContentText = "开";

    public const string P2OffContentText = "关";

    protected override Type GetResourceKindType() => typeof(ToggleSwitchShowCaseLangResourceKind);
}
