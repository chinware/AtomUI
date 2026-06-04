using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Alert;

[LanguageProvider(LanguageCode.zh_TW, AlertShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "用於短消息提示的最簡單用法。";
    public const string MoreTypesTitle = "更多類型";
    public const string MoreTypesDescription = "Alert 有 success、info、warning、error 四種類型。";
    public const string ClosableTitle = "可關閉";
    public const string ClosableDescription = "顯示關閉按鈕。";
    public const string DescriptionTitle = "含描述信息";
    public const string DescriptionDescription = "為提示消息添加額外描述。";
    public const string IconTitle = "圖標";
    public const string IconDescription = "合適的圖標可以讓提示信息更清晰。";
    public const string CustomActionTitle = "自定義操作";
    public const string CustomActionDescription = "自定義操作。";
    public const string LoopBannerTitle = "循環橫幅";
    public const string LoopBannerDescription = "展示循環橫幅。";
    public const string P2DescriptionErrorDescriptionErrorDescriptionErrorDescriptionErrorDesc = "錯誤描述 錯誤描述 錯誤描述 錯誤描述 錯誤描述 錯誤描述";
    public const string P2DescriptionSuccessDescriptionSuccessDescriptionSuccessDescription = "成功描述 成功描述 成功描述";
    public const string P2DescriptionInfoDescriptionInfoDescriptionInfoDescriptionInfoDescript = "信息描述 信息描述 信息描述 信息描述";
    public const string P2DescriptionWarningDescriptionWarningDescriptionWarningDescriptionWar = "警告描述 警告描述 警告描述 警告描述";
    public const string P2DescriptionErrorDescriptionErrorDescriptionErrorDescriptionErrorDesc2 = "錯誤描述 錯誤描述 錯誤描述 錯誤描述";
    public const string P2DescriptionDetailedDescriptionAndAdviceAboutSuccessfulCopywriting = "關於成功提示文案的詳細說明和建議。";
    public const string P2DescriptionAdditionalDescriptionAndInformationAboutCopywriting = "關於提示文案的補充說明和信息。";
    public const string P2DescriptionThisIsAWarningNoticeAboutCopywriting = "這是一條關於提示文案的警告通知。";
    public const string P2DescriptionThisIsAnErrorMessageAboutCopywriting = "這是一條關於提示文案的錯誤信息。";
    public const string P2ContentSuccessText = "成功文本";
    public const string P2ContentInfoText = "信息文本";
    public const string P2ContentWarningText = "警告文本";
    public const string P2ContentErrorText = "錯誤文本";
    public const string P2ContentWarningTextWarningTextWarningTextWarningText = "警告文本 警告文本 警告文本 警告文本 警告文本 警告文本";
    public const string P2ContentUndo = "撤銷";
    public const string P2ContentDetail = "詳情";
    public const string P2ContentDone = "完成";
    public const string P2ContentAccept = "接受";
    public const string P2ContentDecline = "拒絕";
    public const string P2ContentICanBeAReactComponentMultipleReact = "這裡可以是自定義內容、多個控件，或只是一段普通文本，信息描述 信息描述 信息描述 信息描述";

    public const string P2MessageSuccessText = "成功提示文本";

    public const string P2MessageInfoText = "信息提示文本";

    public const string P2MessageWarningText = "警告提示文本";

    public const string P2MessageErrorText = "錯誤提示文本";

    public const string P2MessageSuccessTips = "成功提示";

    public const string P2MessageInformationalNotes = "信息說明";

    public const string P2MessageWarning = "警告";

    public const string P2MessageError = "錯誤";

    protected override Type GetResourceKindType() => typeof(AlertShowCaseLangResourceKind);
}

