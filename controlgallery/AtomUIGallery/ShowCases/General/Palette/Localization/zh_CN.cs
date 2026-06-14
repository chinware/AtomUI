using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Palette;

[LanguageProvider(LanguageCode.zh_CN, PaletteShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string ComponentCategory = "通用";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "预览 AtomUI 在浅色和深色算法下的预设色板。";
    public const string PageDescription = "Palette 展示 AtomUI 主题算法使用的预设主色阶。通过标签页可以对比同一组语义色在浅色和深色模式下生成的色板。";
    public const string InfoNamespaceLabel = "命名空间:";
    public const string InfoPackageLabel = "包:";
    public const string InfoBaseClassLabel = "基础类:";
    public const string P2HeaderLight = "浅色";
    public const string P2HeaderDark = "深色";

    protected override Type GetResourceKindType() => typeof(PaletteShowCaseLangResourceKind);
}
