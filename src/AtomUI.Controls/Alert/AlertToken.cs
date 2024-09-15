using AtomUI.Theme.TokenSystem;
using Avalonia;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class AlertToken : AbstractControlDesignToken
{
    public const string ID = "Alert";

    public AlertToken()
        : base(ID)
    {
    }

    /// <summary>
    /// 默认内间距
    /// </summary>
    public Thickness DefaultPadding { get; set; }

    /// <summary>
    /// 带有描述的内间距
    /// </summary>
    public Thickness WithDescriptionPadding { get; set; }

    /// <summary>
    /// 带有描述的 Message 外间距
    /// </summary>
    public Thickness MessageWithDescriptionMargin { get; set; }

    /// <summary>
    /// 图标默认外间距
    /// </summary>
    public Thickness IconDefaultMargin { get; set; }

    /// <summary>
    /// 图标带描述信息外间距
    /// </summary>
    public Thickness IconWithDescriptionMargin { get; set; }

    /// <summary>
    /// 没有描述时的图标尺寸
    /// </summary>
    public double IconSize { get; set; }

    /// <summary>
    /// 带有描述时的图标尺寸
    /// </summary>
    public double WithDescriptionIconSize { get; set; }

    /// <summary>
    /// 关闭按钮的大小
    /// </summary>
    public double CloseIconSize { get; set; }

    /// <summary>
    /// 额外元素的外间距
    /// </summary>
    public Thickness ExtraElementMargin { get; set; }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        const double paddingHorizontal = 12; // Fixed value here.
        WithDescriptionIconSize = _globalToken.FontToken.FontSizeHeading3;
        DefaultPadding          = new Thickness(paddingHorizontal, _globalToken.PaddingContentVerticalSM);
        WithDescriptionPadding  = new Thickness(_globalToken.PaddingContentHorizontalLG, _globalToken.PaddingMD);

        MessageWithDescriptionMargin = new Thickness(0, 0, 0, _globalToken.MarginXS);
        IconDefaultMargin            = new Thickness(0, 0, _globalToken.MarginXS, 0);
        IconWithDescriptionMargin    = new Thickness(0, 0, _globalToken.MarginSM, 0);
        ExtraElementMargin           = new Thickness(_globalToken.MarginXS, 0, 0, 0);

        CloseIconSize = _globalToken.FontSizeIcon + 2;
        IconSize      = _globalToken.FontToken.FontSizeLG;
    }
}