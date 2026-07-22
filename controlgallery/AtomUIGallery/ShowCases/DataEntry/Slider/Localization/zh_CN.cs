using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Slider;

[LanguageProvider(LanguageCode.zh_CN, SliderShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string ScenarioExamples = "示例";
    public const string ComponentCategory = "数据录入";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "从连续或带刻度的轨道中选择数值或范围。";
    public const string PageDescription = "Slider 支持单值和范围选择、水平或垂直方向、按刻度吸附、格式化提示、刻度标记、包含轨道、禁用状态和键盘交互。";
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "基础滑块。range 为 true 时显示为双滑块模式；disable 为 true 时滑块不可交互。";
    public const string RangeValueBindingTitle = "RangeValue 绑定";
    public const string RangeValueBindingDescription = "RangeValue 现在默认双向绑定，拖动范围会直接更新 ViewModel，无需显式设置绑定模式。";
    public const string CustomizeTooltipTitle = "自定义提示";
    public const string CustomizeTooltipDescription = "使用 tooltip.formatter 格式化 Tooltip 内容；当 tooltip.formatter 为 null 时隐藏提示。";
    public const string VerticalTitle = "垂直方向";
    public const string VerticalDescription = "垂直滑块。";
    public const string GraduatedSliderTitle = "带刻度的滑块";
    public const string GraduatedSliderDescription = "使用 marks 属性标记带刻度滑块，使用 value 或 defaultValue 指定滑块位置。当 included 为 false 时，不同滑块相互独立；当 step 为 null 时，用户只能将滑块拖到刻度上。";
    public const string P2TextEnabled = "Enabled:";
    public const string P2TextIncludedTrue = "included=true";
    public const string P2TextIncludedFalse = "included=false";
    public const string P2TextBoundRangeValue = "绑定范围：";
    public const string P2ContentSetRange = "设为 35-85";
    public const string P2ContentClear = "清空";

}
