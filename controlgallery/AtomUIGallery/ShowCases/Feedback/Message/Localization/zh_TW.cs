using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Message;

[LanguageProvider(LanguageCode.zh_TW, MessageShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "Message 的最簡單用法。";
    public const string OtherTypesTitle = "其他消息類型";
    public const string OtherTypesDescription = "success、error 和 warning 類型的消息。";
    public const string LoadingIndicatorTitle = "帶加載指示器的消息";
    public const string LoadingIndicatorDescription = "顯示一個全局加載指示器，並異步自動關閉。";
    public const string CallbackTitle = "回調";
    public const string CallbackDescription = "上面的示例會在舊消息即將關閉時顯示一條新消息。";
    public const string P2ContentDisplayNormalMessage = "顯示普通消息";
    public const string P2ContentSuccess = "成功";
    public const string P2ContentInfo = "信息";
    public const string P2ContentWarning = "警告";
    public const string P2ContentError = "錯誤";
    public const string P2ContentDisplayALoadingIndicator = "顯示加載指示器";
    public const string P2MessageHelloAtomUIAvalonia = "你好，AtomUI/Avalonia！";
    public const string P2MessageInformation = "這是一條信息消息。";
    public const string P2MessageSuccess = "這是一條成功消息。";
    public const string P2MessageWarning = "這是一條警告消息。";
    public const string P2MessageError = "這是一條錯誤消息。";
    public const string P2MessageActionInProgress = "操作進行中...";
    public const string P2MessageLoadingFinished = "加載完成";

    protected override Type GetResourceKindType() => typeof(MessageShowCaseLangResourceKind);
}

