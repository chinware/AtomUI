using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.QRCode;

[LanguageProvider(LanguageCode.zh_TW, QRCodeShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
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

    protected override Type GetResourceKindType() => typeof(QRCodeShowCaseLangResourceKind);
}

