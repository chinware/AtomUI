using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.DropdownButton;

[LanguageProvider(LanguageCode.zh_TW, DropdownButtonShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最基礎的下拉菜單。";
    public const string ButtonTypesTitle = "按鈕類型";
    public const string ButtonTypesDescription = "支持統一的按鈕類型。";
    public const string ArrowTitle = "箭頭";
    public const string ArrowDescription = "可以顯示箭頭。";
    public const string PlacementTitle = "彈出位置";
    public const string PlacementDescription = "支持 6 種彈出位置。";
    public const string P2HeaderCut = "剪切";
    public const string P2HeaderCopy = "複製";
    public const string P2HeaderDelete = "刪除";
    public const string P2HeaderPaste = "粘貼";
    public const string P2HeaderPasteFromHistory = "從歷史記錄粘貼";
    public const string P2ContentHoverMe = "懸停";
    public const string P2ContentEditFile = "編輯文件";
    public const string P2ContentBottomLeft = "左下方";
    public const string P2ContentBottom = "下方";
    public const string P2ContentBottomRight = "右下方";
    public const string P2ContentTopLeft = "左上方";
    public const string P2ContentTop = "上方";
    public const string P2ContentTopRight = "右上方";

    protected override Type GetResourceKindType() => typeof(DropdownButtonShowCaseLangResourceKind);
}

