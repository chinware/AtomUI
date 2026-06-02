using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Separator;

[LanguageProvider(LanguageCode.zh_CN, SeparatorShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string HorizontalTitle = "水平分割线";
    public const string HorizontalDescription = "Separator 默认是水平分割线，可以在 Separator 中添加文本。";
    public const string DividerWithTitleTitle = "带标题的分割线";
    public const string DividerWithTitleDescription = "带内部标题的分割线，可设置 orientation='left/right' 来对齐标题。";
    public const string PlainTextTitle = "无标题样式文本";
    public const string PlainTextDescription = "通过设置 plain 属性，可以使用非标题样式的分割线文本。";
    public const string SpacingSizeTitle = "设置分割线间距";
    public const string SpacingSizeDescription = "设置间距大小。";
    public const string VerticalTitle = "垂直分割线";
    public const string VerticalDescription = "使用 type='vertical' 可以让分割线垂直显示。";
    public const string VariantTitle = "线型";
    public const string VariantDescription = "分割线默认使用实线样式，也可以改为虚线或点线。";

    protected override Type GetResourceKindType() => typeof(SeparatorShowCaseLangResourceKind);
}
