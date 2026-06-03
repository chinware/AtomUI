using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.CustomizeTheme;

[LanguageProvider(LanguageCode.zh_CN, CustomizeThemeShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string CustomizeDesignTokenTitle = "自定义设计令牌";
    public const string CustomizeDesignTokenDescription = "通过修改主题的 token 属性，可以全局修改 Design Token。部分 token 会影响其他 token，这类 token 称为 Seed Token。";
    public const string PresetAlgorithmsTitle = "使用预设算法";
    public const string PresetAlgorithmsDescription = "通过修改 algorithm 可以快速生成不同风格的主题。Ant Design 5.0 默认提供三套预设算法：DefaultAlgorithm、DarkAlgorithm、CompactAlgorithm。";
    public const string CustomizeComponentTokenTitle = "自定义组件令牌";
    public const string CustomizeComponentTokenDescription = "除 Design Token 外，每个组件也有自己的 Component Token，用于实现组件级样式定制，不同组件之间互不影响。组件的其他 Design Token 也可以用这种方式覆盖。";
    public const string NestedThemeTitle = "嵌套主题";
    public const string NestedThemeDescription = "通过嵌套 ConfigProvider，可以为页面的局部区域应用局部主题。子主题中未修改的 Design Token 会继承父主题。";
    public const string P2PlaceholderTextPleaseInput = "请输入";
    public const string P2ContentPrimaryButton = "主要按钮";
    public const string P2ContentDefaultButton = "默认按钮";
    public const string P2ContentTextButton = "文本按钮";
    public const string P2ContentLinkButton = "链接按钮";
    public const string P2ContentApple = "苹果";
    public const string P2ContentPear = "梨";
    public const string P2ContentOrange = "橙子";
    public const string P2ContentSubmit = "提交";
    public const string P2TextEnableAlgorithm = "启用算法：";
    public const string P2TextDisableAlgorithm = "禁用算法：";
    public const string P2ContentThemeN1 = "主题 1";
    public const string P2ContentThemeN2 = "主题 2";

    protected override Type GetResourceKindType() => typeof(CustomizeThemeShowCaseLangResourceKind);
}
