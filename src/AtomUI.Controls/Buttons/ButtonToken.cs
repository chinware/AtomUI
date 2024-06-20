using AtomUI.Styling;
using AtomUI.TokenSystem;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Media;
using Math = System.Math;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class ButtonToken : AbstractControlDesignToken
{
   public const string ID = "Button";

   /// <summary>
   /// 文字字重
   /// </summary>
   public int FontWeight { get; set; }

   /// <summary>
   /// 默认按钮阴影
   /// </summary>
   public BoxShadow DefaultShadow { get; set; }

   /// <summary>
   /// 主要按钮阴影
   /// </summary>
   public BoxShadow PrimaryShadow { get; set; }

   /// <summary>
   /// 危险按钮阴影
   /// </summary>
   public BoxShadow DangerShadow { get; set; }

   /// <summary>
   /// 主要按钮文本颜色
   /// </summary>
   public Color PrimaryColor { get; set; }

   /// <summary>
   /// 默认按钮文本颜色
   /// </summary>
   public Color DefaultColor { get; set; }

   /// <summary>
   /// 默认按钮背景色
   /// </summary>
   public Color DefaultBg { get; set; }

   /// <summary>
   /// 默认按钮边框颜色
   /// </summary>
   public Color DefaultBorderColor { get; set; }
   
   /// <summary>
   /// 默认的禁用边框颜色
   /// </summary>
   public Color DefaultBorderColorDisabled { get; set; }

   /// <summary>
   /// 危险按钮文本颜色
   /// </summary>
   public Color DangerColor { get; set; }

   /// <summary>
   /// 默认按钮悬浮态背景色
   /// </summary>
   public Color DefaultHoverBg { get; set; }

   /// <summary>
   /// 默认按钮悬浮态文本颜色
   /// </summary>
   public Color DefaultHoverColor { get; set; }

   /// <summary>
   /// 默认按钮悬浮态边框颜色
   /// </summary>
   public Color DefaultHoverBorderColor { get; set; }

   /// <summary>
   /// 默认按钮激活态背景色
   /// </summary>
   public Color DefaultActiveBg { get; set; }

   /// <summary>
   /// 默认按钮激活态文字颜色
   /// </summary>
   public Color DefaultActiveColor { get; set; }

   /// <summary>
   /// 默认按钮激活态边框颜色
   /// </summary>
   public Color DefaultActiveBorderColor { get; set; }

   /// <summary>
   /// 禁用状态边框颜色
   /// </summary>
   public Color BorderColorDisabled { get; set; }

   /// <summary>
   /// 默认幽灵按钮文本颜色
   /// </summary>
   public Color DefaultGhostColor { get; set; }

   /// <summary>
   /// 幽灵按钮背景色
   /// </summary>
   public Color GhostBg { get; set; }

   /// <summary>
   /// 默认幽灵按钮边框颜色
   /// </summary>
   public Color DefaultGhostBorderColor { get; set; }

   /// <summary>
   /// 按钮内间距
   /// </summary>
   public Thickness Padding { get; set; }

   /// <summary>
   /// 大号按钮内间距
   /// </summary>
   public Thickness PaddingLG { get; set; }

   /// <summary>
   /// 小号按钮内间距
   /// </summary>
   public Thickness PaddingSM { get; set; }
   
   /// <summary>
   /// 圆形按钮内间距
   /// </summary>
   public Thickness CirclePadding { get; set; }

   /// <summary>
   /// 只有图标的按钮图标尺寸
   /// </summary>
   public double OnlyIconSize { get; set; }

   /// <summary>
   /// 大号只有图标的按钮图标尺寸
   /// </summary>
   public double OnlyIconSizeLG { get; set; }

   /// <summary>
   /// 小号只有图标的按钮图标尺寸
   /// </summary>
   public double OnlyIconSizeSM { get; set; }
   
   /// <summary>
   /// 图标的按钮图标尺寸
   /// </summary>
   public double IconSize { get; set; }

   /// <summary>
   /// 只有图标的按钮图标尺寸
   /// </summary>
   public double IconSizeLG { get; set; }

   /// <summary>
   /// 只有图标的按钮图标尺寸
   /// </summary>
   public double IconSizeSM { get; set; }

   /// <summary>
   /// 按钮组边框颜色
   /// </summary>
   public Color GroupBorderColor { get; set; }

   /// <summary>
   /// 链接按钮悬浮态背景色
   /// </summary>
   public Color LinkHoverBg { get; set; }

   /// <summary>
   /// 文本按钮悬浮态背景色
   /// </summary>
   public Color TextHoverBg { get; set; }

   /// <summary>
   /// 按钮内容字体大小
   /// </summary>
   public double ContentFontSize { get; set; } = -1;

   /// <summary>
   /// 大号按钮内容字体大小
   /// </summary>
   public double ContentFontSizeLG { get; set; } = -1;

   /// <summary>
   /// 小号按钮内容字体大小
   /// </summary>
   public double ContentFontSizeSM { get; set; } = -1;

   /// <summary>
   /// 按钮内容字体行高
   /// </summary>
   public double ContentLineHeight { get; set; } = -1;

   /// <summary>
   /// 大号按钮内容字体行高
   /// </summary>
   public double ContentLineHeightLG { get; set; } = -1;

   /// <summary>
   /// 小号按钮内容字体行高
   /// </summary>
   public double ContentLineHeightSM { get; set; } = -1;

   public ButtonToken()
      : base(ID)
   {
   }

   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      var fontSize = _globalToken.FontToken.FontSize;
      var fontSizeLG = _globalToken.FontToken.FontSizeLG;
      
      ContentFontSize = !NumberUtils.FuzzyEqual(ContentFontSize, -1) ? ContentFontSize : fontSize;
      ContentFontSizeSM = !NumberUtils.FuzzyEqual(ContentFontSizeSM, -1) ? ContentFontSizeSM : fontSize;
      ContentFontSizeLG = !NumberUtils.FuzzyEqual(ContentFontSizeLG, -1) ? ContentFontSizeLG : fontSizeLG;
      ContentLineHeight = !NumberUtils.FuzzyEqual(ContentLineHeight, -1)
         ? ContentLineHeight
         : CalculatorUtils.CalculateLineHeight(ContentFontSize);
      ContentLineHeightSM =  !NumberUtils.FuzzyEqual(ContentLineHeightSM, -1)
         ? ContentLineHeightSM
         : CalculatorUtils.CalculateLineHeight(ContentFontSizeSM);
      ContentLineHeightLG = !NumberUtils.FuzzyEqual(ContentLineHeightLG, -1)
         ? ContentLineHeightLG
         : CalculatorUtils.CalculateLineHeight(ContentFontSizeLG);

      var controlOutlineWidth = _globalToken.ControlOutlineWidth;
      FontWeight = 400;
      DefaultShadow = new BoxShadow
      {
         OffsetX = 0,
         OffsetY = controlOutlineWidth,
         Blur = 3,
         Spread = 0,
         Color = _globalToken.ColorControlOutline
      };

      PrimaryShadow = new BoxShadow
      {
         OffsetX = 0,
         OffsetY = controlOutlineWidth,
         Blur = 3,
         Spread = 0,
         Color = _globalToken.ColorControlOutline
      };

      DangerShadow = new BoxShadow
      {
         OffsetX = 0,
         OffsetY = controlOutlineWidth,
         Blur = 3,
         Spread = 0,
         Color = _globalToken.ColorErrorOutline
      };
      
      var colorToken = _globalToken.ColorToken;
      var neutralColorToken = colorToken.ColorNeutralToken;
      var primaryColorToken = colorToken.ColorPrimaryToken;
      var lineWidth = _globalToken.SeedToken.LineWidth;

      PrimaryColor = _globalToken.ColorTextLightSolid;
      DangerColor = _globalToken.ColorTextLightSolid;
      BorderColorDisabled = neutralColorToken.ColorBorder;
      DefaultGhostColor = neutralColorToken.ColorBgContainer;
      GhostBg = Colors.Transparent;
      DefaultGhostBorderColor = neutralColorToken.ColorBgContainer;
      
      GroupBorderColor = primaryColorToken.ColorPrimaryHover;
      LinkHoverBg = Colors.Transparent;
      TextHoverBg = _globalToken.ColorBgTextHover;
      DefaultColor = neutralColorToken.ColorText;
      DefaultBg = neutralColorToken.ColorBgContainer;
      DefaultBorderColor = neutralColorToken.ColorBorder;
      DefaultBorderColorDisabled = neutralColorToken.ColorBorder;
      DefaultHoverBg = neutralColorToken.ColorBgContainer;
      DefaultHoverColor = primaryColorToken.ColorPrimaryHover;
      DefaultHoverBorderColor = primaryColorToken.ColorPrimaryHover;
      DefaultActiveBg = neutralColorToken.ColorBgContainer;
      DefaultActiveColor = primaryColorToken.ColorPrimaryActive;
      DefaultActiveBorderColor = primaryColorToken.ColorPrimaryActive;
      
      var controlHeight = _globalToken.SeedToken.ControlHeight;
      var controlHeightSM = base._globalToken.HeightToken.ControlHeightSM;
      var controlHeightLG = _globalToken.HeightToken.ControlHeightLG;
      
      Padding = new Thickness(_globalToken.PaddingContentHorizontal - lineWidth, 
         Math.Max((controlHeight - ContentFontSize * ContentLineHeight) / 2 - lineWidth, 0));
      PaddingLG = new Thickness(_globalToken.PaddingContentHorizontal - lineWidth, 
         Math.Max((controlHeightSM - ContentFontSizeSM * ContentLineHeightSM) / 2 - lineWidth, 0));
      PaddingSM = new Thickness(8 - _globalToken.SeedToken.LineWidth, 
         Math.Max((controlHeightLG - controlHeightLG * controlHeightLG) / 2 - lineWidth, 0));
      CirclePadding = new Thickness(PaddingSM.Left / 2);
      OnlyIconSizeSM = _globalToken.IconSize;
      OnlyIconSize = _globalToken.IconSizeLG;
      OnlyIconSizeLG = _globalToken.IconSizeLG;

      IconSizeSM = _globalToken.IconSizeSM;
      IconSize = _globalToken.IconSize;
      IconSizeLG = _globalToken.IconSize;
   }
}