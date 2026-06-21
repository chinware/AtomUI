using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ColorPicker;

[LanguageProvider(LanguageCode.zh_TW, ColorPickerShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "基礎用法。";
    public const string TriggerSizeTitle = "觸發器尺寸";
    public const string TriggerSizeDescription = "Ant Design 支持小號、默認和大號三種觸發器尺寸。需要大號或小號觸發器時分別設置 size 屬性；省略 size 屬性時使用默認尺寸。";
    public const string P2LabelSizeTypeSmall = "小號";
    public const string P2LabelSizeTypeMiddle = "中號";
    public const string P2LabelSizeTypeLarge = "大號";
    public const string P2LabelSizeTypeCustom = "Custom";
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
    public const string PageSubtitle = "從緊湊觸發器中選擇純色或漸變色。";
    public const string PageDescription = "ColorPicker 支援純色與漸變值、觸發器尺寸、透明度控制、文字渲染、值同步策略、點擊或懸浮觸發，以及預設調色板。";
    public const string ComponentCategory = "資料錄入";
    public const string ComponentStatusStable = "穩定";
    public const string InfoNamespaceLabel = "命名空間";
    public const string InfoPackageLabel = "包";
    public const string InfoBaseClassLabel = "基類";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計變量";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyDefaultValue = "控件載入時應用的初始顏色或漸變。";
    public const string ApiPropertyValue = "當前選中的顏色或漸變值。";
    public const string ApiPropertyFormat = "顏色文字的顯示和輸入格式。";
    public const string ApiPropertyIsAlphaEnabled = "控制是否允許編輯透明度。";
    public const string ApiPropertyIsTextVisible = "在觸發器中顯示格式化後的顏色文字。";
    public const string ApiPropertyIsClearEnabled = "允許用戶清除當前顏色值。";
    public const string ApiPropertySizeType = "設置觸發器尺寸。";
    public const string ApiPropertyTriggerType = "控制選擇器通過點擊或懸浮打開。";
    public const string ApiPropertyValueSyncStrategy = "控制值立即同步，還是在編輯完成後提交。";
    public const string ApiPropertyIsPaletteGroupEnabled = "在選擇面板中顯示預設調色板分組。";
    public const string TokenColumnToken = "變量";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "狀態";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";
    public const string TokenNameColorPickerWidth = "顏色選擇面板寬度。";
    public const string TokenNameColorSpectrumHeight = "色譜區域高度。";
    public const string TokenNameColorPickerHandlerSize = "顏色控件默認操作手柄尺寸。";
    public const string TokenNameColorPickerSliderTrackSize = "顏色滑塊軌道尺寸。";
    public const string TokenNameColorPickerPresetColorSize = "每個預設顏色色塊尺寸。";
    public const string TokenNameColorPickerPresetPanelWidth = "預設顏色面板寬度。";
    public const string TokenNameTriggerPadding = "顏色選擇器觸發器內部間距。";
    public const string TokenNameTriggerTextMargin = "顏色塊與觸發器文字之間的外間距。";
    public const string TokenNameColorBlockDisabledOpacity = "禁用狀態下顏色塊的不透明度。";

    protected override Type GetResourceKindType() => typeof(ColorPickerShowCaseLangResourceKind);
}
