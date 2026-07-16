namespace AtomUI.Theme.TokenSystem;

[GlobalDesignToken]
public partial class DesignToken
{
    // Font Size
    /// <summary>
    /// 小号字体大小
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double FontSizeSM { get; set; }

    /// <summary>
    /// 大号字体大小
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double FontSizeLG { get; set; }

    /// <summary>
    /// 超大号字体大小
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double FontSizeXL { get; set; }

    /// <summary>
    /// 一级标题字号
    /// H1 标签所使用的字号
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double FontSizeHeading1 { get; set; } = 38;

    /// <summary>
    /// 二级标题字号
    /// h2 标签所使用的字号
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double FontSizeHeading2 { get; set; } = 30;

    /// <summary>
    /// 三级标题字号
    /// h3 标签使用的字号
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double FontSizeHeading3 { get; set; } = 24;

    /// <summary>
    /// 四级标题字号
    /// h4 标签使用的字号
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double FontSizeHeading4 { get; set; } = 20;

    /// <summary>
    /// 五级标题字号
    /// h5 标签使用的字号
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double FontSizeHeading5 { get; set; } = 16;

    // LineHeight
    /// <summary>
    /// 文本行高
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double RelativeLineHeight { get; set; }

    /// <summary>
    /// 大型文本行高
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double RelativeLineHeightLG { get; set; }

    /// <summary>
    /// 小型文本行高
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double RelativeLineHeightSM { get; set; }

    // TextHeight
    /// <summary>
    /// Round of fontSize * lineHeight
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double FontHeight { get; set; }

    /// <summary>
    /// Round of fontSizeSM * lineHeightSM
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double FontHeightSM { get; set; }

    /// <summary>
    /// Round of fontSizeLG * lineHeightLG
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double FontHeightLG { get; set; }

    /// <summary>
    /// 一级标题行高
    /// H1 标签所使用的行高
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double RelativeLineHeightHeading1 { get; set; }

    /// <summary>
    /// 二级标题行高
    /// h2 标签所使用的行高
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double RelativeLineHeightHeading2 { get; set; }

    /// <summary>
    /// 三级标题行高
    /// h3 标签所使用的行高
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double RelativeLineHeightHeading3 { get; set; }

    /// <summary>
    /// 四级标题行高
    /// h4 标签所使用的行高
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double RelativeLineHeightHeading4 { get; set; }

    /// <summary>
    /// 五级标题行高
    /// h5 标签所使用的行高
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double RelativeLineHeightHeading5 { get; set; }
}