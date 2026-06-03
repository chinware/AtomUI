using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Tour;

[LanguageProvider(LanguageCode.zh_CN, TourShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的用法。";
    public const string NonModalTitle = "非模态";
    public const string NonModalDescription = "使用 mask={false} 可以让 Tour 变为非模态。同时建议配合 type=primary 使用，以突出引导本身。";
    public const string PlacementTitle = "位置";
    public const string PlacementDescription = "改变引导相对于目标的位置，提供 12 种位置。当 target={null} 时，引导会显示在中心。";
    public const string CustomIndicatorTitle = "自定义指示器";
    public const string CustomIndicatorDescription = "自定义指示器。";
    public const string CustomMaskTitle = "自定义遮罩";
    public const string CustomMaskDescription = "自定义遮罩颜色。";
    public const string CustomActionTitle = "自定义操作";
    public const string CustomActionDescription = "自定义操作。";
    public const string CustomHighlightedAreaStyleTitle = "自定义高亮区域样式";
    public const string CustomHighlightedAreaStyleDescription = "使用 gap 控制高亮区域圆角半径，以及高亮区域和元素之间的偏移。";
    public const string P2TitleUploadFile = "上传文件";
    public const string P2DescriptionPutYourFilesHere = "在这里放置你的文件。";
    public const string P2TitleSave = "保存";
    public const string P2DescriptionSaveYourChanges = "保存你的更改。";
    public const string P2TitleOtherActions = "其他操作";
    public const string P2DescriptionClickToSeeOtherActions = "点击查看其他操作。";
    public const string P2TitleCenter = "居中";
    public const string P2DescriptionDisplayedInTheCenterOfScreen = "显示在屏幕中央。";
    public const string P2TitleRight = "右侧";
    public const string P2DescriptionOnTheRightOfTarget = "位于目标右侧。";
    public const string P2TitleTop = "顶部";
    public const string P2DescriptionOnTheTopOfTarget = "位于目标上方。";
    public const string P2TitleLeft = "左侧";
    public const string P2DescriptionOnTheLeftOfTarget = "位于目标左侧。";
    public const string P2ContentBeginTour = "开始引导";
    public const string P2ContentUpload = "上传";
    public const string P2ContentSave = "保存";
    public const string P2ContentBeginNonModalTour = "开始非模态引导";
    public const string P2ContentSkip = "跳过";
    public const string P2TextRadius = "圆角半径：";
    public const string P2TextHorizontalOffset = "水平偏移：";
    public const string P2TextVerticalOffset = "垂直偏移：";

    protected override Type GetResourceKindType() => typeof(TourShowCaseLangResourceKind);
}
