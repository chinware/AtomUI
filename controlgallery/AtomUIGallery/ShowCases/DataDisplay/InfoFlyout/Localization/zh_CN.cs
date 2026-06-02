using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.InfoFlyout;

[LanguageProvider(LanguageCode.zh_CN, InfoFlyoutShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的示例。浮层大小取决于内容区域。";
    public const string TriggerWaysTitle = "三种触发方式";
    public const string TriggerWaysDescription = "通过鼠标点击、获得焦点和移入触发。";
    public const string PlacementTitle = "弹出位置";
    public const string PlacementDescription = "提供 12 种弹出位置。";
    public const string ArrowTitle = "箭头";
    public const string ArrowDescription = "支持显示、隐藏或保持箭头居中。";

    protected override Type GetResourceKindType() => typeof(InfoFlyoutShowCaseLangResourceKind);
}
