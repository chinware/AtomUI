using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TreeSelect;

[LanguageProvider(LanguageCode.zh_TW, TreeSelectShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最基礎的用法。";
    public const string ScenarioBasic = "基礎";
    public const string ScenarioBehavior = "行為";
    public const string ScenarioAppearance = "外觀";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計 Token";
    public const string PageSubtitle = "在緊湊輸入框中從層級數據裡選擇值。";
    public const string PageDescription = "TreeSelect 將輸入選擇器與樹形導航結合，適用於層級分類、組織節點、權限和其他嵌套選項集合。";
    public const string ComponentCategory = "數據錄入";
    public const string ComponentStatusStable = "穩定";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyItemsSource = "選擇器展示的層級選項。";
    public const string ApiPropertySelectedItems = "當前選中的樹節點。";
    public const string ApiPropertyIsMultiple = "允許選擇多個樹節點。";
    public const string ApiPropertyIsTreeCheckable = "在樹形彈出層中顯示複選框。";
    public const string ApiPropertyIsDefaultExpandAll = "彈出層打開時默認展開樹節點。";
    public const string ApiPropertyIsAllowClear = "選中值後顯示清除入口。";
    public const string ApiPropertyIsFilterEnabled = "允許從輸入框篩選樹節點。";
    public const string ApiPropertyDataLoader = "樹節點展開時異步加載子節點。";
    public const string ApiPropertyPlacement = "控制彈出層相對於輸入框的位置。";
    public const string ApiPropertyMaxCount = "選中值的最大數量，超過限制後更多選項會被禁用。";
    public const string ApiPropertyStyleVariant = "輸入框視覺變體。";
    public const string ApiPropertyStatus = "選擇器的校驗狀態樣式。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "範圍";
    public const string TokenColumnStatus = "狀態";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";
    public const string TokenNameMinPopupWidth = "TreeSelect 使用的最小彈出層寬度。";
    public const string MultipleSelectionTitle = "多選";
    public const string MultipleSelectionDescription = "多選用法。";
    public const string GenerateFromTreeDataTitle = "由樹數據生成";
    public const string GenerateFromTreeDataDescription = "可以使用 treeData 屬性填充樹結構，這是一種快速簡單地提供樹內容的方式。";
    public const string CheckableTitle = "可勾選";
    public const string CheckableDescription = "多選且可勾選。";
    public const string AsynchronousLoadingTitle = "異步加載";
    public const string AsynchronousLoadingDescription = "異步加載樹節點。";
    public const string PlacementTitle = "彈出位置";
    public const string PlacementDescription = "可以通過 placement 手動指定彈出層位置。";
    public const string ShowTreeLineTitle = "顯示樹線";
    public const string ShowTreeLineDescription = "使用 treeLine 顯示線條樣式。";
    public const string VariantsTitle = "變體";
    public const string VariantsDescription = "TreeSelect 提供四種變體：描邊、填充、無邊框和下划線。";
    public const string StatusTitle = "狀態";
    public const string StatusDescription = "通過 status 為 TreeSelect 添加狀態，可設置為錯誤或警告。";
    public const string PrefixAndSuffixTitle = "前綴和後綴";
    public const string PrefixAndSuffixDescription = "自定義 prefix 和 suffixIcon。";
    public const string MaxCountTitle = "最大數量";
    public const string MaxCountDescription = "可以設置 maxCount 屬性控制最多可選項數量。超過限制後，選項會變為禁用狀態。";
    public const string SizeTypeTitle = "尺寸";
    public const string SizeTypeDescription = "TreeSelect 支持大號、中號、小號，也支持通過 Custom 配合本地高度和字號自定義。";
    public const string P2PlaceholderTextPleaseSelect = "請選擇";
    public const string P2TextPlacement = "彈出位置：";
    public const string P2ContentTopleft = "左上";
    public const string P2ContentTopright = "右上";
    public const string P2ContentBottomleft = "左下";
    public const string P2ContentBottomright = "右下";
    public const string P2ContentLarge = "大號";
    public const string P2ContentDefault = "默認";
    public const string P2ContentSmall = "小號";
    public const string P2ContentCustom = "自定義";

    public const string P2OnContentShowIcon = "顯示圖標";

    public const string P2OffContentShowIcon = "顯示圖標";

    public const string P2OnContentTreeLine = "樹線";

    public const string P2OffContentTreeLine = "樹線";

    public const string P2OnContentShowLeafIcon = "顯示葉子圖標";

    public const string P2OffContentShowLeafIcon = "顯示葉子圖標";
    public const string P2AddOnPrefix = "前綴";
    public const string P2HeaderParent1 = "父節點 1";
    public const string P2HeaderParent10 = "父節點 1-0";
    public const string P2HeaderParent11 = "父節點 1-1";
    public const string P2HeaderLeaf1 = "葉子 1";
    public const string P2HeaderLeaf2 = "葉子 2";
    public const string P2HeaderLeaf3 = "葉子 3";
    public const string P2HeaderLeaf4 = "葉子 4";
    public const string P2HeaderLeaf5 = "葉子 5";
    public const string P2HeaderLeaf6 = "葉子 6";
    public const string P2HeaderLeaf11 = "葉子 11";
    public const string P2HeaderMyLeaf = "我的葉子";
    public const string P2HeaderYourLeaf = "你的葉子";
    public const string P2HeaderSss = "節點 SSS";
    public const string P2HeaderNode1 = "節點 1";
    public const string P2HeaderNode2 = "節點 2";
    public const string P2HeaderChildNode = "子節點";
    public const string P2HeaderChildNode1 = "子節點 1";
    public const string P2HeaderChildNode2 = "子節點 2";
    public const string P2HeaderChildNode3 = "子節點 3";
    public const string P2HeaderChildNode4 = "子節點 4";
    public const string P2HeaderChildNode5 = "子節點 5";
    public const string P2HeaderChildNode6 = "子節點 6";
    public const string P2HeaderChildNode7 = "子節點 7";
    public const string P2HeaderExpandToLoad = "展開加載";
    public const string P2HeaderTreeNode = "樹節點";

}
