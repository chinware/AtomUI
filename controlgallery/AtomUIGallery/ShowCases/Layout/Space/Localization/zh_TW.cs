using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Space;

[LanguageProvider(LanguageCode.zh_TW, SpaceShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "為擁擠的組件添加水平間距。";
    public const string VerticalSpaceTitle = "垂直間距";
    public const string VerticalSpaceDescription = "為擁擠的組件添加垂直間距。";
    public const string SizeTitle = "間距尺寸";
    public const string SizeDescription = "使用 size 設置間距，預設 small、middle、large 三種尺寸，也可以自定義間距。未設置 size 時，間距為 small。";
    public const string AlignTitle = "對齊";
    public const string AlignDescription = "配置項目對齊方式。";
    public const string WrapTitle = "自動換行";
    public const string WrapDescription = "自動換行。";
    public const string SplitTitle = "分隔符";
    public const string SplitDescription = "為擁擠的組件添加分隔符。";
    public const string CompactFormTitle = "表單緊湊模式";
    public const string CompactFormDescription = "表單組件的緊湊模式。";
    public const string CompactButtonTitle = "按鈕緊湊模式";
    public const string CompactButtonDescription = "按鈕組件的緊湊示例。";
    public const string VerticalCompactTitle = "垂直緊湊模式";
    public const string VerticalCompactDescription = "Space.Compact 的垂直模式，僅支持 Button。";
    public const string ScenarioBasic = "基礎";
    public const string ScenarioSize = "尺寸";
    public const string ScenarioAlign = "對齊";
    public const string ScenarioCompactForm = "緊湊表單";
    public const string ScenarioCompactButton = "緊湊按鈕";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計變量";
    public const string PageSubtitle = "用一致的間距或緊湊組合排列控件。";
    public const string PageDescription = "Space 提供水平和垂直間距、項目對齊、分隔渲染以及緊湊輸入和按鈕組合，適合高密度工具界面。";
    public const string InfoNamespaceLabel = "命名空間";
    public const string InfoPackageLabel = "包";
    public const string InfoBaseClassLabel = "基類";
    public const string ComponentCategory = "佈局";
    public const string ComponentStatusStable = "穩定";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertySpaceItemSpacing = "控制主軸方向上項目之間的間距。";
    public const string ApiPropertySpaceLineSpacing = "控制換行行或垂直行之間的間距。";
    public const string ApiPropertySpaceOrientation = "設置子控件按水平或垂直方向排列。";
    public const string ApiPropertySpaceItemsAlignment = "控制每一行內子控件在交叉軸上的對齊方式。";
    public const string ApiPropertySpaceSizeType = "使用預設 token 間距尺寸或自定義間距模式。";
    public const string ApiPropertySpaceItemWidth = "設置後為每個項目應用固定寬度。";
    public const string ApiPropertySpaceItemHeight = "設置後為每個項目應用固定高度。";
    public const string ApiPropertySpaceSplitTemplate = "在相鄰項目之間創建分隔內容。";
    public const string ApiPropertyCompactSpaceOrientation = "設置緊湊組合的方向。";
    public const string ApiPropertyCompactSpaceSizeType = "向兼容的緊湊子控件傳遞尺寸。";
    public const string ApiPropertyCompactSpaceItemSize = "用於控制單個緊湊項比例或固定尺寸的附加屬性。";
    public const string TokenColumnToken = "變量";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "範圍";
    public const string TokenColumnStatus = "狀態";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusDefault = "默認";
    public const string TokenNameGapSmallSize = "小號間距尺寸。";
    public const string TokenNameGapMiddleSize = "中號間距尺寸。";
    public const string TokenNameGapLargeSize = "大號間距尺寸。";
    public const string TokenNameAddonBg = "緊湊附加項背景色。";
    public const string TokenNameAddOnPadding = "默認緊湊附加項水平內邊距。";
    public const string TokenNameAddOnPaddingSM = "小號緊湊附加項水平內邊距。";
    public const string TokenNameAddOnPaddingLG = "大號緊湊附加項水平內邊距。";
    public const string P2ConfirmContentAreYouSureToDeleteThisTask = "確定要刪除這個任務嗎？";
    public const string P2OkTextOk = "確定";
    public const string P2CancelTextCancel = "取消";
    public const string P2TitleDeleteTheTask = "刪除任務";
    public const string P2HeaderCard = "卡片";
    public const string P2HeaderReport = "舉報";
    public const string P2HeaderMail = "郵件";
    public const string P2HeaderMobile = "手機";
    public const string P2HeaderN1stItem = "第 1 項";
    public const string P2HeaderN2ndItem = "第 2 項";
    public const string P2HeaderN3rdItem = "第 3 項";
    public const string P2HeaderZhejiang = "浙江";
    public const string P2HeaderJiangsu = "江蘇";
    public const string P2TextXihuDistrictHangzhou = "杭州市西湖區";
    public const string P2TextN1 = "+1";
    public const string P2HeaderOption1 = "選項 1";
    public const string P2HeaderOption2 = "選項 2";
    public const string P2TextInputContent = "輸入內容";
    public const string P2HeaderOption1N1 = "選項 1-1";
    public const string P2HeaderOption2N1 = "選項 2-1";
    public const string P2HeaderOption2N2 = "選項 2-2";
    public const string P2HeaderBetween = "介於";
    public const string P2HeaderExcept = "排除";
    public const string P2PlaceholderTextMinimum = "最小值";
    public const string P2PlaceholderTextText = "~";
    public const string P2PlaceholderTextMaximum = "最大值";
    public const string P2HeaderSignUp = "註冊";
    public const string P2HeaderSignIn = "登錄";
    public const string P2PlaceholderTextEmail = "郵箱";
    public const string P2HeaderTextN1 = "文本 1";
    public const string P2HeaderTextN2 = "文本 2";
    public const string P2PlaceholderTextSelectTime = "選擇時間";
    public const string P2PlaceholderTextSelectAddress = "選擇地址";
    public const string P2HeaderHangzhou = "杭州";
    public const string P2HeaderWestLake = "西湖";
    public const string P2HeaderLingyinShi = "靈隱寺";
    public const string P2HeaderNanjing = "南京";
    public const string P2HeaderZhongHuaMen = "中華門";
    public const string P2PlaceholderTextStartTime = "開始時間";
    public const string P2SecondaryPlaceholderTextEndTime = "結束時間";
    public const string P2PlaceholderTextPleaseSelect = "請選擇";
    public const string P2HeaderParentN1 = "父節點 1";
    public const string P2HeaderParentN1N0 = "父節點 1-0";
    public const string P2HeaderLeaf1 = "葉子節點1";
    public const string P2HeaderLeaf2 = "葉子節點2";
    public const string P2HeaderParentN1N1 = "父節點 1-1";
    public const string P2HeaderLeaf3 = "葉子節點3";
    public const string P2PlaceholderTextInputHere = "在此輸入";
    public const string P2PlaceholderTextAnotherInput = "另一個輸入";
    public const string P2TextCenter = "居中";
    public const string P2ContentPrimary = "主要";
    public const string P2TextBlock = "塊";
    public const string P2ContentButton = "按鈕";
    public const string P2ContentLink = "鏈接";
    public const string P2TextSpace = "Space：";
    public const string P2ContentClickToUpload = "點擊上傳";
    public const string P2ContentConfirm = "確認";
    public const string P2TextCardContent = "卡片內容";
    public const string P2ContentButtonN1 = "按鈕 1";
    public const string P2ContentButtonN2 = "按鈕 2";
    public const string P2ContentButtonN3 = "按鈕 3";
    public const string P2ContentButtonN4 = "按鈕 4";
    public const string P2ContentSubmit = "提交";
    public const string P2ContentSearch = "查詢";
    public const string P2ContentSmall = "小號";
    public const string P2ContentMiddle = "中號";
    public const string P2ContentLarge = "大號";
    public const string P2ContentCustom = "自定義";
    public const string P2ContentDefault = "默認";
    public const string P2ContentDashed = "虛線";

    public const string P2ToolTipTipCopyGitUrl = "複製 Git URL";

    public const string P2ToolTipTipLike = "點贊";

    public const string P2ToolTipTipComment = "評論";

    public const string P2ToolTipTipStar = "收藏";

    public const string P2ToolTipTipHeart = "喜歡";

    public const string P2ToolTipTipShare = "分享";

    public const string P2ToolTipTipDownload = "下載";

    public const string P2ToolTipTipTooltip = "提示";

    protected override Type GetResourceKindType() => typeof(SpaceShowCaseLangResourceKind);
}
