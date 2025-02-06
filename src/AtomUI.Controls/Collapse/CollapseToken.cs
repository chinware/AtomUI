using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class CollapseToken : AbstractControlDesignToken
{
    public const string ID = "Collapse";

    public CollapseToken()
        : base(ID)
    {
    }

    /// <summary>
    /// 折叠面板头部内边距
    /// </summary>
    public Thickness HeaderPadding { get; set; }

    /// <summary>
    /// 折叠面板头部背景
    /// </summary>
    public Color HeaderBg { get; set; }

    /// <summary>
    /// 折叠面板内容内边距
    /// </summary>
    public Thickness ContentPadding { get; set; }

    /// <summary>
    /// 折叠面板内容背景
    /// </summary>
    public Color ContentBg { get; set; }

    #region 内部 Token 定义

    public Thickness CollapseHeaderPaddingSM { get; set; }
    public Thickness CollapseHeaderPaddingLG { get; set; }
    public Thickness CollapseContentPaddingSM { get; set; }
    public Thickness CollapseContentPaddingLG { get; set; }
    public CornerRadius CollapsePanelBorderRadius { get; set; }
    public Thickness LeftExpandButtonMargin { get; set; }
    public Thickness RightExpandButtonMargin { get; set; }

    #endregion

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        HeaderPadding             = new Thickness(SharedToken.Padding, SharedToken.PaddingSM);
        HeaderBg                  = SharedToken.ColorFillAlter;
        ContentPadding            = new Thickness(16, SharedToken.Padding);
        CollapseContentPaddingSM  = new Thickness(SharedToken.PaddingSM);
        CollapseContentPaddingLG  = new Thickness(SharedToken.PaddingLG);
        ContentBg                 = SharedToken.ColorBgContainer;
        CollapseHeaderPaddingSM   = new Thickness(SharedToken.PaddingSM, SharedToken.PaddingXS);
        CollapseHeaderPaddingLG   = new Thickness(SharedToken.PaddingLG, SharedToken.Padding);
        CollapsePanelBorderRadius = SharedToken.BorderRadiusLG;
        LeftExpandButtonMargin    = new Thickness(0, 0, SharedToken.MarginSM, 0);
        RightExpandButtonMargin   = new Thickness(SharedToken.MarginSM, 0, 0, 0);
    }
}