using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ImagePreviewer;

[LanguageProvider(LanguageCode.zh_TW, ImagePreviewerShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string ScenarioExamples = "範例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計令牌";
    public const string ComponentCategory = "資料展示";
    public const string ComponentStatusStable = "穩定";
    public const string PageSubtitle = "在覆蓋層中預覽單張或多張圖片，並支援縮放、移動和切換。";
    public const string PageDescription = "ImagePreviewer 展示圖片封面，點擊後打開預覽介面。它支援容錯圖片、自定義封面、多圖預覽、縮放範圍和預覽視窗生命週期事件。";
    public const string BasicUsageTitle = "基礎用法";
    public const string BasicUsageDescription = "點擊圖片放大預覽。";
    public const string RemoteImageLoadingTitle = "遠程圖片載入";
    public const string RemoteImageLoadingDescription = "直接使用 HTTPS 圖片源 URI 載入遠程圖片。";
    public const string FaultTolerantTitle = "容錯";
    public const string FaultTolerantDescription = "加載失敗時顯示圖片佔位內容。";
    public const string PreviewFromOneImageTitle = "從單張圖片預覽集合";
    public const string PreviewFromOneImageDescription = "從一張圖片預覽圖片集合。";
    public const string CustomPreviewImageTitle = "自定義預覽圖片";
    public const string CustomPreviewImageDescription = "可以設置不同的預覽圖片。";
    public const string MultipleImagePreviewTitle = "多圖預覽";
    public const string MultipleImagePreviewDescription = "點擊左右切換按鈕預覽多張圖片。";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "預設值";
    public const string ApiPropertySourceUri = "ImagePreviewer 使用的單張圖片源 URI。";
    public const string ApiPropertySourceUris = "ImagePreviewer 和 ImageGroupPreviewer 使用的圖片源 URI 列表。";
    public const string ApiPropertyFallbackSourceUri = "配置圖片載入失敗時顯示的容錯圖片源 URI。";
    public const string ApiPropertyIsOpen = "控制預覽覆蓋層或預覽視窗是否打開。";
    public const string ApiPropertyCoverWidth = "圖片封面的寬度。";
    public const string ApiPropertyCoverHeight = "圖片封面的高度。";
    public const string ApiPropertyCurrentIndex = "多圖預覽中的當前圖片索引。";
    public const string ApiPropertyCoverSourceUri = "ImagePreviewer 的自定義封面圖片源 URI。";
    public const string ApiPropertyLoadingContent = "圖片載入中顯示的自定義內容。";
    public const string ApiPropertyLoadingContentTemplate = "用於渲染自定義載入內容的模板。";
    public const string ApiPropertyErrorContent = "圖片載入失敗時顯示的自定義內容。";
    public const string ApiPropertyErrorContentTemplate = "用於渲染自定義失敗內容的模板。";
    public const string ApiPropertyIsShowCoverMask = "是否顯示封面遮罩和預覽提示。";
    public const string ApiPropertyImageScaleStep = "放大和縮小時套用的縮放步長。";
    public const string ApiPropertyImageMinScale = "預覽介面中的最小圖片縮放比例。";
    public const string ApiPropertyImageMaxScale = "預覽介面中的最大圖片縮放比例。";
    public const string ApiEventDialogOpened = "預覽視窗打開後觸發。";
    public const string ApiEventDialogClosing = "預覽視窗關閉前觸發。";
    public const string ApiEventDialogClosed = "預覽視窗關閉後觸發。";
    public const string TokenColumnToken = "令牌";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "範圍";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNamePreviewOperationSize = "預覽操作圖標尺寸。";
    public const string TokenNamePreviewOperationColor = "預覽操作圖標顏色。";
    public const string TokenNamePreviewOperationHoverColor = "預覽操作圖標懸浮顏色。";
    public const string TokenNameImagePreviewSwitchSize = "圖片切換按鈕尺寸。";
    public const string TokenNameMaskBgColor = "封面遮罩背景色。";
    public const string TokenNameDialogMinWidth = "預覽視窗最小寬度。";
    public const string TokenNameDialogMinHeight = "預覽視窗最小高度。";
    public const string TokenNameCoverImageWidth = "預設封面圖片寬度。";
    public const string TokenScopeComponent = "元件";
    public const string TokenStatusStable = "穩定";

    protected override Type GetResourceKindType() => typeof(ImagePreviewerShowCaseLangResourceKind);
}
