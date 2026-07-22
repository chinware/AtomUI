using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.FloatButton;

[LanguageProvider(LanguageCode.zh_CN, FloatButtonShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的用法。";
    public const string TypeTitle = "类型";
    public const string TypeDescription = "通过 type 属性改变 FloatButton 的类型。";
    public const string CommandTitle = "命令";
    public const string CommandDescription = "FloatButtonHost 会将命令转发给真实悬浮按钮，组合内的 FloatButton 子项也能继承宿主数据上下文进行命令绑定。";
    public const string ShapeTitle = "形状";
    public const string ShapeDescription = "通过 shape 属性改变 FloatButton 的形状。";
    public const string TooltipTitle = "带提示的 FloatButton";
    public const string TooltipDescription = "设置 tooltip 属性可以显示带提示的 FloatButton。";
    public const string DescriptionTitle = "描述";
    public const string DescriptionDescription = "设置 description 属性可以显示带描述的 FloatButton。";
    public const string GroupTitle = "FloatButton 组合";
    public const string GroupDescription = "多个按钮一起使用时推荐使用 FloatButtonGroup。通过设置 FloatButton.Group 的 shape 属性可以改变组合形状，组合形状会覆盖内部 FloatButton 的形状。";
    public const string MenuModeTitle = "菜单模式";
    public const string MenuModeDescription = "通过 trigger 打开菜单模式，可选择 hover 或 click。";
    public const string ControlledModeTitle = "受控模式";
    public const string ControlledModeDescription = "通过 open 将组件设置为受控模式，IsOpen 默认双向绑定，并与 trigger 一起工作。";
    public const string PlacementTitle = "弹出位置";
    public const string PlacementDescription = "自定义动画弹出位置，提供上、右、下、左四种预设位置，默认在上方。";
    public const string BadgeTitle = "徽标";
    public const string BadgeDescription = "带 Badge 的 FloatButton。";
    public const string BackTopTitle = "回到顶部";
    public const string BackTopDescription = "BackTop 可以方便地回到页面顶部。";
    public const string P2TooltipSinceN5N25N0 = "自 5.25.0 起";
    public const string P2TooltipDocuments = "文档";
    public const string P2DescriptionHelpInfo = "帮助信息";
    public const string P2TextScrollToBottom = "滚动到底部";
    public const string P2CommandCount = "执行次数";
    public const string P2CommandSource = "来源";
    public const string P2CommandHost = "宿主按钮";
    public const string P2CommandGroupChild = "组合子按钮";
    public const string PageSubtitle = "在滚动区域中持续可用的悬浮操作。";
    public const string PageDescription = "FloatButton 提供重要操作的快速入口，支持组合菜单、徽标和回到顶部行为，并将控件锚定在当前内容区域。";
    public const string ComponentCategory = "通用";
    public const string ComponentStatusStable = ".NET 10";
    public const string ScenarioExamples = "示例";

}
