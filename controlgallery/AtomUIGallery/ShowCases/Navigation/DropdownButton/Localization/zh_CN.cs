using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.DropdownButton;

[LanguageProvider(LanguageCode.zh_CN, DropdownButtonShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string ComponentCategory = "导航";
    public const string ComponentStatusStable = "稳定";
    public const string PageSubtitle = "用于打开上下文菜单操作的按钮。";
    public const string PageDescription = "DropdownButton 将按钮触发器与 MenuFlyout 结合，支持悬停或点击触发、箭头指示、弹出位置、按钮形态和菜单项点击事件转发。";
    public const string InfoNamespaceLabel = "命名空间";
    public const string InfoPackageLabel = "包";
    public const string InfoBaseClassLabel = "基类";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计令牌";

    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的下拉菜单。";
    public const string ButtonTypesTitle = "按钮类型";
    public const string ButtonTypesDescription = "支持统一的按钮类型。";
    public const string ArrowTitle = "箭头";
    public const string ArrowDescription = "可以显示箭头。";
    public const string PlacementTitle = "弹出位置";
    public const string PlacementDescription = "支持 6 种弹出位置。";
    public const string P2HeaderCut = "剪切";
    public const string P2HeaderCopy = "复制";
    public const string P2HeaderDelete = "删除";
    public const string P2HeaderPaste = "粘贴";
    public const string P2HeaderPasteFromHistory = "从历史记录粘贴";
    public const string P2ContentHoverMe = "悬停";
    public const string P2ContentEditFile = "编辑文件";
    public const string P2ContentBottomLeft = "左下方";
    public const string P2ContentBottom = "下方";
    public const string P2ContentBottomRight = "右下方";
    public const string P2ContentTopLeft = "左上方";
    public const string P2ContentTop = "上方";
    public const string P2ContentTopRight = "右上方";

    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyContent = "按钮触发器显示的内容。";
    public const string ApiPropertyDropdownFlyout = "由下拉按钮打开的菜单浮层。";
    public const string ApiPropertyTriggerType = "控制浮层通过点击还是悬停打开。";
    public const string ApiPropertyIsArrowVisible = "控制弹出箭头是否可见。";
    public const string ApiPropertyIsPointAtCenter = "将弹出箭头指向触发器中心。";
    public const string ApiPropertyPlacement = "相对触发器的弹出位置。";
    public const string ApiPropertyPlacementAnchor = "自定义弹出位置使用的锚点。";
    public const string ApiPropertyPlacementGravity = "自定义弹出位置使用的重力方向。";
    public const string ApiPropertyMarginToAnchor = "浮层与锚点之间的距离。";
    public const string ApiPropertyMouseEnterDelay = "悬停打开浮层前的延迟。";
    public const string ApiPropertyMouseLeaveDelay = "悬停关闭浮层前的延迟。";
    public const string ApiPropertyIsShowOpenIndicator = "控制是否显示打开指示图标。";
    public const string ApiPropertyOpenIndicator = "用作打开指示器的图标。";
    public const string ApiPropertyShouldUseOverlayPopup = "控制浮层是否使用 Overlay Popup 宿主。";
    public const string ApiPropertyButtonType = "控制按钮视觉类型。";
    public const string ApiPropertySizeType = "控制按钮尺寸。";
    public const string ApiPropertyIsDanger = "应用危险样式。";
    public const string ApiPropertyIsMotionEnabled = "在主题允许时启用控件动效。";
    public const string ApiPropertyIsWaveSpiritEnabled = "启用按钮波纹反馈效果。";
    public const string ApiPropertyMenuItemClicked = "点击下拉菜单项时触发。";

    public const string TokenColumnToken = "令牌";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "范围";
    public const string TokenColumnStatus = "状态";
    public const string TokenNamePadding = "默认按钮内边距。";
    public const string TokenNamePaddingLG = "大号按钮内边距。";
    public const string TokenNamePaddingSM = "小号按钮内边距。";
    public const string TokenNameContentFontSize = "默认按钮内容字号。";
    public const string TokenNameContentFontSizeLG = "大号按钮内容字号。";
    public const string TokenNameContentFontSizeSM = "小号按钮内容字号。";
    public const string TokenNameIconSize = "默认图标尺寸。";
    public const string TokenNameIconSizeLG = "大号图标尺寸。";
    public const string TokenNameIconSizeSM = "小号图标尺寸。";
    public const string TokenNameIconMargin = "图标与内容之间的间距。";
    public const string TokenNameGutterToFlyout = "按钮与浮层之间的间距。";
    public const string TokenNameDefaultBg = "默认按钮背景色。";
    public const string TokenNameDefaultBorderColor = "默认按钮边框色。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";

    protected override Type GetResourceKindType() => typeof(DropdownButtonShowCaseLangResourceKind);
}
