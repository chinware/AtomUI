using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Message;

[LanguageProvider(LanguageCode.zh_CN, MessageShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "Message 的最简单用法。";
    public const string OtherTypesTitle = "其他消息类型";
    public const string OtherTypesDescription = "success、error 和 warning 类型的消息。";
    public const string LoadingIndicatorTitle = "带加载指示器的消息";
    public const string LoadingIndicatorDescription = "显示一个全局加载指示器，并异步自动关闭。";
    public const string CallbackTitle = "回调";
    public const string CallbackDescription = "上面的示例会在旧消息即将关闭时显示一条新消息。";
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

    protected override Type GetResourceKindType() => typeof(MessageShowCaseLangResourceKind);
}
