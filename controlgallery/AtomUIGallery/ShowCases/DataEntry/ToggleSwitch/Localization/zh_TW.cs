using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ToggleSwitch;

[LanguageProvider(LanguageCode.zh_TW, ToggleSwitchShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最基礎的用法。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "Switch 的禁用狀態。";
    public const string TextAndIconTitle = "文本和圖標";
    public const string TextAndIconDescription = "帶文本和圖標。";
    public const string TwoSizesTitle = "兩種尺寸";
    public const string TwoSizesDescription = "size=Small 表示小尺寸開關。";
    public const string LoadingTitle = "加載中";
    public const string LoadingDescription = "標記開關的等待狀態。";
    public const string P2ContentToggleDisabled = "切換禁用";
    public const string P2ContentToggleLoading = "切換加載";

    public const string P2OnContentOn = "開";

    public const string P2OffContentOff = "關";

    public const string P2OnContentText = "開";

    public const string P2OffContentText = "關";

    protected override Type GetResourceKindType() => typeof(ToggleSwitchShowCaseLangResourceKind);
}

