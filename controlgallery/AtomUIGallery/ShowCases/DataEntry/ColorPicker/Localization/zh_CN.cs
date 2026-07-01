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
    public const string P2LabelSizeTypeSmall = "小号";
    public const string P2LabelSizeTypeMiddle = "中号";
    public const string P2LabelSizeTypeLarge = "大号";
    public const string P2LabelSizeTypeCustom = "Custom";
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
    public const string PageSubtitle = "从紧凑触发器中选择纯色或渐变色。";
    public const string PageDescription = "ColorPicker 支持纯色与渐变值、触发器尺寸、透明度控制、文本渲染、值同步策略、点击或悬浮触发，以及预设调色板。";
    public const string ComponentCategory = "数据录入";
    public const string ComponentStatusStable = "稳定";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计变量";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyDefaultValue = "控件加载时应用的初始颜色或渐变。";
    public const string ApiPropertyValue = "当前选中的颜色或渐变值。";
    public const string ApiPropertyFormat = "颜色文本的显示和输入格式。";
    public const string ApiPropertyIsAlphaEnabled = "控制是否允许编辑透明度。";
    public const string ApiPropertyIsTextVisible = "在触发器中显示格式化后的颜色文本。";
    public const string ApiPropertyIsClearEnabled = "允许用户清除当前颜色值。";
    public const string ApiPropertySizeType = "设置触发器尺寸。";
    public const string ApiPropertyTriggerType = "控制选择器通过点击或悬浮打开。";
    public const string ApiPropertyValueSyncStrategy = "控制值立即同步，还是在编辑完成后提交。";
    public const string ApiPropertyIsPaletteGroupEnabled = "在选择面板中显示预设调色板分组。";
    public const string TokenColumnToken = "变量";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "状态";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";
    public const string TokenNameColorPickerWidth = "颜色选择面板宽度。";
    public const string TokenNameColorSpectrumHeight = "色谱区域高度。";
    public const string TokenNameColorPickerHandlerSize = "颜色控件默认操作手柄尺寸。";
    public const string TokenNameColorPickerSliderTrackSize = "颜色滑块轨道尺寸。";
    public const string TokenNameColorPickerPresetColorSize = "每个预设颜色色块尺寸。";
    public const string TokenNameColorPickerPresetPanelWidth = "预设颜色面板宽度。";
    public const string TokenNameTriggerPadding = "颜色选择器触发器内部间距。";
    public const string TokenNameTriggerTextMargin = "颜色块与触发器文本之间的外间距。";
    public const string TokenNameColorBlockDisabledOpacity = "禁用状态下颜色块的不透明度。";

    protected override Type GetResourceKindType() => typeof(ColorPickerShowCaseLangResourceKind);
}
