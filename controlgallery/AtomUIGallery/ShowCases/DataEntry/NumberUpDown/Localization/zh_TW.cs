using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.NumberUpDown;

[LanguageProvider(LanguageCode.zh_TW, NumberUpDownShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string ScenarioBasic = "基礎";
    public const string ScenarioRange = "範圍";
    public const string ScenarioStyle = "樣式";
    public const string ScenarioAddon = "附加內容";

    public const string BasicUsageTitle = "基礎用法";
    public const string BasicUsageDescription = "僅支持數字輸入的 NumberUpDown。";
    public const string SpinnerModeTitle = "撥輪";
    public const string SpinnerModeDescription = "數字撥輪。";
    public const string HideHandleTitle = "隱藏步進按鈕";
    public const string HideHandleDescription = "通過 ShowButtonSpinner=\"False\" 隱藏步進按鈕。";
    public const string StringModeTitle = "字符串模式（高精度）";
    public const string StringModeDescription = "以字符串形式保留高精度值。";
    public const string KeyboardBehaviorTitle = "鍵盤行為";
    public const string KeyboardBehaviorDescription = "通過 Keyboard 屬性禁用鍵盤步進。";
    public const string MouseWheelBehaviorTitle = "鼠標滾輪行為";
    public const string MouseWheelBehaviorDescription = "輸入框獲得焦點時滾動鼠標滾輪，可按 Increment 增減值。";
    public const string MinMaxTitle = "最小/最大值";
    public const string MinMaxDescription = "限制輸入值範圍。";
    public const string DecimalStepTitle = "小數步長";
    public const string DecimalStepDescription = "通過 Increment 使用小數步長。";
    public const string SizesTitle = "NumberUpDown 尺寸";
    public const string SizesDescription = "NumberUpDown 支持大號（40px）、默認（32px）、小號（24px）和自定義尺寸。";
    public const string VariantsTitle = "變體";
    public const string VariantsDescription = "NumberUpDown 的變體。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "NumberUpDown 禁用樣式的變體。";
    public const string PrePostTabTitle = "前置/後置標籤";
    public const string PrePostTabDescription = "使用前置和後置標籤的示例。";
    public const string WithClearIconTitle = "帶清除圖標";
    public const string WithClearIconDescription = "帶移除圖標的輸入框，點擊圖標可清空全部內容。";
    public const string PrefixAndSuffixTitle = "前綴和後綴";
    public const string PrefixAndSuffixDescription = "在輸入框內部添加前綴或後綴圖標。";
    public const string StatusTitle = "狀態";
    public const string StatusDescription = "通過 status 為 Input 添加狀態，可設置為錯誤或警告。";
    public const string P2PlaceholderTextInputWeight = "輸入重量";
    public const string P2PlaceholderTextKeyboardDisabled = "鍵盤已禁用";
    public const string P2PlaceholderTextFocusAndScrollWheel = "聚焦後滾動鼠標滾輪";
    public const string P2PlaceholderTextInputWithClearIcon = "帶清除圖標的輸入框";
    public const string P2PlaceholderTextEnterYourValue = "輸入數值";
    public const string P2PlaceholderTextError = "錯誤";
    public const string P2PlaceholderTextWarning = "警告";
    public const string P2PlaceholderTextErrorWithPrefix = "帶前綴的錯誤";
    public const string P2PlaceholderTextWarningWithPrefix = "帶前綴的警告";
    public const string P2TextRawValuePrefix = "原始值：";

    public const string P2ContentKeyboardEnabled = "啓用鍵盤";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計 Token";
    public const string PageSubtitle = "輸入並通過鍵盤、滾輪和步進按鈕調整数值。";
    public const string PageDescription =
        "NumberUpDown 組合了數值輸入、高精度字符串模式、最小/最大約束、小數步長、輸入變體、附加內容、清除入口和校驗狀態。";
    public const string ComponentCategory = "數據錄入";
    public const string ComponentStatusStable = "穩定";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyValue = "當前數值。";
    public const string ApiPropertyMinimum = "允許輸入的最小數值。";
    public const string ApiPropertyMaximum = "允許輸入的最大數值。";
    public const string ApiPropertyIncrement = "通過步進按鈕、鍵盤或滾輪增減的數值。";
    public const string ApiPropertyFormatString = "用於顯示數值的格式字符串。";
    public const string ApiPropertyMode = "控件展示模式，可使用默認輸入框模式或三段式撥輪模式。";
    public const string ApiPropertyIsStringMode = "以文本保留高精度輸入，同時保持數值編輯行為。";
    public const string ApiPropertyStringValue = "高精度字符串模式使用的字符串值。";
    public const string ApiPropertyIsKeyboardEnabled = "允許 Up、Down、PageUp、PageDown 等鍵盤步進快捷鍵。";
    public const string ApiPropertyIsAllowClear = "當輸入有內容時顯示清除入口。";
    public const string ApiPropertyClearIcon = "清除入口使用的自定義圖標。";
    public const string ApiPropertySizeType = "輸入框尺寸變體。";
    public const string ApiPropertyStyleVariant = "輸入框視覺變體，例如描邊、填充或無邊框。";
    public const string ApiPropertyStatus = "輸入框表面顯示的校驗狀態。";
    public const string ApiPropertyLeftAddOn = "附加在輸入框框架前方的內容。";
    public const string ApiPropertyRightAddOn = "附加在輸入框框架後方的內容。";
    public const string ApiPropertyInnerLeftContent = "渲染在輸入框內部左側的內容。";
    public const string ApiPropertyInnerRightContent = "渲染在輸入框內部右側的內容。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameControlWidth = "數值輸入框默認寬度。";
    public const string TokenNameHandleWidth = "步進按鈕區域寬度。";
    public const string TokenNameHandleIconSize = "步進按鈕使用的圖標尺寸。";
    public const string TokenNameHandleBg = "步進按鈕區域背景色。";
    public const string TokenNameHandleActiveBg = "步進按鈕區域激活背景色。";
    public const string TokenNameHandleHoverColor = "步進按鈕懸浮時的前景色。";
    public const string TokenNameHandleBorderColor = "步進按鈕區域邊框色。";
    public const string TokenNameFilledHandleBg = "填充變體使用的步進按鈕背景色。";
    public const string TokenScopeComponent = "NumericUpDown";
    public const string TokenStatusStable = "穩定";

}
