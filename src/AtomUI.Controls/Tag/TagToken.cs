using AtomUI.ColorSystem;
using AtomUI.TokenSystem;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class TagToken : AbstractControlDesignToken
{
   public const string ID = "Tag";

   /// <summary>
   /// 默认背景色
   /// </summary>
   public Color DefaultBg { get; set; }

   /// <summary>
   /// 默认文字颜色
   /// </summary>
   public Color DefaultColor { get; set; }

   public double TagFontSize { get; set; }
   public double TagLineHeight { get; set; }
   public double TagIconSize { get; set; }
   public double TagCloseIconSize { get; set; }
   public Thickness TagPadding { get; set; }
   public Color TagBorderlessBg { get; set; }

   public TagToken()
      : base(ID)
   {
   }

   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      var fontToken = _globalToken.FontToken;
      var colorNeutralToken = _globalToken.ColorToken.ColorNeutralToken;
      var lineHeightSM = fontToken.LineHeightSM;
      TagFontSize = fontToken.FontSizeSM;
      TagLineHeight = lineHeightSM * TagFontSize;
      TagCloseIconSize = _globalToken.IconSizeXS;
      TagIconSize = _globalToken.FontSizeIcon;
      TagPadding = new Thickness(8, 0); // Fixed padding.
      // TODO 这个地方需要看看
      DefaultBg = ColorUtils.OnBackground(colorNeutralToken.ColorFillQuaternary, colorNeutralToken.ColorBgContainer);
      TagBorderlessBg = DefaultBg;
      DefaultColor = colorNeutralToken.ColorText;
   }
}