using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Modal;

[LanguageProvider(LanguageCode.zh_TW, ModalShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "基礎模態框。";
    public const string AsynchronouslyCloseTitle = "異步關閉";
    public const string AsynchronouslyCloseDescription = "按下確定按鈕後異步關閉模態框。例如提交表單時可以使用這種模式。";
    public const string MessageBoxStyleTitle = "MessageBox 樣式";
    public const string MessageBoxStyleDescription = "MessageBox 支持 Confirm、Information、Warning 和 Error。";
    public const string LoadingTitle = "加載狀態";
    public const string LoadingDescription = "設置 Modal 的加載狀態。";
    public const string CustomFooterButtonsTitle = "自定義頁腳按鈕";
    public const string CustomFooterButtonsDescription = "可以同時使用標準按鈕和自定義按鈕。";
    public const string OpenDraggableModalTitle = "可拖拽模態框";
    public const string OpenDraggableModalDescription = "自定義模態框內容渲染並啓用拖拽。";
    public const string ManualUpdateDestroyTitle = "手動更新和銷毀";
    public const string ManualUpdateDestroyDescription = "通過實例手動更新和銷毀模態框。";
    public const string CustomizeFooterButtonPropsTitle = "自定義頁腳按鈕屬性";
    public const string CustomizeFooterButtonPropsDescription = "通過指定回調函數修改對話框按鈕屬性。";
    public const string StaticDialogApiTitle = "靜態對話框 API";
    public const string StaticDialogApiDescription = "使用 Dialog 的靜態方法創建對話框。";
    public const string P2TitleBasicModal = "基礎模態框";
    public const string P2TitleBasicWindowModal = "基礎窗口模態框";
    public const string P2TitleAsynchronouslyCloseModal = "異步關閉模態框";
    public const string P2TitleNormal = "普通";
    public const string P2TitleDoYouWantToDeleteTheseItems = "是否刪除這些項目？";
    public const string P2TitleThisIsANotificationMessage = "這是一條通知消息";
    public const string P2TitleOperationSuccessful = "操作成功";
    public const string P2TitleThisIsAnErrorMessage = "這是一條錯誤消息";
    public const string P2TitleThisIsAWarningMessage = "這是一條警告消息";
    public const string P2TitleStaticApi = "靜態 API";
    public const string P2TitleLoadingModal = "加載中模態框";
    public const string P2TitleTitle = "標題";
    public const string P2TitleConfirm = "確認";
    public const string P2TitleDraggableModal = "可拖拽模態框";
    public const string P2ContentOpenModalOverlay = "打開浮層模態框";
    public const string P2TextSomeContents = "一些內容...";
    public const string P2ContentOpenModalWindow = "打開窗口模態框";
    public const string P2ContentOpenModalWithAsyncLogic = "打開帶異步邏輯的模態框";
    public const string P2TextContentOfTheModal = "模態框內容";
    public const string P2TextNativeWindow = "原生窗口：";
    public const string P2TextSomeDescriptions = "一些描述";
    public const string P2ContentConfirm = "確認";
    public const string P2TextSomeMessagesSomeMessages = "一些消息...一些消息...";
    public const string P2ContentInformation = "信息";
    public const string P2ContentSuccess = "成功";
    public const string P2ContentError = "錯誤";
    public const string P2ContentWarning = "警告";
    public const string P2ContentOpenModal = "打開模態框";
    public const string P2ContentCustomButton = "自定義按鈕";
    public const string P2TextBlaBla = "一些文本 ...";
    public const string P2TextJustDonTLearnPhysicsAtSchoolAnd = "只要別在學校學物理，生活就會充滿魔法和奇跡。";
    public const string P2TextDayBeforeYesterdayISawARabbitAnd = "前天我看見一隻兔子，昨天看見一頭鹿，而今天看見了你。";
    public const string P2ContentOpenModalToCloseInN5s = "打開 5 秒後關閉的模態框";
    public const string P2ContentOpenModalWithCustomizedButtonProps = "打開自定義按鈕屬性的模態框";
    public const string P2ContentOpenOverlayDialog = "打開浮層對話框";
    public const string P2ContentOpenWindowDialog = "打開窗口對話框";
    public const string P2ContentOpenCustomviewDialog = "打開自定義視圖對話框";
    public const string P2TextName = "姓名";
    public const string P2TextAge = "年齡";
    public const string P2RunThisModalWillBeDestroyedAfter = "此模態框將在 ";
    public const string P2RunSecond = " 秒後銷毀。";

    protected override Type GetResourceKindType() => typeof(ModalShowCaseLangResourceKind);
}

