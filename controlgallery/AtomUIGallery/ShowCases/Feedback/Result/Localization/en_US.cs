using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Result;

[LanguageProvider(LanguageCode.en_US, ResultShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string SuccessTitle = "Success";
    public const string SuccessDescription = "Show successful results.";
    public const string InfoTitle = "Info";
    public const string InfoDescription = "Show processing results.";
    public const string WarningTitle = "Warning";
    public const string WarningDescription = "The result of the warning.";
    public const string ForbiddenTitle = "403";
    public const string ForbiddenDescription = "you are not authorized to access this page.";
    public const string NotFoundTitle = "404";
    public const string NotFoundDescription = "The page you visited does not exist.";
    public const string ServerErrorTitle = "500";
    public const string ServerErrorDescription = "Something went wrong on server.";
    public const string ErrorTitle = "error";
    public const string ErrorDescription = "Something went wrong on server.";
    public const string CustomIconTitle = "Custom icon";
    public const string CustomIconDescription = "Custom icon.";

    protected override Type GetResourceKindType() => typeof(ResultShowCaseLangResourceKind);
}
