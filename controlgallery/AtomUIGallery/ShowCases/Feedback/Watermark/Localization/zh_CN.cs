using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Watermark;

[LanguageProvider(LanguageCode.zh_CN, WatermarkShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的用法。";
    public const string MultiLineTitle = "多行水印";
    public const string MultiLineDescription = "使用换行指定多行水印内容。";
    public const string ImageWatermarkTitle = "图片水印";
    public const string ImageWatermarkDescription = "通过 image 指定图片地址。为确保图片高清且不被拉伸，请设置宽高，并上传至少两倍于显示宽高的 logo 图片。";
    public const string CustomConfigurationTitle = "自定义配置";
    public const string CustomConfigurationDescription = "通过配置自定义参数预览水印效果。";
    public const string P2WatermarkMultiLineText = "AtomUI\n快乐工作";
    public const string P2TextNaturalInteractionDescription = "数字世界的高速迭代让产品变得更加复杂，而人的意识和注意力资源是有限的。面对这种设计矛盾，追求自然交互将始终是 Ant Design 的一致方向。\n\n自然的用户认知：根据认知心理学，外部信息约有 80% 通过视觉通道获得。界面设计中最重要的视觉元素，包括布局、色彩、插图、图标等，都应充分吸收自然规律，从而降低用户的认知成本，并带来真实、顺畅的感受。在一些场景中，适时加入听觉、触觉等其他感官通道，也可以创造更丰富、更自然的产品体验。\n\n自然的用户行为：在与系统交互时，设计师应充分理解用户、系统角色和任务目标之间的关系，并结合上下文组织系统功能和服务。同时，可以运用行为分析、人工智能、传感器等方法辅助用户做出有效决策，减少用户的额外操作，节省用户的心智和体力资源，让人机交互更加自然。";

    protected override Type GetResourceKindType() => typeof(WatermarkShowCaseLangResourceKind);
}
