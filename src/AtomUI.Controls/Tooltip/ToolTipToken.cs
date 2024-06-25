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
   public double ToolTipMaxWidth { get; set; }
   
   /// <summary>
   /// ToolTip 默认的前景色 
   /// </summary>
   public Color ToolTipColor { get; set; }
   
   /// <summary>
   /// ToolTip 默认的背景色
   /// </summary>
   public Color ToolTipBackground { get; set; }
   
   /// <summary>
   /// ToolTip 默认的圆角
   /// </summary>
   public CornerRadius BorderRadiusOuter { get; set; }
   
   /// <summary>
   /// ToolTip 默认的内间距
   /// </summary>
   public Thickness ToolTipPadding { get; set; }
   
   /// <summary>
   /// ToolTip 箭头三角形大小
   /// </summary>
   public double ToolTipArrowSize { get; set; }

   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();

      ToolTipMaxWidth = 250;
      ToolTipColor = _globalToken.ColorTextLightSolid;
      ToolTipBackground = _globalToken.ColorToken.ColorNeutralToken.ColorBgSpotlight;
      BorderRadiusOuter = new CornerRadius(Math.Max(BorderRadiusOuter.TopLeft, 4),
                                           Math.Max(BorderRadiusOuter.TopRight, 4),
                                           Math.Max(BorderRadiusOuter.BottomLeft, 4),
                                           Math.Max(BorderRadiusOuter.BottomRight, 4));
      ToolTipArrowSize = _globalToken.SeedToken.SizePopupArrow / 1.3;
      ToolTipPadding = new Thickness(_globalToken.PaddingSM, _globalToken.PaddingSM / 2);
   }
}