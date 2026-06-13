using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.DropdownButton;

[LanguageProvider(LanguageCode.zh_TW, DropdownButtonShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string ComponentCategory = "導航";
    public const string ComponentStatusStable = "穩定";
    public const string PageSubtitle = "用於打開上下文菜單操作的按鈕。";
    public const string PageDescription = "DropdownButton 將按鈕觸發器與 MenuFlyout 結合，支持懸停或點擊觸發、箭頭指示、彈出位置、按鈕形態和菜單項點擊事件轉發。";
    public const string InfoNamespaceLabel = "命名空間";
    public const string InfoPackageLabel = "包";
    public const string InfoBaseClassLabel = "基類";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計令牌";

    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最基礎的下拉菜單。";
    public const string ButtonTypesTitle = "按鈕類型";
    public const string ButtonTypesDescription = "支持統一的按鈕類型。";
    public const string ArrowTitle = "箭頭";
    public const string ArrowDescription = "可以顯示箭頭。";
    public const string PlacementTitle = "彈出位置";
    public const string PlacementDescription = "支持 6 種彈出位置。";
    public const string P2HeaderCut = "剪切";
    public const string P2HeaderCopy = "複製";
    public const string P2HeaderDelete = "刪除";
    public const string P2HeaderPaste = "粘貼";
    public const string P2HeaderPasteFromHistory = "從歷史記錄粘貼";
    public const string P2ContentHoverMe = "懸停";
    public const string P2ContentEditFile = "編輯文件";
    public const string P2ContentBottomLeft = "左下方";
    public const string P2ContentBottom = "下方";
    public const string P2ContentBottomRight = "右下方";
    public const string P2ContentTopLeft = "左上方";
    public const string P2ContentTop = "上方";
    public const string P2ContentTopRight = "右上方";

    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyContent = "按鈕觸發器顯示的內容。";
    public const string ApiPropertyDropdownFlyout = "由下拉按鈕打開的菜單浮層。";
    public const string ApiPropertyTriggerType = "控制浮層通過點擊還是懸停打開。";
    public const string ApiPropertyIsArrowVisible = "控制彈出箭頭是否可見。";
    public const string ApiPropertyIsPointAtCenter = "將彈出箭頭指向觸發器中心。";
    public const string ApiPropertyPlacement = "相對觸發器的彈出位置。";
    public const string ApiPropertyPlacementAnchor = "自定義彈出位置使用的錨點。";
    public const string ApiPropertyPlacementGravity = "自定義彈出位置使用的重力方向。";
    public const string ApiPropertyMarginToAnchor = "浮層與錨點之間的距離。";
    public const string ApiPropertyMouseEnterDelay = "懸停打開浮層前的延遲。";
    public const string ApiPropertyMouseLeaveDelay = "懸停關閉浮層前的延遲。";
    public const string ApiPropertyIsShowOpenIndicator = "控制是否顯示打開指示圖標。";
    public const string ApiPropertyOpenIndicator = "用作打開指示器的圖標。";
    public const string ApiPropertyShouldUseOverlayPopup = "控制浮層是否使用 Overlay Popup 宿主。";
    public const string ApiPropertyButtonType = "控制按鈕視覺類型。";
    public const string ApiPropertySizeType = "控制按鈕尺寸。";
    public const string ApiPropertyIsDanger = "應用危險樣式。";
    public const string ApiPropertyIsMotionEnabled = "在主題允許時啟用控件動效。";
    public const string ApiPropertyIsWaveSpiritEnabled = "啟用按鈕波紋反饋效果。";
    public const string ApiPropertyMenuItemClicked = "點擊下拉菜單項時觸發。";

    public const string TokenColumnToken = "令牌";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "範圍";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNamePadding = "默認按鈕內邊距。";
    public const string TokenNamePaddingLG = "大號按鈕內邊距。";
    public const string TokenNamePaddingSM = "小號按鈕內邊距。";
    public const string TokenNameContentFontSize = "默認按鈕內容字號。";
    public const string TokenNameContentFontSizeLG = "大號按鈕內容字號。";
    public const string TokenNameContentFontSizeSM = "小號按鈕內容字號。";
    public const string TokenNameIconSize = "默認圖標尺寸。";
    public const string TokenNameIconSizeLG = "大號圖標尺寸。";
    public const string TokenNameIconSizeSM = "小號圖標尺寸。";
    public const string TokenNameIconMargin = "圖標與內容之間的間距。";
    public const string TokenNameGutterToFlyout = "按鈕與浮層之間的間距。";
    public const string TokenNameDefaultBg = "默認按鈕背景色。";
    public const string TokenNameDefaultBorderColor = "默認按鈕邊框色。";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";

    protected override Type GetResourceKindType() => typeof(DropdownButtonShowCaseLangResourceKind);
}
