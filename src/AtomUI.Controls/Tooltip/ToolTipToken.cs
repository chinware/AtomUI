using AtomUI.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Controls.Tooltip;

[ControlDesignToken]
internal class ToolTipToken : AbstractControlDesignToken
{
   public const string ID = "Tooltip";
   
   public ToolTipToken()
      : base(ID)
   {
   }

   /// <summary>
   /// tooltip 的最大宽度，超过了就换行
   /// </summary>
   public double TooltipMaxWidth { get; set; }
   
   /// <summary>
   /// Tooltip 默认的前景色 
   /// </summary>
   public Color TooltipColor { get; set; }
   
   /// <summary>
   /// Tooltip 默认的背景色
   /// </summary>
   public Color TooltipBackground { get; set; }
}