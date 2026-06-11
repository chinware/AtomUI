using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.PopupConfirm;

[LanguageProvider(LanguageCode.zh_TW, PopupConfirmShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicUsageTitle = "基礎用法";
    public const string BasicUsageDescription = "基礎示例支持確認框的標題和描述屬性。";
    public const string LocaleTextTitle = "本地化文本";
    public const string LocaleTextDescription = "設置 okText 和 cancelText 屬性來自定義按鈕標籤。";
    public const string PlacementTitle = "彈出位置";
    public const string PlacementDescription = "提供 12 種彈出位置。";
    public const string CustomizeIconTitle = "自定義圖標";
    public const string CustomizeIconDescription = "設置 icon 屬性來自定義圖標。";
    public const string P2ConfirmContentAreYouSureToDeleteThisTask = "確定要刪除這個任務嗎？";
    public const string P2OkTextOk = "確定";
    public const string P2CancelTextCancel = "取消";
    public const string P2TitleDeleteTheTask = "刪除任務";
    public const string P2ContentDelete = "刪除";
    public const string P2ContentLt = "LT";
    public const string P2ContentLeft = "左側";
    public const string P2ContentLb = "LB";
    public const string P2ContentTl = "TL";
    public const string P2ContentTop = "頂部";
    public const string P2ContentTr = "TR";
    public const string P2ContentRt = "RT";
    public const string P2ContentRight = "右側";
    public const string P2ContentRb = "RB";
    public const string P2ContentBl = "BL";
    public const string P2ContentBottom = "底部";
    public const string P2ContentBr = "BR";

    protected override Type GetResourceKindType() => typeof(PopupConfirmShowCaseLangResourceKind);
}

