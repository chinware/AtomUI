using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.QRCode;

[LanguageProvider(LanguageCode.zh_TW, QRCodeShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string BasicUsageTitle = "基礎用法";
    public const string BasicUsageDescription = "基礎用法示例。";
    public const string WithIconTitle = "帶 Icon 的例子";
    public const string WithIconDescription = "帶 Icon 的二維碼。";
    public const string DifferentStatusTitle = "不同的狀態";
    public const string DifferentStatusDescription = "可以通過 Status 的值控制二維碼的狀態，提供了 Active、Expired、Loading、Scanned 四個值。";
    public const string CustomStatusRendererTitle = "自定義狀態渲染器";
    public const string CustomStatusRendererDescription = "可以通過 LoadingTemplate、ExpiredTemplate、ScannedTemplate 的值控制二維碼不同狀態的渲染邏輯。";
    public const string CustomSizeTitle = "自定義尺寸";
    public const string CustomSizeDescription = "自定義尺寸。";
    public const string CustomColorTitle = "自定義顏色";
    public const string CustomColorDescription = "自定義顏色。";
    public const string ErrorLevelTitle = "糾錯比例";
    public const string ErrorLevelDescription = "通過設置 errorLevel 調整不同的容錯等級。";
    public const string AdvancedUsageTitle = "高級用法";
    public const string AdvancedUsageDescription = "帶氣泡卡片的例子。";
    public const string P2TextLoading = "Loading...";
    public const string P2TextQRCodeExpired = "二維碼過期";
    public const string P2ContentClickToRefresh = "點擊刷新";
    public const string P2TextScanned = "已掃描";
    public const string P2ContentSmaller = "Smaller";
    public const string P2ContentLarger = "Larger";
    public const string P2ContentHoverMe = "Hover me";
    public const string ScenarioExamples = "範例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計 Token";
    public const string PageSubtitle = "渲染可掃描的二維碼，並支持狀態遮罩和品牌圖標。";
    public const string PageDescription =
        "QRCode 將文字或 URL 編碼成二維碼圖片，並支持配置尺寸、顏色、圖標、糾錯等級和狀態內容。";
    public const string ComponentCategory = "資料展示";
    public const string ComponentStatusStable = "穩定";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "預設值";
    public const string ApiPropertyValue = "編碼到二維碼中的文字或 URL。";
    public const string ApiPropertyIsBordered = "顯示或隱藏二維碼邊框。";
    public const string ApiPropertyColor = "用於渲染二維碼模組的畫刷。";
    public const string ApiPropertySize = "生成二維碼圖片的像素尺寸。";
    public const string ApiPropertyEccLevel = "生成二維碼時使用的糾錯等級。";
    public const string ApiPropertyIconSize = "中間可選圖標的像素尺寸。";
    public const string ApiPropertyIcon = "顯示在二維碼中心的可選圖片。";
    public const string ApiPropertyIconBgColor = "中心圖標背後的背景畫刷。";
    public const string ApiPropertyStatus = "Active、Expired、Loading 或 Scanned 狀態下的視覺遮罩。";
    public const string ApiPropertyLoadingContent = "二維碼載入中時顯示的自定義內容。";
    public const string ApiPropertyExpiredContent = "二維碼過期時顯示的自定義內容。";
    public const string ApiPropertyScannedContent = "二維碼掃描後顯示的自定義內容。";
    public const string ApiEventRefreshRequested = "點擊內置刷新操作時觸發。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameQRCodeTextColor = "繪製二維碼模組時使用的預設顏色。";
    public const string TokenNameQRCodeMaskBackgroundColor = "非 Active 狀態遮罩使用的背景色。";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";

}
