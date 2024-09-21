namespace AtomUI.Theme.TokenSystem;

[GlobalDesignToken]
public partial class GlobalToken
{
    /// <summary>
    /// XXL
    /// </summary>
    public double SizeXXL { get; set; } = 48;

    /// <summary>
    /// XL
    /// </summary>
    public double SizeXL { get; set; } = 32;

    /// <summary>
    /// LG
    /// </summary>
    public double SizeLG { get; set; } = 24;

    /// <summary>
    /// MD
    /// </summary>
    public double SizeMD { get; set; } = 20;

    /// <summary>
    /// Same as size by default, but could be larger in compact mode
    /// </summary>
    public double SizeMS { get; set; }

    /// <summary>
    /// 默认
    /// 默认尺寸
    /// </summary>
    public double Size { get; set; } = 16;

    /// <summary>
    /// SM
    /// </summary>
    public double SizeSM { get; set; } = 12;

    /// <summary>
    /// XS
    /// </summary>
    public double SizeXS { get; set; } = 8;

    /// <summary>
    /// XXS
    /// </summary>
    public double SizeXXS { get; set; } = 4;
}