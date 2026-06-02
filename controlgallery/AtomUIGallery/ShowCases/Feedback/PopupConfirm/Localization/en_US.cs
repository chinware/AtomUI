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

    protected override Type GetResourceKindType() => typeof(PopupConfirmShowCaseLangResourceKind);
}
