using AtomUI.Theme.Language;
using AtomUIGallery.Workspace.Views;

namespace AtomUIGallery.Workspace.Localization.WorkspaceWindowLang;

[LanguageProvider(LanguageCode.en_US, WorkspaceWindow.LanguageId, Constants.LanguageCatalog)]
internal class en_US : LanguageProvider
{
    public const string MenuItemSettings = "Settings";
    public const string MenuItemTheme = "Theme";
    public const string MenuItemLanguage = "Language";
    
    public const string MenuItemWindowOptions = "Window Options";
    public const string MenuItemEnableFullScreen = "Enable FullScreen";
    public const string MenuItemEnablePin = "Enable Pin";
    public const string MenuItemEnableMinimize = "Enable Minimize";
    public const string MenuItemEnableMaximize = "Enable Maximize";
    public const string MenuItemEnableMove = "Enable Move";
    public const string MenuItemEnableResize = "Enable Resize";
        
    public const string MenuItemDarkMode = "Dark Mode";
    public const string MenuItemCompactMode = "Compact Mode";
    public const string MenuItemEnableMotion = "Enable Motion";
    public const string MenuItemEnableWaveSpirit = "Enable WaveSpirit";
}