using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.InfoFlyout;

[LanguageProvider(LanguageCode.zh_TW, InfoFlyoutShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最基礎的示例。浮層大小取決於內容區域。";
    public const string TriggerWaysTitle = "三種觸發方式";
    public const string TriggerWaysDescription = "通過鼠標點擊、獲得焦點和移入觸發。";
    public const string PlacementTitle = "彈出位置";
    public const string PlacementDescription = "提供 12 種彈出位置。";
    public const string ArrowTitle = "箭頭";
    public const string ArrowDescription = "支持顯示、隱藏或保持箭頭居中。";
    public const string P2TextTheMostBasicExample = "這是最基礎的示例。";
    public const string P2ContentHoverMe = "移入我";
    public const string P2ContentFocusMe = "聚焦我";
    public const string P2ContentClickMe = "點擊我";
    public const string P2ContentShow = "顯示";
    public const string P2ContentHide = "隱藏";
    public const string P2ContentCenter = "居中";

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
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計 Token";
    public const string PageSubtitle = "在浮層中展示更豐富的上下文內容。";
    public const string PageDescription =
        "InfoFlyout 可以把自定義浮層錨定到目標控件上，並支持配置觸發方式、彈出位置和箭頭行為。";
    public const string ComponentCategory = "數據展示";
    public const string ComponentStatusStable = "穩定";
    public const string InfoNamespaceLabel = "命名空間";
    public const string InfoPackageLabel = "包";
    public const string InfoBaseClassLabel = "基類";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyFlyout = "由宿主顯示的 Flyout 實例。";
    public const string ApiPropertyTrigger = "打開浮層的交互方式。";
    public const string ApiPropertyPlacement = "相對目標控件的首選彈出位置。";
    public const string ApiPropertyIsArrowVisible = "顯示或隱藏浮層箭頭。";
    public const string ApiPropertyIsPointAtCenter = "讓箭頭指向目標控件中心。";
    public const string ApiPropertyShouldUseOverlayPopup = "打開浮層時使用 overlay popup 宿主。";
    public const string ApiPropertyMarginToAnchor = "浮層表面與錨點之間的距離。";
    public const string ApiPropertyMouseEnterDelay = "懸停打開浮層前的延遲，單位毫秒。";
    public const string ApiPropertyMouseLeaveDelay = "懸停關閉浮層前的延遲，單位毫秒。";
    public const string ApiPropertyContent = "顯示在浮層表面內的內容。";
    public const string ApiPropertyIsLightDismissEnabled = "允許點擊或聚焦到浮層外部時關閉浮層。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameMarginToAnchor = "浮層和錨點之間的默認距離。";
    public const string TokenNameOverlayHostShadow = "浮層由 overlay 層承載時使用的陰影。";
    public const string TokenNamePopupRootShadow = "popup 根表面使用的陰影。";
    public const string TokenNameHorizontalOffset = "浮層定位的默認水平偏移。";
    public const string TokenNameVerticalOffset = "浮層定位的默認垂直偏移。";
    public const string TokenScopeComponent = "FlyoutHost";
    public const string TokenStatusStable = "穩定";

    protected override Type GetResourceKindType() => typeof(InfoFlyoutShowCaseLangResourceKind);
}
