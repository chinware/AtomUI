using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Mentions;

[LanguageProvider(LanguageCode.zh_CN, MentionsShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最简单的用法。";
    public const string VariantsTitle = "变体";
    public const string VariantsDescription = "Mentions 提供四种变体：描边、填充、无边框和下划线。";
    public const string AsynchronousLoadingTitle = "异步加载";
    public const string AsynchronousLoadingDescription = "异步加载。";
    public const string CustomizeTriggerTokenTitle = "自定义触发标记";
    public const string CustomizeTriggerTokenDescription = "通过 prefix 属性自定义触发标记，默认为 @，也支持数组。";
    public const string DisabledOrReadOnlyTitle = "禁用或只读";
    public const string DisabledOrReadOnlyDescription = "配置 disabled 和 readOnly。";
    public const string PlacementTitle = "弹出位置";
    public const string PlacementDescription = "改变建议列表的弹出位置。";
    public const string StatusTitle = "状态";
    public const string StatusDescription = "通过 status 为 Mentions 添加状态，可设置为错误或警告。";
    public const string AutoSizeTitle = "自动高度";
    public const string AutoSizeDescription = "高度自动调整。";
    public const string WithClearIconTitle = "带清除图标";
    public const string WithClearIconDescription = "自定义清除按钮。";
    public const string P2PlaceholderTextOutlined = "线框风格";
    public const string P2PlaceholderTextFilled = "填充风格";
    public const string P2PlaceholderTextBorderless = "无边框";
    public const string P2PlaceholderTextUnderlined = "下划线";
    public const string P2PlaceholderTextInputToMentionPeopleToMentionTag = "输入 @ 提及成员，输入 # 提及标签";
    public const string P2PlaceholderTextThisIsDisabledMentions = "这是禁用状态的 Mentions";
    public const string P2PlaceholderTextThisIsReadonlyMentions = "这是只读状态的 Mentions";

    protected override Type GetResourceKindType() => typeof(MentionsShowCaseLangResourceKind);
}
