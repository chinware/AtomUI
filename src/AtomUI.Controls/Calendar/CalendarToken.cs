using AtomUI.Media;
using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class CalendarToken : AbstractControlDesignToken
{
   public const string ID = "Calendar";
   
   public CalendarToken()
      : base(ID)
   {
   }
   
   /// <summary>
   /// 单元格悬浮态背景色
   /// </summary>
   public Color CellHoverBg { get; set; }
   
   /// <summary>
   /// 选取范围内的单元格背景色
   /// </summary>
   public Color CellActiveWithRangeBg { get; set; }

   /// <summary>
   /// 选取范围内的单元格悬浮态背景色
   /// </summary>
   public Color CellHoverWithRangeBg { get; set; }
   
   /// <summary>
   /// 单元格禁用态背景色
   /// </summary>
   public Color CellBgDisabled { get; set; }
   
   /// <summary>
   /// 选取范围时单元格边框色
   /// </summary>
   public Color CellRangeBorderColor { get; set; }
   
   /// <summary>
   /// 单元格高度
   /// </summary>
   public double CellHeight { get; set; }
   
   /// <summary>
   /// 单元格宽度
   /// </summary>
   public double CellWidth { get; set; }
   
   /// <summary>
   /// 单元格文本高度
   /// </summary>
   public double TextHeight { get; set; }
   
   /// <summary>
   /// 十年/年/季/月/周单元格高度
   /// </summary>
   public double WithoutTimeCellHeight { get; set; }
   
   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();

      var colorPrimary = _globalToken.ColorToken.ColorPrimaryToken.ColorPrimary;
      
      CellHoverBg = _globalToken.ControlItemBgHover;
      CellActiveWithRangeBg = _globalToken.ControlItemBgActive;
      CellHoverWithRangeBg = colorPrimary.Lighten(35);
      CellRangeBorderColor = colorPrimary.Lighten(20);
      CellBgDisabled = _globalToken.ColorBgContainerDisabled;
      CellWidth = _globalToken.HeightToken.ControlHeightSM * 1.5;
      CellHeight = _globalToken.HeightToken.ControlHeightSM;
      TextHeight = _globalToken.HeightToken.ControlHeightLG;
      WithoutTimeCellHeight = _globalToken.HeightToken.ControlHeightLG * 1.65;
   }
}