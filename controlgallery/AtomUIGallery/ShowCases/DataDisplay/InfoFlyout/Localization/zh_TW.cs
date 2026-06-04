using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.InfoFlyout;

[LanguageProvider(LanguageCode.zh_TW, InfoFlyoutShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最基礎的示例。浮層大小取決於內容區域。";
    public const string TriggerWaysTitle = "三種觸發方式";
    public const string TriggerWaysDescription = "通過鼠標點擊、獲得焦點和移入觸發。";
    public const string PlacementTitle = "彈出位置";
    public const string PlacementDescription = "提供 12 種彈出位置。";
    public const string ArrowTitle = "箭頭";
    public const string ArrowDescription = "支持顯示、隱藏或保持箭頭居中。";
    public const string P2TextTheMostBasicExample = "這是最基礎的示例。";
    public const string P2ContentHoverMe = "移入我";
    public const string P2ContentFocusMe = "聚焦我";
    public const string P2ContentClickMe = "點擊我";
    public const string P2ContentShow = "顯示";
    public const string P2ContentHide = "隱藏";
    public const string P2ContentCenter = "居中";

    public const string P2ContentLT = "左上";

    public const string P2ContentLeft = "左側";

    public const string P2ContentLB = "左下";

    public const string P2ContentTL = "上左";

    public const string P2ContentTop = "上方";

    public const string P2ContentTR = "上右";

    public const string P2ContentRT = "右上";

    public const string P2ContentRight = "右側";

    public const string P2ContentRB = "右下";

    public const string P2ContentBL = "下左";

    public const string P2ContentBottom = "下方";

    public const string P2ContentBR = "下右";

    protected override Type GetResourceKindType() => typeof(InfoFlyoutShowCaseLangResourceKind);
}

