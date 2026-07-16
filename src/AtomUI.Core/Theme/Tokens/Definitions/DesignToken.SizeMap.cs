namespace AtomUI.Theme.TokenSystem;

[GlobalDesignToken]
public partial class DesignToken
{
    /// <summary>
    /// XXL
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double SizeXXL { get; set; } = 48;

    /// <summary>
    /// XL
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double SizeXL { get; set; } = 32;

    /// <summary>
    /// LG
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double SizeLG { get; set; } = 24;

    /// <summary>
    /// MD
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double SizeMD { get; set; } = 20;

    /// <summary>
    /// Same as size by default, but could be larger in compact mode
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double SizeMS { get; set; }

    /// <summary>
    /// 默认
    /// 默认尺寸
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double Size { get; set; } = 16;

    /// <summary>
    /// SM
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double SizeSM { get; set; } = 12;

    /// <summary>
    /// XS
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double SizeXS { get; set; } = 8;

    /// <summary>
    /// XXS
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double SizeXXS { get; set; } = 4;
}