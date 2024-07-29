using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class SegmentedToken : AbstractControlDesignToken
{
   public const string ID = "Segmented";

   public SegmentedToken()
      : base(ID) { }

   /// <summary>
   /// 选项文本颜色
   /// </summary>
   public Color ItemColor { get; set; }

   /// <summary>
   /// 选项悬浮态文本颜色
   /// </summary>
   public Color ItemHoverColor { get; set; }

   /// <summary>
   /// 选项悬浮态背景颜色
   /// </summary>
   public Color ItemHoverBg { get; set; }

   /// <summary>
   /// 选项激活态背景颜色
   /// </summary>
   public Color ItemActiveBg { get; set; }

   /// <summary>
   /// 选项选中时背景颜色
   /// </summary>
   public Color ItemSelectedBg { get; set; }

   /// <summary>
   /// 选项选中时文字颜色
   /// </summary>
   public Color ItemSelectedColor { get; set; }

   /// <summary>
   /// Segmented 控件容器的 padding
   /// </summary>
   public Thickness TrackPadding { get; set; }

   /// <summary>
   /// Segmented 控件容器背景色
   /// </summary>
   public Color TrackBg { get; set; }

   // 内部 token
   public Thickness SegmentedItemPadding { get; set; }
   public Thickness SegmentedItemPaddingSM { get; set; }
   public Thickness SegmentedTextLabelMargin { get; set; }

   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      var colorNeutralToken = _globalToken.ColorToken.ColorNeutralToken;
      TrackPadding = new Thickness(_globalToken.StyleToken.LineWidthBold);
      TrackBg = colorNeutralToken.ColorBgLayout;
      ItemColor = _globalToken.ColorTextLabel;
      ItemHoverColor = colorNeutralToken.ColorText;
      ItemHoverBg = colorNeutralToken.ColorFillSecondary;
      ItemSelectedBg = colorNeutralToken.ColorBgElevated;
      ItemActiveBg = colorNeutralToken.ColorFill;
      ItemSelectedColor = colorNeutralToken.ColorText;
      var lineWidth = _globalToken.SeedToken.LineWidth;
      SegmentedItemPadding = new Thickness(
         Math.Max(_globalToken.ControlPadding - lineWidth, 0),
         0,
         Math.Max(_globalToken.ControlPadding - lineWidth, 0),
         0);
      SegmentedItemPaddingSM = new Thickness(
         Math.Max(_globalToken.ControlPaddingSM - lineWidth, 0),
         0,
         Math.Max(_globalToken.ControlPaddingSM - lineWidth, 0),
         0);
      SegmentedTextLabelMargin = new Thickness(_globalToken.PaddingXXS, 0, 0, 0);
   }
}