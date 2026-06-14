using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Palette;

[LanguageProvider(LanguageCode.zh_TW, PaletteShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string ComponentCategory = "通用";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "預覽 AtomUI 在淺色和深色演算法下的預設色板。";
    public const string PageDescription = "Palette 展示 AtomUI 主題演算法使用的預設主色階。透過標籤頁可以對比同一組語義色在淺色和深色模式下生成的色板。";
    public const string InfoNamespaceLabel = "命名空間:";
    public const string InfoPackageLabel = "套件:";
    public const string InfoBaseClassLabel = "基礎類:";
    public const string P2HeaderLight = "淺色";
    public const string P2HeaderDark = "深色";

    protected override Type GetResourceKindType() => typeof(PaletteShowCaseLangResourceKind);
}
