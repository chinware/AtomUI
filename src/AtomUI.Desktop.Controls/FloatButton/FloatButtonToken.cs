using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class FloatButtonToken : AbstractControlDesignToken
{
    public const string ID = "FloatButton";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);
    
    public FloatButtonToken()
        : base(ID)
    {
    }
    
    /// <summary>
    /// FloatButton 尺寸
    /// Size of FloatButton
    /// </summary>
    public double FloatButtonSize { get; set; }
    
    /// <summary>
    /// FloatButton 图标尺寸
    /// Icon size of FloatButton
    /// </summary>
    public double FloatButtonIconSize { get; set; }
    
    /// <summary>
    /// 正方形的徽章偏移大小
    /// </summary>
    public double SquareBadgeOffset { get; set; }
    
    /// <summary>
    /// 原型的徽章偏移大小
    /// </summary>
    public double CircleBadgeOffset { get; set; }
    
    /// <summary>
    /// 主要按钮文本颜色
    /// </summary>
    public Color PrimaryColor { get; set; }
    
    /// <summary>
    /// 描述文本行高
    /// </summary>
    public double DescriptionLineHeight { get; set; }
    
    /// <summary>
    /// 默认的基于定位的水平偏移值
    /// </summary>
    public double FloatOffsetX { get; set; }
    
    /// <summary>
    /// 默认的基于定位的垂直偏移值
    /// </summary>
    public double FloatOffsetY { get; set; }
    
    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        PrimaryColor          = SharedToken.ColorTextLightSolid;
        FloatButtonIconSize   = SharedToken.FontSizeIcon * 1.5;
        FloatButtonSize       = SharedToken.ControlHeightLG;
        DescriptionLineHeight = SharedToken.FontSizeSM * 1.2;
        FloatOffsetX          = SharedToken.UniformlyMargin;
        FloatOffsetY          = SharedToken.UniformlyMargin;
        var r       = Math.Sqrt(2);
        var offsetR = (r - 1) / r;
        SquareBadgeOffset = SharedToken.BorderRadius.BottomLeft * offsetR;
        CircleBadgeOffset = SharedToken.ControlHeight / 2 * offsetR;
    }
    
    protected override Type GetTokenKindType() => typeof(FloatButtonTokenKind);
}