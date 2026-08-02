using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.CustomizeTheme;

[LanguageProvider(LanguageCode.zh_CN, CustomizeThemeShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string ScenarioExamples = "示例";
    public const string ComponentCategory = "通用";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "通过 Global Token、算法、控件 Token 和局部嵌套作用域定制 AtomUI 主题。";
    public const string PageDescription = "Customize Theme 展示 ThemeConfigProvider 如何覆盖 Global Token、应用预设算法、配置控件 Token 和嵌套主题上下文，而不需要修改应用全局主题。";
    public const string ApiMemberThemeConfigProviderConfig = "不可变的局部主题配置；运行期更新需要替换完整配置值。";
    public const string ApiMemberThemeConfigInherit = "是否继承并合并父级主题配置。";
    public const string ApiMemberThemeConfigAlgorithms = "有序的 ThemeAlgorithm 枚举值；null 表示继承父级算法链。";
    public const string ApiMemberThemeConfigTokens = "按 Token 名称索引的全局 Token 覆盖。";
    public const string ApiMemberThemeConfigControls = "按 ControlTokenIdentity 索引的控件主题配置。";
    public const string ApiMemberControlThemeConfigAlgorithm = "控件算法模式：Unspecified、Disabled、Global 或 Custom。";
    public const string ApiMemberControlThemeConfigAlgorithms = "仅在 Algorithm 为 Custom 时使用的有序 ThemeAlgorithm 枚举值。";
    public const string ApiMemberControlThemeConfigTokens = "仅应用到目标 Control identity 的 Global Token 与 Own Token 覆盖。";
    public const string CustomizeDesignTokenTitle = "自定义设计令牌";
    public const string CustomizeDesignTokenDescription = "通过修改主题的 token 属性，可以全局修改 Design Token。部分 token 会影响其他 token，这类 token 称为 Seed Token。";
    public const string PresetAlgorithmsTitle = "使用预设算法";
    public const string PresetAlgorithmsDescription = "通过修改 algorithm 可以快速生成不同风格的主题。Ant Design 5.0 默认提供三套预设算法：DefaultAlgorithm、DarkAlgorithm、CompactAlgorithm。";
    public const string RuntimeTokenUpdatesTitle = "运行期令牌更新";
    public const string RuntimeTokenUpdatesDescription = "使用新的不可变 ThemeConfig 替换 ThemeConfigProvider.Config，即可发布一个局部 Snapshot，并且不会追加资源 Provider 层。";
    public const string CustomizeControlTokenTitle = "自定义控件 Token";
    public const string CustomizeControlTokenDescription = "每个对外可主题化的控件都有独立 Control identity。可以在该控件配置下覆盖任意 Global Token 以及该控件的 Own Token；覆盖只影响目标控件，不会泄漏到其他控件。";
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
    public const string P2ContentUseBlue = "蓝色";
    public const string P2ContentUseGreen = "绿色";
    public const string P2ContentUseMagenta = "洋红";
    public const string P2TextEnableAlgorithm = "启用算法：";
    public const string P2TextDisableAlgorithm = "禁用算法：";
    public const string P2ContentThemeN1 = "主题 1";
    public const string P2ContentThemeN2 = "主题 2";
    public const string P2ContentThemeN3 = "不继承";

}
