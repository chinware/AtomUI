using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class MenuToken : AbstractControlDesignToken
{
   public const string ID = "Menu";
   
   public MenuToken()
      : base(ID)
   {
   }
   
   /// <summary>
   /// 菜单的圆角
   /// </summary>
   public CornerRadius MenuBorderRadius { get; set; }
   
   /// <summary>
   /// 子菜单指示三角形的大小
   /// </summary>
   public double MenuArrowSize { get; set; }
   
   /// <summary>
   /// 子菜单指示三角形的位移
   /// </summary>
   public double MenuArrowOffset { get; set; }
   
   /// <summary>
   /// 菜单面板的宽度，一般用于画边框
   /// </summary>
   public double MenuPanelWidth { get; set; }
   
   /// <summary>
   /// 菜单间距
   /// </summary>
   public Thickness MenuMargin { get; set; }
   
   /// <summary>
   /// 分离菜单项的高度，这个用于菜单中快捷功能的图标显示
   /// TODO 暂时还没实现，但是最终会实现
   /// </summary>
   public double MenuTearOffHeight { get; set; }
   
   /// <summary>
   /// 菜单内容边距
   /// </summary>
   public double MenuContentPadding { get; set; }
   
   /// <summary>
   /// 弹出框背景色
   /// </summary>
   public Color MenuBgColor { get; set; }
   
   /// <summary>
   /// 菜单项文字颜色
   /// </summary>
   public Color ItemColor { get; set; }
   
   /// <summary>
   /// 菜单项文字悬浮颜色
   /// </summary>
   public Color ItemHoverColor { get; set; }
   
   /// <summary>
   /// 菜单项文字禁用颜色
   /// </summary>
   public Color ItemDisabledColor { get; set; }
   
   /// <summary>
   /// 危险菜单项文字颜色
   /// </summary>
   public Color DangerItemColor { get; set; }
   
   /// <summary>
   /// 危险菜单项文字悬浮颜色
   /// </summary>
   public Color DangerItemHoverColor { get; set; }
   
   /// <summary>
   /// 菜单项背景色
   /// </summary>
   public Color ItemBg { get; set; }
   
   /// <summary>
   /// 菜单项悬浮态背景色
   /// </summary>
   public Color ItemHoverBg { get; set; }
   
   /// <summary>
   /// 菜单项高度
   /// </summary>
   public double ItemHeight { get; set; }
   
   /// <summary>
   /// 图标尺寸
   /// </summary>
   public double ItemIconSize { get; set; }
   
   /// <summary>
   /// 图标与文字间距
   /// </summary>
   public double ItemIconMarginInlineEnd { get; set; }
   
   /// <summary>
   /// 菜单项的圆角
   /// </summary>
   public CornerRadius ItemBorderRadius { get; set; }
   
   /// <summary>
   /// 菜单项横向内间距
   /// </summary>
   public Thickness ItemPaddingInline { get; set; }
   
   /// <summary>
   /// 菜单阴影
   /// </summary>
   public BoxShadows MenuBoxShadows { get; set; }
   
   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      var colorErrorToken = _globalToken.ColorToken.ColorErrorToken;
      var colorNeutralToken = _globalToken.ColorToken.ColorNeutralToken;
      
      var colorTextDisabled = _globalToken.ColorTextDisabled;
      var colorError = colorErrorToken.ColorError;
      var colorTextSecondary = colorNeutralToken.ColorTextQuaternary;
      var colorBgContainer = colorNeutralToken.ColorBgContainer;
      var colorBgTextHover = _globalToken.ColorBgTextHover;
      var colorBgElevated = colorNeutralToken.ColorBgElevated;
      var padding = _globalToken.Padding;
      var fontSize = _globalToken.FontToken.FontSize;
      var controlHeightSM = _globalToken.HeightToken.ControlHeightSM;

      MenuBorderRadius = _globalToken.StyleToken.BorderRadiusLG;
      MenuPanelWidth = _globalToken.SeedToken.LineWidth;
      MenuBorderRadius = _globalToken.SeedToken.BorderRadius;
      ItemColor = colorTextSecondary;
      ItemHoverColor = colorTextSecondary;
      ItemBg = colorBgContainer;
      ItemHoverBg = colorBgTextHover;

      // Disabled
      ItemDisabledColor = colorTextDisabled;
      
      // Danger
      DangerItemColor = colorError;
      DangerItemHoverColor = colorError;
      
      ItemHeight = controlHeightSM;
      MenuBgColor = colorBgElevated;
      
      ItemPaddingInline = new Thickness(padding);
      ItemIconSize = fontSize;
      ItemIconMarginInlineEnd = controlHeightSM - fontSize;
      MenuArrowSize = (fontSize / 7.0) * 5.0;
      MenuArrowOffset = MenuArrowSize * 0.5;
      MenuTearOffHeight = ItemHeight * 1.2; // 暂时这么定义吧
      MenuContentPadding = _globalToken.PaddingXXS / 2; // 先默认一个最小的内容间距
      MenuMargin = new Thickness(1);

      MenuBoxShadows = _globalToken.BoxShadowsSecondary;
   }
}