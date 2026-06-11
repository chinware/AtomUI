using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Result;

[LanguageProvider(LanguageCode.en_US, ResultShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
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
    public const string P2HeaderSuccessfullyPurchasedCloudServerEcs = "Successfully Purchased Cloud Server ECS!";
    public const string P2HeaderYourOperationHasBeenExecuted = "Your operation has been executed.";
    public const string P2HeaderThereAreSomeProblemsWithYourOperation = "There are some problems with your operation.";
    public const string P2HeaderSubmissionFailed = "Submission Failed";
    public const string P2HeaderGreatWeHaveDoneAllTheOperations = "Great, we have done all the operations!";
    public const string P2SubHeaderOrderNumberCloudServerConfiguration = "Order number: 2017182818828182881 Cloud server configuration takes 1-5 minutes, please wait.";
    public const string P2SubHeaderForbidden = "Sorry, you are not authorized to access this page.";
    public const string P2SubHeaderNotFound = "Sorry, the page you visited does not exist.";
    public const string P2SubHeaderServerError = "Sorry, something went wrong.";
    public const string P2SubHeaderSubmissionFailed = "Please check and modify the following information before resubmitting.";
    public const string P2ContentGoConsole = "Go Console";
    public const string P2ContentBuyAgain = "Buy Again";
    public const string P2ContentBackHome = "Back Home";
    public const string P2TextTheContentYouSubmittedHasTheFollowingError = "The content you submitted has the following error:";
    public const string P2TextYourAccountHasBeenFrozen = "Your account has been frozen.";
    public const string P2TextThawImmediately = "Thaw immediately >";
    public const string P2TextYourAccountIsNotYetEligibleToApply = "Your account is not yet eligible to apply.";
    public const string P2TextApplyUnlock = "Apply Unlock >";
    public const string P2ContentNext = "Next";

    protected override Type GetResourceKindType() => typeof(ResultShowCaseLangResourceKind);
}
