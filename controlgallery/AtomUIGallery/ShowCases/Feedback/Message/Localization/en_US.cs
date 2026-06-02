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

    protected override Type GetResourceKindType() => typeof(MessageShowCaseLangResourceKind);
}
