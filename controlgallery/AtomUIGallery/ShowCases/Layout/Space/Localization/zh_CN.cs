using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Space;

[LanguageProvider(LanguageCode.zh_CN, SpaceShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "为拥挤的组件添加水平间距。";
    public const string VerticalSpaceTitle = "垂直间距";
    public const string VerticalSpaceDescription = "为拥挤的组件添加垂直间距。";
    public const string SizeTitle = "间距尺寸";
    public const string SizeDescription = "使用 size 设置间距，预设 small、middle、large 三种尺寸，也可以自定义间距。未设置 size 时，间距为 small。";
    public const string AlignTitle = "对齐";
    public const string AlignDescription = "配置项目对齐方式。";
    public const string WrapTitle = "自动换行";
    public const string WrapDescription = "自动换行。";
    public const string SplitTitle = "分隔符";
    public const string SplitDescription = "为拥挤的组件添加分隔符。";
    public const string CompactFormTitle = "表单紧凑模式";
    public const string CompactFormDescription = "表单组件的紧凑模式。";
    public const string CompactButtonTitle = "按钮紧凑模式";
    public const string CompactButtonDescription = "按钮组件的紧凑示例。";
    public const string VerticalCompactTitle = "垂直紧凑模式";
    public const string VerticalCompactDescription = "Space.Compact 的垂直模式，仅支持 Button。";

    protected override Type GetResourceKindType() => typeof(SpaceShowCaseLangResourceKind);
}
