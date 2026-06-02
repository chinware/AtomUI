using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Watermark;

[LanguageProvider(LanguageCode.zh_CN, WatermarkShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的用法。";
    public const string MultiLineTitle = "多行水印";
    public const string MultiLineDescription = "使用换行指定多行水印内容。";
    public const string ImageWatermarkTitle = "图片水印";
    public const string ImageWatermarkDescription = "通过 image 指定图片地址。为确保图片高清且不被拉伸，请设置宽高，并上传至少两倍于显示宽高的 logo 图片。";
    public const string CustomConfigurationTitle = "自定义配置";
    public const string CustomConfigurationDescription = "通过配置自定义参数预览水印效果。";

    protected override Type GetResourceKindType() => typeof(WatermarkShowCaseLangResourceKind);
}
