using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.NumberUpDown;

[LanguageProvider(LanguageCode.zh_TW, NumberUpDownShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string ScenarioBasic = "基礎";
    public const string ScenarioRange = "範圍";
    public const string ScenarioStyle = "樣式";
    public const string ScenarioAddon = "附加內容";

    public const string BasicUsageTitle = "基礎用法";
    public const string BasicUsageDescription = "僅支持數字輸入的 NumberUpDown。";
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
    public const string SizesTitle = "NumberUpDown 三種尺寸";
    public const string SizesDescription = "輸入框有三種尺寸：大號（40px）、默認（32px）和小號（24px）。";
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

    protected override Type GetResourceKindType() => typeof(NumberUpDownShowCaseLangResourceKind);
}

