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
    public const string General_Separator = "Separator";
    public const string Layout = "Layout";
    public const string Layout_FlexPanel = "FlexPanel";
    public const string Layout_Splitter = "Splitter";
    public const string Navigation = "Navigation";
    public const string Navigation_Breadcrumb = "Breadcrumb";
    public const string Navigation_ButtonSpinner = "ButtonSpinner";
    public const string Navigation_ComboBox = "ComboBox";
    public const string Navigation_DropdownButton = "DropdownButton";
    public const string DataEntry = "Data Entry";
    public const string DataEntry_CheckBox = "CheckBox";
    public const string DataEntry_ToggleSwitch = "ToggleSwitch";
    public const string DataEntry_RadioButton = "RadioButton";
    public const string DataEntry_Rate = "Rate";
    public const string DataEntry_Slider = "Slider";
    public const string DataDisplay = "Data Display";
    public const string DataDisplay_Avatar = "Avatar";
    public const string DataDisplay_Calendar = "Calendar";
    public const string DataDisplay_Card = "Card";
    public const string DataDisplay_Carousel = "Carousel";
    public const string DataDisplay_Collapse = "Collapse";
    public const string DataDisplay_Empty = "Empty";
    public const string DataDisplay_Expander = "Expander";
    public const string DataDisplay_Tag = "Tag";
    public const string DataDisplay_Segmented = "Segmented";
    public const string DataDisplay_GroupBox = "GroupBox";
    public const string Feedback = "Feedback";
    public const string Feedback_Alert = "Alert";
    public const string Feedback_Message = "Message";
    public const string Feedback_Notification = "Notification";
    public const string Feedback_PopupConfirm = "PopupConfirm";
    public const string Feedback_Skeleton = "Skeleton";
    public const string Feedback_Spin = "Spin";

    protected override Type GetResourceKindType() => typeof(CaseNavigationLangResourceKind);
}
