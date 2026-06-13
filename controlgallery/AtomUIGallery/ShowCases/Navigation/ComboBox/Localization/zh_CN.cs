using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ComboBox;

[LanguageProvider(LanguageCode.zh_CN, ComboBoxShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string ComponentCategory = "导航";
    public const string ComponentStatusStable = "稳定";
    public const string PageSubtitle = "用于从紧凑弹出列表中选择内容的选择输入控件。";
    public const string PageDescription = "ComboBox 将输入式布局与下拉选择结合，支持项模板、前后置标签、内部前后缀、校验状态和尺寸形态。";
    public const string InfoNamespaceLabel = "命名空间";
    public const string InfoPackageLabel = "包";
    public const string InfoBaseClassLabel = "基类";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计令牌";

    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "基础的组合框用法。";
    public const string ItemsSourceTitle = "通过 ItemsSource 生成 ComboBoxItem";
    public const string ItemsSourceDescription = "基于 ItemsSource 和模板生成结构。";
    public const string DisabledTitle = "禁用状态";
    public const string DisabledDescription = "禁用状态的组合框。";
    public const string ThreeSizesTitle = "三种尺寸";
    public const string ThreeSizesDescription = "ComboBox 提供三种尺寸：大号（40px）、默认（32px）和小号（24px）。";
    public const string VariantsTitle = "不同形态";
    public const string VariantsDescription = "输入框的不同形态。";
    public const string PrePostTabTitle = "前置/后置标签";
    public const string PrePostTabDescription = "前置和后置标签的使用示例。";
    public const string PrefixSuffixTitle = "前缀和后缀";
    public const string PrefixSuffixDescription = "在输入框内部添加前缀或后缀图标。";
    public const string StatusTitle = "状态";
    public const string StatusDescription = "通过 status 为输入框添加错误或警告状态。";
    public const string P2PlaceholderTextPleaseSelect = "请选择";
    public const string P2ContentPoemLine1 = "床前明月光";
    public const string P2ContentPoemLine2 = "疑是地上霜";
    public const string P2ContentPoemLine3 = "举头望明月";
    public const string P2ContentPoemLine4 = "低头思故乡";

    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyItemsSource = "用于生成弹出项的集合。";
    public const string ApiPropertySelectedItem = "当前选中项。";
    public const string ApiPropertySelectedIndex = "当前选中项索引。";
    public const string ApiPropertyPlaceholderText = "未选择内容时显示的占位文本。";
    public const string ApiPropertyLeftAddOn = "显示在输入区域外左侧的内容。";
    public const string ApiPropertyRightAddOn = "显示在输入区域外右侧的内容。";
    public const string ApiPropertyContentLeftAddOn = "显示在输入区域内选中内容前方的内容。";
    public const string ApiPropertyContentRightAddOn = "显示在输入区域内选中内容后方的内容。";
    public const string ApiPropertySizeType = "控制 ComboBox 尺寸。";
    public const string ApiPropertyStyleVariant = "控制描边、填充和无边框视觉形态。";
    public const string ApiPropertyStatus = "应用校验状态样式。";
    public const string ApiPropertyIsAllowClear = "允许清除当前选择。";
    public const string ApiPropertyOptionFontSize = "覆盖弹出选项字号。";
    public const string ApiPropertyDropDownDisplayPageSize = "控制出现滚动前展示的选项数量。";
    public const string ApiPropertyShouldUseOverlayPopup = "控制下拉是否使用 Overlay Popup 宿主。";
    public const string ApiPropertyIsMotionEnabled = "在主题允许时启用控件动效。";

    public const string TokenColumnToken = "令牌";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "范围";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameControlWidth = "继承自 ButtonSpinner 的默认控件宽度。";
    public const string TokenNameHandleWidth = "下拉操作柄宽度。";
    public const string TokenNameHandleIconSize = "下拉操作柄图标尺寸。";
    public const string TokenNameHandleBg = "下拉操作柄背景色。";
    public const string TokenNameHandleActiveBg = "下拉操作柄激活背景色。";
    public const string TokenNameHandleHoverColor = "下拉操作柄悬浮前景色。";
    public const string TokenNameHandleBorderColor = "下拉操作柄边框色。";
    public const string TokenNameFilledHandleBg = "填充形态下的下拉操作柄背景色。";
    public const string TokenNameInputFontSize = "继承自 LineEdit 的默认输入字号。";
    public const string TokenNameInputFontSizeLG = "继承自 LineEdit 的大号输入字号。";
    public const string TokenNameInputFontSizeSM = "继承自 LineEdit 的小号输入字号。";
    public const string TokenNamePopupContentPadding = "下拉弹出内容内边距。";
    public const string TokenNameItemColor = "选项文本色。";
    public const string TokenNameItemHoverColor = "选项悬浮文本色。";
    public const string TokenNameItemSelectedColor = "选中选项文本色。";
    public const string TokenNameItemDisabledColor = "禁用选项文本色。";
    public const string TokenNameItemBgColor = "选项背景色。";
    public const string TokenNameItemHoverBgColor = "选项悬浮背景色。";
    public const string TokenNameItemSelectedBgColor = "选中选项背景色。";
    public const string TokenNameItemPadding = "选项内容内边距。";
    public const string TokenNameItemMargin = "选项外边距。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";

    protected override Type GetResourceKindType() => typeof(ComboBoxShowCaseLangResourceKind);
}
