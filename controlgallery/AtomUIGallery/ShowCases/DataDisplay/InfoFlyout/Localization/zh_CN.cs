using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.InfoFlyout;

[LanguageProvider(LanguageCode.zh_CN, InfoFlyoutShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的示例。浮层大小取决于内容区域。";
    public const string TriggerWaysTitle = "三种触发方式";
    public const string TriggerWaysDescription = "通过鼠标点击、获得焦点和移入触发。";
    public const string PlacementTitle = "弹出位置";
    public const string PlacementDescription = "提供 12 种弹出位置。";
    public const string ArrowTitle = "箭头";
    public const string ArrowDescription = "支持显示、隐藏或保持箭头居中。";
    public const string P2TextTheMostBasicExample = "这是最基础的示例。";
    public const string P2ContentHoverMe = "移入我";
    public const string P2ContentFocusMe = "聚焦我";
    public const string P2ContentClickMe = "点击我";
    public const string P2ContentShow = "显示";
    public const string P2ContentHide = "隐藏";
    public const string P2ContentCenter = "居中";

    public const string P2ContentLT = "左上";

    public const string P2ContentLeft = "左侧";

    public const string P2ContentLB = "左下";

    public const string P2ContentTL = "上左";

    public const string P2ContentTop = "上方";

    public const string P2ContentTR = "上右";

    public const string P2ContentRT = "右上";

    public const string P2ContentRight = "右侧";

    public const string P2ContentRB = "右下";

    public const string P2ContentBL = "下左";

    public const string P2ContentBottom = "下方";

    public const string P2ContentBR = "下右";

    protected override Type GetResourceKindType() => typeof(InfoFlyoutShowCaseLangResourceKind);
}
