using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class TabControlToken : AbstractControlDesignToken
{
   public const string ID = "TabControl";

   public TabControlToken()
      : base(ID) { }
   
   /// <summary>
   /// 卡片标签页背景色
   /// </summary>
   public Color CardBg { get; set; }
   
   /// <summary>
   /// 卡片标签页高度
   /// </summary>
   public double CardHeight { get; set; }
   
   /// <summary>
   /// 卡片标签页内间距
   /// </summary>
   public Thickness CardPadding { get; set; }
   
   /// <summary>
   /// 小号卡片标签页内间距
   /// </summary>
   public Thickness CardPaddingSM { get; set; }
   
   /// <summary>
   /// 大号卡片标签页内间距
   /// </summary>
   public Thickness CardPaddingLG { get; set; }
   
   /// <summary>
   /// 标齐页标题文本大小
   /// </summary>
   public double TitleFontSize { get; set; }
   
   /// <summary>
   /// 大号标签页标题文本大小
   /// </summary>
   public double TitleFontSizeLG { get; set; }
   
   /// <summary>
   /// 小号标签页标题文本大小
   /// </summary>
   public double TitleFontSizeSM { get; set; }
   
   /// <summary>
   /// 指示条颜色
   /// </summary>
   public Color InkBarColor { get; set; }
   
   /// <summary>
   /// 横向标签页外间距
   /// </summary>
   public Thickness HorizontalMargin { get; set; }
   
   /// <summary>
   /// 横向标签页标签间距
   /// </summary>
   public double HorizontalItemGutter { get; set; }
   
   /// <summary>
   /// 横向标签页标签外间距
   /// </summary>
   public Thickness HorizontalItemMargin { get; set; }
   
   /// <summary>
   /// 横向标签页标签内间距
   /// </summary>
   public Thickness HorizontalItemPadding { get; set; }
   
   /// <summary>
   /// 大号横向标签页标签内间距
   /// </summary>
   public Thickness HorizontalItemPaddingLG { get; set; }
   
   /// <summary>
   /// 小号横向标签页标签内间距
   /// </summary>
   public Thickness HorizontalItemPaddingSM { get; set; }
   
   /// <summary>
   /// 纵向标签页标签内间距
   /// </summary>
   public Thickness VerticalItemPadding { get; set; }
   
   /// <summary>
   /// 纵向标签页标签外间距
   /// </summary>
   public Thickness VerticalItemMargin { get; set; }
   
   /// <summary>
   /// 标签文本颜色
   /// </summary>
   public Color ItemColor { get; set; }
   
   /// <summary>
   /// 标签悬浮态文本颜色
   /// </summary>
   public Color ItemHoverColor { get; set; }
   
   /// <summary>
   /// 标签选中态文本颜色
   /// </summary>
   public Color ItemSelectedColor { get; set; }
   
   /// <summary>
   /// 卡片标签间距
   /// </summary>
   public double CardGutter { get; set; }

   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      var lineHeight = _globalToken.FontToken.LineHeight;
      var lineWidth = _globalToken.SeedToken.LineWidth;
      var fontToken = _globalToken.FontToken;
      var colorToken = _globalToken.ColorToken;
      
      CardBg = _globalToken.ColorFillAlter;
      CardHeight = _globalToken.HeightToken.ControlHeightLG;
      CardPadding = new Thickness(_globalToken.Padding, (CardHeight - Math.Round(_globalToken.FontToken.FontSize * lineHeight)) / 2 - lineWidth);
      CardPaddingSM = new Thickness(_globalToken.Padding, _globalToken.PaddingXXS * 1.5);
      CardPaddingLG = new Thickness(top:_globalToken.PaddingXS,
                                    bottom:_globalToken.PaddingXXS * 1.5, 
                                    left:_globalToken.Padding, 
                                    right:_globalToken.Padding);
      TitleFontSize = fontToken.FontSize;
      TitleFontSizeLG = fontToken.FontSizeLG;
      TitleFontSizeSM = fontToken.FontSize;
      InkBarColor = colorToken.ColorPrimaryToken.ColorPrimary;
      HorizontalMargin = new Thickness(0, 0, _globalToken.Margin, 0);
      HorizontalItemGutter = 32;
      HorizontalItemMargin = new Thickness();

      HorizontalItemPadding = new Thickness(0, _globalToken.PaddingSM);
      HorizontalItemPaddingSM = new Thickness(0, _globalToken.PaddingXS);
      HorizontalItemPaddingLG = new Thickness(0, _globalToken.Padding);
      VerticalItemPadding = new Thickness(_globalToken.PaddingLG, _globalToken.PaddingXS);
      VerticalItemMargin = new Thickness(0, _globalToken.Margin, 0, 0);
      
      ItemColor = colorToken.ColorNeutralToken.ColorText;
      ItemSelectedColor = colorToken.ColorPrimaryToken.ColorPrimary;
      ItemHoverColor = colorToken.ColorPrimaryToken.ColorPrimaryHover;

      CardGutter = _globalToken.MarginXXS / 2;
   }
}