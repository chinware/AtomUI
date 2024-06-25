using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class ToolTipToken : AbstractControlDesignToken
{
   public const string ID = "ToolTip";
   
   public ToolTipToken()
      : base(ID)
   {
   }

   /// <summary>
   /// tooltip 的最大宽度，超过了就换行
   /// </summary>
   public double TooltipMaxWidth { get; set; }
   
   /// <summary>
   /// ToolTip 默认的前景色 
   /// </summary>
   public Color TooltipColor { get; set; }
   
   /// <summary>
   /// ToolTip 默认的背景色
   /// </summary>
   public Color TooltipBackground { get; set; }
   
   /// <summary>
   /// ToolTip 默认的圆角
   /// </summary>
   public CornerRadius BorderRadiusOuter { get; set; }
   
   /// <summary>
   /// ToolTip 默认的内间距
   /// </summary>
   public Thickness ToolTipPadding { get; set; }

   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();

      TooltipMaxWidth = 250;
      TooltipColor = _globalToken.ColorTextLightSolid;
      TooltipBackground = _globalToken.ColorToken.ColorNeutralToken.ColorBgSpotlight;
      BorderRadiusOuter = new CornerRadius(Math.Max(BorderRadiusOuter.TopLeft, 4),
                                           Math.Max(BorderRadiusOuter.TopRight, 4),
                                           Math.Max(BorderRadiusOuter.BottomLeft, 4),
                                           Math.Max(BorderRadiusOuter.BottomRight, 4));
      ToolTipPadding = new Thickness(_globalToken.PaddingSM, _globalToken.PaddingSM / 2);
   }
}