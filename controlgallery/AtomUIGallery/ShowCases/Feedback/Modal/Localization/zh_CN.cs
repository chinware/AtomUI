using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Modal;

[LanguageProvider(LanguageCode.zh_CN, ModalShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
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

    protected override Type GetResourceKindType() => typeof(ModalShowCaseLangResourceKind);
}
