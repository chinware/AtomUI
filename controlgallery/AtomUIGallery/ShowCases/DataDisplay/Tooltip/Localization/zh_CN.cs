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
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计变量";
    public const string PageSubtitle = "在用户悬停或聚焦元素时展示上下文提示。";
    public const string PageDescription =
        "Tooltip 提供简洁的辅助说明，支持弹出位置、箭头行为以及预设或自定义颜色，适合轻量级上下文引导。";
    public const string ComponentCategory = "数据展示";
    public const string ComponentStatusStable = "稳定";
    public const string InfoNamespaceLabel = "命名空间";
    public const string InfoPackageLabel = "包";
    public const string InfoBaseClassLabel = "基类";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyTip = "Tooltip 中显示的内容。";
    public const string ApiPropertyPlacement = "相对于目标控件的首选弹出位置。";
    public const string ApiPropertyIsArrowVisible = "在弹出位置支持时显示或隐藏 Tooltip 箭头。";
    public const string ApiPropertyIsPointAtCenter = "让箭头指向目标控件中心。";
    public const string ApiPropertyPresetColor = "Tooltip 背景使用的预设颜色。";
    public const string ApiPropertyColor = "Tooltip 背景使用的自定义颜色。";
    public const string ApiPropertyShowDelay = "Tooltip 显示前的延迟时间，单位毫秒。";
    public const string ApiPropertyShowOnDisabled = "允许在禁用目标控件上显示 Tooltip。";
    public const string TokenColumnToken = "变量";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameToolTipBackground = "Tooltip 默认背景色。";
    public const string TokenNameToolTipColor = "Tooltip 默认前景色。";
    public const string TokenNameToolTipMaxWidth = "Tooltip 内容换行前的最大宽度。";
    public const string TokenNameBorderRadiusOuter = "Tooltip 表面的外层圆角。";
    public const string TokenNamePadding = "Tooltip 内容内边距。";
    public const string TokenNameMotionDuration = "Tooltip 打开和关闭动画的持续时间。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";

    protected override Type GetResourceKindType() => typeof(TooltipShowCaseLangResourceKind);
}
