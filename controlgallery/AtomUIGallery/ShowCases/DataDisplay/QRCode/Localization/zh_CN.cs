using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.QRCode;

[LanguageProvider(LanguageCode.zh_CN, QRCodeShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string BasicUsageTitle = "基础用法";
    public const string BasicUsageDescription = "基础用法示例。";
    public const string WithIconTitle = "带 Icon 的例子";
    public const string WithIconDescription = "带 Icon 的二维码。";
    public const string DifferentStatusTitle = "不同的状态";
    public const string DifferentStatusDescription = "可以通过 Status 的值控制二维码的状态，提供了 Active、Expired、Loading、Scanned 四个值。";
    public const string CustomStatusRendererTitle = "自定义状态渲染器";
    public const string CustomStatusRendererDescription = "可以通过 LoadingTemplate、ExpiredTemplate、ScannedTemplate 的值控制二维码不同状态的渲染逻辑。";
    public const string CustomSizeTitle = "自定义尺寸";
    public const string CustomSizeDescription = "自定义尺寸。";
    public const string CustomColorTitle = "自定义颜色";
    public const string CustomColorDescription = "自定义颜色。";
    public const string ErrorLevelTitle = "纠错比例";
    public const string ErrorLevelDescription = "通过设置 errorLevel 调整不同的容错等级。";
    public const string AdvancedUsageTitle = "高级用法";
    public const string AdvancedUsageDescription = "带气泡卡片的例子。";
    public const string P2TextLoading = "Loading...";
    public const string P2TextQRCodeExpired = "二维码过期";
    public const string P2ContentClickToRefresh = "点击刷新";
    public const string P2TextScanned = "已扫描";
    public const string P2ContentSmaller = "Smaller";
    public const string P2ContentLarger = "Larger";
    public const string P2ContentHoverMe = "Hover me";
    public const string ScenarioExamples = "示例";
    public const string PageSubtitle = "渲染可扫描的二维码，并支持状态遮罩和品牌图标。";
    public const string PageDescription =
        "QRCode 将文本或 URL 编码成二维码图片，并支持配置尺寸、颜色、图标、纠错等级和状态内容。";
    public const string ComponentCategory = "数据展示";
    public const string ComponentStatusStable = "稳定";
    public const string ApiEventRefreshRequested = "点击内置刷新操作时触发。";

}
