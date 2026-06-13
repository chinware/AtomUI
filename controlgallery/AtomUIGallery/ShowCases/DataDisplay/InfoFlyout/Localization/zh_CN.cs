using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.InfoFlyout;

[LanguageProvider(LanguageCode.zh_CN, InfoFlyoutShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
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
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计 Token";
    public const string PageSubtitle = "在浮层中展示更丰富的上下文内容。";
    public const string PageDescription =
        "InfoFlyout 可以把自定义浮层锚定到目标控件上，并支持配置触发方式、弹出位置和箭头行为。";
    public const string ComponentCategory = "数据展示";
    public const string ComponentStatusStable = "稳定";
    public const string InfoNamespaceLabel = "命名空间";
    public const string InfoPackageLabel = "包";
    public const string InfoBaseClassLabel = "基类";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyFlyout = "由宿主显示的 Flyout 实例。";
    public const string ApiPropertyTrigger = "打开浮层的交互方式。";
    public const string ApiPropertyPlacement = "相对目标控件的首选弹出位置。";
    public const string ApiPropertyIsArrowVisible = "显示或隐藏浮层箭头。";
    public const string ApiPropertyIsPointAtCenter = "让箭头指向目标控件中心。";
    public const string ApiPropertyShouldUseOverlayPopup = "打开浮层时使用 overlay popup 宿主。";
    public const string ApiPropertyMarginToAnchor = "浮层表面与锚点之间的距离。";
    public const string ApiPropertyMouseEnterDelay = "悬停打开浮层前的延迟，单位毫秒。";
    public const string ApiPropertyMouseLeaveDelay = "悬停关闭浮层前的延迟，单位毫秒。";
    public const string ApiPropertyContent = "显示在浮层表面内的内容。";
    public const string ApiPropertyIsLightDismissEnabled = "允许点击或聚焦到浮层外部时关闭浮层。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameMarginToAnchor = "浮层和锚点之间的默认距离。";
    public const string TokenNameOverlayHostShadow = "浮层由 overlay 层承载时使用的阴影。";
    public const string TokenNamePopupRootShadow = "popup 根表面使用的阴影。";
    public const string TokenNameHorizontalOffset = "浮层定位的默认水平偏移。";
    public const string TokenNameVerticalOffset = "浮层定位的默认垂直偏移。";
    public const string TokenScopeComponent = "FlyoutHost";
    public const string TokenStatusStable = "稳定";

    protected override Type GetResourceKindType() => typeof(InfoFlyoutShowCaseLangResourceKind);
}
