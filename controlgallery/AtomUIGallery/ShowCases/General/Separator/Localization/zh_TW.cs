using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Separator;

[LanguageProvider(LanguageCode.zh_TW, SeparatorShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ComponentCategory = "通用";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "用水平或垂直分割線分隔內容區塊。";
    public const string PageDescription = "Separator 用於建立內容之間的視覺節奏，支持標題文本、普通文本樣式、垂直分割、線型變體和不同間距尺寸。";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyTitle = "顯示在水平分割線中的文本。";
    public const string ApiPropertyTitlePosition = "設置標題在分割線左側、居中或右側顯示。";
    public const string ApiPropertyTitleColor = "用於渲染分割線標題文本的畫刷。";
    public const string ApiPropertyLineColor = "用於渲染分割線線條的畫刷。";
    public const string ApiPropertyOrientation = "控制分割線是水平還是垂直方向。";
    public const string ApiPropertyOrientationMargin = "標題靠左或靠右時，與最近邊緣之間的距離。";
    public const string ApiPropertyVariant = "在線、點線和虛線之間切換分割線樣式。";
    public const string ApiPropertyLineWidth = "不受渲染縮放影響的分割線線寬。";
    public const string ApiPropertyIsPlain = "使用普通正文樣式顯示標題，而不是標題樣式。";
    public const string ApiPropertySizeType = "控制水平分割線的間距密度。";
    public const string ApiPropertyVerticalSeparatorOrientation = "VerticalSeparator 會把 Separator 的方向覆蓋為垂直。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "範圍";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameTextPaddingInline = "標題兩側的內聯內間距，單位為 em。";
    public const string TokenNameOrientationMarginPercent = "未指定 orientation margin 時，標題到邊緣的默認比例。";
    public const string TokenNameVerticalMarginInline = "垂直分割線使用的水平外間距。";
    public const string TokenNameHorizontalMarginBlockSM = "小號水平分割線的垂直外間距。";
    public const string TokenNameHorizontalMarginBlock = "水平分割線的默認垂直外間距。";
    public const string TokenNameHorizontalMarginBlockLG = "大號水平分割線的垂直外間距。";
    public const string TokenNameHorizontalWithTextGutterMargin = "帶標題水平分割線使用的垂直外間距。";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";
    public const string HorizontalTitle = "水平分割線";
    public const string HorizontalDescription = "Separator 默認是水平分割線，可以在 Separator 中添加文本。";
    public const string DividerWithTitleTitle = "帶標題的分割線";
    public const string DividerWithTitleDescription = "帶內部標題的分割線，可設置 orientation='left/right' 來對齊標題。";
    public const string PlainTextTitle = "無標題樣式文本";
    public const string PlainTextDescription = "通過設置 plain 屬性，可以使用非標題樣式的分割線文本。";
    public const string SpacingSizeTitle = "設置分割線間距";
    public const string SpacingSizeDescription = "設置間距大小。";
    public const string VerticalTitle = "垂直分割線";
    public const string VerticalDescription = "使用 type='vertical' 可以讓分割線垂直顯示。";
    public const string VariantTitle = "線型";
    public const string VariantDescription = "分割線默認使用實線樣式，也可以改為虛線或點線。";
    public const string P2TitleText = "文本";
    public const string P2TitleLeftText = "左側文本";
    public const string P2TitleRightText = "右側文本";
    public const string P2TitleLeftTextWithN0Orientationmargin = "orientationMargin 為 0 的左側文本";
    public const string P2TitleRightTextWithN50pxOrientationmargin = "orientationMargin 為 50px 的右側文本";
    public const string P2TitleLeftText2 = "左側文本";
    public const string P2TitleRightText2 = "右側文本";
    public const string P2TitleSolid = "實線";
    public const string P2TitleDotted = "點線";
    public const string P2TitleDashed = "虛線";
    public const string P2TextLoremIpsumDolorSitAmetConsecteturAdipiscingElit = "這是一段用於演示分割線效果的示例文本。分割線可以組織內容層次，讓頁面結構更加清晰。";
    public const string P2TextItem1 = "項目 1";
    public const string P2TextItem2 = "項目 2";
    public const string P2TextItem3 = "項目 3";

    protected override Type GetResourceKindType() => typeof(SeparatorShowCaseLangResourceKind);
}
