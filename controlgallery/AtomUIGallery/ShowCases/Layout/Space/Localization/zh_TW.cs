using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Space;

[LanguageProvider(LanguageCode.zh_TW, SpaceShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
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

