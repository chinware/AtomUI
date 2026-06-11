using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Breadcrumb;

[LanguageProvider(LanguageCode.en_US, BreadcrumbShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicUsageTitle = "Basic Usage";
    public const string BasicUsageDescription = "The simplest use.";
    public const string WithIconTitle = "With an Icon";
    public const string WithIconDescription = "The icon should be placed in front of the text.";
    public const string WithParamsTitle = "With Params";
    public const string WithParamsDescription = "With route params.";
    public const string ConfiguringSeparatorTitle = "Configuring the Separator";
    public const string ConfiguringSeparatorDescription = "The separator can be customized by setting the separator property: separator equals >";
    public const string ConfiguringSeparatorIndependentlyTitle = "Configuring the Separator Independently";
    public const string ConfiguringSeparatorIndependentlyDescription = "Customize separator for each other.";
    public const string GenerateByTemplateTitle = "Generate BreadcrumbItem by template";
    public const string GenerateByTemplateDescription = "Generate BreadcrumbItem by template.";
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
