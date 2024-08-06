using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class LineEditToken : AbstractControlDesignToken
{
   public const string ID = "LineEdit";
   
   public LineEditToken()
      : base(ID)
   {
   }
   
   /// <summary>
   /// 输入框内边距
   /// </summary>
   public Thickness Padding { get; set; }
   
   /// <summary>
   /// 小号输入框内边距
   /// </summary>
   public Thickness PaddingSM { get; set; }
   
   /// <summary>
   /// 大号输入框内边距
   /// </summary>
   public Thickness PaddingLG { get; set; }
   
   /// <summary>
   /// 前/后置标签背景色
   /// </summary>
   public Color AddonBg { get; set; }
   
   /// <summary>
   /// 悬浮态边框色
   /// </summary>
   public Color HoverBorderColor { get; set; }
   
   /// <summary>
   /// 激活态边框色
   /// </summary>
   public Color ActiveBorderColor { get; set; }
   
   /// <summary>
   /// 激活态阴影
   /// </summary>
   public BoxShadow ActiveShadow { get; set; }
   
   /// <summary>
   /// 错误状态时激活态阴影
   /// </summary>
   public BoxShadow ErrorActiveShadow { get; set; }
   
   /// <summary>
   /// 警告状态时激活态阴影
   /// </summary>
   public BoxShadow WarningActiveShadow { get; set; }
   
   /// <summary>
   /// 输入框 hover 状态时背景颜色
   /// </summary>
   public Color HoverBg { get; set; }
   
   /// <summary>
   /// 输入框激活状态时背景颜色
   /// </summary>
   public Color ActiveBg { get; set; }
   
   /// <summary>
   /// 字体大小
   /// </summary>
   public double InputFontSize { get; set; }
   
   /// <summary>
   /// 大号字体大小
   /// </summary>
   public double InputFontSizeLG { get; set; }
   
   /// <summary>
   /// 小号字体大小
   /// </summary>
   public double InputFontSizeSM { get; set; }

   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      var fontSize = _globalToken.FontToken.FontSize;
      var lineHeight = _globalToken.FontToken.LineHeight;
      var lineHeightLG = _globalToken.FontToken.LineHeightLG;
      var lineWidth = _globalToken.SeedToken.LineWidth;
      Padding = new Thickness(_globalToken.PaddingSM - lineWidth, 
                              Math.Round(((_globalToken.SeedToken.ControlHeight - fontSize * lineHeight) / 2) * 10) / 10 - lineWidth);
      PaddingSM = new Thickness(_globalToken.ControlPaddingSM - lineWidth, 
                              Math.Round(((_globalToken.HeightToken.ControlHeightSM - fontSize * lineHeight) / 2) * 10) / 10 - lineWidth);
      PaddingLG = new Thickness(_globalToken.ControlPadding - lineWidth, 
                                Math.Ceiling(((_globalToken.HeightToken.ControlHeightLG - fontSize * lineHeightLG) / 2) * 10) / 10 - lineWidth);

      AddonBg = _globalToken.ColorFillAlter;
      ActiveBorderColor = _globalToken.ColorToken.ColorPrimaryToken.ColorPrimary;
      HoverBorderColor = _globalToken.ColorToken.ColorPrimaryToken.ColorPrimaryHover;
      ActiveShadow = new BoxShadow()
      {
         Spread = _globalToken.ControlOutlineWidth,
         Color = _globalToken.ColorControlOutline
      };
      ErrorActiveShadow = new BoxShadow()
      {
         Spread = _globalToken.ControlOutlineWidth,
         Color = _globalToken.ColorErrorOutline
      };
      WarningActiveShadow = new BoxShadow()
      {
         Spread = _globalToken.ControlOutlineWidth,
         Color = _globalToken.ColorWarningOutline
      };
      HoverBg = _globalToken.ColorToken.ColorNeutralToken.ColorBgContainer;
      ActiveBg = _globalToken.ColorToken.ColorNeutralToken.ColorBgContainer;
      InputFontSize = _globalToken.FontToken.FontSize;
      InputFontSizeLG = _globalToken.FontToken.FontSizeLG;
      InputFontSizeSM = _globalToken.FontToken.FontSizeSM;
   }
}