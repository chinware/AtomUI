using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Alert;

[LanguageProvider(LanguageCode.en_US, AlertShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The simplest usage for short messages.";
    public const string MoreTypesTitle = "More types";
    public const string MoreTypesDescription = "There are 4 types of Alert: success, info, warning, error.";
    public const string ClosableTitle = "Closable";
    public const string ClosableDescription = "To show close button.";
    public const string DescriptionTitle = "Description";
    public const string DescriptionDescription = "Additional description for alert message.";
    public const string IconTitle = "Icon";
    public const string IconDescription = "A relevant icon makes alert information clearer.";
    public const string CustomActionTitle = "Custom action";
    public const string CustomActionDescription = "Custom action.";
    public const string LoopBannerTitle = "Loop Banner";
    public const string LoopBannerDescription = "Show a loop banner.";
    public const string P2DescriptionErrorDescriptionErrorDescriptionErrorDescriptionErrorDesc = "Error Description Error Description Error Description Error Description Error Description Error Description";
    public const string P2DescriptionSuccessDescriptionSuccessDescriptionSuccessDescription = "Success Description Success Description Success Description";
    public const string P2DescriptionInfoDescriptionInfoDescriptionInfoDescriptionInfoDescript = "Info Description Info Description Info Description Info Description";
    public const string P2DescriptionWarningDescriptionWarningDescriptionWarningDescriptionWar = "Warning Description Warning Description Warning Description Warning Description";
    public const string P2DescriptionErrorDescriptionErrorDescriptionErrorDescriptionErrorDesc2 = "Error Description Error Description Error Description Error Description";
    public const string P2DescriptionDetailedDescriptionAndAdviceAboutSuccessfulCopywriting = "Detailed description and advice about successful copywriting.";
    public const string P2DescriptionAdditionalDescriptionAndInformationAboutCopywriting = "Additional description and information about copywriting.";
    public const string P2DescriptionThisIsAWarningNoticeAboutCopywriting = "This is a warning notice about copywriting.";
    public const string P2DescriptionThisIsAnErrorMessageAboutCopywriting = "This is an error message about copywriting.";
    public const string P2ContentSuccessText = "Success Text";
    public const string P2ContentInfoText = "Info Text";
    public const string P2ContentWarningText = "Warning Text";
    public const string P2ContentErrorText = "Error Text";
    public const string P2ContentWarningTextWarningTextWarningTextWarningText = "Warning Text Warning Text Warning Text Warning Text Warning Text Warning TextWarning Text";
    public const string P2ContentUndo = "UNDO";
    public const string P2ContentDetail = "Detail";
    public const string P2ContentDone = "Done";
    public const string P2ContentAccept = "Accept";
    public const string P2ContentDecline = "Decline";
    public const string P2ContentICanBeAReactComponentMultipleReact = "I can be custom content, multiple controls, or just some text, Info Description Info Description Info Description Info Description";

    public const string P2MessageSuccessText = "Success Text";

    public const string P2MessageInfoText = "Info Text";

    public const string P2MessageWarningText = "Warning Text";

    public const string P2MessageErrorText = "Error Text";

    public const string P2MessageSuccessTips = "Success Tips";

    public const string P2MessageInformationalNotes = "Informational Notes";

    public const string P2MessageWarning = "Warning";

    public const string P2MessageError = "Error";

    protected override Type GetResourceKindType() => typeof(AlertShowCaseLangResourceKind);
}
