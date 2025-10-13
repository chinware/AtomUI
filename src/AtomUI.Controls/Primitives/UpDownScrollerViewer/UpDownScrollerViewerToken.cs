using AtomUI.Theme.TokenSystem;
using Avalonia;

namespace AtomUI.Controls.Primitives;

[ControlDesignToken]
internal class UpDownScrollerViewerToken : AbstractControlDesignToken
{
    public const string ID = "UpDownScrollerViewer";
    
    /// <summary>
    /// 滚动按钮 Icon 大小
    /// </summary>
    public double ScrollButtonIconSize { get; set; }

    /// <summary>
    /// 滚动按内边距
    /// </summary>
    public Thickness ScrollButtonPadding { get; set; }

    /// <summary>
    /// 滚动按内边距
    /// </summary>
    public Thickness ScrollButtonMargin { get; set; }
    
    public UpDownScrollerViewerToken()
        : base(ID)
    {
    }
    
    public override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        ScrollButtonIconSize = SharedToken.IconSizeSM;
        ScrollButtonPadding  = SharedToken.PaddingXS;
        ScrollButtonMargin   = new Thickness(SharedToken.UniformlyMarginXXS / 2);
    }
}