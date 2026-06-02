using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Message;

[LanguageProvider(LanguageCode.zh_CN, MessageShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "Message 的最简单用法。";
    public const string OtherTypesTitle = "其他消息类型";
    public const string OtherTypesDescription = "success、error 和 warning 类型的消息。";
    public const string LoadingIndicatorTitle = "带加载指示器的消息";
    public const string LoadingIndicatorDescription = "显示一个全局加载指示器，并异步自动关闭。";
    public const string CallbackTitle = "回调";
    public const string CallbackDescription = "上面的示例会在旧消息即将关闭时显示一条新消息。";

    protected override Type GetResourceKindType() => typeof(MessageShowCaseLangResourceKind);
}
