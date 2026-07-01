using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Icon;

[LanguageProvider(LanguageCode.zh_TW, IconShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string ComponentCategory = "通用";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "按主題風格瀏覽 AtomUI Ant Design 圖標。";
    public const string PageDescription = "Icon 為 AtomUI 控件和應用提供 Ant Design 圖標集。通過標籤切換線框、填充和雙色圖標主題，並在當前圖庫內搜索圖標。";
    public const string P2HeaderOutlined = "線框風格";
    public const string P2HeaderFilled = "填充風格";
    public const string P2HeaderTwoTone = "雙色風格";

    protected override Type GetResourceKindType() => typeof(IconShowCaseLangResourceKind);
}
