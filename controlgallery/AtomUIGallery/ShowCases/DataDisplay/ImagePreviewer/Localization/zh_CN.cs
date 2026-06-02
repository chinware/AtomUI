using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ImagePreviewer;

[LanguageProvider(LanguageCode.zh_CN, ImagePreviewerShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicUsageTitle = "基础用法";
    public const string BasicUsageDescription = "点击图片放大预览。";
    public const string FaultTolerantTitle = "容错";
    public const string FaultTolerantDescription = "加载失败时显示图片占位内容。";
    public const string PreviewFromOneImageTitle = "从单张图片预览集合";
    public const string PreviewFromOneImageDescription = "从一张图片预览图片集合。";
    public const string CustomPreviewImageTitle = "自定义预览图片";
    public const string CustomPreviewImageDescription = "可以设置不同的预览图片。";
    public const string MultipleImagePreviewTitle = "多图预览";
    public const string MultipleImagePreviewDescription = "点击左右切换按钮预览多张图片。";

    protected override Type GetResourceKindType() => typeof(ImagePreviewerShowCaseLangResourceKind);
}
