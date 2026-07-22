using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.TimePicker;

[LanguageProvider(LanguageCode.zh_TW, TimePickerShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "點擊 TimePicker 後，可以在面板中選擇或輸入時間。";
    public const string BindingTitle = "SelectedTime 綁定";
    public const string BindingDescription = "SelectedTime、RangeStartSelectedTime 與 RangeEndSelectedTime 默認與 ViewModel 雙向同步，無需顯式設置 Binding Mode=TwoWay。";
    public const string PickerDisplayTimeTitle = "彈出面板顯示時間";
    public const string PickerDisplayTimeDescription = "打開彈出面板時定位到指定時間，但不提交選中值。";
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
    public const string P2TextSelectedTime = "選中值：";
    public const string P2TextSelectedTimeRange = "選中範圍：";
    public const string P2ContentSetNoon = "設置為中午";
    public const string P2ContentSetWorkHours = "設置工作時間";
    public const string P2ContentClear = "清空";
    public const string PageSubtitle = "從彈出時間面板中選擇單個時間或時間範圍。";
    public const string PageDescription = "TimePicker 支援 12 小時和 24 小時制、尺寸變體、禁用狀態、分鐘和秒的步進選項、視覺變體、校驗狀態以及範圍選擇。";
    public const string ComponentCategory = "資料錄入";
    public const string ComponentStatusStable = "穩定";
    public const string ScenarioExamples = "示例";

}
