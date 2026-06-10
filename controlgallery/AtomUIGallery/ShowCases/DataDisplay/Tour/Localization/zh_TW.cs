using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Tour;

[LanguageProvider(LanguageCode.zh_TW, TourShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最基礎的用法。";
    public const string NonModalTitle = "非模態";
    public const string NonModalDescription = "使用 mask={false} 可以讓 Tour 變為非模態。同時建議配合 type=primary 使用，以突出引導本身。";
    public const string PlacementTitle = "位置";
    public const string PlacementDescription = "改變引導相對於目標的位置，提供 12 種位置。當 target={null} 時，引導會顯示在中心。";
    public const string CustomIndicatorTitle = "自定義指示器";
    public const string CustomIndicatorDescription = "自定義指示器。";
    public const string CustomMaskTitle = "自定義遮罩";
    public const string CustomMaskDescription = "自定義遮罩顏色。";
    public const string CustomActionTitle = "自定義操作";
    public const string CustomActionDescription = "自定義操作。";
    public const string CustomHighlightedAreaStyleTitle = "自定義高亮區域樣式";
    public const string CustomHighlightedAreaStyleDescription = "使用 gap 控制高亮區域圓角半徑，以及高亮區域和元素之間的偏移。";
    public const string P2TitleUploadFile = "上傳文件";
    public const string P2DescriptionPutYourFilesHere = "在這裡放置你的文件。";
    public const string P2TitleSave = "保存";
    public const string P2DescriptionSaveYourChanges = "保存你的更改。";
    public const string P2TitleOtherActions = "其他操作";
    public const string P2DescriptionClickToSeeOtherActions = "點擊查看其他操作。";
    public const string P2TitleCenter = "居中";
    public const string P2DescriptionDisplayedInTheCenterOfScreen = "顯示在屏幕中央。";
    public const string P2TitleRight = "右側";
    public const string P2DescriptionOnTheRightOfTarget = "位於目標右側。";
    public const string P2TitleTop = "頂部";
    public const string P2DescriptionOnTheTopOfTarget = "位於目標上方。";
    public const string P2TitleLeft = "左側";
    public const string P2DescriptionOnTheLeftOfTarget = "位於目標左側。";
    public const string P2ContentBeginTour = "開始引導";
    public const string P2ContentUpload = "上傳";
    public const string P2ContentSave = "保存";
    public const string P2ContentBeginNonModalTour = "開始非模態引導";
    public const string P2ContentSkip = "跳過";
    public const string P2TextRadius = "圓角半徑：";
    public const string P2TextHorizontalOffset = "水平偏移：";
    public const string P2TextVerticalOffset = "垂直偏移：";

    protected override Type GetResourceKindType() => typeof(TourShowCaseLangResourceKind);
}

