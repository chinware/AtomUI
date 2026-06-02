using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using AtomUIGallery.Workspace.Views;

namespace AtomUIGallery.Workspace.Localization.CaseNavigationLang;

[LanguageProvider(LanguageCode.en_US, CaseNavigation.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string General = "General";
    public const string General_AboutUs = "About Us";
    public const string General_Palette = "Palette";
    public const string General_Icons = "Icons";
    public const string General_Button = "Button";
    public const string General_FloatButton = "Float Button";
    public const string General_SplitButton = "Split Button";
    public const string General_Separator = "Separator";
    public const string General_CustomizeTheme = "Customize Theme";

    public const string Layout = "Layout";
    public const string Layout_FlexPanel = "Flex Panel";
    public const string Layout_Grid = "Grid";
    public const string Layout_Space = "Space";
    public const string Layout_Splitter = "Splitter";

    public const string Navigation = "Navigation";
    public const string Navigation_Breadcrumb = "Breadcrumb";
    public const string Navigation_ButtonSpinner = "Button Spinner";
    public const string Navigation_ComboBox = "Combo Box";
    public const string Navigation_DropdownButton = "Dropdown Button";
    public const string Navigation_Menu = "Menu";
    public const string Navigation_Pagination = "Pagination";
    public const string Navigation_Steps = "Steps";
    public const string Navigation_TabControl = "Tab Control";

    public const string DataEntry = "Data Entry";
    public const string DataEntry_AutoComplete = "Auto Complete";
    public const string DataEntry_Cascader = "Cascader";
    public const string DataEntry_CheckBox = "Check Box";
    public const string DataEntry_ColorPicker = "Color Picker";
    public const string DataEntry_DatePicker = "Date Picker";
    public const string DataEntry_TimePicker = "Time Picker";
    public const string DataEntry_Form = "Form";
    public const string DataEntry_LineEdit = "Line Edit";
    public const string DataEntry_Mentions = "Mentions";
    public const string DataEntry_NumberUpDown = "Number Up Down";
    public const string DataEntry_RadioButton = "Radio Button";
    public const string DataEntry_Rate = "Rate";
    public const string DataEntry_Select = "Select";
    public const string DataEntry_Slider = "Slider";
    public const string DataEntry_ToggleSwitch = "Toggle Switch";
    public const string DataEntry_TreeSelect = "Tree Select";
    public const string DataEntry_Transfer = "Transfer";
    public const string DataEntry_Upload = "Upload";

    public const string DataDisplay = "Data Display";
    public const string DataDisplay_Avatar = "Avatar";
    public const string DataDisplay_Badge = "Badge";
    public const string DataDisplay_Calendar = "Calendar";
    public const string DataDisplay_Card = "Card";
    public const string DataDisplay_Carousel = "Carousel";
    public const string DataDisplay_Collapse = "Collapse";
    public const string DataDisplay_Descriptions = "Descriptions";
    public const string DataDisplay_DataGrid = "Data Grid";
    public const string DataDisplay_Expander = "Expander";
    public const string DataDisplay_Empty = "Empty";
    public const string DataDisplay_ImagePreviewer = "Image Previewer";
    public const string DataDisplay_GroupBox = "Group Box";
    public const string DataDisplay_InfoFlyout = "Info Flyout";
    public const string DataDisplay_List = "List";
    public const string DataDisplay_QRCode = "QR Code";
    public const string DataDisplay_Segmented = "Segmented";
    public const string DataDisplay_Statistic = "Statistic";
    public const string DataDisplay_Tag = "Tag";
    public const string DataDisplay_Timeline = "Timeline";
    public const string DataDisplay_TreeView = "Tree View";
    public const string DataDisplay_Tooltip = "Tooltip";
    public const string DataDisplay_Tour = "Tour";

    public const string Feedback = "Feedback";
    public const string Feedback_Alert = "Alert";
    public const string Feedback_Drawer = "Drawer";
    public const string Feedback_Message = "Message";
    public const string Feedback_Modal = "Modal";
    public const string Feedback_Notification = "Notification";
    public const string Feedback_PopupConfirm = "Popup Confirm";
    public const string Feedback_ProgressBar = "Progress Bar";
    public const string Feedback_Result = "Result";
    public const string Feedback_Skeleton = "Skeleton";
    public const string Feedback_Spin = "Spin";
    public const string Feedback_Watermark = "Watermark";

    protected override Type GetResourceKindType() => typeof(CaseNavigationLangResourceKind);
}
