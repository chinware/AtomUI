using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Button;

[LanguageProvider(LanguageCode.zh_CN, ButtonShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string TypeTitle = "按钮类型";
    public const string TypeDescription = "Ant Design 中包含主按钮、默认按钮、虚线按钮、文本按钮和链接按钮。";
    public const string ButtonShapeTitle = "按钮形状";
    public const string ButtonShapeDescription = "展示支持的按钮形状，例如主按钮、默认按钮、虚线按钮和文本按钮等。";
    public const string SizeTitle = "按钮尺寸";
    public const string SizeDescription = "AtomUI 支持小号、默认和大号三种按钮尺寸。需要大号或小号按钮时可设置 size 属性；省略 size 属性时使用默认尺寸。";
    public const string IconTitle = "图标";
    public const string IconDescription = "可以通过 icon 属性添加图标，并使用 iconPosition 调整图标位置。";
    public const string LoadingTitle = "加载状态";
    public const string LoadingDescription = "通过设置 Button 的 loading 属性，可以为按钮添加加载指示。";
    public const string BlockButtonTitle = "通栏按钮";
    public const string BlockButtonDescription = "block 属性会让按钮宽度适配父容器。";
    public const string DangerButtonsTitle = "危险按钮";
    public const string DangerButtonsDescription = "danger 是 Ant Design 4.0 之后提供的按钮属性。";
    public const string GhostButtonTitle = "幽灵按钮";
    public const string GhostButtonDescription = "ghost 属性会让按钮背景透明，通常用于有色背景上。";
    public const string DisabledTitle = "禁用状态";
    public const string DisabledDescription = "为按钮添加 disabled 属性可以将按钮标记为禁用状态。";

    protected override Type GetResourceKindType() => typeof(ButtonShowCaseLangResourceKind);
}
