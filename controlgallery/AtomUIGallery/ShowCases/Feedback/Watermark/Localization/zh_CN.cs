using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Watermark;

[LanguageProvider(LanguageCode.zh_CN, WatermarkShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的用法。";
    public const string MultiLineTitle = "多行水印";
    public const string MultiLineDescription = "使用换行指定多行水印内容。";
    public const string ImageWatermarkTitle = "图片水印";
    public const string ImageWatermarkDescription = "通过 image 指定图片地址。为确保图片高清且不被拉伸，请设置宽高，并上传至少两倍于显示宽高的 logo 图片。";
    public const string CustomConfigurationTitle = "自定义配置";
    public const string CustomConfigurationDescription = "通过配置自定义参数预览水印效果。";
    public const string PageSubtitle = "用于受保护内容的文字或图片水印。";
    public const string PageDescription = "Watermark 将重复的文字或图片 Glyph 附加到目标区域，用于标识归属或降低未经授权复用的风险。";
    public const string ComponentCategory = "反馈";
    public const string ComponentStatusStable = ".NET 10";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyGlyph = "渲染在目标 Layoutable 上方的附加水印 Glyph。";
    public const string ApiPropertyHorizontalSpace = "重复水印 Glyph 之间的水平间距。";
    public const string ApiPropertyVerticalSpace = "重复水印行之间的垂直间距。";
    public const string ApiPropertyHorizontalOffset = "每行第一个 Glyph 之前的水平偏移。";
    public const string ApiPropertyVerticalOffset = "第一行水印之前的垂直偏移。";
    public const string ApiPropertyRotate = "应用到每个水印 Glyph 的旋转角度。";
    public const string ApiPropertyOpacity = "渲染水印 Glyph 时使用的不透明度。";
    public const string ApiPropertyIsMirrorUsed = "对交替 Glyph 使用反向旋转形成镜像效果。";
    public const string ApiPropertyIsCrossUsed = "偏移交替行以形成交错布局。";
    public const string ApiPropertyText = "TextGlyph 渲染的文本。";
    public const string ApiPropertyFontSize = "TextGlyph 使用的字体大小。";
    public const string ApiPropertyForeground = "用于渲染 TextGlyph 的画刷。";
    public const string ApiPropertySource = "ImageGlyph 渲染的图片源。";
    public const string ApiPropertyHeight = "ImageGlyph 的渲染高度。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "范围";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameNoComponentToken = "Watermark 当前没有组件专属 Design Token；视觉配置由 Glyph 属性控制。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusNotApplicable = "N/A";
    public const string P2WatermarkMultiLineText = "AtomUI\n快乐工作";
    public const string P2TextNaturalInteractionDescription = "数字世界的高速迭代让产品变得更加复杂，而人的意识和注意力资源是有限的。面对这种设计矛盾，追求自然交互将始终是 Ant Design 的一致方向。\n\n自然的用户认知：根据认知心理学，外部信息约有 80% 通过视觉通道获得。界面设计中最重要的视觉元素，包括布局、色彩、插图、图标等，都应充分吸收自然规律，从而降低用户的认知成本，并带来真实、顺畅的感受。在一些场景中，适时加入听觉、触觉等其他感官通道，也可以创造更丰富、更自然的产品体验。\n\n自然的用户行为：在与系统交互时，设计师应充分理解用户、系统角色和任务目标之间的关系，并结合上下文组织系统功能和服务。同时，可以运用行为分析、人工智能、传感器等方法辅助用户做出有效决策，减少用户的额外操作，节省用户的心智和体力资源，让人机交互更加自然。";

}
