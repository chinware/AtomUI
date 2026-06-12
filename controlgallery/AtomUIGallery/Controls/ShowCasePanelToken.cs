using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using AtomUIGallery.Controls.DesignTokens;
using Avalonia;

namespace AtomUIGallery.Controls;

[ControlDesignToken]
internal class ShowCasePanelToken : AbstractControlDesignToken
{
    public const string ID = "ShowCasePanel";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);

    public Thickness ContentMargin { get; set; }
    public double MinItemWidth { get; set; }
    public int MaxColumns { get; set; }
    public double ColumnGap { get; set; }
    public double RowGap { get; set; }

    public ShowCasePanelToken()
        : base(ID)
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        ContentMargin = new Thickness(SharedToken.SizeUnit * 6);
        MinItemWidth  = SharedToken.SizeUnit * 90;
        MaxColumns    = 2;
        ColumnGap     = SharedToken.SizeUnit * 4;
        RowGap        = SharedToken.SizeUnit * 4;
    }

    protected override Type GetTokenKindType() => typeof(ShowCasePanelTokenKind);
}
