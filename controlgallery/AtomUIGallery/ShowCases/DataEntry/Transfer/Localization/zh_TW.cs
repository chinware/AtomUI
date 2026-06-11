using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Transfer;

[LanguageProvider(LanguageCode.zh_TW, TransferShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "Transfer 的基礎用法需要提供源數據、目標 keys 數組，以及渲染和部分回調函數。";
    public const string ScenarioBasic = "基礎";
    public const string ScenarioAdvanced = "高級";
    public const string ScenarioTreeStatus = "樹與狀態";
    public const string OneWayTitle = "單向模式";
    public const string OneWayDescription = "使用 oneWay 讓 Transfer 呈現單向樣式。";
    public const string SearchTitle = "搜索";
    public const string SearchDescription = "帶搜索框的 Transfer。";
    public const string AdvancedTitle = "高級用法";
    public const string AdvancedDescription = "Transfer 的高級用法。可以自定義穿梭按鈕標籤、列寬和列高，以及頁腳中展示的內容。";
    public const string PaginationTitle = "分頁";
    public const string PaginationDescription = "通過分頁承載大量條目。";
    public const string TreeTransferTitle = "樹形穿梭框";
    public const string TreeTransferDescription = "使用 Tree 組件自定義渲染列表。";
    public const string StatusTitle = "狀態";
    public const string StatusDescription = "通過 status 為 Transfer 添加狀態，可設置為錯誤或警告。";
    public const string P2SourceTitle = "源列表";
    public const string P2TargetTitle = "目標列表";
    public const string P2TextText = "-";
    public const string P2HeaderName = "姓名";
    public const string P2HeaderTag = "標籤";
    public const string P2HeaderDescription = "描述";
    public const string P2ContentLeftButtonReload = "重新加載左側";
    public const string P2ContentRightButtonReload = "重新加載右側";

    public const string P2OnContentDisable = "禁用";

    public const string P2OffContentEnable = "啓用";

    public const string P2FilterPlaceholderTextSearchHere = "在此搜索";

    public const string P2ToSourceButtonTextToLeft = "移到左側";

    public const string P2ToTargetButtonTextToRight = "移到右側";

    public const string P2OnContentOnyWay = "單向";

    public const string P2OffContentOnyWay = "單向";
    public const string P2ItemContentFormat = "內容{0}";
    public const string P2ItemDescriptionFormat = "內容{0}的描述";
    public const string P2TagCat = "貓";
    public const string P2TagDog = "狗";
    public const string P2TagBird = "鳥";

    protected override Type GetResourceKindType() => typeof(TransferShowCaseLangResourceKind);
}

