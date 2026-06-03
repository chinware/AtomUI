using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Alert;

[LanguageProvider(LanguageCode.zh_CN, AlertShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "用于短消息提示的最简单用法。";
    public const string MoreTypesTitle = "更多类型";
    public const string MoreTypesDescription = "Alert 有 success、info、warning、error 四种类型。";
    public const string ClosableTitle = "可关闭";
    public const string ClosableDescription = "显示关闭按钮。";
    public const string DescriptionTitle = "含描述信息";
    public const string DescriptionDescription = "为提示消息添加额外描述。";
    public const string IconTitle = "图标";
    public const string IconDescription = "合适的图标可以让提示信息更清晰。";
    public const string CustomActionTitle = "自定义操作";
    public const string CustomActionDescription = "自定义操作。";
    public const string LoopBannerTitle = "循环横幅";
    public const string LoopBannerDescription = "展示循环横幅。";
    public const string P2DescriptionErrorDescriptionErrorDescriptionErrorDescriptionErrorDesc = "错误描述 错误描述 错误描述 错误描述 错误描述 错误描述";
    public const string P2DescriptionSuccessDescriptionSuccessDescriptionSuccessDescription = "成功描述 成功描述 成功描述";
    public const string P2DescriptionInfoDescriptionInfoDescriptionInfoDescriptionInfoDescript = "信息描述 信息描述 信息描述 信息描述";
    public const string P2DescriptionWarningDescriptionWarningDescriptionWarningDescriptionWar = "警告描述 警告描述 警告描述 警告描述";
    public const string P2DescriptionErrorDescriptionErrorDescriptionErrorDescriptionErrorDesc2 = "错误描述 错误描述 错误描述 错误描述";
    public const string P2DescriptionDetailedDescriptionAndAdviceAboutSuccessfulCopywriting = "关于成功提示文案的详细说明和建议。";
    public const string P2DescriptionAdditionalDescriptionAndInformationAboutCopywriting = "关于提示文案的补充说明和信息。";
    public const string P2DescriptionThisIsAWarningNoticeAboutCopywriting = "这是一条关于提示文案的警告通知。";
    public const string P2DescriptionThisIsAnErrorMessageAboutCopywriting = "这是一条关于提示文案的错误信息。";
    public const string P2ContentSuccessText = "成功文本";
    public const string P2ContentInfoText = "信息文本";
    public const string P2ContentWarningText = "警告文本";
    public const string P2ContentErrorText = "错误文本";
    public const string P2ContentWarningTextWarningTextWarningTextWarningText = "警告文本 警告文本 警告文本 警告文本 警告文本 警告文本";
    public const string P2ContentUndo = "撤销";
    public const string P2ContentDetail = "详情";
    public const string P2ContentDone = "完成";
    public const string P2ContentAccept = "接受";
    public const string P2ContentDecline = "拒绝";
    public const string P2ContentICanBeAReactComponentMultipleReact = "这里可以是自定义内容、多个控件，或只是一段普通文本，信息描述 信息描述 信息描述 信息描述";

    public const string P2MessageSuccessText = "成功提示文本";

    public const string P2MessageInfoText = "信息提示文本";

    public const string P2MessageWarningText = "警告提示文本";

    public const string P2MessageErrorText = "错误提示文本";

    public const string P2MessageSuccessTips = "成功提示";

    public const string P2MessageInformationalNotes = "信息说明";

    public const string P2MessageWarning = "警告";

    public const string P2MessageError = "错误";

    protected override Type GetResourceKindType() => typeof(AlertShowCaseLangResourceKind);
}
