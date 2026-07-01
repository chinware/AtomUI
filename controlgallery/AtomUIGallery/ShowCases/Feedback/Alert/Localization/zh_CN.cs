using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Alert;

[LanguageProvider(LanguageCode.zh_CN, AlertShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "用于短消息提示的最简单用法。";
    public const string MoreTypesTitle = "更多类型";
    public const string MoreTypesDescription = "Alert 有 success、info、warning、error 四种类型。";
    public const string ClosableTitle = "可关闭";
    public const string ClosableDescription = "显示关闭按钮。";
    public const string DescriptionTitle = "含描述信息";
    public const string DescriptionDescription = "为提示消息添加额外描述。";
    public const string IconTitle = "图标";
    public const string IconDescription = "合适的图标可以让提示信息更清晰。";
    public const string CustomActionTitle = "自定义操作";
    public const string CustomActionDescription = "自定义操作。";
    public const string LoopBannerTitle = "循环横幅";
    public const string LoopBannerDescription = "展示循环横幅。";
    public const string PageSubtitle = "醒目的上下文反馈信息。";
    public const string PageDescription = "Alert 用于展示重要状态信息和可选操作，不打断当前工作流。";
    public const string ComponentCategory = "反馈";
    public const string ComponentStatusStable = ".NET 10";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计令牌";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyType = "设置反馈类型和对应的状态色。";
    public const string ApiPropertyMessage = "提示框中展示的主要文本内容。";
    public const string ApiPropertyDescription = "展示在主要文本下方的可选辅助说明。";
    public const string ApiPropertyIsShowIcon = "显示与提示类型匹配的状态图标。";
    public const string ApiPropertyIsMessageMarqueeEnabled = "长单行内容可使用跑马灯方式展示。";
    public const string ApiPropertyIsClosable = "显示关闭按钮并启用关闭请求。";
    public const string ApiPropertyCloseIcon = "关闭按钮使用的自定义图标。";
    public const string ApiPropertyExtraAction = "展示在提示框右侧的可选操作内容。";
    public const string ApiEventCloseRequest = "用户点击关闭按钮时触发。";
    public const string TokenColumnToken = "令牌";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameDefaultPadding = "不含描述信息的紧凑提示内间距。";
    public const string TokenNameWithDescriptionPadding = "包含描述信息时的提示内间距。";
    public const string TokenNameMessageWithDescriptionMargin = "包含描述信息时消息文本下方的外间距。";
    public const string TokenNameIconDefaultMargin = "紧凑提示中的图标外间距。";
    public const string TokenNameIconWithDescriptionMargin = "包含描述信息时的图标外间距。";
    public const string TokenNameIconSize = "紧凑提示中的状态图标尺寸。";
    public const string TokenNameWithDescriptionIconSize = "包含描述信息时的状态图标尺寸。";
    public const string TokenNameCloseIconSize = "关闭按钮图标尺寸。";
    public const string TokenNameExtraElementMargin = "关闭按钮和额外操作元素前的外间距。";
    public const string TokenNameDescriptionLabelMargin = "描述文本标签上方的外间距。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";
    public const string P2DescriptionErrorDescriptionErrorDescriptionErrorDescriptionErrorDesc = "错误描述 错误描述 错误描述 错误描述 错误描述 错误描述";
    public const string P2DescriptionSuccessDescriptionSuccessDescriptionSuccessDescription = "成功描述 成功描述 成功描述";
    public const string P2DescriptionInfoDescriptionInfoDescriptionInfoDescriptionInfoDescript = "信息描述 信息描述 信息描述 信息描述";
    public const string P2DescriptionWarningDescriptionWarningDescriptionWarningDescriptionWar = "警告描述 警告描述 警告描述 警告描述";
    public const string P2DescriptionErrorDescriptionErrorDescriptionErrorDescriptionErrorDesc2 = "错误描述 错误描述 错误描述 错误描述";
    public const string P2DescriptionDetailedDescriptionAndAdviceAboutSuccessfulCopywriting = "关于成功提示文案的详细说明和建议。";
    public const string P2DescriptionAdditionalDescriptionAndInformationAboutCopywriting = "关于提示文案的补充说明和信息。";
    public const string P2DescriptionThisIsAWarningNoticeAboutCopywriting = "这是一条关于提示文案的警告通知。";
    public const string P2DescriptionThisIsAnErrorMessageAboutCopywriting = "这是一条关于提示文案的错误信息。";
    public const string P2ContentSuccessText = "成功文本";
    public const string P2ContentInfoText = "信息文本";
    public const string P2ContentWarningText = "警告文本";
    public const string P2ContentErrorText = "错误文本";
    public const string P2ContentWarningTextWarningTextWarningTextWarningText = "警告文本 警告文本 警告文本 警告文本 警告文本 警告文本";
    public const string P2ContentUndo = "撤销";
    public const string P2ContentDetail = "详情";
    public const string P2ContentDone = "完成";
    public const string P2ContentAccept = "接受";
    public const string P2ContentDecline = "拒绝";
    public const string P2ContentICanBeAReactComponentMultipleReact = "这里可以是自定义内容、多个控件，或只是一段普通文本，信息描述 信息描述 信息描述 信息描述";

    public const string P2MessageSuccessText = "成功提示文本";

    public const string P2MessageInfoText = "信息提示文本";

    public const string P2MessageWarningText = "警告提示文本";

    public const string P2MessageErrorText = "错误提示文本";

    public const string P2MessageSuccessTips = "成功提示";

    public const string P2MessageInformationalNotes = "信息说明";

    public const string P2MessageWarning = "警告";

    public const string P2MessageError = "错误";

    protected override Type GetResourceKindType() => typeof(AlertShowCaseLangResourceKind);
}
