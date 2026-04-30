using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Media;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class PopupHostToken : AbstractControlDesignToken
{
    public const string ID = "PopupHost";
    
    /// <summary>
    /// Popup 圆角
    /// </summary>
    public CornerRadius BorderRadius { get; set; }

    /// <summary>
    /// OverlayHost 类型的阴影
    /// </summary>
    public BoxShadows OverlayHostShadow { get; set; }
    
    /// <summary>
    /// PopupRoot 类型的阴影
    /// </summary>
    public BoxShadows PopupRootShadow { get; set; }
    
    /// <summary>
    /// 顶层弹出菜单，距离顶层菜单项的边距
    /// </summary>
    public double MarginToAnchor { get; set; }
    
    public PopupHostToken()
        : base(ID)
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        OverlayHostShadow = SharedToken.BoxShadowsSecondary;
        PopupRootShadow = new BoxShadows(new BoxShadow
        {
            OffsetX = 0,
            OffsetY = 1,
            Blur    = 6,
            Spread  = 0,
            Color   = ColorUtils.FromRgbF(0.14, 0, 0, 0)
        }, [new BoxShadow
        {
            OffsetX = 0,
            OffsetY = 0,
            Blur    = 4,
            Spread  = 0,
            Color   = ColorUtils.FromRgbF(0.10, 0, 0, 0)
        }]);
        BorderRadius   = SharedToken.BorderRadiusLG;
        MarginToAnchor = SharedToken.UniformlyMarginXXS;
    }
    
    protected override Type GetTokenKindType() => typeof(PopupHostTokenKind);
}