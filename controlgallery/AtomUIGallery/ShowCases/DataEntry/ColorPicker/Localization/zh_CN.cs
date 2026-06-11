using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ColorPicker;

[LanguageProvider(LanguageCode.zh_CN, ColorPickerShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "基础用法。";
    public const string TriggerSizeTitle = "触发器尺寸";
    public const string TriggerSizeDescription = "Ant Design 支持小号、默认和大号三种触发器尺寸。需要大号或小号触发器时分别设置 size 属性；省略 size 属性时使用默认尺寸。";
    public const string LineGradientTitle = "线性渐变";
    public const string LineGradientDescription = "通过 mode 将颜色设置为单色或渐变色。";
    public const string RenderingTriggerTextTitle = "渲染触发器文本";
    public const string RenderingTriggerTextDescription = "当 showText 为 true 时渲染触发器默认文本。自定义文本时，可以将 showText 作为函数返回自定义文本。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "设置为禁用状态。";
    public const string DisabledAlphaTitle = "禁用透明度";
    public const string DisabledAlphaDescription = "禁用颜色透明度。";
    public const string ClearColorTitle = "清除颜色";
    public const string ClearColorDescription = "清除颜色。";
    public const string ControlledModeTitle = "受控模式";
    public const string ControlledModeDescription = "将组件设置为受控模式。受控时会锁定显示颜色。";
    public const string CustomTriggerEventTitle = "自定义触发事件";
    public const string CustomTriggerEventDescription = "自定义颜色面板的触发事件，可选 click 和 hover。";
    public const string ColorFormatTitle = "颜色格式";
    public const string ColorFormatDescription = "编码格式，支持 HEX、HSB、RGB。";
    public const string PresetColorsTitle = "预设颜色";
    public const string PresetColorsDescription = "设置颜色选择器的预设颜色。";

    protected override Type GetResourceKindType() => typeof(ColorPickerShowCaseLangResourceKind);
}
