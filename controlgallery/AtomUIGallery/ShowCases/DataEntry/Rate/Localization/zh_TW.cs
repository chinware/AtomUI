using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Rate;

[LanguageProvider(LanguageCode.zh_TW, RateShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計變數";
    public const string ComponentCategory = "數據錄入";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "用星級、半星、自定義字符和提示文案收集輕量評分。";
    public const string PageDescription = "Rate 用於讓用戶按有序等級表達偏好或質量評價，支持清除、半選、只讀、鍵盤交互、自定義圖形和本地化文案。";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyIsAllowClear = "允許再次點擊當前評分值時清除評分。";
    public const string ApiPropertyIsAllowHalf = "允許選擇半星評分值。";
    public const string ApiPropertyCharacter = "每個評分項使用的自定義視覺內容。";
    public const string ApiPropertyStarColor = "覆蓋已選評分項顏色。";
    public const string ApiPropertyStarBgColor = "覆蓋未選評分項顏色。";
    public const string ApiPropertyCount = "評分項總數。";
    public const string ApiPropertyValue = "當前選中的評分值，預設 TwoWay 綁定，並支援 Avalonia 資料驗證。";
    public const string ApiPropertyDefaultValue = "未顯式設置 Value 時使用的初始評分值。";
    public const string ApiPropertyIsKeyboardEnabled = "控制是否啟用鍵盤交互。";
    public const string ApiPropertyToolTips = "每個評分值對應的提示文本列表。";
    public const string ApiPropertySizeType = "控制小、中、大三種評分尺寸。";
    public const string ApiPropertyIsMotionEnabled = "啟用或禁用評分動效。";
    public const string ApiPropertyValueChanged = "選中評分值變化時觸發。";
    public const string ApiPropertyHoverValueChanged = "懸浮評分值變化時觸發。";
    public const string TokenColumnToken = "變數";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "範圍";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameStarColor = "已選評分項顏色。";
    public const string TokenNameStarSize = "默認評分項尺寸。";
    public const string TokenNameStarSizeSM = "小號評分項尺寸。";
    public const string TokenNameStarSizeLG = "大號評分項尺寸。";
    public const string TokenNameStarHoverScale = "評分項懸浮時應用的縮放比例。";
    public const string TokenNameStarBg = "未選評分項背景色。";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最簡單的用法。";
    public const string TwoWayBindingTitle = "雙向綁定";
    public const string TwoWayBindingDescription = "Value 預設使用 TwoWay 綁定，使用者評分和 ViewModel 更新會保持同步。";
    public const string HalfStarTitle = "半星";
    public const string HalfStarDescription = "支持選擇半星。";
    public const string ShowCopywritingTitle = "顯示文案";
    public const string ShowCopywritingDescription = "在評分組件中添加文案。";
    public const string ReadOnlyTitle = "只讀";
    public const string ReadOnlyDescription = "只讀狀態，不能使用鼠標交互。";
    public const string ClearStarTitle = "清除星級";
    public const string ClearStarDescription = "支持設置再次點擊時允許清除星級。";
    public const string OtherCharacterTitle = "其他字符";
    public const string OtherCharacterDescription = "將默認星形替換為其他字符，例如字母、數字、圖標字體，甚至中文文字。";
    public const string P2TextIsallowclearTrue = "允許清除：true";
    public const string P2TextIsallowclearFalse = "允許清除：false";
    public const string P2ContentSetFourStars = "設為 4 星";
    public const string P2ContentClear = "清空";
    public const string P2TwoWayValueSummaryFormat = "目前評分：{0:0.#}";
    public const string P2TooltipTerrible = "糟糕";
    public const string P2TooltipBad = "不好";
    public const string P2TooltipNormal = "一般";
    public const string P2TooltipGood = "好";
    public const string P2TooltipWonderful = "很棒";

}
