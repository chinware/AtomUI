using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class StatisticToken : AbstractControlDesignToken
{
    public const string ID = "Statistic";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);
    
    /// <summary>
    /// 标题字体大小
    /// Title font size
    /// </summary>
    public double TitleFontSize { get; set; }
    
    /// <summary>
    /// 内容字体大小
    /// Content font size
    /// </summary>
    public double ContentFontSize { get; set; }
    
    public StatisticToken()
        : base(ID)
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        TitleFontSize = SharedToken.FontSize;
        ContentFontSize = SharedToken.FontSizeHeading3;
    }
    
    protected override Type GetTokenKindType() => typeof(StatisticTokenKind);
}