using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ColorPicker;

[LanguageProvider(LanguageCode.zh_TW, ColorPickerShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "基礎用法。";
    public const string TriggerSizeTitle = "觸發器尺寸";
    public const string TriggerSizeDescription = "Ant Design 支持小號、默認和大號三種觸發器尺寸。需要大號或小號觸發器時分別設置 size 屬性；省略 size 屬性時使用默認尺寸。";
    public const string LineGradientTitle = "線性漸變";
    public const string LineGradientDescription = "通過 mode 將顏色設置為單色或漸變色。";
    public const string RenderingTriggerTextTitle = "渲染觸發器文本";
    public const string RenderingTriggerTextDescription = "當 showText 為 true 時渲染觸發器默認文本。自定義文本時，可以將 showText 作為函數返回自定義文本。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "設置為禁用狀態。";
    public const string DisabledAlphaTitle = "禁用透明度";
    public const string DisabledAlphaDescription = "禁用顏色透明度。";
    public const string ClearColorTitle = "清除顏色";
    public const string ClearColorDescription = "清除顏色。";
    public const string ControlledModeTitle = "受控模式";
    public const string ControlledModeDescription = "將組件設置為受控模式。受控時會鎖定顯示顏色。";
    public const string CustomTriggerEventTitle = "自定義觸發事件";
    public const string CustomTriggerEventDescription = "自定義顏色面板的觸發事件，可選 click 和 hover。";
    public const string ColorFormatTitle = "顏色格式";
    public const string ColorFormatDescription = "編碼格式，支持 HEX、HSB、RGB。";
    public const string PresetColorsTitle = "預設顏色";
    public const string PresetColorsDescription = "設置顏色選擇器的預設顏色。";

    protected override Type GetResourceKindType() => typeof(ColorPickerShowCaseLangResourceKind);
}

