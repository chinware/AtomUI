using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class TimelineToken : AbstractControlDesignToken
{
   public const string ID = "Timeline";

   public TimelineToken()
      : this(ID) { }

   protected TimelineToken(string id)
      : base(id) { }

   /// <summary>
   /// Timeline 轨迹颜色
   /// </summary>
   public Color TailColor { get; set; }

   /// <summary>
   /// 轨迹宽度
   /// </summary>
   public double TailWidth { get; set; }

   /// <summary>
   /// 节点边框宽度
   /// </summary>
   public double DotBorderWidth { get; set; }

   /// <summary>
   /// 列表项文字选中颜色
   /// </summary>
   public Color ItemSelectedColor { get; set; }

   /// <summary>
   /// 节点背景色
   /// </summary>
   public Color DotBg { get; set; }

   /// <summary>
   /// 列表项背景色
   /// </summary>
   public Color ItemBgColor { get; set; }

   /// <summary>
   /// 时间项下间距
   /// </summary>
   public Thickness ItemPaddingBottom { get; set; }
   
   /// <summary>
   /// right margin
   /// </summary>
   public Thickness RightMargin { get; set; }
   
   /// <summary>
   /// left margin
   /// </summary>
   public Thickness LeftMargin { get; set; }
   
   /// <summary>
   /// 最后一个Item的Content最小高度
   /// </summary>
   public double LastItemContentMinHeight { get; set; }
   
   /// <summary>
   /// 字体大小
   /// </summary>
   public double FontSize { get; set; }
   
   /// <summary>
   /// item head size
   /// </summary>
   public double ItemHeadSize { get; set; }
   
   /// <summary>
   /// custom head size
   /// </summary>
   public double CustomHeadSize { get; set; }
   
   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      

      TailColor = _globalToken.ColorSplit;
      TailWidth = _globalToken.LineWidthBold;
      DotBorderWidth = _globalToken.Wireframe
         ? _globalToken.LineWidthBold
         : _globalToken.LineWidth * 3;
      DotBg = _globalToken.ColorBgContainer;
      ItemPaddingBottom = new Thickness(0, 0, 0, _globalToken.Padding * 1.25);
      
      LeftMargin = new Thickness(_globalToken.Margin, 0, 0, 0);
      RightMargin = new Thickness(0, 0, _globalToken.MarginSM, 0);

      LastItemContentMinHeight = _globalToken.ControlHeightLG * 1.2;
      FontSize                 = _globalToken.FontSize;
      ItemHeadSize             = 10;
      CustomHeadSize           = _globalToken.FontSize;

   }
}