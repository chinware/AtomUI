using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Menu;

[LanguageProvider(LanguageCode.zh_TW, MenuShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string ScenarioBasic = "基礎";
    public const string ScenarioFeatures = "特性";
    public const string ScenarioItemsSource = "數據源";
    public const string ScenarioContext = "上下文";
    public const string ScenarioNavMenu = "導航菜單";

    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最簡單的用法。";
    public const string MenuItemItemsSourceTitle = "通過 ItemsSource 生成 MenuItem";
    public const string MenuItemItemsSourceDescription = "基於 ItemsSource 和模板生成結構。";
    public const string InlineNavMenuItemsSourceTitle = "通過 ItemsSource 生成內聯 NavMenu";
    public const string InlineNavMenuItemsSourceDescription = "基於 ItemsSource 和模板生成內聯 NavMenu 結構。";
    public const string IconAndSubmenuTitle = "圖標和子菜單";
    public const string IconAndSubmenuDescription = "帶圖標和子菜單的用法。";
    public const string ToggleTypeTitle = "帶 ToggleType 的菜單項";
    public const string ToggleTypeDescription = "在菜單中渲染復選框或單選按鈕。";
    public const string ScrollableTitle = "可滾動菜單";
    public const string ScrollableDescription = "當菜單項過多時，會出現上下滾動按鈕。";
    public const string ContextMenuTitle = "上下文菜單";
    public const string ContextMenuDescription = "右鍵打開上下文菜單。";
    public const string ContextMenuItemsSourceTitle = "上下文菜單：通過 ItemsSource 生成 MenuItem";
    public const string ContextMenuItemsSourceDescription = "右鍵打開上下文菜單。";
    public const string MenuFlyoutTitle = "菜單浮出層";
    public const string MenuFlyoutDescription = "右鍵顯示上下文浮出層。";
    public const string VerticalNavMenuTitle = "垂直導航菜單";
    public const string VerticalNavMenuDescription = "子菜單以彈出層方式打開。";
    public const string InlineMenuTitle = "內聯菜單";
    public const string InlineMenuDescription = "帶內聯子菜單的垂直菜單。";
    public const string TopNavigationTitle = "頂部導航";
    public const string TopNavigationDescription = "水平頂部導航菜單。";
    public const string SwitchMenuTypeTitle = "切換菜單類型";
    public const string SwitchMenuTypeDescription = "展示 inline 和 vertical 模式之間的動態切換。";
    public const string DefaultOpenedPathsTitle = "默認展開路徑";
    public const string DefaultOpenedPathsDescription = "可以默認展開或選中指定路徑。";
    public const string NavMenuItemItemsSourceTitle = "通過 ItemsSource 生成 NavMenuItem";
    public const string NavMenuItemItemsSourceDescription = "基於 ItemsSource 和模板生成結構。";
    public const string P2HeaderFile = "_文件";
    public const string P2HeaderNewTextFile = "新建文本文件";
    public const string P2HeaderNewFile = "新建文件";
    public const string P2HeaderNewWindow = "新建窗口";
    public const string P2HeaderEdit = "_編輯";
    public const string P2HeaderUndo = "撤銷";
    public const string P2HeaderCut = "剪切";
    public const string P2HeaderDisabledItem = "禁用項";
    public const string P2HeaderSave = "保存";
    public const string P2HeaderSaveAs = "另存為...";
    public const string P2HeaderSaveAll = "全部保存";
    public const string P2HeaderExit = "退出";
    public const string P2HeaderCopy = "複製";
    public const string P2HeaderDelete = "刪除";
    public const string P2HeaderPaste = "粘貼";
    public const string P2HeaderPasteFromHistory = "從歷史記錄粘貼";
    public const string P2HeaderMenuA = "_菜單 A";
    public const string P2HeaderDisabled = "禁用";
    public const string P2HeaderMenu = "_菜單";
    public const string P2HeaderMenuItem = "菜單項";
    public const string P2TextRightClickToShowContextMenu = "右鍵顯示上下文菜單";
    public const string P2TitleNormal = "普通";
    public const string P2TextRightClickToShowContextFlyout = "右鍵顯示上下文浮出層";
    public const string P2TitleGeneratedByTemplate = "通過模板生成";
    public const string P2HeaderNavigationOne = "導航一";
    public const string P2HeaderNavigationTwo = "導航二";
    public const string P2HeaderNavigationThreeSubmenu = "導航三 - 子菜單";
    public const string P2HeaderItemN1 = "項目 1";
    public const string P2HeaderOptionN1 = "選項 1";
    public const string P2HeaderOptionN2 = "選項 2";
    public const string P2HeaderItemN2 = "項目 2";
    public const string P2HeaderOptionN3 = "選項 3";
    public const string P2HeaderOptionN4 = "選項 4";
    public const string P2HeaderNavigationFour = "導航四";
    public const string P2TextChangeMode = "切換模式";
    public const string P2TextChangeStyle = "切換樣式";

    protected override Type GetResourceKindType() => typeof(MenuShowCaseLangResourceKind);
}

