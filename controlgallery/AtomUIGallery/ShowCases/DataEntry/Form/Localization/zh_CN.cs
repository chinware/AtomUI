using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Form;

[LanguageProvider(LanguageCode.zh_CN, FormShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicUsageTitle = "基础用法";
    public const string BasicUsageDescription = "基础表单数据控制，包含布局、初始值、校验和提交。";
    public const string FormMethodsTitle = "表单方法";
    public const string FormMethodsDescription = "通过 Form 调用表单方法。";
    public const string CustomizedFormControlsTitle = "自定义表单控件";
    public const string CustomizedFormControlsDescription = "Form 中也可以使用自定义或第三方表单控件。控件必须实现 IFormItemAware：";
    public const string OtherFormControlsTitle = "其他表单控件";
    public const string OtherFormControlsDescription = "演示上述示例中未展示的表单控件校验配置。";
    public const string DynamicFormItemTitle = "动态表单项";
    public const string DynamicFormItemDescription = "动态添加或移除表单项。add 函数支持配置初始值。";
    public const string FormLayoutTitle = "表单布局";
    public const string FormLayoutDescription = "表单有三种布局：水平、垂直和行内。";
    public const string LabelCanWrapTitle = "标签可换行";
    public const string LabelCanWrapDescription = "开启 labelWrap 后，标签文本过长时会自动换行。";
    public const string InlineLoginFormTitle = "行内登录表单";
    public const string InlineLoginFormDescription = "行内登录表单常用于导航栏。";
    public const string LoginFormTitle = "登录表单";
    public const string LoginFormDescription = "普通登录表单，可以包含更多元素。";
    public const string RegistrationTitle = "注册";
    public const string RegistrationDescription = "填写此表单创建新账号。";
    public const string FormVariantsTitle = "表单变体";
    public const string FormVariantsDescription = "改变表单中所有组件的变体，可选描边、填充、无边框和下划线。";
    public const string RequiredStyleTitle = "必填样式";
    public const string RequiredStyleDescription = "通过 requiredMark 切换必填或可选样式。";
    public const string NoBlockRuleTitle = "非阻塞规则";
    public const string NoBlockRuleDescription = "带 warningOnly 的规则不会阻止表单提交。";
    public const string ValidateTriggerTitle = "校验触发时机";
    public const string ValidateTriggerDescription = "在异步校验场景中，高频校验会给后端带来压力。可以通过 validateTrigger 改变校验时机，通过 validateDebounce 改变校验频率，或通过 validateFirst 设置校验短路。";
    public const string ValidateOnlyTitle = "仅校验";
    public const string ValidateOnlyDescription = "通过 validateFields 的 validateOnly 动态调整提交按钮的禁用状态。";
    public const string TimeRelatedControlsTitle = "时间相关控件";
    public const string TimeRelatedControlsDescription = "时间相关组件的值是 dayjs 对象，提交到服务端前需要预处理。";
    public const string CustomizedValidationTitle = "自定义校验";
    public const string CustomizedValidationDescription = "不使用 Form 时，也可以通过 validateStatus、help、hasFeedback 等属性自定义校验状态和信息。";

    protected override Type GetResourceKindType() => typeof(FormShowCaseLangResourceKind);
}
