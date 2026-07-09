using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Message;

[LanguageProvider(LanguageCode.en_US, MessageShowCase.LanguageId)]
internal partial class en_US
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The simplest usage for Message.";
    public const string OtherTypesTitle = "Other types of message";
    public const string OtherTypesDescription = "Messages of success, error and warning types.";
    public const string LoadingIndicatorTitle = "Message with loading indicator";
    public const string LoadingIndicatorDescription = "Display a global loading indicator, which is dismissed by itself asynchronously.";
    public const string CallbackTitle = "Callback";
    public const string CallbackDescription = "The above example will display a new message when the old message is about to close.";
    public const string ComponentCategory = "Feedback";
    public const string ComponentStatusStable = "Stable";
    public const string PageSubtitle = "Global prompt messages for lightweight operation feedback.";
    public const string PageDescription = "Message displays brief feedback at the top layer of the current window. It is useful for save results, validation feedback, async progress, and chained completion notices.";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ApiColumnMember = "Member";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyMessageContent = "Text content displayed by the message card.";
    public const string ApiPropertyMessageType = "Message semantic type. It controls the default icon and visual status.";
    public const string ApiPropertyMessageIcon = "Optional custom icon. When unset, the icon is selected from the message type.";
    public const string ApiPropertyMessageExpiration = "Auto-close delay. Use TimeSpan.Zero to keep the message until it is closed manually.";
    public const string ApiPropertyMessageOnClose = "Callback invoked after the message is closed.";
    public const string ApiPropertyManagerPosition = "Screen position used by the window message manager.";
    public const string ApiPropertyManagerMaxItems = "Maximum number of visible messages. Older visible messages close when the limit is exceeded.";
    public const string ApiPropertyManagerIsMotionEnabled = "Enables the message open and close motion.";
    public const string ApiMethodManagerShow = "Shows an IMessage instance and applies optional style classes.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";
    public const string TokenNameMessageContentBg = "Background color of the message card.";
    public const string TokenNameMessageContentPadding = "Inner padding of the message card content.";
    public const string TokenNameMessageCardHeight = "Default height reserved by the message card.";
    public const string TokenNameMessageIconSize = "Size of the message status icon.";
    public const string TokenNameMessageIconMargin = "Outer margin around the message status icon.";
    public const string TokenNameMessageTopMargin = "Top-level margin used when stacking message cards.";
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

}
