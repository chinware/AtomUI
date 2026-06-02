using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Result;

[LanguageProvider(LanguageCode.zh_CN, ResultShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string SuccessTitle = "成功";
    public const string SuccessDescription = "展示成功结果。";
    public const string InfoTitle = "信息";
    public const string InfoDescription = "展示处理中结果。";
    public const string WarningTitle = "警告";
    public const string WarningDescription = "展示警告结果。";
    public const string ForbiddenTitle = "403";
    public const string ForbiddenDescription = "你无权访问此页面。";
    public const string NotFoundTitle = "404";
    public const string NotFoundDescription = "你访问的页面不存在。";
    public const string ServerErrorTitle = "500";
    public const string ServerErrorDescription = "服务器发生错误。";
    public const string ErrorTitle = "错误";
    public const string ErrorDescription = "服务器发生错误。";
    public const string CustomIconTitle = "自定义图标";
    public const string CustomIconDescription = "自定义图标。";

    protected override Type GetResourceKindType() => typeof(ResultShowCaseLangResourceKind);
}
