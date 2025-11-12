using AtomUI.Theme.TokenSystem;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class TextAreaToken : AbstractControlDesignToken
{
    public const string ID = "TextArea";
    
    public TextAreaToken()
        : base(ID)
    {
    }
    
    /// <summary>
    /// 字体大小
    /// </summary>
    public double FontSize { get; set; }

    /// <summary>
    /// 大号字体大小
    /// </summary>
    public double FontSizeLG { get; set; }

    /// <summary>
    /// 小号字体大小
    /// </summary>
    public double FontSizeSM { get; set; }

    public override void CalculateTokenValues()
    {
        base.CalculateTokenValues();
        FontSize   = SharedToken.FontSize;
        FontSizeLG = SharedToken.FontSizeLG;
        FontSizeSM = SharedToken.FontSizeSM;
    }
}