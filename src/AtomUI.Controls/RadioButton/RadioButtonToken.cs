using AtomUI.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class RadioButtonToken : AbstractControlDesignToken
{
   public const string ID = "RadioButton";

   /// <summary>
   /// 单选框大小，除去文字部分的
   /// </summary>
   public double RadioSize { get; set; }

   /// <summary>
   /// 单选框圆点大小
   /// </summary>
   public double DotSize { get; set; }

   /// <summary>
   /// 单选框圆点禁用颜色
   /// </summary>
   public Color DotColorDisabled { get; set; }
   
   /// 内部使用
   public Color RadioColor { get; set; }
   public Color RadioBgColor { get; set; }
   public double DotPadding { get; set; }

   public RadioButtonToken()
      : base(ID)
   {
   }

   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      var colorNeutralToken = _globalToken.ColorToken.ColorNeutralToken;
      var colorPrimaryToken = _globalToken.ColorToken.ColorPrimaryToken;
      var seedToken = _globalToken.SeedToken;
      bool wireFrame = seedToken.Wireframe;
      int lineWidth = seedToken.LineWidth;
      double fontSizeLG = _globalToken.FontToken.FontSizeLG;
      Color colorBgContainer = colorNeutralToken.ColorBgContainer;
      Color colorPrimary = colorPrimaryToken.ColorPrimary;
      Color colorWhite = _globalToken.ColorToken.ColorWhite;

      int dotPadding = 4; // 魔术值，需要看有没有好办法消除
      double radioSize = fontSizeLG;

      double radioDotSize = wireFrame
         ? radioSize - dotPadding * 2
         : radioSize - (dotPadding + lineWidth) * 2;
      
      DotPadding = dotPadding;
      RadioSize = radioSize;
      DotSize = radioDotSize;
      DotColorDisabled = _globalToken.ColorTextDisabled;
      
      // internal
      RadioColor = wireFrame ? colorPrimary : colorWhite;
      RadioBgColor = wireFrame ? colorBgContainer : colorPrimary;
   }
}