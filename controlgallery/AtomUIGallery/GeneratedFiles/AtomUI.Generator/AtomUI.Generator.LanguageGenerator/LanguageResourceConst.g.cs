using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUIGallery.Localization
{
    public enum CaseNavigationLangResourceKind
    {
        DataDisplay,
        DataEntry,
        Feedback,
        General,
        Layout,
        Navigation
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