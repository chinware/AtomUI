using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Slider;

[LanguageProvider(LanguageCode.zh_TW, SliderShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string ScenarioExamples = "示例";
    public const string ComponentCategory = "數據錄入";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "從連續或帶刻度的軌道中選擇數值或範圍。";
    public const string PageDescription = "Slider 支持單值和範圍選擇、水平或垂直方向、按刻度吸附、格式化提示、刻度標記、包含軌道、禁用狀態和鍵盤交互。";
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "基礎滑塊。range 為 true 時顯示為雙滑塊模式；disable 為 true 時滑塊不可交互。";
    public const string MultiHandleTitle = "多點組合";
    public const string MultiHandleDescription = "範圍多個點組合。";
    public const string DisabledHandleTitle = "禁用指定滑塊";
    public const string DisabledHandleDescription = "將 disabled 設為陣列，可以單獨禁用 range 模式下特定的 handle。禁用後該 handle 會作為移動邊界，其他 handle 無法越過。";
    public const string RangeValuesBindingTitle = "RangeValues 綁定";
    public const string RangeValuesBindingDescription = "RangeValues 默認雙向綁定，拖動範圍會直接更新 ViewModel，無需顯式設置綁定模式。";
    public const string CustomizeTooltipTitle = "自定義提示";
    public const string CustomizeTooltipDescription = "使用 tooltip.formatter 格式化 Tooltip 內容；當 tooltip.formatter 為 null 時隱藏提示。";
    public const string VerticalTitle = "垂直方向";
    public const string VerticalDescription = "垂直滑塊。";
    public const string GraduatedSliderTitle = "帶刻度的滑塊";
    public const string GraduatedSliderDescription = "使用 marks 屬性標記帶刻度滑塊，使用 value 或 defaultValue 指定滑塊位置。當 included 為 false 時，不同滑塊相互獨立；當 step 為 null 時，用戶只能將滑塊拖到刻度上。";
    public const string P2TextEnabled = "Enabled:";
    public const string P2TextIncludedTrue = "included=true";
    public const string P2TextIncludedFalse = "included=false";
    public const string P2TextBoundRangeValues = "綁定範圍：";
    public const string P2ContentSetRange = "設為 35-85";
    public const string P2ContentClear = "清空";
    public const string P2ContentDisabledHandle1 = "Disabled Handle 1";
    public const string P2ContentDisabledHandle2 = "Disabled Handle 2";
    public const string P2ContentDisabledHandle3 = "Disabled Handle 3";

}
