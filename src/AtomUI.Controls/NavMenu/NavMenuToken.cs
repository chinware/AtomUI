using AtomUI.Media;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls.NavMenu;

[ControlDesignToken]
internal class NavMenuToken : AbstractControlDesignToken
{
    public const string ID = "NavMenu";
    
    /// <summary>
    /// 弹出菜单的宽度
    /// </summary>
    public double DropDownWidth { get; set; }
    
    /// <summary>
    /// 分组标题文字颜色
    /// </summary>
    public Color GroupTitleColor { get; set; }
    
    /// <summary>
    /// 分组标题文字高度
    /// </summary>
    public double GroupTitleLineHeight { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public double GroupTitleFontSize { get; set; }
    
    /// <summary>
    /// 菜单项的圆角
    /// </summary>
    public CornerRadius ItemBorderRadius { get; set; }
    
    /// <summary>
    /// 子菜单项的圆角
    /// </summary>
    public CornerRadius SubMenuItemBorderRadius { get; set; }
    
    /// <summary>
    /// 菜单项文字颜色
    /// </summary>
    public Color ItemColor { get; set; }
    
    /// <summary>
    /// 菜单项文字悬浮颜色
    /// </summary>
    public Color ItemHoverColor { get; set; }
    
    /// <summary>
    /// 水平菜单项文字悬浮颜色
    /// </summary>
    public Color HorizontalItemHoverColor { get; set; }
    
    /// <summary>
    /// 菜单项文字选中颜色
    /// </summary>
    public Color ItemSelectedColor { get; set; }
    
    /// <summary>
    /// 水平菜单项文字选中颜色
    /// </summary>
    public Color HorizontalItemSelectedColor { get; set; }
    
    /// <summary>
    /// 菜单项文字禁用颜色
    /// </summary>
    public Color ItemDisabledColor { get; set; }
    
    /// <summary>
    /// 危险菜单项文字颜色
    /// </summary>
    public Color DangerItemColor { get; set; }
    
    /// <summary>
    /// 危险菜单项文字悬浮颜色
    /// </summary>
    public Color DangerItemHoverColor { get; set; }
    
    /// <summary>
    /// 危险菜单项文字选中颜色
    /// </summary>
    public Color DangerItemSelectedColor { get; set; }
    
    /// <summary>
    /// 危险菜单项文字激活颜色
    /// </summary>
    public Color DangerItemActiveBg { get; set; }
    
    /// <summary>
    /// 危险菜单项文字选中颜色
    /// </summary>
    public Color DangerItemSelectedBg { get; set; }
    
    /// <summary>
    /// 菜单项背景色
    /// </summary>
    public Color ItemBg { get; set; }
    
    /// <summary>
    /// 菜单项悬浮态背景色
    /// </summary>
    public Color ItemHoverBg { get; set; }
    
    /// <summary>
    /// 子菜单项背景色
    /// </summary>
    public Color SubMenuItemBg { get; set; }
    
    /// <summary>
    /// 菜单项激活态背景色
    /// </summary>
    public Color ItemActiveBg { get; set; }
    
    /// <summary>
    /// 菜单项选中态背景色
    /// </summary>
    public Color ItemSelectedBg { get; set; }
    
    /// <summary>
    /// 水平菜单项选中态背景色
    /// </summary>
    public Color HorizontalItemSelectedBg { get; set; }
    
    /// <summary>
    /// 菜单项指示条宽度
    /// </summary>
    public double ActiveBarWidth { get; set; }
    
    /// <summary>
    /// 菜单项指示条高度
    /// </summary>
    public double ActiveBarHeight { get; set; }
    
    /// <summary>
    /// 菜单项指示条边框宽度
    /// </summary>
    public double ActiveBarBorderWidth { get; set; }
    
    /// <summary>
    /// 菜单项横向外间距
    /// </summary>
    public Thickness ItemMargin { get; set; }
    
    /// <summary>
    /// 菜单项横向内间距
    /// </summary>
    public Thickness ItemPadding { get; set; }
    
    /// <summary>
    /// 横向菜单项横悬浮态背景色
    /// </summary>
    public Color HorizontalItemHoverBg { get; set; }
    
    /// <summary>
    /// 横向菜单项圆角
    /// </summary>
    public CornerRadius HorizontalItemBorderRadius { get; set; }
    
    /// <summary>
    /// 菜单项高度
    /// </summary>
    public double ItemHeight { get; set; }
    
    /// <summary>
    /// 收起后的宽度
    /// </summary>
    public double CollapsedWidth { get; set; }
    
    /// <summary>
    /// 弹出框背景色
    /// </summary>
    public Color PopupBg { get; set; }
    
    /// <summary>
    /// 横向菜单行高
    /// </summary>
    public double HorizontalLineHeight { get; set; }
    
    /// <summary>
    /// 图标与文字间距
    /// </summary>
    public Thickness IconMargin { get; set; }
    
    /// <summary>
    /// 图标尺寸
    /// </summary>
    public double IconSize { get; set; }
    
    /// <summary>
    /// 收起时图标尺寸
    /// </summary>
    public double CollapsedIconSize { get; set; }
    
    // Dark
    /// <summary>
    /// 暗色模式下的浮层菜单的背景颜色
    /// </summary>
    public Color DarkPopupBg { get; set; }
    
    /// <summary>
    /// 暗色模式下的菜单项文字颜色
    /// </summary>
    public Color DarkItemColor { get; set; }
    
    /// <summary>
    /// 暗色模式下的危险菜单项文字颜色
    /// </summary>
    public Color DarkDangerItemColor { get; set; }
    
    /// <summary>
    /// 暗色模式下的菜单项背景
    /// </summary>
    public Color DarkItemBg { get; set; }
    
    /// <summary>
    /// 暗色模式下的子菜单项背景
    /// </summary>
    public Color DarkSubMenuItemBg { get; set; }
    
    /// <summary>
    /// 暗色模式下的菜单项选中颜色
    /// </summary>
    public Color DarkItemSelectedColor { get; set; }
    
    /// <summary>
    /// 暗色模式下的菜单项选中背景
    /// </summary>
    public Color DarkItemSelectedBg { get; set; }
    
    /// <summary>
    /// 暗色模式下的菜单项悬浮背景
    /// </summary>
    public Color DarkItemHoverBg { get; set; }
    
    /// <summary>
    /// 暗色模式下的分组标题文字颜色
    /// </summary>
    public Color DarkGroupTitleColor { get; set; }
    
    /// <summary>
    /// 暗色模式下的菜单项悬浮颜色
    /// </summary>
    public Color DarkItemHoverColor { get; set; }
    
    /// <summary>
    /// 暗色模式下的菜单项禁用颜色
    /// </summary>
    public Color DarkItemDisabledColor { get; set; }
    
    /// <summary>
    /// 暗色模式下的危险菜单项选中背景
    /// </summary>
    public Color DarkDangerItemSelectedBg { get; set; }
    
    /// <summary>
    /// 暗色模式下的危险菜单项悬浮文字背景
    /// </summary>
    public Color DarkDangerItemHoverColor { get; set; }
    
    /// <summary>
    /// 暗色模式下的危险菜单项选中文字颜色
    /// </summary>
    public Color DarkDangerItemSelectedColor { get; set; }
    
    /// <summary>
    /// 暗色模式下的危险菜单项激活态背景
    /// </summary>
    public Color DarkDangerItemActiveBg { get; set; }
    
    // 内部属性
    public double ItemWidth { get; set; }

    #region 菜单相关 Token

    /// <summary>
    /// 水平菜单高度
    /// </summary>
    public double MenuHorizontalHeight { get; set; }
    
    /// <summary>
    /// 菜单箭头尺寸
    /// </summary>
    public double MenuArrowSize { get; set; }
    
    /// <summary>
    /// 菜单箭头偏移量
    /// </summary>
    public double MenuArrowOffset { get; set; }
    
    /// <summary>
    /// 子菜单背景色
    /// </summary>
    public Color MenuSubMenuBg { get; set; }

    #endregion
    
    public NavMenuToken()
        : base(ID)
    {
    }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        
        var colorTextLightSolid = _globalToken.ColorTextLightSolid;
        
        var activeBarWidth = !double.IsNaN(ActiveBarWidth) ? ActiveBarWidth : 0.0d;
        var activeBarBorderWidth = !double.IsNaN(ActiveBarBorderWidth)
            ? ActiveBarBorderWidth
            : _globalToken.SeedToken.LineWidth;
        var itemMargin    = ItemMargin != default ? ItemMargin : new Thickness(_globalToken.MarginXXS, _globalToken.MarginXXS);
        var colorTextDark = ColorUtils.FromRgbF(
            0.65d,
            colorTextLightSolid.GetRedF(),
            colorTextLightSolid.GetGreenF(),
            colorTextLightSolid.GetBlueF());
        
        var colorNeutralToken = _globalToken.ColorToken.ColorNeutralToken;
        var colorPrimaryToken = _globalToken.ColorToken.ColorPrimaryToken;
        var colorErrorToken = _globalToken.ColorToken.ColorErrorToken;
        
        DropDownWidth               = 160d;
        ItemBorderRadius            = _globalToken.StyleToken.BorderRadiusLG;
        SubMenuItemBorderRadius     = _globalToken.StyleToken.BorderRadiusSM;
        ItemColor                   = colorNeutralToken.ColorText;
        ItemHoverColor              = colorNeutralToken.ColorText;
        HorizontalItemHoverColor    = colorPrimaryToken.ColorPrimary;
        GroupTitleColor             = _globalToken.ColorTextDescription;
        ItemSelectedColor           = colorPrimaryToken.ColorPrimary;
        HorizontalItemSelectedColor = colorPrimaryToken.ColorPrimary;
        ItemBg                      = colorNeutralToken.ColorBgContainer;
        ItemHoverBg                 = _globalToken.ColorBgTextHover;
        ItemActiveBg                = _globalToken.ColorFillContent;
        SubMenuItemBg               = _globalToken.ColorFillAlter;
        ItemSelectedBg              = _globalToken.ControlItemBgActive;
        HorizontalItemSelectedBg    = Colors.Transparent;
        ActiveBarWidth              = activeBarWidth;
        ActiveBarHeight             = _globalToken.StyleToken.LineWidthBold;
        ActiveBarBorderWidth        = activeBarBorderWidth;
        
        // Disabled
        ItemDisabledColor = _globalToken.ColorTextDisabled;
        
        // Danger
        DangerItemColor         = colorErrorToken.ColorError;
        DangerItemHoverColor    = colorErrorToken.ColorError;
        DangerItemSelectedColor = colorErrorToken.ColorError;
        DangerItemActiveBg      = colorErrorToken.ColorError;
        DangerItemSelectedBg    = colorErrorToken.ColorError;

        ItemMargin                 = itemMargin;
        HorizontalItemBorderRadius = new CornerRadius(0);
        HorizontalItemHoverBg      = Colors.Transparent;
        ItemHeight                 = _globalToken.HeightToken.ControlHeightLG;
        GroupTitleLineHeight       = _globalToken.SeedToken.ControlHeight;
        CollapsedWidth             = _globalToken.HeightToken.ControlHeightLG * 2;
        PopupBg                    = colorNeutralToken.ColorBgElevated;
        ItemPadding                = new Thickness(_globalToken.Padding, 0);
        HorizontalLineHeight = _globalToken.HeightToken.ControlHeightLG * 1.15;
        IconSize = _globalToken.FontToken.FontSize;
        IconMargin = new Thickness(0, 0, _globalToken.HeightToken.ControlHeightSM - _globalToken.FontToken.FontSize, 0);
        CollapsedIconSize = _globalToken.FontToken.FontSizeLG;
        GroupTitleFontSize = _globalToken.FontToken.FontSize;
        
        // Disabled
        DarkItemDisabledColor = ColorUtils.FromRgbF(0.25d,
            colorTextLightSolid.GetRedF(),
            colorTextLightSolid.GetGreenF(),
            colorTextLightSolid.GetBlueF());
        
        // Dark
        DarkItemColor       = colorTextDark;
        DarkDangerItemColor = colorErrorToken.ColorError;
        DarkItemBg          = Color.Parse("#001529");
        DarkPopupBg         = Color.Parse("#001529");
        DarkSubMenuItemBg   = Color.Parse("#000c17");

        DarkItemSelectedColor       = colorTextLightSolid;
        DarkItemSelectedBg          = colorPrimaryToken.ColorPrimary;
        DarkDangerItemSelectedBg    = colorErrorToken.ColorError;
        DarkItemHoverBg             = Colors.Transparent;
        DarkGroupTitleColor         = colorTextDark;
        DarkItemHoverColor          = colorTextLightSolid;
        DarkDangerItemHoverColor    = colorErrorToken.ColorErrorHover;
        DarkDangerItemSelectedColor = colorTextLightSolid;
        DarkDangerItemActiveBg      = colorErrorToken.ColorError;
    }
    
}