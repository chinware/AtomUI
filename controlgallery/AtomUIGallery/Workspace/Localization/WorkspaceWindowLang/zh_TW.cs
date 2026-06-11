using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using AtomUIGallery.Workspace.Views;

namespace AtomUIGallery.Workspace.Localization.WorkspaceWindowLang;

[LanguageProvider(LanguageCode.zh_TW, WorkspaceWindow.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string WindowTitle = "AtomUI 桌面控件庫";

    public const string MenuItemSettings = "設置";
    public const string MenuItemTheme = "主題";
    public const string MenuItemLanguage = "語言";

    public const string MenuItemWindowOptions = "窗口選項";
    public const string MenuItemEnableFullScreen = "開啓全屏";
    public const string MenuItemEnablePin = "開啓窗口固定";
    public const string MenuItemEnableMinimize = "開啓最小化";
    public const string MenuItemEnableMaximize = "開啓最大化";
    public const string MenuItemEnableMove = "開啓窗口移動";
    public const string MenuItemEnableResize = "開啓窗口設置大小";

    public const string MenuItemDarkMode = "暗黑模式";
    public const string MenuItemCompactMode = "緊湊模式";
    public const string MenuItemEnableMotion = "開啓動效";
    public const string MenuItemEnableWaveSpirit = "開啓波浪動畫";

    protected override Type GetResourceKindType() => typeof(WorkspaceWindowLangResourceKind);
}

