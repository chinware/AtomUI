using AtomUI.Styling;
using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class MenuToken : AbstractControlDesignToken
{
   public const string ID = "Menu";
   
   public MenuToken()
      : base(ID)
   {
   }
   
   /// <summary>
   /// 菜单的圆角
   /// </summary>
   public CornerRadius MenuPopupBorderRadius { get; set; }
   
   /// <summary>
   /// 菜单 Popup 阴影
   /// </summary>
   public BoxShadows MenuPopupBoxShadows { get; set; }
   
   /// <summary>
   /// 菜单内容边距
   /// </summary>
   public Thickness MenuPopupContentPadding { get; set; }
   
   /// <summary>
   /// 菜单 Popup 最小宽度
   /// </summary>
   public double MenuPopupMinWidth { get; set; }
   
   /// <summary>
   /// 菜单 Popup 最大宽度
   /// </summary>
   public double MenuPopupMaxWidth { get; set; }
   
   /// <summary>
   /// 菜单 Popup 最小高度
   /// </summary>
   public double MenuPopupMinHeight { get; set; }
   
   /// <summary>
   /// 菜单 Popup 最大高度
   /// </summary>
   public double MenuPopupMaxHeight { get; set; }
   
   /// <summary>
   /// 子菜单指示三角形的大小
   /// </summary>
   public double MenuArrowSize { get; set; }
   
   /// <summary>
   /// 子菜单指示三角形的位移
   /// </summary>
   public double MenuArrowOffset { get; set; }
   
   /// <summary>
   /// 分离菜单项的高度，这个用于菜单中快捷功能的图标显示
   /// TODO 暂时还没实现，但是最终会实现
   /// </summary>
   public double MenuTearOffHeight { get; set; }
   
   /// <summary>
   /// 弹出框背景色
   /// </summary>
   public Color MenuBgColor { get; set; }
   
   /// <summary>
   /// 菜单项文字颜色
   /// </summary>
   public Color ItemColor { get; set; }
   
   /// <summary>
   /// 快捷键颜色
   /// </summary>
   public Color KeyGestureColor { get; set; }
   
   /// <summary>
   /// 菜单项边距
   /// </summary>
   public Thickness ItemMargin { get; set; }
   
   /// <summary>
   /// 菜单项文字悬浮颜色
   /// </summary>
   public Color ItemHoverColor { get; set; }
   
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
   /// 菜单项背景色
   /// </summary>
   public Color ItemBg { get; set; }
   
   /// <summary>
   /// 菜单项悬浮态背景色
   /// </summary>
   public Color ItemHoverBg { get; set; }
   
   /// <summary>
   /// 菜单项高度
   /// </summary>
   public double ItemHeight { get; set; }
   
   /// <summary>
   /// 图标尺寸
   /// </summary>
   public double ItemIconSize { get; set; }
   
   /// <summary>
   /// 图标与文字间距
   /// </summary>
   public double ItemIconMarginInlineEnd { get; set; }
   
   /// <summary>
   /// 菜单项的圆角
   /// </summary>
   public CornerRadius ItemBorderRadius { get; set; }
   
   /// <summary>
   /// 菜单项横向内间距
   /// </summary>
   public Thickness ItemPaddingInline { get; set; }
   
   /// <summary>
   /// 顶层菜单项颜色
   /// </summary>
   public Color TopLevelItemColor { get; set; }
   
   /// <summary>
   /// 顶层菜单项选中颜色
   /// </summary>
   public Color TopLevelItemSelectedColor { get; set; }
   
   /// <summary>
   /// 顶层菜单项鼠标放上去的颜色
   /// </summary>
   public Color TopLevelItemHoverColor { get; set; }
   
   /// <summary>
   /// 顶层菜单项背景色
   /// </summary>
   public Color TopLevelItemBg { get; set; }
   
   /// <summary>
   /// 顶层菜单项选中时背景色
   /// </summary>
   public Color TopLevelItemSelectedBg { get; set; }
   
   /// <summary>
   /// 顶层菜单项鼠标放上去背景色
   /// </summary>
   public Color TopLevelItemHoverBg { get; set; }
   
   /// <summary>
   /// 顶层菜单项小号圆角
   /// </summary>
   public CornerRadius TopLevelItemBorderRadiusSM { get; set; }
   
   /// <summary>
   /// 顶层菜单项圆角
   /// </summary>
   public CornerRadius TopLevelItemBorderRadius { get; set; }
   
   /// <summary>
   /// 顶层菜单项大号圆角
   /// </summary>
   public CornerRadius TopLevelItemBorderRadiusLG { get; set; }
   
   /// <summary>
   /// 顶层菜单项小号内间距
   /// </summary>
   public Thickness TopLevelItemPaddingSM { get; set; }
   
   /// <summary>
   /// 顶层菜单项间距
   /// </summary>
   public Thickness TopLevelItemPadding { get; set; }
   
   /// <summary>
   /// 顶层菜单项大号内间距
   /// </summary>
   public Thickness TopLevelItemPaddingLG { get; set; }

   /// <summary>
   /// 顶层菜单项小号字体
   /// </summary>
   public double TopLevelItemFontSizeSM { get; set; } = double.NaN;
   
   /// <summary>
   /// 顶层菜单项字体
   /// </summary>
   public double TopLevelItemFontSize { get; set; } = double.NaN;
   
   /// <summary>
   /// 顶层菜单项大号字体
   /// </summary>
   public double TopLevelItemFontSizeLG { get; set; } = double.NaN;
   
   /// <summary>
   /// 顶层菜单项内容字体行高
   /// </summary>
   public double TopLevelItemLineHeight { get; set; } = double.NaN;

   /// <summary>
   /// 大号顶层菜单项内容字体行高
   /// </summary>
   public double TopLevelItemLineHeightLG { get; set; } = double.NaN;

   /// <summary>
   /// 小号顶层菜单项内容字体行高
   /// </summary>
   public double TopLevelItemLineHeightSM { get; set; } = double.NaN;
   
   /// <summary>
   /// 顶层弹出菜单，距离顶层菜单项的边距
   /// </summary>
   public double TopLevelItemPopupMarginToAnchor { get; set; }
   
   /// <summary>
   /// 菜单分割项的高度
   /// </summary>
   public double SeparatorItemHeight { get; set; }
   
   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      var colorErrorToken = _globalToken.ColorToken.ColorErrorToken;
      var colorNeutralToken = _globalToken.ColorToken.ColorNeutralToken;
      
      var colorTextDisabled = _globalToken.ColorTextDisabled;
      var colorError = colorErrorToken.ColorError;
      var colorTextSecondary = colorNeutralToken.ColorTextQuaternary;
      var colorBgContainer = colorNeutralToken.ColorBgContainer;
      var colorBgTextHover = _globalToken.ColorBgTextHover;
      var colorBgElevated = colorNeutralToken.ColorBgElevated;
      var padding = _globalToken.Padding;
      var controlHeight = _globalToken.SeedToken.ControlHeight;
      var controlHeightSM = base._globalToken.HeightToken.ControlHeightSM;
      var controlHeightLG = _globalToken.HeightToken.ControlHeightLG;
      
      var fontSize = _globalToken.FontToken.FontSize;
      var fontSizeLG = _globalToken.FontToken.FontSizeLG;

      KeyGestureColor = colorTextSecondary;
      ItemBorderRadius = _globalToken.SeedToken.BorderRadius;
      ItemColor = colorNeutralToken.ColorText;
      ItemHoverColor = ItemColor;
      ItemBg = colorBgContainer;
      ItemHoverBg = colorBgTextHover;
      ItemMargin = new Thickness(0, 0, _globalToken.MarginXXS, 0);

      // Disabled
      ItemDisabledColor = colorTextDisabled;
      
      // Danger
      DangerItemColor = colorError;
      DangerItemHoverColor = colorError;
      
      ItemHeight = controlHeightSM;
      MenuBgColor = colorBgElevated;
      
      ItemPaddingInline = new Thickness(padding, _globalToken.PaddingXXS);
      ItemIconSize = fontSize;
      ItemIconMarginInlineEnd = controlHeightSM - fontSize;
      
      TopLevelItemColor = colorNeutralToken.ColorText;
      TopLevelItemSelectedColor = colorNeutralToken.ColorTextSecondary;
      TopLevelItemHoverColor = colorNeutralToken.ColorTextSecondary;

      TopLevelItemBg = colorBgContainer;
      TopLevelItemHoverBg = colorBgTextHover;
      TopLevelItemSelectedBg = colorBgTextHover;

      TopLevelItemBorderRadiusSM = _globalToken.StyleToken.BorderRadiusSM;
      TopLevelItemBorderRadius = _globalToken.SeedToken.BorderRadius;
      TopLevelItemBorderRadiusLG = _globalToken.StyleToken.BorderRadiusLG;
      
      TopLevelItemFontSize = !double.IsNaN(TopLevelItemFontSize) ? TopLevelItemFontSize : fontSize;
      TopLevelItemFontSizeSM = !double.IsNaN(TopLevelItemFontSizeSM) ? TopLevelItemFontSizeSM : fontSize;
      TopLevelItemFontSizeLG = !double.IsNaN(TopLevelItemFontSizeLG) ? TopLevelItemFontSizeLG : fontSizeLG;
      
      TopLevelItemLineHeight = !double.IsNaN(TopLevelItemLineHeight)
         ? TopLevelItemLineHeight
         : CalculatorUtils.CalculateLineHeight(TopLevelItemFontSize) * TopLevelItemFontSize;
      TopLevelItemLineHeightSM = !double.IsNaN(TopLevelItemLineHeightSM)
         ? TopLevelItemLineHeightSM
         : CalculatorUtils.CalculateLineHeight(TopLevelItemFontSizeSM) * TopLevelItemFontSizeSM;
      TopLevelItemLineHeightLG = !double.IsNaN(TopLevelItemLineHeightLG)
         ? TopLevelItemLineHeightLG
         : CalculatorUtils.CalculateLineHeight(TopLevelItemFontSizeLG) * TopLevelItemFontSizeLG;

      TopLevelItemPaddingSM = new Thickness(_globalToken.PaddingContentHorizontalXS * 0.7, 
                                            Math.Max((controlHeightSM - TopLevelItemLineHeightSM) / 2, 0));
      TopLevelItemPadding = new Thickness(_globalToken.PaddingContentHorizontalXS, 
                              Math.Max((controlHeight - TopLevelItemLineHeight) / 2, 0));
      TopLevelItemPaddingLG = new Thickness(_globalToken.PaddingContentHorizontalSM, 
                                            Math.Max((controlHeightLG - TopLevelItemLineHeightLG) / 2, 0));

      TopLevelItemPopupMarginToAnchor = _globalToken.MarginXXS;
      
      MenuPopupBorderRadius = _globalToken.StyleToken.BorderRadiusLG;
      
      MenuPopupMinWidth = 120;
      MenuPopupMaxWidth = 800;

      MenuPopupMinHeight = ItemHeight * 3;
      MenuPopupMaxHeight = ItemHeight * 30;

      SeparatorItemHeight = _globalToken.SeedToken.LineWidth * 5; // 上下两像素，留一像素给自己
      
      MenuArrowSize = (fontSize / 7.0) * 5.0;
      MenuArrowOffset = MenuArrowSize * 0.5;
      MenuTearOffHeight = ItemHeight * 1.2; // 暂时这么定义吧
      
      MenuPopupContentPadding = new Thickness(_globalToken.PaddingXXS, MenuPopupBorderRadius.TopLeft / 2);
      MenuPopupBoxShadows = _globalToken.BoxShadowsSecondary;
   }
}