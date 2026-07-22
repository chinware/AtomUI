using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.ImagePreviewer;

[LanguageProvider(LanguageCode.zh_CN, ImagePreviewerShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string ScenarioExamples = "示例";
    public const string ComponentCategory = "数据展示";
    public const string ComponentStatusStable = "稳定";
    public const string PageSubtitle = "在覆盖层中预览单张或多张图片，并支持缩放、移动和切换。";
    public const string PageDescription = "ImagePreviewer 展示图片封面，点击后打开预览界面。它支持容错图片、封面索引、多图预览、缩放范围和预览窗口生命周期事件。";
    public const string BasicUsageTitle = "基础用法";
    public const string BasicUsageDescription = "点击图片放大预览。";
    public const string RemoteImageLoadingTitle = "远程图片加载";
    public const string RemoteImageLoadingDescription = "直接使用 HTTPS 图片源 URI 加载远程图片。";
    public const string FaultTolerantTitle = "容错";
    public const string FaultTolerantDescription = "加载失败时显示图片占位内容。";
    public const string TwentyRemoteImagesTitle = "20 张远程图片";
    public const string TwentyRemoteImagesDescription = "使用 20 张固定公共图片演示封面加载、并发限制和邻近预加载。";
    public const string PreviewFromOneImageTitle = "从单张图片预览集合";
    public const string PreviewFromOneImageDescription = "从一张图片预览图片集合。";
    public const string CustomPreviewImageTitle = "自定义预览图片";
    public const string CustomPreviewImageDescription = "选择来源集合中的某一张作为关闭态封面。";
    public const string MultipleImagePreviewTitle = "多图预览";
    public const string MultipleImagePreviewDescription = "点击左右切换按钮预览多张图片。";
    public const string ApiEventDialogOpened = "预览窗口打开后触发。";
    public const string ApiEventDialogClosing = "预览窗口关闭前触发。";
    public const string ApiEventDialogClosed = "预览窗口关闭后触发。";

}
