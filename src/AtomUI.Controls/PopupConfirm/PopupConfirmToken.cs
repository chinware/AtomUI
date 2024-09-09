using AtomUI.Theme.TokenSystem;
using Avalonia;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class PopupConfirmToken : AbstractControlDesignToken
{
    public const string ID = "PopupConfirm";

    public PopupConfirmToken()
        : base(ID)
    {
    }

    /// <summary>
    ///     菜单 Popup 最小宽度
    /// </summary>
    public double PopupMinWidth { get; set; }

    /// <summary>
    ///     菜单 Popup 最小高度
    /// </summary>
    public double PopupMinHeight { get; set; }

    /// <summary>
    ///     按钮的外边距
    /// </summary>
    public Thickness ButtonMargin { get; set; }

    /// <summary>
    ///     Icon 外边距
    /// </summary>
    public Thickness IconMargin { get; set; }

    /// <summary>
    ///     主内容区域外边距
    /// </summary>
    public Thickness ContentContainerMargin { get; set; }

    /// <summary>
    ///     标题栏外边距
    /// </summary>
    public Thickness TitleMargin { get; set; }

    /// <summary>
    ///     按钮区域外边距
    /// </summary>
    public Thickness ButtonContainerMargin { get; set; }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        PopupMinWidth  = 240;
        PopupMinHeight = 80;
        ButtonMargin   = new Thickness(_globalToken.MarginXS, 0, 0, 0);
        IconMargin = new Thickness(_globalToken.MarginXS, _globalToken.MarginXS + _globalToken.MarginXXS / 2,
            _globalToken.MarginXS, 0);
        ContentContainerMargin = new Thickness(0, 0, 0, _globalToken.MarginXS);
        TitleMargin            = new Thickness(0, _globalToken.MarginXS, 0, _globalToken.MarginXS);
        ButtonContainerMargin  = new Thickness(0, 0, 0, _globalToken.MarginXS / 2);
    }
}