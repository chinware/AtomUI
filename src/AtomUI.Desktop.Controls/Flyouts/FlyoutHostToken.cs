using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class FlyoutHostToken : AbstractControlDesignToken
{
    public const string ID = "FlyoutHost";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);
    
    /// <summary>
    /// 默认 Popup 和 PlacementTarget 的间距
    /// </summary>
    public double MarginToAnchor { get; set; }
    
    /// <summary>
    /// OverlayHost 类型的阴影
    /// </summary>
    public BoxShadows OverlayHostShadow { get; set; }
    
    /// <summary>
    /// PopupRoot 类型的阴影
    /// </summary>
    public BoxShadows PopupRootShadow { get; set; }

    public FlyoutHostToken()
        : base(ID)
    {
    }
    
    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        MarginToAnchor    = SharedToken.UniformlyMarginXXS;
        OverlayHostShadow = SharedToken.BoxShadowsSecondary;
        PopupRootShadow = new BoxShadows(new BoxShadow
        {
            OffsetX = 0,
            OffsetY = 1,
            Blur    = 6,
            Spread  = 0,
            Color   = ColorUtils.FromRgbF(0.12, 0, 0, 0)
        }, [new BoxShadow
        {
            OffsetX = 0,
            OffsetY = 0,
            Blur    = 2,
            Spread  = 0,
            Color   = ColorUtils.FromRgbF(0.08, 0, 0, 0)
        }]);
    }
    
    protected override Type GetTokenKindType() => typeof(FlyoutHostTokenKind);
}