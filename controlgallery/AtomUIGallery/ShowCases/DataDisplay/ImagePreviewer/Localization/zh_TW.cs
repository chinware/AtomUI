using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.ImagePreviewer;

[LanguageProvider(LanguageCode.zh_TW, ImagePreviewerShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string ScenarioExamples = "範例";
    public const string ComponentCategory = "資料展示";
    public const string ComponentStatusStable = "穩定";
    public const string PageSubtitle = "在覆蓋層中預覽單張或多張圖片，並支援縮放、移動和切換。";
    public const string PageDescription = "ImagePreviewer 展示圖片封面，點擊後打開預覽介面。它支援容錯圖片、封面索引、多圖預覽、縮放範圍和預覽視窗生命週期事件。";
    public const string BasicUsageTitle = "基礎用法";
    public const string BasicUsageDescription = "點擊圖片放大預覽。";
    public const string RemoteImageLoadingTitle = "遠程圖片載入";
    public const string RemoteImageLoadingDescription = "直接使用 HTTPS 圖片源 URI 載入遠程圖片。";
    public const string FaultTolerantTitle = "容錯";
    public const string FaultTolerantDescription = "加載失敗時顯示圖片佔位內容。";
    public const string TwentyRemoteImagesTitle = "20 張遠程圖片";
    public const string TwentyRemoteImagesDescription = "使用 20 張固定公共圖片演示封面載入、並發限制和鄰近預載入。";
    public const string PreviewFromOneImageTitle = "從單張圖片預覽集合";
    public const string PreviewFromOneImageDescription = "從一張圖片預覽圖片集合。";
    public const string CustomPreviewImageTitle = "自定義預覽圖片";
    public const string CustomPreviewImageDescription = "選擇來源集合中的某一張作為關閉態封面。";
    public const string MultipleImagePreviewTitle = "多圖預覽";
    public const string MultipleImagePreviewDescription = "點擊左右切換按鈕預覽多張圖片。";
    public const string ApiEventDialogOpened = "預覽視窗打開後觸發。";
    public const string ApiEventDialogClosing = "預覽視窗關閉前觸發。";
    public const string ApiEventDialogClosed = "預覽視窗關閉後觸發。";

}
