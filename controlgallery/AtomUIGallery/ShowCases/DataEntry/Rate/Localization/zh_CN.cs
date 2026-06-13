using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Rate;

[LanguageProvider(LanguageCode.zh_CN, RateShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计变量";
    public const string ComponentCategory = "数据录入";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "用星级、半星、自定义字符和提示文案收集轻量评分。";
    public const string PageDescription = "Rate 用于让用户按有序等级表达偏好或质量评价，支持清除、半选、只读、键盘交互、自定义图形和本地化文案。";
    public const string InfoNamespaceLabel = "命名空间：";
    public const string InfoPackageLabel = "包：";
    public const string InfoBaseClassLabel = "基类：";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyIsAllowClear = "允许再次点击当前评分值时清除评分。";
    public const string ApiPropertyIsAllowHalf = "允许选择半星评分值。";
    public const string ApiPropertyCharacter = "每个评分项使用的自定义视觉内容。";
    public const string ApiPropertyStarColor = "覆盖已选评分项颜色。";
    public const string ApiPropertyStarBgColor = "覆盖未选评分项颜色。";
    public const string ApiPropertyCount = "评分项总数。";
    public const string ApiPropertyValue = "当前选中的评分值。";
    public const string ApiPropertyDefaultValue = "未显式设置 Value 时使用的初始评分值。";
    public const string ApiPropertyIsKeyboardEnabled = "控制是否启用键盘交互。";
    public const string ApiPropertyToolTips = "每个评分值对应的提示文本列表。";
    public const string ApiPropertySizeType = "控制小、中、大三种评分尺寸。";
    public const string ApiPropertyIsMotionEnabled = "启用或禁用评分动效。";
    public const string ApiPropertyValueChanged = "选中评分值变化时触发。";
    public const string ApiPropertyHoverValueChanged = "悬浮评分值变化时触发。";
    public const string TokenColumnToken = "变量";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "范围";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameStarColor = "已选评分项颜色。";
    public const string TokenNameStarSize = "默认评分项尺寸。";
    public const string TokenNameStarSizeSM = "小号评分项尺寸。";
    public const string TokenNameStarSizeLG = "大号评分项尺寸。";
    public const string TokenNameStarHoverScale = "评分项悬浮时应用的缩放比例。";
    public const string TokenNameStarBg = "未选评分项背景色。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最简单的用法。";
    public const string HalfStarTitle = "半星";
    public const string HalfStarDescription = "支持选择半星。";
    public const string ShowCopywritingTitle = "显示文案";
    public const string ShowCopywritingDescription = "在评分组件中添加文案。";
    public const string ReadOnlyTitle = "只读";
    public const string ReadOnlyDescription = "只读状态，不能使用鼠标交互。";
    public const string ClearStarTitle = "清除星级";
    public const string ClearStarDescription = "支持设置再次点击时允许清除星级。";
    public const string OtherCharacterTitle = "其他字符";
    public const string OtherCharacterDescription = "将默认星形替换为其他字符，例如字母、数字、图标字体，甚至中文文字。";
    public const string P2TextIsallowclearTrue = "允许清除：true";
    public const string P2TextIsallowclearFalse = "允许清除：false";
    public const string P2TooltipTerrible = "糟糕";
    public const string P2TooltipBad = "不好";
    public const string P2TooltipNormal = "一般";
    public const string P2TooltipGood = "好";
    public const string P2TooltipWonderful = "很棒";

    protected override Type GetResourceKindType() => typeof(RateShowCaseLangResourceKind);
}
