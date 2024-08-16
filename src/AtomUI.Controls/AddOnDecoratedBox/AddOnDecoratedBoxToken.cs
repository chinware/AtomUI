using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class AddOnDecoratedBoxToken : AbstractControlDesignToken
{
   public const string ID = "AddOnDecoratedBox";
   
   public AddOnDecoratedBoxToken()
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
   /// hover 状态时背景颜色
   /// </summary>
   public Color HoverBg { get; set; }
   
   /// <summary>
   /// 激活状态时背景颜色
   /// </summary>
   public Color ActiveBg { get; set; }
   
   /// <summary>
   /// 字体大小
   /// </summary>
   public double FontSize { get; set; }
   
   /// <summary>
   /// 大号字体大小
   /// </summary>
   public double FontSizeLG { get; set; }
   
   /// <summary>
   /// 小号字体大小
   /// </summary>
   public double FontSizeSM { get; set; }
   
   /// <summary>
   /// AddOn 内边距
   /// </summary>
   public Thickness AddOnPadding { get; set; }
   
   /// <summary>
   /// AddOn 小号内边距
   /// </summary>
   public Thickness AddOnPaddingSM { get; set; }
   
   /// <summary>
   /// AddOn 大号内边距
   /// </summary>
   public Thickness AddOnPaddingLG { get; set; }
   
   /// <summary>
   /// 左边内部小组件的边距
   /// </summary>
   public Thickness LeftInnerAddOnMargin { get; set; }
   
   /// <summary>
   /// 右边内部小组件的边距
   /// </summary>
   public Thickness RightInnerAddOnMargin { get; set; }
   
      internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      var fontSize = _globalToken.FontToken.FontSize;
      var fontSizeLG = _globalToken.FontToken.FontSizeLG;
      var lineHeight = _globalToken.FontToken.LineHeight;
      var lineHeightLG = _globalToken.FontToken.LineHeightLG;
      var lineWidth = _globalToken.SeedToken.LineWidth;
      Padding = new Thickness(_globalToken.PaddingSM - lineWidth, 
                              Math.Round(((_globalToken.SeedToken.ControlHeight - fontSize * lineHeight) / 2) * 10) / 10 - lineWidth);
      PaddingSM = new Thickness(_globalToken.ControlPaddingSM - lineWidth, 
                              Math.Round(((_globalToken.HeightToken.ControlHeightSM - fontSize * lineHeight) / 2) * 10) / 10 - lineWidth);
      PaddingLG = new Thickness(_globalToken.ControlPadding - lineWidth, 
                                Math.Ceiling(((_globalToken.HeightToken.ControlHeightLG - fontSizeLG * lineHeightLG) / 2) * 10) / 10 - lineWidth);
      AddOnPadding = new Thickness(_globalToken.PaddingSM, 0);
      AddOnPaddingSM = new Thickness(_globalToken.ControlPaddingSM, 0);
      AddOnPaddingLG = new Thickness(_globalToken.ControlPadding, 0);

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
      FontSize = _globalToken.FontToken.FontSize;
      FontSizeLG = _globalToken.FontToken.FontSizeLG;
      FontSizeSM = _globalToken.FontToken.FontSizeSM;

      LeftInnerAddOnMargin = new Thickness(0, 0, _globalToken.MarginXXS, 0);
      RightInnerAddOnMargin = new Thickness(_globalToken.MarginXXS, 0, 0, 0);
   }
}