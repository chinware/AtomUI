using AtomUI.Media;
using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls.Switch;

[ControlDesignToken]
internal class ToggleSwitchToken : AbstractControlDesignToken
{
   public const string ID = "ToggleSwitch";
   
   public ToggleSwitchToken()
      : base(ID)
   {
   }
   
   /// <summary>
   /// 开关高度
   /// </summary>
   public double TrackHeight { get; set; }

   /// <summary>
   /// 小号开关高度
   /// </summary>
   public double TrackHeightSM { get; set; }
   
   /// <summary>
   /// 开关最小宽度
   /// </summary>
   public double TrackMinWidth { get; set; }
   
   /// <summary>
   /// 小号开关最小宽度
   /// </summary>
   public double TrackMinWidthSM { get; set; }
   
   /// <summary>
   /// 开关内边距
   /// </summary>
   public double TrackPadding { get; set; }
   
   /// <summary>
   /// 开关把手背景色
   /// </summary>
   public Color HandleBg { get; set; }

   /// <summary>
   /// 开关把手阴影
   /// </summary>
   public BoxShadow HandleShadow { get; set; }
   
   /// <summary>
   /// 开关把手大小
   /// </summary>
   public Size HandleSize { get; set; }
   
   /// <summary>
   /// 小号开关把手大小
   /// </summary>
   public Size HandleSizeSM { get; set; }
   
   /// <summary>
   /// 内容区域最小边距
   /// </summary>
   public double InnerMinMargin { get; set; }
   
   /// <summary>
   /// 内容区域最大边距
   /// </summary>
   public double InnerMaxMargin { get; set; }
   
   /// <summary>
   /// 小号开关内容区域最小边距
   /// </summary>
   public double InnerMinMarginSM { get; set; }
   
   /// <summary>
   /// 小号开关内容区域最大边距
   /// </summary>
   public double InnerMaxMarginSM { get; set; }
   
   /// <summary>
   /// 正常状态的图标大小
   /// </summary>
   public double IconSize { get; set; }
   
   /// <summary>
   /// 小号状态的图标大小
   /// </summary>
   public double IconSizeSM { get; set; }
   
   /// 内部 Token
   /// 单位毫秒
   public TimeSpan SwitchDuration { get; set; }
   public Color SwitchColor { get; set; }
   public double SwitchDisabledOpacity { get; set; }
   public double SwitchLoadingIconSize { get; set; }
   public Color SwitchLoadingIconColor { get; set; }
   public double ExtraInfoFontSize { get; set; }
   public double ExtraInfoFontSizeSM { get; set; }
   
   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      var fontToken = _globalToken.FontToken;
      double fontSize = fontToken.FontSize;
      double lineHeight = fontToken.LineHeight;
      double controlHeight = _globalToken.SeedToken.ControlHeight;
      
      double height = fontSize * lineHeight;
      double heightSM = controlHeight / 2;
      double padding = 2; // Fixed value
      double handleSize = height - padding * 2;
      double handleSizeSM = heightSM - padding * 2;

      TrackHeight = height;
      TrackHeightSM = heightSM;
      TrackMinWidth = handleSize * 2 + padding * 4; 
      TrackMinWidthSM = handleSizeSM * 2 + padding * 2;
      TrackPadding = padding; // Fixed value
      HandleBg = _globalToken.ColorToken.ColorWhite;
      HandleSize = new Size(handleSize, handleSize);
      HandleSizeSM = new Size(handleSizeSM, handleSizeSM);
      
      InnerMinMargin = handleSize / 2;
      InnerMaxMargin = handleSize + padding * 3;
      InnerMinMarginSM = handleSizeSM / 2 - padding;
      InnerMaxMarginSM = handleSizeSM + padding * 3;
      
      SwitchDuration = _globalToken.StyleToken.MotionDurationMid;
      SwitchColor = _globalToken.ColorToken.ColorPrimaryToken.ColorPrimary;
      SwitchDisabledOpacity = _globalToken.OpacityLoading;
      SwitchLoadingIconSize = _globalToken.FontSizeIcon * 0.75;
      SwitchLoadingIconColor = ColorUtils.FromRgbF(SwitchDisabledOpacity, 0, 0, 0);

      ExtraInfoFontSize = _globalToken.FontToken.FontSizeSM;
      ExtraInfoFontSizeSM = ExtraInfoFontSize - 1;

      HandleShadow = new BoxShadow
      {
         OffsetX = 0,
         OffsetY = 2,
         Blur = 4,
         Color = Color.FromArgb((int)(255 * 0.2),0, 35, 11)
      };

      IconSize = TrackHeightSM;
      IconSizeSM = TrackHeightSM - _globalToken.PaddingXXS;
   }
}