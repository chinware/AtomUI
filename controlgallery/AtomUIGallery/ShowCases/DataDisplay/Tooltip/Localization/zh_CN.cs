using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Tooltip;

[LanguageProvider(LanguageCode.zh_CN, TooltipShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最简单的用法。";
    public const string PlacementTitle = "弹出位置";
    public const string PlacementDescription = "提供 12 种弹出位置。";
    public const string ArrowTitle = "箭头";
    public const string ArrowDescription = "支持显示、隐藏或保持箭头居中。";
    public const string ColorfulTooltipTitle = "多彩提示";
    public const string ColorfulTooltipDescription = "预设了一系列多彩 Tooltip 样式，可用于不同场景。";

    protected override Type GetResourceKindType() => typeof(TooltipShowCaseLangResourceKind);
}
