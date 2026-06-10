using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Menu;

[LanguageProvider(LanguageCode.zh_CN, MenuShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string ScenarioBasic = "基础";
    public const string ScenarioFeatures = "特性";
    public const string ScenarioItemsSource = "数据源";
    public const string ScenarioContext = "上下文";
    public const string ScenarioNavMenu = "导航菜单";

    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最简单的用法。";
    public const string MenuItemItemsSourceTitle = "通过 ItemsSource 生成 MenuItem";
    public const string MenuItemItemsSourceDescription = "基于 ItemsSource 和模板生成结构。";
    public const string InlineNavMenuItemsSourceTitle = "通过 ItemsSource 生成内联 NavMenu";
    public const string InlineNavMenuItemsSourceDescription = "基于 ItemsSource 和模板生成内联 NavMenu 结构。";
    public const string IconAndSubmenuTitle = "图标和子菜单";
    public const string IconAndSubmenuDescription = "带图标和子菜单的用法。";
    public const string ToggleTypeTitle = "带 ToggleType 的菜单项";
    public const string ToggleTypeDescription = "在菜单中渲染复选框或单选按钮。";
    public const string ScrollableTitle = "可滚动菜单";
    public const string ScrollableDescription = "当菜单项过多时，会出现上下滚动按钮。";
    public const string ContextMenuTitle = "上下文菜单";
    public const string ContextMenuDescription = "右键打开上下文菜单。";
    public const string ContextMenuItemsSourceTitle = "上下文菜单：通过 ItemsSource 生成 MenuItem";
    public const string ContextMenuItemsSourceDescription = "右键打开上下文菜单。";
    public const string MenuFlyoutTitle = "菜单浮出层";
    public const string MenuFlyoutDescription = "右键显示上下文浮出层。";
    public const string VerticalNavMenuTitle = "垂直导航菜单";
    public const string VerticalNavMenuDescription = "子菜单以弹出层方式打开。";
    public const string InlineMenuTitle = "内联菜单";
    public const string InlineMenuDescription = "带内联子菜单的垂直菜单。";
    public const string TopNavigationTitle = "顶部导航";
    public const string TopNavigationDescription = "水平顶部导航菜单。";
    public const string SwitchMenuTypeTitle = "切换菜单类型";
    public const string SwitchMenuTypeDescription = "展示 inline 和 vertical 模式之间的动态切换。";
    public const string DefaultOpenedPathsTitle = "默认展开路径";
    public const string DefaultOpenedPathsDescription = "可以默认展开或选中指定路径。";
    public const string NavMenuItemItemsSourceTitle = "通过 ItemsSource 生成 NavMenuItem";
    public const string NavMenuItemItemsSourceDescription = "基于 ItemsSource 和模板生成结构。";
    public const string P2HeaderFile = "_文件";
    public const string P2HeaderNewTextFile = "新建文本文件";
    public const string P2HeaderNewFile = "新建文件";
    public const string P2HeaderNewWindow = "新建窗口";
    public const string P2HeaderEdit = "_编辑";
    public const string P2HeaderUndo = "撤销";
    public const string P2HeaderCut = "剪切";
    public const string P2HeaderDisabledItem = "禁用项";
    public const string P2HeaderSave = "保存";
    public const string P2HeaderSaveAs = "另存为...";
    public const string P2HeaderSaveAll = "全部保存";
    public const string P2HeaderExit = "退出";
    public const string P2HeaderCopy = "复制";
    public const string P2HeaderDelete = "删除";
    public const string P2HeaderPaste = "粘贴";
    public const string P2HeaderPasteFromHistory = "从历史记录粘贴";
    public const string P2HeaderMenuA = "_菜单 A";
    public const string P2HeaderDisabled = "禁用";
    public const string P2HeaderMenu = "_菜单";
    public const string P2HeaderMenuItem = "菜单项";
    public const string P2TextRightClickToShowContextMenu = "右键显示上下文菜单";
    public const string P2TitleNormal = "普通";
    public const string P2TextRightClickToShowContextFlyout = "右键显示上下文浮出层";
    public const string P2TitleGeneratedByTemplate = "通过模板生成";
    public const string P2HeaderNavigationOne = "导航一";
    public const string P2HeaderNavigationTwo = "导航二";
    public const string P2HeaderNavigationThreeSubmenu = "导航三 - 子菜单";
    public const string P2HeaderItemN1 = "项目 1";
    public const string P2HeaderOptionN1 = "选项 1";
    public const string P2HeaderOptionN2 = "选项 2";
    public const string P2HeaderItemN2 = "项目 2";
    public const string P2HeaderOptionN3 = "选项 3";
    public const string P2HeaderOptionN4 = "选项 4";
    public const string P2HeaderNavigationFour = "导航四";
    public const string P2TextChangeMode = "切换模式";
    public const string P2TextChangeStyle = "切换样式";

    protected override Type GetResourceKindType() => typeof(MenuShowCaseLangResourceKind);
}
