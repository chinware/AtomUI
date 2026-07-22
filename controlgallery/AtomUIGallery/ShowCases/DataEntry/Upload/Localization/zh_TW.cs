using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Upload;

[LanguageProvider(LanguageCode.zh_TW, UploadShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string UploadByClickingTitle = "點擊上傳";
    public const string UploadByClickingDescription = "經典模式。點擊上傳按鈕時彈出文件選擇對話框。";
    public const string ScenarioBasic = "基礎";
    public const string ScenarioPictures = "圖片";
    public const string ScenarioConstraints = "限制";
    public const string AvatarTitle = "頭像";
    public const string AvatarDescription = "點擊上傳用戶頭像，並通過 beforeUpload 校驗圖片大小和格式。";
    public const string DefaultFilesTitle = "默認文件";
    public const string DefaultFilesDescription = "透過綁定 Files 提供初始的可觀察上傳文件列表。";
    public const string PicturesWallTitle = "圖片牆";
    public const string PicturesWallDescription = "用戶上傳圖片後，縮略圖會顯示在 picture-card 列表中，觸發器始終作為獨立 append 入口呈現。";
    public const string PictureCircleTypeTitle = "圓形圖片卡片";
    public const string PictureCircleTypeDescription = "picture-card 的另一種展示形式。";
    public const string DragAndDropTitle = "拖拽上傳";
    public const string DragAndDropDescription = "可以將文件拖拽到指定區域上傳，也可以通過選擇文件上傳。";
    public const string PicturesWithListStyleTitle = "列表樣式圖片";
    public const string PicturesWithListStyleDescription = "如果上傳文件是圖片，可以顯示縮略圖。";
    public const string MaxCountTitle = "最大數量";
    public const string MaxCountDescription = "使用 maxCount 限制文件數量。當 maxCount 為 1 時會替換當前文件。";
    public const string FileAndDirectoryTitle = "文件與目錄";
    public const string FileAndDirectoryDescription = "組合兩個 UploadTrigger，分別觸發文件選擇和目錄選擇。";
    public const string UploadPngOnlyTitle = "僅上傳 PNG 文件";
    public const string UploadPngOnlyDescription = "beforeUpload 返回 false 或拒絕 promise 時只會阻止上傳行為，被阻止的文件仍會顯示在文件列表中。本示例通過返回 UPLOAD.LIST_IGNORE 將被阻止的文件排除在列表外。";
    public const string P2ContentClickToUpload = "點擊上傳";
    public const string P2TextUpload = "上傳";
    public const string P2ContentUploadMaxN1 = "上傳（最多：1）";
    public const string P2ContentUploadMaxN3 = "上傳（最多：3）";
    public const string P2ContentUploadDirectory = "上傳目錄";
    public const string P2ContentUploadPngOnly = "僅上傳 PNG";
    public const string P2ErrorServer500 = "服務器錯誤 500";
    public const string P2ErrorUpload = "上傳失敗！";
    public const string P2CancelJpgPngOnly = "只能上傳 JPG/PNG 文件！";
    public const string P2CancelImageSize = "圖片必須小於 2MB！";
    public const string P2CancelPngOnly = "只能上傳 PNG 文件！";
    public const string P2UploadSuccessFormat = "{0} 上傳成功！";
    public const string PageSubtitle = "選擇、校驗、預覽並上傳文件，支援列表和圖片展示方式。";
    public const string ScrollableListTitle = "可滾動列表";
    public const string ScrollableListDescription = "UploadList 自己管理滾動區域，長文件列表不需要外層 ScrollViewer。";
    public const string SuccessAutoRemoveTitle = "成功後自動移除";
    public const string SuccessAutoRemoveDescription = "透過 SuccessAutoRemoveDelay 在上傳成功後延遲移除文件。";
    public const string PageDescription = "Upload 支援組合式文件和目錄觸發器、拖拽區域、可觀察 Files 狀態、圖片列表、最大數量限制、成功清理和文件類型過濾。";
    public const string ComponentCategory = "資料錄入";
    public const string ComponentStatusStable = "穩定";
    public const string ScenarioExamples = "示例";

}
