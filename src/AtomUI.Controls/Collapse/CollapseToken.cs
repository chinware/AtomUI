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

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        HeaderPadding             = new Thickness(_globalToken.Padding, _globalToken.PaddingSM);
        HeaderBg                  = _globalToken.ColorFillAlter;
        ContentPadding            = new Thickness(16, _globalToken.Padding);
        CollapseContentPaddingSM  = new Thickness(_globalToken.PaddingSM);
        CollapseContentPaddingLG  = new Thickness(_globalToken.PaddingLG);
        ContentBg                 = _globalToken.ColorToken.ColorNeutralToken.ColorBgContainer;
        CollapseHeaderPaddingSM   = new Thickness(_globalToken.PaddingSM, _globalToken.PaddingXS);
        CollapseHeaderPaddingLG   = new Thickness(_globalToken.PaddingLG, _globalToken.Padding);
        CollapsePanelBorderRadius = _globalToken.StyleToken.BorderRadiusLG;
        LeftExpandButtonMargin    = new Thickness(0, 0, _globalToken.MarginSM, 0);
        RightExpandButtonMargin   = new Thickness(_globalToken.MarginSM, 0, 0, 0);
    }

    #region 内部 Token 定义

    public Thickness CollapseHeaderPaddingSM { get; set; }
    public Thickness CollapseHeaderPaddingLG { get; set; }
    public Thickness CollapseContentPaddingSM { get; set; }
    public Thickness CollapseContentPaddingLG { get; set; }
    public CornerRadius CollapsePanelBorderRadius { get; set; }
    public Thickness LeftExpandButtonMargin { get; set; }
    public Thickness RightExpandButtonMargin { get; set; }

    #endregion
}