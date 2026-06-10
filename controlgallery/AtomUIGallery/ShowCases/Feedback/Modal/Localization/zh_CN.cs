using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Modal;

[LanguageProvider(LanguageCode.zh_CN, ModalShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "基础模态框。";
    public const string AsynchronouslyCloseTitle = "异步关闭";
    public const string AsynchronouslyCloseDescription = "按下确定按钮后异步关闭模态框。例如提交表单时可以使用这种模式。";
    public const string MessageBoxStyleTitle = "MessageBox 样式";
    public const string MessageBoxStyleDescription = "MessageBox 支持 Confirm、Information、Warning 和 Error。";
    public const string LoadingTitle = "加载状态";
    public const string LoadingDescription = "设置 Modal 的加载状态。";
    public const string CustomFooterButtonsTitle = "自定义页脚按钮";
    public const string CustomFooterButtonsDescription = "可以同时使用标准按钮和自定义按钮。";
    public const string OpenDraggableModalTitle = "可拖拽模态框";
    public const string OpenDraggableModalDescription = "自定义模态框内容渲染并启用拖拽。";
    public const string ManualUpdateDestroyTitle = "手动更新和销毁";
    public const string ManualUpdateDestroyDescription = "通过实例手动更新和销毁模态框。";
    public const string CustomizeFooterButtonPropsTitle = "自定义页脚按钮属性";
    public const string CustomizeFooterButtonPropsDescription = "通过指定回调函数修改对话框按钮属性。";
    public const string StaticDialogApiTitle = "静态对话框 API";
    public const string StaticDialogApiDescription = "使用 Dialog 的静态方法创建对话框。";
    public const string P2TitleBasicModal = "基础模态框";
    public const string P2TitleBasicWindowModal = "基础窗口模态框";
    public const string P2TitleAsynchronouslyCloseModal = "异步关闭模态框";
    public const string P2TitleNormal = "普通";
    public const string P2TitleDoYouWantToDeleteTheseItems = "是否删除这些项目？";
    public const string P2TitleThisIsANotificationMessage = "这是一条通知消息";
    public const string P2TitleOperationSuccessful = "操作成功";
    public const string P2TitleThisIsAnErrorMessage = "这是一条错误消息";
    public const string P2TitleThisIsAWarningMessage = "这是一条警告消息";
    public const string P2TitleStaticApi = "静态 API";
    public const string P2TitleLoadingModal = "加载中模态框";
    public const string P2TitleTitle = "标题";
    public const string P2TitleConfirm = "确认";
    public const string P2TitleDraggableModal = "可拖拽模态框";
    public const string P2ContentOpenModalOverlay = "打开浮层模态框";
    public const string P2TextSomeContents = "一些内容...";
    public const string P2ContentOpenModalWindow = "打开窗口模态框";
    public const string P2ContentOpenModalWithAsyncLogic = "打开带异步逻辑的模态框";
    public const string P2TextContentOfTheModal = "模态框内容";
    public const string P2TextNativeWindow = "原生窗口：";
    public const string P2TextSomeDescriptions = "一些描述";
    public const string P2ContentConfirm = "确认";
    public const string P2TextSomeMessagesSomeMessages = "一些消息...一些消息...";
    public const string P2ContentInformation = "信息";
    public const string P2ContentSuccess = "成功";
    public const string P2ContentError = "错误";
    public const string P2ContentWarning = "警告";
    public const string P2ContentOpenModal = "打开模态框";
    public const string P2ContentCustomButton = "自定义按钮";
    public const string P2TextBlaBla = "一些文本 ...";
    public const string P2TextJustDonTLearnPhysicsAtSchoolAnd = "只要别在学校学物理，生活就会充满魔法和奇迹。";
    public const string P2TextDayBeforeYesterdayISawARabbitAnd = "前天我看见一只兔子，昨天看见一头鹿，而今天看见了你。";
    public const string P2ContentOpenModalToCloseInN5s = "打开 5 秒后关闭的模态框";
    public const string P2ContentOpenModalWithCustomizedButtonProps = "打开自定义按钮属性的模态框";
    public const string P2ContentOpenOverlayDialog = "打开浮层对话框";
    public const string P2ContentOpenWindowDialog = "打开窗口对话框";
    public const string P2ContentOpenCustomviewDialog = "打开自定义视图对话框";
    public const string P2TextName = "姓名";
    public const string P2TextAge = "年龄";
    public const string P2RunThisModalWillBeDestroyedAfter = "此模态框将在 ";
    public const string P2RunSecond = " 秒后销毁。";

    protected override Type GetResourceKindType() => typeof(ModalShowCaseLangResourceKind);
}
