using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.PopupConfirm;

[LanguageProvider(LanguageCode.zh_TW, PopupConfirmShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicUsageTitle = "基礎用法";
    public const string BasicUsageDescription = "基礎示例支持確認框的標題和描述屬性。";
    public const string LocaleTextTitle = "本地化文本";
    public const string LocaleTextDescription = "設置 okText 和 cancelText 屬性來自定義按鈕標籤。";
    public const string PlacementTitle = "彈出位置";
    public const string PlacementDescription = "提供 12 種彈出位置。";
    public const string CustomizeIconTitle = "自定義圖標";
    public const string CustomizeIconDescription = "設置 icon 屬性來自定義圖標。";
    public const string ComponentCategory = "反饋";
    public const string ComponentStatusStable = "穩定";
    public const string PageSubtitle = "在輕量級危險操作或風險操作前進行就地確認。";
    public const string PageDescription = "PopupConfirm 將觸發控件與確認浮層組合在一起，適用於需要快速二次確認、但不需要阻塞式模態框的操作。";
    public const string InfoNamespaceLabel = "命名空間";
    public const string InfoPackageLabel = "包";
    public const string InfoBaseClassLabel = "基類";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計變量";
    public const string ApiColumnMember = "成員";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyPopupConfirmTitle = "顯示在確認浮層中的標題文本。";
    public const string ApiPropertyPopupConfirmConfirmContent = "顯示在確認浮層標題下方的內容。";
    public const string ApiPropertyPopupConfirmConfirmContentTemplate = "用於渲染自定義確認內容的可選模板。";
    public const string ApiPropertyPopupConfirmOkText = "確認按鈕顯示的文本。";
    public const string ApiPropertyPopupConfirmCancelText = "取消按鈕顯示的文本。";
    public const string ApiPropertyPopupConfirmOkButtonType = "確認操作使用的按鈕類型。";
    public const string ApiPropertyPopupConfirmIsShowCancelButton = "控制是否顯示取消按鈕。";
    public const string ApiPropertyPopupConfirmIcon = "顯示在確認標題前的可選圖標。";
    public const string ApiPropertyPopupConfirmConfirmStatus = "確認內容的語義狀態。";
    public const string ApiEventPopupConfirmCancelled = "選擇取消操作時觸發。";
    public const string ApiEventPopupConfirmConfirmed = "選擇確認操作時觸發。";
    public const string ApiEventPopupConfirmPopupClick = "點擊確認浮層任一按鈕後觸發。";
    public const string TokenColumnToken = "變量";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "範圍";
    public const string TokenColumnStatus = "狀態";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";
    public const string TokenNamePopupMinWidth = "確認浮層面板的最小寬度。";
    public const string TokenNamePopupMinHeight = "確認浮層面板的最小高度。";
    public const string TokenNameButtonSpacing = "確認浮層按鈕之間的間距。";
    public const string TokenNameIconMargin = "狀態圖標外邊距。";
    public const string TokenNameContentContainerMargin = "主確認內容區域外邊距。";
    public const string TokenNameButtonContainerMargin = "按鈕操作區域外邊距。";
    public const string TokenNameTitleMargin = "確認標題外邊距。";
    public const string P2ConfirmContentAreYouSureToDeleteThisTask = "確定要刪除這個任務嗎？";
    public const string P2OkTextOk = "確定";
    public const string P2CancelTextCancel = "取消";
    public const string P2TitleDeleteTheTask = "刪除任務";
    public const string P2ContentDelete = "刪除";
    public const string P2ContentLt = "左上側";
    public const string P2ContentLeft = "左側";
    public const string P2ContentLb = "左下側";
    public const string P2ContentTl = "上左側";
    public const string P2ContentTop = "頂部";
    public const string P2ContentTr = "上右側";
    public const string P2ContentRt = "右上側";
    public const string P2ContentRight = "右側";
    public const string P2ContentRb = "右下側";
    public const string P2ContentBl = "下左側";
    public const string P2ContentBottom = "底部";
    public const string P2ContentBr = "下右側";

    protected override Type GetResourceKindType() => typeof(PopupConfirmShowCaseLangResourceKind);
}
