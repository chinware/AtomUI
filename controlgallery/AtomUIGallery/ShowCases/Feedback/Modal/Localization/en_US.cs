using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Modal;

[LanguageProvider(LanguageCode.en_US, ModalShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic modal.";
    public const string AsynchronouslyCloseTitle = "Asynchronously close";
    public const string AsynchronouslyCloseDescription = "Asynchronously close a modal dialog when the OK button is pressed. For example, you can use this pattern when you submit a form.";
    public const string MessageBoxStyleTitle = "MessageBox Style";
    public const string MessageBoxStyleDescription = "MessageBox supports Confirm, Information, Warning, and Error";
    public const string LoadingTitle = "Loading";
    public const string LoadingDescription = "Set the loading status of Modal.";
    public const string CustomFooterButtonsTitle = "Custom Footer Buttons";
    public const string CustomFooterButtonsDescription = "You can use standard buttons along with custom buttons.";
    public const string OpenDraggableModalTitle = "Open Draggable Modal";
    public const string OpenDraggableModalDescription = "Custom modal content render and enable implements draggable.";
    public const string ManualUpdateDestroyTitle = "Manual to update destroy";
    public const string ManualUpdateDestroyDescription = "Manually updating and destroying a modal through instance.";
    public const string CustomizeFooterButtonPropsTitle = "Customize footer buttons props";
    public const string CustomizeFooterButtonPropsDescription = "Change dialog button properties by specifying a callback function.";
    public const string StaticDialogApiTitle = "Static Dialog API";
    public const string StaticDialogApiDescription = "Creating a Dialog using the static method of Dialog.";
    public const string P2TitleBasicModal = "Basic Modal";
    public const string P2TitleBasicWindowModal = "Basic Window Modal";
    public const string P2TitleAsynchronouslyCloseModal = "Asynchronously close Modal";
    public const string P2TitleNormal = "Normal";
    public const string P2TitleDoYouWantToDeleteTheseItems = "Do you want to delete these items?";
    public const string P2TitleThisIsANotificationMessage = "This is a notification message";
    public const string P2TitleOperationSuccessful = "Operation successful";
    public const string P2TitleThisIsAnErrorMessage = "This is an error message";
    public const string P2TitleThisIsAWarningMessage = "This is a warning message";
    public const string P2TitleStaticApi = "Static API";
    public const string P2TitleLoadingModal = "Loading Modal";
    public const string P2TitleTitle = "Title";
    public const string P2TitleConfirm = "Confirm";
    public const string P2TitleDraggableModal = "Draggable Modal";
    public const string P2ContentOpenModalOverlay = "Open Modal Overlay";
    public const string P2TextSomeContents = "Some contents...";
    public const string P2ContentOpenModalWindow = "Open Modal Window";
    public const string P2ContentOpenModalWithAsyncLogic = "Open Modal with async logic";
    public const string P2TextContentOfTheModal = "Content of the modal";
    public const string P2TextNativeWindow = "Native Window:";
    public const string P2TextSomeDescriptions = "Some descriptions";
    public const string P2ContentConfirm = "Confirm";
    public const string P2TextSomeMessagesSomeMessages = "some messages...some messages...";
    public const string P2ContentInformation = "Information";
    public const string P2ContentSuccess = "Success";
    public const string P2ContentError = "Error";
    public const string P2ContentWarning = "Warning";
    public const string P2ContentOpenModal = "Open Modal";
    public const string P2ContentCustomButton = "Custom Button";
    public const string P2TextBlaBla = "Bla bla ...";
    public const string P2TextJustDonTLearnPhysicsAtSchoolAnd = "Just don't learn physics at school and your life will be full of magic and miracles.";
    public const string P2TextDayBeforeYesterdayISawARabbitAnd = "Day before yesterday I saw a rabbit, and yesterday a deer, and today, you.";
    public const string P2ContentOpenModalToCloseInN5s = "Open modal to close in 5s";
    public const string P2ContentOpenModalWithCustomizedButtonProps = "Open Modal with customized button props";
    public const string P2ContentOpenOverlayDialog = "Open Overlay Dialog";
    public const string P2ContentOpenWindowDialog = "Open Window Dialog";
    public const string P2ContentOpenCustomviewDialog = "Open CustomView Dialog";
    public const string P2TextName = "Name";
    public const string P2TextAge = "Age";
    public const string P2RunThisModalWillBeDestroyedAfter = "This modal will be destroyed after ";
    public const string P2RunSecond = " second.";

    protected override Type GetResourceKindType() => typeof(ModalShowCaseLangResourceKind);
}
