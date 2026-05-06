using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUIGallery.Localization
{
    public enum CaseNavigationLangResourceKind
    {
        DataDisplay,
        DataDisplay_Avatar,
        DataDisplay_Badge,
        DataDisplay_Calendar,
        DataDisplay_Card,
        DataDisplay_Carousel,
        DataDisplay_Collapse,
        DataDisplay_Empty,
        DataDisplay_Expander,
        DataDisplay_GroupBox,
        DataDisplay_InfoFlyout,
        DataDisplay_List,
        DataDisplay_QRCode,
        DataDisplay_Segmented,
        DataDisplay_Tag,
        DataDisplay_Timeline,
        DataDisplay_Tour,
        DataDisplay_TreeView,
        DataEntry,
        DataEntry_AutoComplete,
        DataEntry_CheckBox,
        DataEntry_DatePicker,
        DataEntry_LineEdit,
        DataEntry_NumberUpDown,
        DataEntry_RadioButton,
        DataEntry_Rate,
        DataEntry_Slider,
        DataEntry_TimePicker,
        DataEntry_ToggleSwitch,
        Feedback,
        Feedback_Alert,
        Feedback_Message,
        Feedback_Notification,
        Feedback_PopupConfirm,
        Feedback_ProgressBar,
        Feedback_Result,
        Feedback_Skeleton,
        Feedback_Spin,
        Feedback_Watermark,
        General,
        General_AboutUs,
        General_Button,
        General_Icons,
        General_Palette,
        General_Separator,
        General_SplitButton,
        Layout,
        Layout_FlexPanel,
        Layout_Splitter,
        Navigation,
        Navigation_Breadcrumb,
        Navigation_ButtonSpinner,
        Navigation_ComboBox,
        Navigation_DropdownButton,
        Navigation_Menu,
        Navigation_Pagination,
        Navigation_TabControl
    }

    public class CaseNavigationLangResourceExtension : LanguageResourceExtension<CaseNavigationLangResourceKind>
    {
        public CaseNavigationLangResourceExtension()
        {
        }

        public CaseNavigationLangResourceExtension(CaseNavigationLangResourceKind kind) : base(kind)
        {
        }
    }
}

namespace AtomUIGallery.Localization
{
    public enum WorkspaceWindowLangResourceKind
    {
        MenuItemCompactMode,
        MenuItemDarkMode,
        MenuItemEnableFullScreen,
        MenuItemEnableMaximize,
        MenuItemEnableMinimize,
        MenuItemEnableMotion,
        MenuItemEnableMove,
        MenuItemEnablePin,
        MenuItemEnableResize,
        MenuItemEnableWaveSpirit,
        MenuItemLanguage,
        MenuItemSettings,
        MenuItemTheme,
        MenuItemWindowOptions
    }

    public class WorkspaceWindowLangResourceExtension : LanguageResourceExtension<WorkspaceWindowLangResourceKind>
    {
        public WorkspaceWindowLangResourceExtension()
        {
        }

        public WorkspaceWindowLangResourceExtension(WorkspaceWindowLangResourceKind kind) : base(kind)
        {
        }
    }
}