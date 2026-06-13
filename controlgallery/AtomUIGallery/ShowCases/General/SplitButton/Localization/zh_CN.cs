using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.SplitButton;

[LanguageProvider(LanguageCode.zh_CN, SplitButtonShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的 SplitButton。";
    public const string SizeTitle = "尺寸";
    public const string SizeDescription = "AtomUI 支持小号、默认和大号三种按钮尺寸。需要大号或小号按钮时，可将 size 属性分别设置为 large 或 small；省略 size 属性时使用默认尺寸。";
    public const string DangerButtonsTitle = "危险按钮";
    public const string DangerButtonsDescription = "danger 是 antd 4.0 之后的按钮属性。";
    public const string CustomIconTitle = "自定义图标";
    public const string CustomIconDescription = "自定义浮出按钮图标。";
    public const string FlyoutTriggerTypeTitle = "浮出触发类型";
    public const string FlyoutTriggerTypeDescription = "支持两种触发类型。";
    public const string P2HeaderCut = "剪切";
    public const string P2HeaderCopy = "复制";
    public const string P2HeaderDelete = "删除";
    public const string P2ContentHoverMe = "悬停我";
    public const string P2ContentLarge = "大号";
    public const string P2ContentMiddle = "中号";
    public const string P2ContentSmall = "小号";
    public const string P2ContentDefault = "默认";
    public const string P2ContentPrimary = "主要";
    public const string P2ContentClickMe = "点我";
    public const string PageSubtitle = "带附加浮出菜单的主操作按钮。";
    public const string PageDescription = "SplitButton 将主命令和次级浮出触发器组合在一起，支持尺寸、危险样式、主要样式、自定义指示图标以及悬停或点击菜单。";
    public const string ComponentCategory = "通用";
    public const string ComponentStatusStable = ".NET 10";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计变量";
    public const string InfoNamespaceLabel = "命名空间";
    public const string InfoPackageLabel = "包";
    public const string InfoBaseClassLabel = "基类";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyContent = "显示在主按钮区域中的内容。";
    public const string ApiPropertyCommand = "主按钮执行的命令。";
    public const string ApiPropertyCommandParameter = "传递给主命令的参数。";
    public const string ApiPropertyFlyout = "次级按钮显示的浮出层。";
    public const string ApiPropertyHotKey = "与主命令关联的键盘手势。";
    public const string ApiPropertyTriggerType = "打开浮出层的指针交互方式。";
    public const string ApiPropertyPlacement = "浮出弹层的请求位置。";
    public const string ApiPropertyPlacementAnchor = "浮出定位使用的弹层锚点。";
    public const string ApiPropertyPlacementGravity = "浮出定位使用的弹层重力方向。";
    public const string ApiPropertyGutterToFlyout = "按钮与浮出层之间的间距。";
    public const string ApiPropertyMouseEnterDelay = "使用悬停触发时打开前的延迟。";
    public const string ApiPropertyMouseLeaveDelay = "使用悬停触发时关闭前的延迟。";
    public const string ApiPropertySizeType = "按钮预设尺寸。";
    public const string ApiPropertyIcon = "显示在主按钮中的可选图标。";
    public const string ApiPropertyOpenIndicator = "显示在次级浮出触发器中的图标。";
    public const string ApiPropertyIsDanger = "应用危险视觉状态。";
    public const string ApiPropertyIsPrimaryButtonType = "为 SplitButton 使用主要按钮样式。";
    public const string ApiPropertyIsMotionEnabled = "控制浮出层动效。";
    public const string ApiPropertyShouldUseOverlayPopup = "控制浮出层是否使用 overlay popup。";
    public const string ApiPropertyIsWaveSpiritEnabled = "控制点击波纹反馈。";
    public const string TokenColumnToken = "变量";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "范围";
    public const string TokenColumnStatus = "状态";
    public const string TokenNamePadding = "默认按钮内边距。";
    public const string TokenNamePaddingLG = "大号按钮内边距。";
    public const string TokenNamePaddingSM = "小号按钮内边距。";
    public const string TokenNameIconSize = "默认图标尺寸。";
    public const string TokenNameIconSizeLG = "大号图标尺寸。";
    public const string TokenNameIconSizeSM = "小号图标尺寸。";
    public const string TokenNameGroupBorderColor = "分组按钮部分之间使用的边框颜色。";
    public const string TokenNameGutterToFlyout = "按钮与浮出层之间的默认间距。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";

    protected override Type GetResourceKindType() => typeof(SplitButtonShowCaseLangResourceKind);
}
