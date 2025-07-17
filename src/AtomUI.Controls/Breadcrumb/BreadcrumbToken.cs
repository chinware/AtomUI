using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class BreadcrumbToken : AbstractControlDesignToken
{
    public const string ID = "Breadcrumb";
    
    /// <summary>
    /// 图标大小
    /// </summary>
    public double IconFontSize { get; set; }

    /// <summary>
    /// 面包屑项文字颜色
    /// </summary>
    public Color ItemColor { get; set; }
    
    /// <summary>
    /// 最后一项文字颜色
    /// </summary>
    public Color LastItemColor { get; set; }
    
    /// <summary>
    /// 链接文字颜色
    /// </summary>
    public Color LinkColor { get; set; }
    
    /// <summary>
    /// 链接文字悬浮颜色
    /// </summary>
    public Color LinkHoverColor { get; set; }
    
    /// <summary>
    /// 分隔符颜色
    /// </summary>
    public Color SeparatorColor { get; set; }
    
    /// <summary>
    /// 分隔符外间距
    /// </summary>
    public Thickness SeparatorMargin { get; set; }
    
    public BreadcrumbToken()
        : base(ID)
    {
    }
    
    protected internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        IconFontSize = SharedToken.FontSize;
        var universalColor = SharedToken.ColorText;
        ItemColor       = universalColor;
        LastItemColor   = universalColor;
        LinkColor       = universalColor;
        //LinkHoverColor  = SharedToken.ColorLinkHover;
        LinkHoverColor  = Colors.Red;
        SeparatorColor  = universalColor;
        SeparatorMargin = SharedToken.MarginMD;
        /*
        ContainerSize    = SharedToken.ControlHeight;
        ContainerSizeLG  = SharedToken.ControlHeightLG;
        ContainerSizeSM  = SharedToken.ControlHeightSM;
        TextFontSize     = Math.Round((SharedToken.FontSizeLG + SharedToken.FontSizeXL) / 2);
        TextFontSizeLG   = SharedToken.FontSizeHeading3;
        TextFontSizeSM   = SharedToken.FontSize;
        GroupSpace       = SharedToken.UniformlyMarginXXS;
        GroupOverlapping = SharedToken.UniformlyMarginXS;
        GroupBorderColor = SharedToken.ColorBorderBg;
        AvatarBg    = SharedToken.ColorTextPlaceholder;
        AvatarColor = SharedToken.ColorTextLightSolid;
        */
    }
}