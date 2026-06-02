using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TreeView;

[LanguageProvider(LanguageCode.zh_CN, TreeViewShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的用法，展示如何使用 checkable、selectable、disabled、defaultExpandKeys 等属性。";
    public const string GenerateByTemplateTitle = "使用模板生成";
    public const string GenerateByTemplateDescription = "可以使用 Template 机制生成树节点。";
    public const string BlockNodeTitle = "块级节点";
    public const string BlockNodeDescription = "块级节点。";
    public const string TreeWithLineTitle = "带连接线的树";
    public const string TreeWithLineDescription = "节点之间带连接线的树。通过 showLine 开启，并可通过 switcherIcon 自定义预设图标。";
    public const string DraggableTitle = "可拖拽";
    public const string DraggableDescription = "拖拽 treeNode 可将其插入到其他 treeNode 之后，或插入到其他父级 TreeNode 中。";
    public const string CustomizeExpandIconTitle = "自定义展开/折叠图标";
    public const string CustomizeExpandIconDescription = "自定义树节点的展开/折叠图标。";
    public const string AsyncLoadDataTitle = "异步加载数据";
    public const string AsyncLoadDataDescription = "点击展开 treeNode 时异步加载数据。";
    public const string SearchableByTemplateTitle = "通过 TreeDataTemplate 搜索";
    public const string SearchableByTemplateDescription = "使用 ItemsSource 数据源的可搜索树。";
    public const string SearchableTitle = "可搜索";
    public const string SearchableDescription = "可搜索的树。";
    public const string ContextMenuTitle = "上下文菜单";
    public const string ContextMenuDescription = "右键任意节点可打开上下文菜单。切换 IsSelectOnRightClick 可在跟随右键节点（默认）和保持现有选择不变之间切换。";

    protected override Type GetResourceKindType() => typeof(TreeViewShowCaseLangResourceKind);
}
