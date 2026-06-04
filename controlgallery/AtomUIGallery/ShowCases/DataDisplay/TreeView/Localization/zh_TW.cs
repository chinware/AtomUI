using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TreeView;

[LanguageProvider(LanguageCode.zh_TW, TreeViewShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string ScenarioBasic = "基礎";
    public const string ScenarioAdvanced = "進階";
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最基礎的用法，展示如何使用 checkable、selectable、disabled、defaultExpandKeys 等屬性。";
    public const string GenerateByTemplateTitle = "使用模板生成";
    public const string GenerateByTemplateDescription = "可以使用 Template 機制生成樹節點。";
    public const string BlockNodeTitle = "塊級節點";
    public const string BlockNodeDescription = "塊級節點。";
    public const string TreeWithLineTitle = "帶連接線的樹";
    public const string TreeWithLineDescription = "節點之間帶連接線的樹。通過 showLine 開啓，並可通過 switcherIcon 自定義預設圖標。";
    public const string DraggableTitle = "可拖拽";
    public const string DraggableDescription = "拖拽 treeNode 可將其插入到其他 treeNode 之後，或插入到其他父級 TreeNode 中。";
    public const string CustomizeExpandIconTitle = "自定義展開/折疊圖標";
    public const string CustomizeExpandIconDescription = "自定義樹節點的展開/折疊圖標。";
    public const string AsyncLoadDataTitle = "異步加載數據";
    public const string AsyncLoadDataDescription = "點擊展開 treeNode 時異步加載數據。";
    public const string SearchableByTemplateTitle = "通過 TreeDataTemplate 搜索";
    public const string SearchableByTemplateDescription = "使用 ItemsSource 數據源的可搜索樹。";
    public const string SearchableTitle = "可搜索";
    public const string SearchableDescription = "可搜索的樹。";
    public const string ContextMenuTitle = "上下文菜單";
    public const string ContextMenuDescription = "右鍵任意節點可打開上下文菜單。切換 IsSelectOnRightClick 可在跟隨右鍵節點（默認）和保持現有選擇不變之間切換。";
    public const string P2HeaderParentN1 = "父節點 1";
    public const string P2HeaderParentN1N0 = "父節點 1-0";
    public const string P2HeaderLeaf = "葉子節點";
    public const string P2HeaderParentN1N1 = "父節點 1-1";
    public const string P2HeaderSss = "示例一";
    public const string P2HeaderCcc = "示例二";
    public const string P2HeaderXxx = "示例三";
    public const string P2HeaderAaaa = "示例四";
    public const string P2HeaderParent = "父節點";
    public const string P2HeaderChildN1 = "子節點 1";
    public const string P2HeaderChildN2 = "子節點 2";
    public const string P2HeaderLeafN1 = "葉子節點 1";
    public const string P2HeaderLeafN2 = "葉子節點 2";
    public const string P2HeaderParentN1N2 = "父節點 1-2";
    public const string P2HeaderParentN2 = "父節點 2";
    public const string P2HeaderParentN2N0 = "父節點 2-0";
    public const string P2HeaderN0N0 = "0-0";
    public const string P2HeaderN0N0N0 = "0-0-0";
    public const string P2HeaderN0N0N0N0 = "0-0-0-0";
    public const string P2HeaderN0N0N0N1 = "0-0-0-1";
    public const string P2HeaderN0N0N0N2 = "0-0-0-2";
    public const string P2HeaderN0N0N1 = "0-0-1";
    public const string P2HeaderN0N0N1N0 = "0-0-1-0";
    public const string P2HeaderN0N0N1N1 = "0-0-1-1";
    public const string P2HeaderN0N0N1N2 = "0-0-1-2";
    public const string P2HeaderN0N0N2 = "0-0-2";
    public const string P2HeaderN0N1 = "0-1";
    public const string P2HeaderN0N1N0 = "0-1-0";
    public const string P2HeaderN0N1N0N0 = "0-1-0-0";
    public const string P2HeaderN0N1N0N1 = "0-1-0-1";
    public const string P2HeaderN0N1N0N2 = "0-1-0-2";
    public const string P2HeaderN0N1N1 = "0-1-1";
    public const string P2HeaderN0N1N1N0 = "0-1-1-0";
    public const string P2HeaderN0N1N1N1 = "0-1-1-1";
    public const string P2HeaderN0N1N1N2 = "0-1-1-2";
    public const string P2HeaderN0N1N2 = "0-1-2";
    public const string P2HeaderN0N2 = "0-2";
    public const string P2PlaceholderTextSearch = "搜索";
    public const string P2HeaderNewNode = "新節點";
    public const string P2HeaderRename = "重命名";
    public const string P2HeaderDelete = "刪除";
    public const string P2TextShowline = "顯示連接線：";
    public const string P2TextShowicon = "顯示圖標：";
    public const string P2TextShowleaficon = "顯示葉子節點圖標：";
    public const string P2TextNodeHoverMode = "節點懸停模式：";
    public const string P2ContentDefault = "默認";
    public const string P2ContentBlock = "塊";
    public const string P2ContentWholeline = "整行";
    public const string P2TextIsselectonrightclick = "右鍵時選擇節點：";
    public const string P2HeaderExpandToLoad = "展開加載";
    public const string P2HeaderTreeNode = "樹節點";
    public const string P2HeaderChildNode = "子節點";
    public const string P2HeaderNodeFallback = "節點";
    public const string P2HeaderNewNodeFormat = "{0} / 新建 ({1})";
    public const string P2HeaderRenamedFormat = "{0}（已重命名）";

    protected override Type GetResourceKindType() => typeof(TreeViewShowCaseLangResourceKind);
}

