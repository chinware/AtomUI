using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class AvatarToken : AbstractControlDesignToken
{
    public const string ID = "Avatar";

    public AvatarToken()
        : base(ID)
    {
    }

    /**
     * 头像尺寸
     */
    public double ContainerSize { get; set; }

    /**
     * 大号头像尺寸
     */
    public double ContainerSizeLG { get; set; }

    /**
     * 小号头像尺寸
     */
    public double ContainerSizeSM { get; set; }

    /**
     * 头像组重叠宽度
     */
    public double GroupOverlapping { get; set; }

    /**
    * 头像组边框颜色
    */
    public Color GroupBorderColor { get; set; }

    /**
     * 头像组边框颜色
     */
    public double GroupSpace { get; set; }

    /**
     * 头像文字大小
     */
    public double TextFontSize { get; set; }

    /**
     * 大号头像文字大小
     */
    public double TextFontSizeLG { get; set; }
    
    /**
     * 小号头像文字大小
     */
    public double TextFontSizeSM { get; set; }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();

        ContainerSize    = SharedToken.ControlHeight;
        ContainerSizeLG  = SharedToken.ControlHeightLG;
        ContainerSizeSM  = SharedToken.ControlHeightSM;
        TextFontSize     = Math.Round((SharedToken.FontSizeLG + SharedToken.FontSizeXL) / 2);
        TextFontSizeLG   = SharedToken.FontSizeHeading3;
        TextFontSizeSM   = SharedToken.FontSize;
        GroupSpace       = SharedToken.MarginXXS;
        GroupOverlapping = -SharedToken.MarginXS;
        GroupBorderColor = SharedToken.ColorBorderBg;
    }
}