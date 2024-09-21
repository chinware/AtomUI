using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class ExpanderToken : AbstractControlDesignToken
{
    public const string ID = "Expander";

    public ExpanderToken()
        : base(ID)
    {
    }

    /// <summary>
    /// 折叠面板头部内边距
    /// </summary>
    public Thickness HeaderPadding { get; set; }

    /// <summary>
    /// 折叠面板头部小号内边距
    /// </summary>
    public Thickness HeaderPaddingSM { get; set; }

    /// <summary>
    /// 折叠面板头部大号内边距
    /// </summary>
    public Thickness HeaderPaddingLG { get; set; }

    /// <summary>
    /// 折叠面板头部背景
    /// </summary>
    public Color HeaderBg { get; set; }

    /// <summary>
    /// 折叠面板内容内边距
    /// </summary>
    public Thickness ContentPadding { get; set; }

    /// <summary>
    /// 折叠面板内容小号内边距
    /// </summary>
    public Thickness ContentPaddingSM { get; set; }

    /// <summary>
    /// 折叠面板内容大号内边距
    /// </summary>
    public Thickness ContentPaddingLG { get; set; }

    /// <summary>
    /// 折叠面板内容背景
    /// </summary>
    public Color ContentBg { get; set; }

    #region 内部 Token 定义

    public CornerRadius ExpanderBorderRadius { get; set; }
    public Thickness LeftExpandButtonHMargin { get; set; }
    public Thickness RightExpandButtonHMargin { get; set; }
    public Thickness LeftExpandButtonVMargin { get; set; }
    public Thickness RightExpandButtonVMargin { get; set; }

    #endregion

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        HeaderPadding            = new Thickness(_globalToken.Padding, _globalToken.PaddingSM);
        HeaderBg                 = _globalToken.ColorFillAlter;
        ContentPadding           = new Thickness(16, _globalToken.Padding);
        ContentPaddingSM         = new Thickness(_globalToken.PaddingSM);
        ContentPaddingLG         = new Thickness(_globalToken.PaddingLG);
        ContentBg                = _globalToken.ColorBgContainer;
        HeaderPaddingSM          = new Thickness(_globalToken.PaddingSM, _globalToken.PaddingXS);
        HeaderPaddingLG          = new Thickness(_globalToken.PaddingLG, _globalToken.Padding);
        ExpanderBorderRadius     = _globalToken.BorderRadiusLG;
        LeftExpandButtonHMargin  = new Thickness(0, 0, _globalToken.MarginSM, 0);
        RightExpandButtonHMargin = new Thickness(_globalToken.MarginSM, 0, 0, 0);
        LeftExpandButtonVMargin  = new Thickness(0, 0, 0, _globalToken.MarginSM);
        RightExpandButtonVMargin = new Thickness(0, _globalToken.MarginSM, 0, 0);
    }
}