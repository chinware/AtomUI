using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Mentions;

[LanguageProvider(LanguageCode.zh_CN, MentionsShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最简单的用法。";
    public const string VariantsTitle = "变体";
    public const string VariantsDescription = "Mentions 提供四种变体：描边、填充、无边框和下划线。";
    public const string AsynchronousLoadingTitle = "异步加载";
    public const string AsynchronousLoadingDescription = "异步加载。";
    public const string CustomizeTriggerTokenTitle = "自定义触发标记";
    public const string CustomizeTriggerTokenDescription = "通过 prefix 属性自定义触发标记，默认为 @，也支持数组。";
    public const string DisabledOrReadOnlyTitle = "禁用或只读";
    public const string DisabledOrReadOnlyDescription = "配置 disabled 和 readOnly。";
    public const string PlacementTitle = "弹出位置";
    public const string PlacementDescription = "改变建议列表的弹出位置。";
    public const string StatusTitle = "状态";
    public const string StatusDescription = "通过 status 为 Mentions 添加状态，可设置为错误或警告。";
    public const string AutoSizeTitle = "自动高度";
    public const string AutoSizeDescription = "高度自动调整。";
    public const string WithClearIconTitle = "带清除图标";
    public const string WithClearIconDescription = "自定义清除按钮。";
    public const string P2PlaceholderTextOutlined = "线框风格";
    public const string P2PlaceholderTextFilled = "填充风格";
    public const string P2PlaceholderTextBorderless = "无边框";
    public const string P2PlaceholderTextUnderlined = "下划线";
    public const string P2PlaceholderTextInputToMentionPeopleToMentionTag = "输入 @ 提及成员，输入 # 提及标签";
    public const string P2PlaceholderTextThisIsDisabledMentions = "这是禁用状态的 Mentions";
    public const string P2PlaceholderTextThisIsReadonlyMentions = "这是只读状态的 Mentions";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计 Token";
    public const string PageSubtitle = "在输入过程中提及成员、标签或自定义实体。";
    public const string PageDescription =
        "Mentions 提供基于触发符的候选弹窗、异步选项加载、自定义触发符、输入变体、弹出位置、状态和自动高度场景。";
    public const string ComponentCategory = "数据录入";
    public const string ComponentStatusStable = "稳定";
    public const string InfoNamespaceLabel = "命名空间";
    public const string InfoPackageLabel = "包";
    public const string InfoBaseClassLabel = "基类";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyValue = "Mentions 输入框的当前文本值。";
    public const string ApiPropertyDefaultValue = "控件创建时应用的初始文本值。";
    public const string ApiPropertyOptionsSource = "用于提及建议的静态选项数据源。";
    public const string ApiPropertyOptionsAsyncLoader = "根据当前提及上下文提供选项的异步加载器。";
    public const string ApiPropertyOptionTemplate = "用于渲染每一个建议选项的模板。";
    public const string ApiPropertyTriggerPrefix = "打开候选列表的触发符。";
    public const string ApiPropertySplit = "选择提及选项后插入的分隔文本。";
    public const string ApiPropertyIsAllowClear = "当输入有内容时显示清除入口。";
    public const string ApiPropertyClearIcon = "清除入口使用的自定义图标。";
    public const string ApiPropertyStyleVariant = "输入框视觉变体，例如描边、填充、无边框或下划线。";
    public const string ApiPropertyStatus = "Mentions 表面显示的校验状态。";
    public const string ApiPropertyPlacement = "候选列表弹窗的首选弹出位置。";
    public const string ApiPropertyIsAutoSize = "允许输入框高度随文本内容增长。";
    public const string ApiPropertyLines = "初始可见文本行数。";
    public const string ApiPropertyMinLines = "自动高度使用的最小可见文本行数。";
    public const string ApiPropertyMaxLines = "自动高度使用的最大可见文本行数。";
    public const string ApiPropertyDisplayCandidateCount = "用于计算弹窗高度的候选行数。";
    public const string ApiPropertyIsReadOnly = "禁止编辑，同时保留内容可读。";
    public const string ApiPropertyAsyncLoadDebounce = "调用异步选项加载器前的延迟时间。";
    public const string ApiPropertyAsyncLoadTimeout = "异步选项加载的最大等待时间。";
    public const string ApiPropertyShouldUseOverlayPopup = "打开建议时使用 overlay popup 宿主。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "状态";
    public const string TokenNamePopupContentPadding = "候选弹窗内部边距。";
    public const string TokenNameOptionHeight = "每个建议选项的高度。";
    public const string TokenNameMinPopupWidth = "候选弹窗最小宽度。";
    public const string TokenScopeComponent = "Mentions";
    public const string TokenStatusStable = "稳定";

    protected override Type GetResourceKindType() => typeof(MentionsShowCaseLangResourceKind);
}
