using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Tooltip;

[LanguageProvider(LanguageCode.zh_TW, TooltipShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最簡單的用法。";
    public const string PlacementTitle = "彈出位置";
    public const string PlacementDescription = "提供 12 種彈出位置。";
    public const string ArrowTitle = "箭頭";
    public const string ArrowDescription = "支持顯示、隱藏或保持箭頭居中。";
    public const string ColorfulTooltipTitle = "多彩提示";
    public const string ColorfulTooltipDescription = "預設了一系列多彩提示樣式，可用於不同場景。";
    public const string P2TextTooltipWillShowOnMouseEnter = "鼠標移入時顯示提示。";
    public const string P2ContentShow = "顯示";
    public const string P2ContentHide = "隱藏";
    public const string P2ContentCenter = "居中";
    public const string P2TextPresets = "預設";
    public const string P2TextCustom = "自定義";

    public const string P2ToolTipTipPromptText = "提示文本";

    public const string P2ContentLT = "左上";

    public const string P2ContentLeft = "左側";

    public const string P2ContentLB = "左下";

    public const string P2ContentTL = "上左";

    public const string P2ContentTop = "上方";

    public const string P2ContentTR = "上右";

    public const string P2ContentRT = "右上";

    public const string P2ContentRight = "右側";

    public const string P2ContentRB = "右下";

    public const string P2ContentBL = "下左";

    public const string P2ContentBottom = "下方";

    public const string P2ContentBR = "下右";

    public const string P2ContentBlue = "藍色";

    public const string P2ContentRed = "紅色";

    public const string P2ContentVolcano = "火山色";

    public const string P2ContentOrange = "橙色";

    public const string P2ContentGold = "金色";

    public const string P2ContentYellow = "黃色";

    public const string P2ContentLime = "青檸色";

    public const string P2ContentGreen = "綠色";

    public const string P2ContentCyan = "青色";

    public const string P2ContentGeekBlue = "極客藍";

    public const string P2ContentPurple = "紫色";

    public const string P2ContentPink = "粉色";

    public const string P2ContentMagenta = "品紅";

    public const string P2ContentGrey = "灰色";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計變量";
    public const string PageSubtitle = "在用戶懸停或聚焦元素時展示上下文提示。";
    public const string PageDescription =
        "Tooltip 提供簡潔的輔助說明，支持彈出位置、箭頭行爲以及預設或自定義顏色，適合輕量級上下文引導。";
    public const string ComponentCategory = "數據展示";
    public const string ComponentStatusStable = "穩定";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyTip = "Tooltip 中顯示的內容。";
    public const string ApiPropertyPlacement = "相對於目標控件的首選彈出位置。";
    public const string ApiPropertyIsArrowVisible = "在彈出位置支持時顯示或隱藏 Tooltip 箭頭。";
    public const string ApiPropertyIsPointAtCenter = "讓箭頭指向目標控件中心。";
    public const string ApiPropertyPresetColor = "Tooltip 背景使用的預設顏色。";
    public const string ApiPropertyColor = "Tooltip 背景使用的自定義顏色。";
    public const string ApiPropertyShowDelay = "Tooltip 顯示前的延遲時間，單位毫秒。";
    public const string ApiPropertyShowOnDisabled = "允許在禁用目標控件上顯示 Tooltip。";
    public const string TokenColumnToken = "變量";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameToolTipBackground = "Tooltip 默認背景色。";
    public const string TokenNameToolTipColor = "Tooltip 默認前景色。";
    public const string TokenNameToolTipMaxWidth = "Tooltip 內容換行前的最大寬度。";
    public const string TokenNameBorderRadiusOuter = "Tooltip 表面的外層圓角。";
    public const string TokenNamePadding = "Tooltip 內容內邊距。";
    public const string TokenNameMotionDuration = "Tooltip 打開和關閉動畫的持續時間。";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";

    protected override Type GetResourceKindType() => typeof(TooltipShowCaseLangResourceKind);
}
