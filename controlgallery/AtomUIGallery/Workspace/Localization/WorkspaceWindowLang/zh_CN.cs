using AtomUI.Theme.Language;
using AtomUIGallery.Workspace.Views;

namespace AtomUIGallery.Workspace.Localization.WorkspaceWindowLang;

[LanguageProvider(LanguageCode.zh_CN, WorkspaceWindow.LanguageId, Constants.LanguageCatalog)]
internal class zh_CN : LanguageProvider
{
    public const string MenuItemSettings = "设置";
    public const string MenuItemTheme = "主题";
    public const string MenuItemLanguage = "语言";
    
    public const string MenuItemWindowOptions = "窗口选项";
    public const string MenuItemEnableFullScreen = "开启全屏";
    public const string MenuItemEnablePin = "开启窗口固定";
    public const string MenuItemEnableMinimize = "开启最小化";
    public const string MenuItemEnableMaximize = "开启最大化";
    public const string MenuItemEnableMove = "开启窗口移动";
    public const string MenuItemEnableResize = "开启窗口设置大小";
    
    public const string MenuItemDarkMode = "暗黑模式";
    public const string MenuItemCompactMode = "紧凑模式";
    public const string MenuItemEnableMotion = "开启动效";
    public const string MenuItemEnableWaveSpirit = "开启波浪动画";
}