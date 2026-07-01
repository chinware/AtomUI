using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TimePicker;

[LanguageProvider(LanguageCode.zh_TW, TimePickerShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "點擊 TimePicker 後，可以在面板中選擇或輸入時間。";
    public const string HourFormatsTitle = "12 小時和 24 小時格式";
    public const string HourFormatsDescription = "TimePicker 支持 12 小時和 24 小時兩種時間格式。";
    public const string ThreeSizesTitle = "三種尺寸";
    public const string ThreeSizesDescription = "輸入框提供大號、中號和小號三種尺寸。大號用於表單，中號為默認尺寸。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "TimePicker 的禁用狀態。";
    public const string IntervalOptionTitle = "間隔選項";
    public const string IntervalOptionDescription = "通過 MinuteIncrement 和 SecondIncrement 顯示步進選項。";
    public const string TwelveHoursTitle = "12 小時制";
    public const string TwelveHoursDescription = "12 小時格式的 TimePicker，默認格式為 h:mm:ss a。";
    public const string VariantsTitle = "變體";
    public const string VariantsDescription = "無邊框風格組件。";
    public const string StatusTitle = "狀態";
    public const string StatusDescription = "通過 status 為 TimePicker 添加狀態，可設置為錯誤或警告。";
    public const string TimeRangePickerTitle = "時間範圍選擇器";
    public const string TimeRangePickerDescription = "使用 RangeTimePicker 進行時間範圍選擇。";
    public const string P2PlaceholderTextSelectTime = "選擇時間";
    public const string P2PlaceholderTextOutline = "描邊風格";
    public const string P2PlaceholderTextFilled = "填充風格";
    public const string P2PlaceholderTextBorderless = "無邊框";
    public const string P2PlaceholderTextStartTime = "開始時間";
    public const string P2SecondaryPlaceholderTextEndTime = "結束時間";
    public const string P2TextExpandDirection = "選擇器尺寸：";
    public const string P2ContentLarge = "大號";
    public const string P2ContentDefault = "默認";
    public const string P2ContentSmall = "小號";
    public const string P2ContentCustom = "自定義";
    public const string PageSubtitle = "從彈出時間面板中選擇單個時間或時間範圍。";
    public const string PageDescription = "TimePicker 支援 12 小時和 24 小時制、尺寸變體、禁用狀態、分鐘和秒的步進選項、視覺變體、校驗狀態以及範圍選擇。";
    public const string ComponentCategory = "資料錄入";
    public const string ComponentStatusStable = "穩定";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計變數";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertySelectedTime = "TimePicker 當前選中的時間值。";
    public const string ApiPropertyDefaultTime = "TimePicker 重置行為使用的初始時間。";
    public const string ApiPropertyIsNeedConfirm = "要求使用者確認所選時間後再提交。";
    public const string ApiPropertyIsShowNow = "是否在彈出面板中顯示「此刻」快捷操作。";
    public const string ApiPropertyMinuteIncrement = "生成分鐘選項時使用的步進值。";
    public const string ApiPropertySecondIncrement = "生成秒選項時使用的步進值。";
    public const string ApiPropertyClockIdentifier = "選擇 12 小時或 24 小時時鐘顯示。";
    public const string ApiPropertyRangeStartSelectedTime = "RangeTimePicker 當前選中的開始時間。";
    public const string ApiPropertyRangeEndSelectedTime = "RangeTimePicker 當前選中的結束時間。";
    public const string ApiPropertyRangeStartDefaultTime = "RangeTimePicker 重置行為使用的初始開始時間。";
    public const string ApiPropertyRangeEndDefaultTime = "RangeTimePicker 重置行為使用的初始結束時間。";
    public const string TokenColumnToken = "變數";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "狀態";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";
    public const string TokenNameItemHeight = "每個可選時間項的高度。";
    public const string TokenNameItemWidth = "小時、分鐘和秒列的寬度。";
    public const string TokenNamePeriodHostWidth = "上午/下午選擇列的寬度。";
    public const string TokenNameItemPadding = "每個可選時間項的內邊距。";
    public const string TokenNameButtonsMargin = "彈出操作按鈕區域的頂部外邊距。";
    public const string TokenNameRangePickerArrowMargin = "範圍輸入之間箭頭的外邊距。";
    public const string TokenNameRangePickerIndicatorThickness = "範圍選擇指示器的厚度。";
    public const string TokenNameHeaderMargin = "時間面板頭部下方的外邊距。";

    protected override Type GetResourceKindType() => typeof(TimePickerShowCaseLangResourceKind);
}
