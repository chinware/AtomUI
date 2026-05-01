using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using AtomUIGallery.Workspace.Views;

namespace AtomUIGallery.Workspace.Localization.CaseNavigationLang;

[LanguageProvider(LanguageCode.en_US, CaseNavigation.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string General = "General";
    public const string General_AboutUs = "AboutUS";
    public const string General_Palette = "Palette";
    public const string General_Icons = "Icons";
    public const string General_Button = "Button";
    public const string General_SplitButton = "SplitButton";
    public const string Layout = "Layout";
    public const string Layout_FlexPanel = "FlexPanel";
    public const string Layout_Splitter = "Splitter";
    public const string Navigation = "Navigation";
    public const string Navigation_Breadcrumb = "Breadcrumb";
    public const string Navigation_ButtonSpinner = "ButtonSpinner";
    public const string DataEntry = "Data Entry";
    public const string DataDisplay = "Data Display";
    public const string Feedback = "Feedback";
    public const string Feedback_Message = "Message";
    public const string Feedback_Notification = "Notification";
    public const string Feedback_PopupConfirm = "PopupConfirm";

    protected override Type GetResourceKindType() => typeof(CaseNavigationLangResourceKind);
}
