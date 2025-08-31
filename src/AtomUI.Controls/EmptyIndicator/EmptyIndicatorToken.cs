using AtomUI.Theme.TokenSystem;
using Avalonia;

namespace AtomUI.Controls;

[ControlDesignToken]
public class EmptyIndicatorToken : AbstractControlDesignToken
{
    public const string ID = "EmptyIndicator";

    /// <summary>
    /// 空图片的高度
    /// </summary>
    public double EmptyImgHeight { get; set; }

    public double EmptyImgHeightSM { get; set; }
    public double EmptyImgHeightMD { get; set; }
    
    public Thickness DescriptionMargin { get; set; }
    public Thickness DescriptionMarginSM { get; set; }

    public EmptyIndicatorToken()
        : base(ID)
    {
    }

    public override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        var controlHeightLG = SharedToken.ControlHeightLG;
        EmptyImgHeight      = controlHeightLG * 2.5;
        EmptyImgHeightMD    = controlHeightLG;
        EmptyImgHeightSM    = controlHeightLG * 0.875;
        DescriptionMargin   = new Thickness(0, SharedToken.UniformlyMarginSM, 0, 0);
        DescriptionMarginSM = new Thickness(0, SharedToken.UniformlyMarginXS, 0, 0);
    }
}