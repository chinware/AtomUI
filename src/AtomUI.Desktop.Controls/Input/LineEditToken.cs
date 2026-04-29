using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class LineEditToken : AbstractControlDesignToken
{
    public const string ID = "LineEdit";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);
    
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
    
    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        InputFontSize   = SharedToken.FontSize;
        InputFontSizeLG = SharedToken.FontSizeLG;
        InputFontSizeSM = SharedToken.FontSizeSM;
    }

    protected override Type GetTokenKindType() => typeof(LineEditTokenKind);
}