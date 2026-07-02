using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.CustomizeTheme;

[LanguageProvider(LanguageCode.zh_CN, CustomizeThemeShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计令牌";
    public const string ComponentCategory = "通用";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "通过共享令牌、算法、组件令牌和局部嵌套作用域定制 AtomUI 主题。";
    public const string PageDescription = "Customize Theme 展示 ThemeConfigProvider 如何覆盖 Seed Token、派生 Token、预设算法、组件级 Token 和嵌套主题上下文，而不需要修改应用全局主题。";
    public const string ApiColumnMember = "成员";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiMemberThemeConfigProviderAlgorithms = "当前 Provider 应用的主题算法，例如 Default、Dark 或 Compact。";
    public const string ApiMemberThemeConfigProviderSharedTokenSetters = "共享令牌覆盖，会影响 Provider 下的所有后代控件。";
    public const string ApiMemberThemeConfigProviderControlTokenInfoSetters = "按控件 TokenId 分组的组件令牌覆盖配置。";
    public const string ApiMemberTokenSetterKey = "需要覆盖的 Design Token 键。";
    public const string ApiMemberTokenSetterValue = "用于覆盖 Token 的字符串值。";
    public const string ApiMemberTokenSetterCatalog = "可选的 Token 目录，用于指定某个目录下的 Token。";
    public const string ApiMemberControlTokenInfoSetter = "控件令牌 ID，例如 Button 或 AddOnDecoratedBox。";
    public const string ApiMemberControlTokenInfoSetterEnableAlgorithm = "组件令牌覆盖是否参与主题算法计算。";
    public const string ApiMemberControlTokenInfoSetterSetters = "应用到当前组件令牌的 TokenSetter 集合。";
    public const string TokenColumnToken = "令牌";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameColorPrimary = "交互控件使用的主品牌色。";
    public const string TokenNameBorderRadius = "用于派生组件圆角的共享 Seed Token。";
    public const string TokenNameColorBgContainer = "控件和内容容器使用的背景色。";
    public const string TokenNameButtonColorPrimary = "Button 组件的主色覆盖。";
    public const string TokenNameAddOnDecoratedBoxColorPrimary = "AddOnDecoratedBox 组件主色覆盖，用于输入装饰区域。";
    public const string TokenScopeShared = "共享";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";
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

}
