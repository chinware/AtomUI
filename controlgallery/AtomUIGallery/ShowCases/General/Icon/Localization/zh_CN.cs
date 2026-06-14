using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Icon;

[LanguageProvider(LanguageCode.zh_CN, IconShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string ComponentCategory = "通用";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "按主题风格浏览 AtomUI Ant Design 图标。";
    public const string PageDescription = "Icon 为 AtomUI 控件和应用提供 Ant Design 图标集。通过标签切换线框、填充和双色图标主题，并在当前图库内搜索图标。";
    public const string InfoNamespaceLabel = "命名空间：";
    public const string InfoPackageLabel = "包：";
    public const string InfoBaseClassLabel = "基类：";
    public const string P2HeaderOutlined = "线框风格";
    public const string P2HeaderFilled = "填充风格";
    public const string P2HeaderTwoTone = "双色风格";

    protected override Type GetResourceKindType() => typeof(IconShowCaseLangResourceKind);
}
