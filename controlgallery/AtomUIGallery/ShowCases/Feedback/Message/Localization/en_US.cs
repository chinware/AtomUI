using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Message;

[LanguageProvider(LanguageCode.en_US, MessageShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The simplest usage for Message.";
    public const string OtherTypesTitle = "Other types of message";
    public const string OtherTypesDescription = "Messages of success, error and warning types.";
    public const string LoadingIndicatorTitle = "Message with loading indicator";
    public const string LoadingIndicatorDescription = "Display a global loading indicator, which is dismissed by itself asynchronously.";
    public const string CallbackTitle = "Callback";
    public const string CallbackDescription = "The above example will display a new message when the old message is about to close.";
    public const string P2ContentDisplayNormalMessage = "Display normal message";
    public const string P2ContentSuccess = "Success";
    public const string P2ContentInfo = "Info";
    public const string P2ContentWarning = "Warning";
    public const string P2ContentError = "Error";
    public const string P2ContentDisplayALoadingIndicator = "Display a loading indicator";
    public const string P2MessageHelloAtomUIAvalonia = "Hello, AtomUI/Avalonia!";
    public const string P2MessageInformation = "This is an information message.";
    public const string P2MessageSuccess = "This is a success message.";
    public const string P2MessageWarning = "This is a warning message.";
    public const string P2MessageError = "This is an error message.";
    public const string P2MessageActionInProgress = "Action in progress...";
    public const string P2MessageLoadingFinished = "Loading finished";

    protected override Type GetResourceKindType() => typeof(MessageShowCaseLangResourceKind);
}
