using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.ImagePreviewer;

[LanguageProvider(LanguageCode.zh_CN, ImagePreviewerShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计令牌";
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
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertySource = "ImagePreviewer 使用的主图片来源，支持 URI 和按需数据流。";
    public const string ApiPropertySources = "ImagePreviewer 和 ImageGroupPreviewer 使用的主图片来源列表。";
    public const string ApiPropertyFallbackSource = "所有配置图片来源都加载失败时显示的容错图片来源。";
    public const string ApiPropertyIsOpen = "控制预览覆盖层或预览窗口是否打开，默认支持双向绑定。";
    public const string ApiPropertyCoverWidth = "图片封面的宽度。";
    public const string ApiPropertyCoverHeight = "图片封面的高度。";
    public const string ApiPropertyCurrentIndex = "多图预览中的当前图片索引，默认支持双向绑定。";
    public const string ApiPropertyCoverIndex = "关闭态封面使用的来源图片索引，只影响展示。";
    public const string ApiPropertyMaxConcurrentLoads = "同一时间允许执行的最大图片加载任务数。";
    public const string ApiPropertyPreloadCount = "当前预览图片前后需要预加载的邻近图片数量。";
    public const string ApiPropertyPreviewTitle = "预览窗口标题栏显示的显式标题。";
    public const string ApiPropertyPreviewTitleIcon = "显示在预览窗口标题左侧的可选图标。";
    public const string ApiPropertyPreviewTitleResolver = "PreviewTitle 为空时用于解析预览窗口标题的 resolver。";
    public const string ApiPropertyLoadingContent = "图片加载中显示的自定义内容。";
    public const string ApiPropertyLoadingContentTemplate = "用于渲染自定义加载内容的模板。";
    public const string ApiPropertyErrorContent = "图片加载失败时显示的自定义内容。";
    public const string ApiPropertyErrorContentTemplate = "用于渲染自定义失败内容的模板。";
    public const string ApiPropertyIsShowCoverMask = "是否显示封面遮罩和预览提示。";
    public const string ApiPropertyImageScaleStep = "放大和缩小时应用的缩放步长。";
    public const string ApiPropertyImageMinScale = "预览界面中的最小图片缩放比例。";
    public const string ApiPropertyImageMaxScale = "预览界面中的最大图片缩放比例。";
    public const string ApiEventDialogOpened = "预览窗口打开后触发。";
    public const string ApiEventDialogClosing = "预览窗口关闭前触发。";
    public const string ApiEventDialogClosed = "预览窗口关闭后触发。";
    public const string TokenColumnToken = "令牌";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "范围";
    public const string TokenColumnStatus = "状态";
    public const string TokenNamePreviewOperationSize = "预览操作图标尺寸。";
    public const string TokenNamePreviewOperationColor = "预览操作图标颜色。";
    public const string TokenNamePreviewOperationHoverColor = "预览操作图标悬浮颜色。";
    public const string TokenNameImagePreviewSwitchSize = "图片切换按钮尺寸。";
    public const string TokenNameMaskBgColor = "封面遮罩背景色。";
    public const string TokenNameDialogMinWidth = "预览窗口最小宽度。";
    public const string TokenNameDialogMinHeight = "预览窗口最小高度。";
    public const string TokenNameCoverImageWidth = "默认封面图片宽度。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";

}
