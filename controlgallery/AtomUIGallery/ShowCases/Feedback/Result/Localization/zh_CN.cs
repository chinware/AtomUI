using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Result;

[LanguageProvider(LanguageCode.zh_CN, ResultShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
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
    public const string P2HeaderSuccessfullyPurchasedCloudServerEcs = "云服务器 ECS 购买成功！";
    public const string P2HeaderYourOperationHasBeenExecuted = "你的操作已执行。";
    public const string P2HeaderThereAreSomeProblemsWithYourOperation = "你的操作存在一些问题。";
    public const string P2HeaderSubmissionFailed = "提交失败";
    public const string P2HeaderGreatWeHaveDoneAllTheOperations = "很好，所有操作已完成！";
    public const string P2SubHeaderOrderNumberCloudServerConfiguration = "订单号：2017182818828182881。云服务器配置需要 1-5 分钟，请稍候。";
    public const string P2SubHeaderForbidden = "抱歉，你无权访问此页面。";
    public const string P2SubHeaderNotFound = "抱歉，你访问的页面不存在。";
    public const string P2SubHeaderServerError = "抱歉，发生了一些错误。";
    public const string P2SubHeaderSubmissionFailed = "请检查并修改以下信息后重新提交。";
    public const string P2ContentGoConsole = "进入控制台";
    public const string P2ContentBuyAgain = "再次购买";
    public const string P2ContentBackHome = "返回首页";
    public const string P2TextTheContentYouSubmittedHasTheFollowingError = "你提交的内容存在以下错误：";
    public const string P2TextYourAccountHasBeenFrozen = "你的账户已被冻结。";
    public const string P2TextThawImmediately = "立即解冻 >";
    public const string P2TextYourAccountIsNotYetEligibleToApply = "你的账户暂不符合申请条件。";
    public const string P2TextApplyUnlock = "申请解锁 >";
    public const string P2ContentNext = "下一步";

    protected override Type GetResourceKindType() => typeof(ResultShowCaseLangResourceKind);
}
