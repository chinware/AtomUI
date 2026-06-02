using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.SplitButton;

[LanguageProvider(LanguageCode.zh_CN, SplitButtonShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的 SplitButton。";
    public const string SizeTitle = "尺寸";
    public const string SizeDescription = "AtomUI 支持小号、默认和大号三种按钮尺寸。需要大号或小号按钮时，可将 size 属性分别设置为 large 或 small；省略 size 属性时使用默认尺寸。";
    public const string DangerButtonsTitle = "危险按钮";
    public const string DangerButtonsDescription = "danger 是 antd 4.0 之后的按钮属性。";
    public const string CustomIconTitle = "自定义图标";
    public const string CustomIconDescription = "自定义浮出按钮图标。";
    public const string FlyoutTriggerTypeTitle = "浮出触发类型";
    public const string FlyoutTriggerTypeDescription = "支持两种触发类型。";

    protected override Type GetResourceKindType() => typeof(SplitButtonShowCaseLangResourceKind);
}
