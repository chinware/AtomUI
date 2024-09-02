using AtomUI.Theme.Styling;
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
   public double ItemPaddingBottom { get; set; }
   
   /// <summary>
   /// content left margin
   /// </summary>
   public Thickness ContentMargin { get; set; }

   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      

      TailColor = _globalToken.ColorSplit;
      TailWidth = _globalToken.StyleToken.LineWidthBold;
      DotBorderWidth = _globalToken.SeedToken.Wireframe
         ? _globalToken.StyleToken.LineWidthBold
         : _globalToken.SeedToken.LineWidth * 3;
      DotBg = _globalToken.ColorToken.ColorNeutralToken.ColorBgContainer;
      ItemPaddingBottom = _globalToken.Padding * 1.25;
      
      ContentMargin = new Thickness(_globalToken.Margin, 0, 0, 0);
   }
}