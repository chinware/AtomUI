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

    protected override Type GetResourceKindType() => typeof(ModalShowCaseLangResourceKind);
}
