using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.PopupConfirm;

[LanguageProvider(LanguageCode.en_US, PopupConfirmShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicUsageTitle = "Basic usage";
    public const string BasicUsageDescription = "The basic example supports the title and description props of confirmation.";
    public const string LocaleTextTitle = "Locale text";
    public const string LocaleTextDescription = "Set okText and cancelText props to customize the button's labels.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "There are 12 placement options available.";
    public const string CustomizeIconTitle = "Customize icon";
    public const string CustomizeIconDescription = "Set icon props to customize the icon.";
    public const string ComponentCategory = "Feedback";
    public const string ComponentStatusStable = "Stable";
    public const string PageSubtitle = "Inline confirmation before a lightweight destructive or risky action.";
    public const string PageDescription = "PopupConfirm combines a trigger control with a confirmation flyout. It is useful when an operation needs a quick second step but does not justify a blocking modal dialog.";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ApiColumnMember = "Member";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyPopupConfirmTitle = "Title text displayed in the confirmation flyout.";
    public const string ApiPropertyPopupConfirmConfirmContent = "Content displayed below the title in the confirmation flyout.";
    public const string ApiPropertyPopupConfirmConfirmContentTemplate = "Optional template used to render custom confirmation content.";
    public const string ApiPropertyPopupConfirmOkText = "Text displayed by the confirm button.";
    public const string ApiPropertyPopupConfirmCancelText = "Text displayed by the cancel button.";
    public const string ApiPropertyPopupConfirmOkButtonType = "Button type used by the confirm action.";
    public const string ApiPropertyPopupConfirmIsShowCancelButton = "Controls whether the cancel button is displayed.";
    public const string ApiPropertyPopupConfirmIcon = "Optional icon displayed before the confirmation title.";
    public const string ApiPropertyPopupConfirmConfirmStatus = "Semantic status of the confirmation content.";
    public const string ApiEventPopupConfirmCancelled = "Raised when the cancel action is chosen.";
    public const string ApiEventPopupConfirmConfirmed = "Raised when the confirm action is chosen.";
    public const string ApiEventPopupConfirmPopupClick = "Raised after either confirmation flyout button is clicked.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";
    public const string TokenNamePopupMinWidth = "Minimum width of the popup confirmation panel.";
    public const string TokenNamePopupMinHeight = "Minimum height of the popup confirmation panel.";
    public const string TokenNameButtonSpacing = "Spacing between popup confirmation buttons.";
    public const string TokenNameIconMargin = "Outer margin around the status icon.";
    public const string TokenNameContentContainerMargin = "Outer margin of the main confirmation content area.";
    public const string TokenNameButtonContainerMargin = "Outer margin of the button action area.";
    public const string TokenNameTitleMargin = "Outer margin around the confirmation title.";
    public const string P2ConfirmContentAreYouSureToDeleteThisTask = "Are you sure to delete this task?";
    public const string P2OkTextOk = "Ok";
    public const string P2CancelTextCancel = "Cancel";
    public const string P2TitleDeleteTheTask = "Delete the task";
    public const string P2ContentDelete = "Delete";
    public const string P2ContentLt = "LT";
    public const string P2ContentLeft = "Left";
    public const string P2ContentLb = "LB";
    public const string P2ContentTl = "TL";
    public const string P2ContentTop = "Top";
    public const string P2ContentTr = "TR";
    public const string P2ContentRt = "RT";
    public const string P2ContentRight = "Right";
    public const string P2ContentRb = "RB";
    public const string P2ContentBl = "BL";
    public const string P2ContentBottom = "Bottom";
    public const string P2ContentBr = "BR";

    protected override Type GetResourceKindType() => typeof(PopupConfirmShowCaseLangResourceKind);
}
