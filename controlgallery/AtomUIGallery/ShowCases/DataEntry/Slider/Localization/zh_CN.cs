using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Slider;

[LanguageProvider(LanguageCode.zh_CN, SliderShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "基础滑块。range 为 true 时显示为双滑块模式；disable 为 true 时滑块不可交互。";
    public const string CustomizeTooltipTitle = "自定义提示";
    public const string CustomizeTooltipDescription = "使用 tooltip.formatter 格式化 Tooltip 内容；当 tooltip.formatter 为 null 时隐藏提示。";
    public const string VerticalTitle = "垂直方向";
    public const string VerticalDescription = "垂直滑块。";
    public const string GraduatedSliderTitle = "带刻度的滑块";
    public const string GraduatedSliderDescription = "使用 marks 属性标记带刻度滑块，使用 value 或 defaultValue 指定滑块位置。当 included 为 false 时，不同滑块相互独立；当 step 为 null 时，用户只能将滑块拖到刻度上。";
    public const string P2TextEnabled = "Enabled:";
    public const string P2TextIncludedTrue = "included=true";
    public const string P2TextIncludedFalse = "included=false";

    protected override Type GetResourceKindType() => typeof(SliderShowCaseLangResourceKind);
}
