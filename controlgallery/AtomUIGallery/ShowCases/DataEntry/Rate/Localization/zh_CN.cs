using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Rate;

[LanguageProvider(LanguageCode.zh_CN, RateShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string ScenarioExamples = "示例";
    public const string ComponentCategory = "数据录入";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "用星级、半星、自定义字符和提示文案收集轻量评分。";
    public const string PageDescription = "Rate 用于让用户按有序等级表达偏好或质量评价，支持清除、半选、只读、键盘交互、自定义图形和本地化文案。";
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最简单的用法。";
    public const string TwoWayBindingTitle = "双向绑定";
    public const string TwoWayBindingDescription = "Value 默认使用 TwoWay 绑定，用户评分和 ViewModel 更新会保持同步。";
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
    public const string P2ContentSetFourStars = "设为 4 星";
    public const string P2ContentClear = "清空";
    public const string P2TwoWayValueSummaryFormat = "当前评分：{0:0.#}";
    public const string P2TooltipTerrible = "糟糕";
    public const string P2TooltipBad = "不好";
    public const string P2TooltipNormal = "一般";
    public const string P2TooltipGood = "好";
    public const string P2TooltipWonderful = "很棒";

}
