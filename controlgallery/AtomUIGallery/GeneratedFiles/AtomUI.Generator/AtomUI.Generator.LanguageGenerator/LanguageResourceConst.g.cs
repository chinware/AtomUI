using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUIGallery.Localization
{
    public enum CaseNavigationLangResourceKind
    {
        DataDisplay,
        DataDisplay_Avatar,
        DataDisplay_Calendar,
        DataDisplay_Card,
        DataDisplay_Carousel,
        DataDisplay_Collapse,
        DataEntry,
        DataEntry_CheckBox,
        DataEntry_RadioButton,
        DataEntry_Rate,
        Feedback,
        Feedback_Alert,
        Feedback_Message,
        Feedback_Notification,
        Feedback_PopupConfirm,
        Feedback_Skeleton,
        Feedback_Spin,
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
        Navigation_ComboBox
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