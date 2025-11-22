using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class DescriptionsToken : AbstractControlDesignToken
{
    public const string ID = "Descriptions";
    
    /// <summary>
    /// 标签背景色
    /// Background color of label
    /// </summary>
    public Color LabelBg { get; set; }
    
    /// <summary>
    /// 标签文字颜色
    /// Text color of label
    /// </summary>
    public Color LabelColor { get; set; }
    
    /// <summary>
    /// 标题文字颜色
    /// Text color of title
    /// </summary>
    public Color TitleColor { get; set; }
    
    /// <summary>
    /// 标题下间距
    /// Bottom margin of title
    /// </summary>
    public Thickness TitleMargin { get; set; }
    
    /// <summary>
    /// 子项下间距
    /// Bottom padding of item
    /// </summary>
    public Thickness ItemPadding { get; set; }
    
    /// <summary>
    /// 冒号间距
    /// margin of colon
    /// </summary>
    public Thickness ColonMargin { get; set; }
    
    /// <summary>
    /// 内容区域文字颜色
    /// Text color of content
    /// </summary>
    public Color ContentColor { get; set; }
    
    /// <summary>
    /// 额外区域文字颜色
    /// Text color of extra area
    /// </summary>
    public Color ExtraColor { get; set; }
    
    public DescriptionsToken()
        : base(ID)
    {
    }
    
    public override void CalculateTokenValues()
    {
        base.CalculateTokenValues();
        LabelBg      = SharedToken.ColorFillAlter;
        LabelColor   = SharedToken.ColorTextTertiary;
        TitleColor   = SharedToken.ColorText;
        TitleMargin  = new Thickness(0, 0, 0, SharedToken.FontSizeSM * SharedToken.RelativeLineHeightSM);
        ItemPadding  = new Thickness(0, 0, SharedToken.UniformlyPadding, SharedToken.UniformlyPadding);
        ColonMargin  = new Thickness(SharedToken.UniformlyMarginXXS / 2, 0, SharedToken.UniformlyMarginXS, 0);
        ContentColor = SharedToken.ColorText;
        ExtraColor   = SharedToken.ColorText;
    }
}