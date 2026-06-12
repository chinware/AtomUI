using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Calendar;

[LanguageProvider(LanguageCode.zh_TW, CalendarShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string PageSubtitle = "在月曆面板中選擇日期。";
    public const string PageDescription = "Calendar 以月、年、十年檢視呈現日期，並支援單選、範圍選擇和多範圍選擇。";
    public const string ComponentCategory = "資料展示";
    public const string ComponentStatusStable = "穩定";
    public const string InfoNamespaceLabel = "命名空間";
    public const string InfoPackageLabel = "套件";
    public const string InfoBaseClassLabel = "基底類別";
    public const string ScenarioExamples = "範例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計變數";

    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "Calendar 的最簡單用法。";

    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "預設值";
    public const string ApiPropertyFirstDayOfWeek = "設定每週的起始星期。";
    public const string ApiPropertyIsTodayHighlighted = "控制是否在月檢視中突顯今天。";
    public const string ApiPropertyHeaderBackground = "設定日曆標頭背景筆刷。";
    public const string ApiPropertyDisplayMode = "控制日曆顯示月、年或十年內容。";
    public const string ApiPropertySelectionMode = "控制使用者可單選日期、選擇單個範圍、多個範圍或禁止選擇。";
    public const string ApiPropertySelectedDate = "取得或設定主要選取日期。";
    public const string ApiPropertyDisplayDate = "取得或設定日曆目前顯示的日期。";
    public const string ApiPropertyDisplayDateStart = "設定可顯示的最早日期。";
    public const string ApiPropertyDisplayDateEnd = "設定可顯示的最晚日期。";
    public const string ApiPropertyIsMotionEnabled = "控制日曆切換動效是否啟用。";

    public const string TokenColumnToken = "變數";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameCellHoverBg = "單元格懸浮態背景色。";
    public const string TokenNameCellActiveWithRangeBg = "選取範圍內單元格背景色。";
    public const string TokenNameCellHoverWithRangeBg = "選取範圍內單元格懸浮態背景色。";
    public const string TokenNameCellBgDisabled = "停用單元格背景色。";
    public const string TokenNameCellRangeBorderColor = "範圍選擇單元格邊框色。";
    public const string TokenNameCellHeight = "日曆單元格高度。";
    public const string TokenNameCellWidth = "日曆單元格寬度。";
    public const string TokenNameCellLineHeight = "日曆單元格文字行高。";
    public const string TokenNamePanelContentPadding = "日曆面板內容內邊距。";
    public const string TokenNameItemPanelMinWidth = "日曆項面板最小寬度。";
    public const string TokenNameItemPanelMinHeight = "日曆項面板最小高度。";
    public const string TokenScopeComponent = "元件";
    public const string TokenStatusStable = "穩定";

    protected override Type GetResourceKindType() => typeof(CalendarShowCaseLangResourceKind);
}
