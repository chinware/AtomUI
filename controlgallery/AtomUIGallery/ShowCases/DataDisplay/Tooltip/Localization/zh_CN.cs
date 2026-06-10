using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Tooltip;

[LanguageProvider(LanguageCode.zh_CN, TooltipShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最简单的用法。";
    public const string PlacementTitle = "弹出位置";
    public const string PlacementDescription = "提供 12 种弹出位置。";
    public const string ArrowTitle = "箭头";
    public const string ArrowDescription = "支持显示、隐藏或保持箭头居中。";
    public const string ColorfulTooltipTitle = "多彩提示";
    public const string ColorfulTooltipDescription = "预设了一系列多彩提示样式，可用于不同场景。";
    public const string P2TextTooltipWillShowOnMouseEnter = "鼠标移入时显示提示。";
    public const string P2ContentShow = "显示";
    public const string P2ContentHide = "隐藏";
    public const string P2ContentCenter = "居中";
    public const string P2TextPresets = "预设";
    public const string P2TextCustom = "自定义";

    public const string P2ToolTipTipPromptText = "提示文本";

    public const string P2ContentLT = "左上";

    public const string P2ContentLeft = "左侧";

    public const string P2ContentLB = "左下";

    public const string P2ContentTL = "上左";

    public const string P2ContentTop = "上方";

    public const string P2ContentTR = "上右";

    public const string P2ContentRT = "右上";

    public const string P2ContentRight = "右侧";

    public const string P2ContentRB = "右下";

    public const string P2ContentBL = "下左";

    public const string P2ContentBottom = "下方";

    public const string P2ContentBR = "下右";

    public const string P2ContentBlue = "蓝色";

    public const string P2ContentRed = "红色";

    public const string P2ContentVolcano = "火山色";

    public const string P2ContentOrange = "橙色";

    public const string P2ContentGold = "金色";

    public const string P2ContentYellow = "黄色";

    public const string P2ContentLime = "青柠色";

    public const string P2ContentGreen = "绿色";

    public const string P2ContentCyan = "青色";

    public const string P2ContentGeekBlue = "极客蓝";

    public const string P2ContentPurple = "紫色";

    public const string P2ContentPink = "粉色";

    public const string P2ContentMagenta = "品红";

    public const string P2ContentGrey = "灰色";

    protected override Type GetResourceKindType() => typeof(TooltipShowCaseLangResourceKind);
}
