using AtomUI.Theme.TokenSystem;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class LineEditToken : AbstractControlDesignToken
{
    public const string ID = "LineEdit";

    public LineEditToken()
        : this(ID)
    {
    }

    protected LineEditToken(string id)
        : base(id)
    {
    }

    /// <summary>
    /// 字体大小
    /// </summary>
    public double InputFontSize { get; set; }

    /// <summary>
    /// 大号字体大小
    /// </summary>
    public double InputFontSizeLG { get; set; }

    /// <summary>
    /// 小号字体大小
    /// </summary>
    public double InputFontSizeSM { get; set; }
    
    public override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        InputFontSize   = SharedToken.FontSize;
        InputFontSizeLG = SharedToken.FontSizeLG;
        InputFontSizeSM = SharedToken.FontSizeSM;
    }
}