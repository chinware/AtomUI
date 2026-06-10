using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ImagePreviewer;

[LanguageProvider(LanguageCode.zh_TW, ImagePreviewerShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicUsageTitle = "基礎用法";
    public const string BasicUsageDescription = "點擊圖片放大預覽。";
    public const string FaultTolerantTitle = "容錯";
    public const string FaultTolerantDescription = "加載失敗時顯示圖片佔位內容。";
    public const string PreviewFromOneImageTitle = "從單張圖片預覽集合";
    public const string PreviewFromOneImageDescription = "從一張圖片預覽圖片集合。";
    public const string CustomPreviewImageTitle = "自定義預覽圖片";
    public const string CustomPreviewImageDescription = "可以設置不同的預覽圖片。";
    public const string MultipleImagePreviewTitle = "多圖預覽";
    public const string MultipleImagePreviewDescription = "點擊左右切換按鈕預覽多張圖片。";

    protected override Type GetResourceKindType() => typeof(ImagePreviewerShowCaseLangResourceKind);
}

