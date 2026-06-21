using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ToggleSwitch;

[LanguageProvider(LanguageCode.zh_CN, ToggleSwitchShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计变量";
    public const string ComponentCategory = "数据录入";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "在两个互斥状态之间切换，并支持文本、图标、加载和尺寸变体。";
    public const string PageDescription = "ToggleSwitch 用于即时的开关选择，支持禁用、加载、自定义开关内容、图标内容、尺寸变体、动效和波纹反馈。";
    public const string InfoNamespaceLabel = "命名空间：";
    public const string InfoPackageLabel = "包：";
    public const string InfoBaseClassLabel = "基类：";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyIsChecked = "从 ToggleButton 继承的当前选中状态。";
    public const string ApiPropertyGrooveBackground = "覆盖开关轨道背景画刷。";
    public const string ApiPropertyOnContent = "开关选中时显示的内容。";
    public const string ApiPropertyOnContentTemplate = "用于渲染 OnContent 的模板。";
    public const string ApiPropertyOffContent = "开关未选中时显示的内容。";
    public const string ApiPropertyOffContentTemplate = "用于渲染 OffContent 的模板。";
    public const string ApiPropertySizeType = "控制 Large、Middle、Small 或 Custom 开关尺寸。";
    public const string ApiPropertyIsLoading = "显示加载指示器和等待交互状态。";
    public const string ApiPropertyIsMotionEnabled = "启用或禁用开关动效。";
    public const string ApiPropertyIsWaveSpiritEnabled = "启用或禁用波纹反馈。";
    public const string TokenColumnToken = "变量";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "范围";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameTrackHeight = "默认开关轨道高度。";
    public const string TokenNameTrackHeightSM = "小号开关轨道高度。";
    public const string TokenNameTrackMinWidth = "默认开关轨道最小宽度。";
    public const string TokenNameTrackMinWidthSM = "小号开关轨道最小宽度。";
    public const string TokenNameTrackPadding = "开关轨道内部边距。";
    public const string TokenNameHandleBg = "开关把手背景色。";
    public const string TokenNameHandleShadow = "开关把手阴影。";
    public const string TokenNameHandleSize = "默认开关把手尺寸。";
    public const string TokenNameHandleSizeSM = "小号开关把手尺寸。";
    public const string TokenNameInnerMinMargin = "默认内容布局的最小内部边距。";
    public const string TokenNameInnerMaxMargin = "默认内容布局的最大内部边距。";
    public const string TokenNameInnerMinMarginSM = "小号内容布局的最小内部边距。";
    public const string TokenNameInnerMaxMarginSM = "小号内容布局的最大内部边距。";
    public const string TokenNameIconSize = "默认内容图标尺寸。";
    public const string TokenNameIconSizeSM = "小号内容图标尺寸。";
    public const string TokenNameSwitchColor = "开关激活色。";
    public const string TokenNameSwitchDisabledOpacity = "禁用开关状态使用的透明度。";
    public const string TokenNameExtraInfoFontSize = "默认开关文本字号。";
    public const string TokenNameExtraInfoFontSizeSM = "小号开关文本字号。";
    public const string TokenNameLoadingAnimationDuration = "加载指示器动画周期。";
    public const string TokenNameOffStateLoadIndicatorColor = "关闭状态下的加载指示器颜色。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的用法。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "Switch 的禁用状态。";
    public const string TextAndIconTitle = "文本和图标";
    public const string TextAndIconDescription = "带文本和图标。";
    public const string TwoSizesTitle = "尺寸";
    public const string TwoSizesDescription = "SizeType 控制预设和 Custom 开关尺寸模式。";
    public const string LoadingTitle = "加载中";
    public const string LoadingDescription = "标记开关的等待状态。";
    public const string P2ContentToggleDisabled = "切换禁用";
    public const string P2ContentToggleLoading = "切换加载";
    public const string P2ContentCustom = "自定义";

    public const string P2OnContentOn = "开";

    public const string P2OffContentOff = "关";

    public const string P2OnContentText = "开";

    public const string P2OffContentText = "关";

    protected override Type GetResourceKindType() => typeof(ToggleSwitchShowCaseLangResourceKind);
}
