using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal sealed class SpaceToken : AbstractControlDesignToken
{

    /// <summary>
    /// 小间距尺寸
    /// </summary>
    public double GapSmallSize  { get; set; }
    /// <summary>
    /// 中等间距尺寸
    /// </summary>
    public double GapMiddleSize  { get; set; }
    
    /// <summary>
    /// 大间距尺寸
    /// </summary>
    public double GapLargeSize  { get; set; }
    
    /// <summary>
    /// Add On 背景色
    /// </summary>
    public Color AddonBg { get; set; }
    
    /// <summary>
    /// AddOn 内边距
    /// </summary>
    public Thickness AddOnPadding { get; set; }

    /// <summary>
    /// AddOn 小号内边距
    /// </summary>
    public Thickness AddOnPaddingSM { get; set; }

    /// <summary>
    /// AddOn 大号内边距
    /// </summary>
    public Thickness AddOnPaddingLG { get; set; }
    
    public SpaceToken()

    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        GapSmallSize  = EffectiveGlobalToken.SpacingXS;
        GapMiddleSize = EffectiveGlobalToken.Spacing;
        GapLargeSize  = EffectiveGlobalToken.SpacingLG;
        AddonBg       = EffectiveGlobalToken.ColorFillAlter;
        
        AddOnPadding   = new Thickness(EffectiveGlobalToken.UniformlyPaddingSM, 0);
        AddOnPaddingSM = new Thickness(EffectiveGlobalToken.ControlPaddingHorizontalSM, 0);
        AddOnPaddingLG = new Thickness(EffectiveGlobalToken.ControlPaddingHorizontal, 0);
    }
    
}
