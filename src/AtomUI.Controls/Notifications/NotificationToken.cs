using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class NotificationToken : AbstractControlDesignToken
{
   public const string ID = "Notification";
   
   public NotificationToken()
      : base(ID)
   {
   }

   /// <summary>
   /// 动画最大高度
   /// </summary>
   public double AnimationMaxHeight { get; set; }
   
   /// <summary>
   /// 提醒框背景色
   /// </summary>
   public Color NotificationBg { get; set; }
   
   /// <summary>
   /// 提醒框内边距
   /// </summary>
   public Thickness NotificationPadding { get; set; }
   
   /// <summary>
   /// 提醒框图标尺寸
   /// </summary>
   public double NotificationIconSize { get; set; }
   
   /// <summary>
   /// 提醒框图标外边距
   /// </summary>
   public Thickness NotificationIconMargin { get; set; }
   
   /// <summary>
   /// 提醒框关闭按钮尺寸
   /// </summary>
   public double NotificationCloseButtonSize { get; set; }
   
   /// <summary>
   /// 提醒框底部外边距
   /// </summary>
   public Thickness NotificationMarginBottom { get; set; }
   
   /// <summary>
   /// 提醒框边缘外边距
   /// </summary>
   public Thickness NotificationMarginEdge { get; set; }
   
   /// <summary>
   /// 提醒框进度条背景色
   /// </summary>
   public IBrush? NotificationProgressBg { get; set; }
   
   /// <summary>
   /// 提醒框进度条高度
   /// </summary>
   public double NotificationProgressHeight { get; set; }
   
   /// <summary>
   /// 进度条外边距
   /// </summary>
   public Thickness NotificationProgressMargin { get; set; }
   
   /// <summary>
   /// 提醒框宽度
   /// </summary>
   public double NotificationWidth { get; set; }
   
   /// <summary>
   /// 内容外边距
   /// </summary>
   public Thickness NotificationContentMargin { get; set; }
   
   /// <summary>
   /// 标题栏的外边距
   /// </summary>
   public Thickness HeaderMargin { get; set; }

   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      NotificationProgressHeight = 2;
      NotificationProgressMargin = new Thickness(0, 0, 0, 1);
      NotificationContentMargin = new Thickness(0, 0, 0, _globalToken.PaddingMD);
      NotificationPadding = new Thickness(_globalToken.PaddingLG, _globalToken.PaddingMD, _globalToken.PaddingLG, 0);
      NotificationBg = _globalToken.ColorToken.ColorNeutralToken.ColorBgElevated;
      NotificationIconSize = _globalToken.FontToken.FontSizeLG * _globalToken.FontToken.LineHeightLG;
      NotificationCloseButtonSize = _globalToken.HeightToken.ControlHeightLG * 0.55;
      NotificationMarginBottom = new Thickness(0, 0, 0, _globalToken.Margin);
      NotificationMarginEdge = new Thickness(_globalToken.MarginLG, _globalToken.MarginLG, _globalToken.MarginLG, 0);
      AnimationMaxHeight = 150;
      
      NotificationProgressBg = new LinearGradientBrush()
      {
         StartPoint = new RelativePoint(0, 0.5, RelativeUnit.Relative),
         EndPoint = new RelativePoint(1, 0.5, RelativeUnit.Relative),
         GradientStops = new GradientStops()
         {
            new GradientStop { Color = _globalToken.ColorToken.ColorPrimaryToken.ColorPrimaryHover, Offset = 0},
            new GradientStop { Color = _globalToken.ColorToken.ColorPrimaryToken.ColorPrimary, Offset = 1}
         }
      };
      NotificationWidth = 384;
      HeaderMargin = new Thickness(0, 0, 0, _globalToken.MarginXS);
      NotificationIconMargin = new Thickness(0, 0, _globalToken.MarginSM, 0);
   }
}