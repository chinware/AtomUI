using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.DropdownButton;

[LanguageProvider(LanguageCode.zh_CN, DropdownButtonShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的下拉菜单。";
    public const string ButtonTypesTitle = "按钮类型";
    public const string ButtonTypesDescription = "支持统一的按钮类型。";
    public const string ArrowTitle = "箭头";
    public const string ArrowDescription = "可以显示箭头。";
    public const string PlacementTitle = "弹出位置";
    public const string PlacementDescription = "支持 6 种弹出位置。";
    public const string P2HeaderCut = "剪切";
    public const string P2HeaderCopy = "复制";
    public const string P2HeaderDelete = "删除";
    public const string P2HeaderPaste = "粘贴";
    public const string P2HeaderPasteFromHistory = "从历史记录粘贴";
    public const string P2ContentHoverMe = "悬停";
    public const string P2ContentEditFile = "编辑文件";
    public const string P2ContentBottomLeft = "左下方";
    public const string P2ContentBottom = "下方";
    public const string P2ContentBottomRight = "右下方";
    public const string P2ContentTopLeft = "左上方";
    public const string P2ContentTop = "上方";
    public const string P2ContentTopRight = "右上方";

    protected override Type GetResourceKindType() => typeof(DropdownButtonShowCaseLangResourceKind);
}
