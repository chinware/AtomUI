using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Message;

[LanguageProvider(LanguageCode.zh_CN, MessageShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "Message 的最简单用法。";
    public const string OtherTypesTitle = "其他消息类型";
    public const string OtherTypesDescription = "success、error 和 warning 类型的消息。";
    public const string LoadingIndicatorTitle = "带加载指示器的消息";
    public const string LoadingIndicatorDescription = "显示一个全局加载指示器，并异步自动关闭。";
    public const string CallbackTitle = "回调";
    public const string CallbackDescription = "上面的示例会在旧消息即将关闭时显示一条新消息。";
    public const string ComponentCategory = "反馈";
    public const string ComponentStatusStable = "稳定";
    public const string PageSubtitle = "用于轻量级操作反馈的全局提示消息。";
    public const string PageDescription = "Message 在当前窗口顶层显示简短反馈，适用于保存结果、校验反馈、异步进度和连续完成提示。";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计变量";
    public const string ApiColumnMember = "成员";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyMessageContent = "消息卡片显示的文本内容。";
    public const string ApiPropertyMessageType = "消息语义类型，用于控制默认图标和视觉状态。";
    public const string ApiPropertyMessageIcon = "可选自定义图标。未设置时会根据消息类型选择默认图标。";
    public const string ApiPropertyMessageExpiration = "自动关闭延迟。使用 TimeSpan.Zero 可保持消息直到手动关闭。";
    public const string ApiPropertyMessageOnClose = "消息关闭后触发的回调。";
    public const string ApiPropertyManagerPosition = "窗口消息管理器使用的屏幕位置。";
    public const string ApiPropertyManagerMaxItems = "可见消息最大数量，超出限制时较早的可见消息会关闭。";
    public const string ApiPropertyManagerIsMotionEnabled = "是否启用消息打开和关闭动效。";
    public const string ApiMethodManagerShow = "显示一个 IMessage 实例，并应用可选样式类。";
    public const string TokenColumnToken = "变量";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "范围";
    public const string TokenColumnStatus = "状态";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";
    public const string TokenNameMessageContentBg = "消息卡片的背景色。";
    public const string TokenNameMessageContentPadding = "消息卡片内容区域的内边距。";
    public const string TokenNameMessageCardHeight = "消息卡片预留的默认高度。";
    public const string TokenNameMessageIconSize = "消息状态图标尺寸。";
    public const string TokenNameMessageIconMargin = "消息状态图标的外边距。";
    public const string TokenNameMessageTopMargin = "消息卡片堆叠时使用的顶层外边距。";
    public const string P2ContentDisplayNormalMessage = "显示普通消息";
    public const string P2ContentSuccess = "成功";
    public const string P2ContentInfo = "信息";
    public const string P2ContentWarning = "警告";
    public const string P2ContentError = "错误";
    public const string P2ContentDisplayALoadingIndicator = "显示加载指示器";
    public const string P2MessageHelloAtomUIAvalonia = "你好，AtomUI/Avalonia！";
    public const string P2MessageInformation = "这是一条信息消息。";
    public const string P2MessageSuccess = "这是一条成功消息。";
    public const string P2MessageWarning = "这是一条警告消息。";
    public const string P2MessageError = "这是一条错误消息。";
    public const string P2MessageActionInProgress = "操作进行中...";
    public const string P2MessageLoadingFinished = "加载完成";

}
