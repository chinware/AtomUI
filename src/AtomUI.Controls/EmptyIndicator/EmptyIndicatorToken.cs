using AtomUI.Theme.TokenSystem;

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

    public EmptyIndicatorToken()
        : base(ID)
    {
    }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        var controlHeightLG = _globalToken.HeightToken.ControlHeightLG;
        EmptyImgHeight   = controlHeightLG * 2.5;
        EmptyImgHeightMD = controlHeightLG;
        EmptyImgHeightSM = controlHeightLG * 0.875;
    }
}