using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.PopupConfirm;

[LanguageProvider(LanguageCode.en_US, PopupConfirmShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicUsageTitle = "Basic usage";
    public const string BasicUsageDescription = "The basic example supports the title and description props of confirmation.";
    public const string LocaleTextTitle = "Locale text";
    public const string LocaleTextDescription = "Set okText and cancelText props to customize the button's labels.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "There are 12 placement options available.";
    public const string CustomizeIconTitle = "Customize icon";
    public const string CustomizeIconDescription = "Set icon props to customize the icon.";
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
