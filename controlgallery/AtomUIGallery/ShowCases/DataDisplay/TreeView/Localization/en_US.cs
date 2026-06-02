using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TreeView;

[LanguageProvider(LanguageCode.en_US, TreeViewShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic usage, tell you how to use checkable, selectable, disabled, defaultExpandKeys, and etc.";
    public const string GenerateByTemplateTitle = "Generate by template";
    public const string GenerateByTemplateDescription = "You can use the Template mechanism to generate tree nodes";
    public const string BlockNodeTitle = "Block Node";
    public const string BlockNodeDescription = "Block Node.";
    public const string TreeWithLineTitle = "Tree with line";
    public const string TreeWithLineDescription = "Tree with connected line between nodes, turn on by showLine, customize the preset icon by switcherIcon.";
    public const string DraggableTitle = "draggable";
    public const string DraggableDescription = "Drag treeNode to insert after the other treeNode or insert into the other parent TreeNode.";
    public const string CustomizeExpandIconTitle = "Customize collapse/expand icon";
    public const string CustomizeExpandIconDescription = "customize collapse/expand icon of tree node";
    public const string AsyncLoadDataTitle = "load data asynchronously";
    public const string AsyncLoadDataDescription = "To load data asynchronously when click to expand a treeNode.";
    public const string SearchableByTemplateTitle = "Searchable by TreeDataTemplate";
    public const string SearchableByTemplateDescription = "Searchable Tree use ItemsSource datasource.";
    public const string SearchableTitle = "Searchable";
    public const string SearchableDescription = "Searchable Tree.";
    public const string ContextMenuTitle = "Context menu";
    public const string ContextMenuDescription = "Right-click any node to open a context menu. Toggle IsSelectOnRightClick to switch between following the right-clicked node (default) and keeping the existing selection untouched.";

    protected override Type GetResourceKindType() => typeof(TreeViewShowCaseLangResourceKind);
}
