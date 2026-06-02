using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Menu;

[LanguageProvider(LanguageCode.zh_CN, MenuShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
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

    protected override Type GetResourceKindType() => typeof(MenuShowCaseLangResourceKind);
}
