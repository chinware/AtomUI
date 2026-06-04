using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Breadcrumb;

[LanguageProvider(LanguageCode.zh_TW, BreadcrumbShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicUsageTitle = "基礎用法";
    public const string BasicUsageDescription = "最簡單的用法。";
    public const string WithIconTitle = "帶圖標";
    public const string WithIconDescription = "圖標應放在文本前面。";
    public const string WithParamsTitle = "帶參數";
    public const string WithParamsDescription = "帶有路由參數的用法。";
    public const string ConfiguringSeparatorTitle = "配置分隔符";
    public const string ConfiguringSeparatorDescription = "可以通過設置 separator 屬性來自定義分隔符：separator 等於 >";
    public const string ConfiguringSeparatorIndependentlyTitle = "單獨配置分隔符";
    public const string ConfiguringSeparatorIndependentlyDescription = "為每一項單獨自定義分隔符。";
    public const string GenerateByTemplateTitle = "通過模板生成 BreadcrumbItem";
    public const string GenerateByTemplateDescription = "通過模板生成 BreadcrumbItem。";
    public const string P2ContentHome = "Home";
    public const string P2ContentApplicationCenter = "Application Center";
    public const string P2ContentApplicationList = "Application List";
    public const string P2ContentAnApplication = "An Application";
    public const string P2ContentApplication = "Application";
    public const string P2ContentUsers = "Users";
    public const string P2ContentParam = "Param";
    public const string P2ContentLocation = "Location";

    protected override Type GetResourceKindType() => typeof(BreadcrumbShowCaseLangResourceKind);
}

