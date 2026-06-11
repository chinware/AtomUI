using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Result;

[LanguageProvider(LanguageCode.zh_TW, ResultShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string SuccessTitle = "成功";
    public const string SuccessDescription = "展示成功結果。";
    public const string InfoTitle = "信息";
    public const string InfoDescription = "展示處理中結果。";
    public const string WarningTitle = "警告";
    public const string WarningDescription = "展示警告結果。";
    public const string ForbiddenTitle = "403";
    public const string ForbiddenDescription = "你無權訪問此頁面。";
    public const string NotFoundTitle = "404";
    public const string NotFoundDescription = "你訪問的頁面不存在。";
    public const string ServerErrorTitle = "500";
    public const string ServerErrorDescription = "服務器發生錯誤。";
    public const string ErrorTitle = "錯誤";
    public const string ErrorDescription = "服務器發生錯誤。";
    public const string CustomIconTitle = "自定義圖標";
    public const string CustomIconDescription = "自定義圖標。";
    public const string P2HeaderSuccessfullyPurchasedCloudServerEcs = "雲服務器 ECS 購買成功！";
    public const string P2HeaderYourOperationHasBeenExecuted = "你的操作已執行。";
    public const string P2HeaderThereAreSomeProblemsWithYourOperation = "你的操作存在一些問題。";
    public const string P2HeaderSubmissionFailed = "提交失敗";
    public const string P2HeaderGreatWeHaveDoneAllTheOperations = "很好，所有操作已完成！";
    public const string P2SubHeaderOrderNumberCloudServerConfiguration = "訂單號：2017182818828182881。雲服務器配置需要 1-5 分鐘，請稍候。";
    public const string P2SubHeaderForbidden = "抱歉，你無權訪問此頁面。";
    public const string P2SubHeaderNotFound = "抱歉，你訪問的頁面不存在。";
    public const string P2SubHeaderServerError = "抱歉，發生了一些錯誤。";
    public const string P2SubHeaderSubmissionFailed = "請檢查並修改以下信息後重新提交。";
    public const string P2ContentGoConsole = "進入控制台";
    public const string P2ContentBuyAgain = "再次購買";
    public const string P2ContentBackHome = "返回首頁";
    public const string P2TextTheContentYouSubmittedHasTheFollowingError = "你提交的內容存在以下錯誤：";
    public const string P2TextYourAccountHasBeenFrozen = "你的賬戶已被凍結。";
    public const string P2TextThawImmediately = "立即解凍 >";
    public const string P2TextYourAccountIsNotYetEligibleToApply = "你的賬戶暫不符合申請條件。";
    public const string P2TextApplyUnlock = "申請解鎖 >";
    public const string P2ContentNext = "下一步";

    protected override Type GetResourceKindType() => typeof(ResultShowCaseLangResourceKind);
}

