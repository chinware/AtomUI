using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.DatePicker;

[LanguageProvider(LanguageCode.zh_TW, DatePickerShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "點擊 DatePicker 後，可以在面板中選擇或輸入日期。";
    public const string BindingTitle = "SelectedDateTime 綁定";
    public const string BindingDescription = "SelectedDateTime、RangeStartSelectedDate 與 RangeEndSelectedDate 默認與 ViewModel 雙向同步，無需顯式設置 Binding Mode=TwoWay。";
    public const string PickerDisplayDateTitle = "彈出面板顯示日期";
    public const string PickerDisplayDateDescription = "打開彈出面板時定位到指定日期，但不提交選中值。";
    public const string RangePickerTitle = "範圍選擇器";
    public const string RangePickerDescription = "通過 picker 屬性設置範圍選擇器類型。";
    public const string NeedConfirmTitle = "需要確認";
    public const string NeedConfirmDescription = "DatePicker 會根據 picker 屬性自動判斷是否顯示確認按鈕，也可以通過 needConfirm 屬性控制。設置 needConfirm 後，用戶必須點擊確認按鈕完成選擇；否則在選擇器失去焦點或選擇日期時提交選擇。";
    public const string ChooseTimeTitle = "選擇時間";
    public const string ChooseTimeDescription = "該屬性提供額外的時間選擇。當 showTime 為對象時，其屬性會傳遞給內置 TimePicker。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "DatePicker 的禁用狀態。也可以設置為數組來禁用其中一個輸入框。";
    public const string ThreeSizesTitle = "三種尺寸";
    public const string ThreeSizesDescription = "輸入框提供小號、中號和大號三種尺寸。省略 size 時使用中號。";
    public const string StatusTitle = "狀態";
    public const string StatusDescription = "通過 status 為 DatePicker 添加狀態，可設置為錯誤或警告。";
    public const string VariantsTitle = "變體";
    public const string VariantsDescription = "無邊框風格組件。";
    public const string PlacementTitle = "彈出位置";
    public const string PlacementDescription = "可以通過 placement 手動指定彈出層位置。";
    public const string P2PlaceholderTextSelectDate = "選擇日期";
    public const string P2PlaceholderTextSelectWeek = "選擇週";
    public const string P2PlaceholderTextSelectMonth = "選擇月份";
    public const string P2PlaceholderTextSelectQuarter = "選擇季度";
    public const string P2PlaceholderTextSelectYear = "選擇年份";
    public const string P2SecondaryPlaceholderTextEndDate = "結束日期";
    public const string P2PlaceholderTextSelectTime = "選擇時間";
    public const string P2PlaceholderTextStartDate = "開始日期";
    public const string P2PlaceholderTextStartWeek = "開始週";
    public const string P2SecondaryPlaceholderTextEndWeek = "結束週";
    public const string P2PlaceholderTextStartMonth = "開始月份";
    public const string P2SecondaryPlaceholderTextEndMonth = "結束月份";
    public const string P2PlaceholderTextStartQuarter = "開始季度";
    public const string P2SecondaryPlaceholderTextEndQuarter = "結束季度";
    public const string P2PlaceholderTextStartYear = "開始年份";
    public const string P2SecondaryPlaceholderTextEndYear = "結束年份";
    public const string P2PlaceholderTextOutline = "描邊風格";
    public const string P2PlaceholderTextFilled = "填充風格";
    public const string P2PlaceholderTextBorderless = "無邊框";
    public const string P2TextExpandDirection = "選擇器尺寸：";
    public const string P2ContentLarge = "大號";
    public const string P2ContentDefault = "默認";
    public const string P2ContentSmall = "小號";
    public const string P2ContentCustom = "自定義";
    public const string P2TextPlacement = "彈出位置：";
    public const string P2TextSelectedDateTime = "選中值：";
    public const string P2TextSelectedDateRange = "選中範圍：";
    public const string P2ContentSetTomorrow = "設置為明天";
    public const string P2ContentSetThisWeek = "設置為本週";
    public const string P2ContentClear = "清空";
    public const string P2ContentTopleft = "左上";
    public const string P2ContentTopright = "右上";
    public const string P2ContentBottomleft = "左下";
    public const string P2ContentBottomright = "右下";
    public const string PageSubtitle = "從日曆面板中選擇日期、日期範圍和可選時間。";
    public const string PageDescription = "DatePicker 支援單選與範圍選擇、確認流程、時間選擇、禁用狀態、尺寸變體、校驗狀態、視覺變體和自定義彈出位置。";
    public const string ComponentCategory = "資料錄入";
    public const string ComponentStatusStable = "穩定";
    public const string ScenarioExamples = "示例";

}
