using AtomUI.Media;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

// vertical: part   (水平时，垂直方向命名为 part)
// horizontal: full (水平时，水平方向命名为 full)
[ControlDesignToken]
public class SliderToken : AbstractControlDesignToken
{
   public const string ID = "Slider";

   public SliderToken()
      : base(ID) { }
   
   /// <summary>
   /// 滑动条控件的高度
   /// </summary>
   public double SliderTrackSize { get; set; }
   
   /// <summary>
   /// 轨道高度
   /// </summary>
   public double RailSize { get; set; }
   
   /// <summary>
   /// Mark 的大小
   /// </summary>
   public double MarkSize { get; set; }
      
   /// <summary>
   /// Thumb 推荐的大小，方便风格使用，是最大需要的大小
   /// </summary>
   public double ThumbSize { get; set; }
   
   /// <summary>
   /// 滑块尺寸
   /// </summary>
   public double ThumbCircleSize { get; set; }
   
   /// <summary>
   /// 滑块尺寸（悬浮态）
   /// </summary>
   public double ThumbCircleSizeHover { get; set; }
   
   /// <summary>
   /// 滑块边框宽度
   /// </summary>
   public Thickness ThumbCircleBorderThickness { get; set; }
   
   /// <summary>
   /// 滑块边框宽度（悬浮态）
   /// </summary>
   public Thickness ThumbCircleBorderThicknessHover { get; set; }
   
   /// <summary>
   /// 轨道背景色
   /// </summary>
   public Color RailBg { get; set; }
   
   /// <summary>
   /// 轨道背景色（悬浮态）
   /// </summary>
   public Color RailHoverBg { get; set; }
   
   /// <summary>
   /// 轨道已覆盖部分背景色
   /// </summary>
   public Color TrackBg { get; set; }
   
   /// <summary>
   /// 轨道已覆盖部分背景色（悬浮态）
   /// </summary>
   public Color TrackHoverBg { get; set; }
   
   /// <summary>
   /// Mark 的背景颜色
   /// </summary>
   public Color MarkBorderColor { get; set; }
   
   /// <summary>
   /// Mark 的背景 hover 效果
   /// </summary>
   public Color MarkBorderColorHover { get; set; }
   
   /// <summary>
   /// Mark 的背景激活时候效果
   /// </summary>
   public Color MarkBorderColorActive { get; set; }
   
   /// <summary>
   /// 滑块颜色
   /// </summary>
   public Color ThumbCircleBorderColor { get; set; }
   
   /// <summary>
   /// 滑块颜色 hover
   /// </summary>
   public Color ThumbCircleBorderHoverColor { get; set; }
   
   /// <summary>
   /// 滑块激活态颜色
   /// </summary>
   public Color ThumbCircleBorderActiveColor { get; set; }
   
   /// <summary>
   /// 滑块禁用颜色
   /// </summary>
   public Color ThumbCircleBorderColorDisabled { get; set; }
   
   /// <summary>
   /// 滑块的 outline 环的颜色
   /// </summary>
   public Color ThumbOutlineColor { get; set; }
   
   /// <summary>
   /// 滑块的 outline 环的厚度
   /// </summary>
   public Thickness ThumbOutlineThickness { get; set; }
   
   /// <summary>
   /// 轨道禁用态背景色
   /// </summary>
   public Color TrackBgDisabled { get; set; }
   
   public Thickness SliderPaddingHorizontal { get; set; }
   public Thickness SliderPaddingVertical { get; set; }
   public Thickness MarginPartWithMark { get; set; }
   
   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      // Thumb line width is always width-er 1px
      var increaseThumbWidth = 1d;
      var controlSize = _globalToken.HeightToken.ControlHeightLG / 4;
      var controlSizeHover = _globalToken.HeightToken.ControlHeightSM / 2;
      var handleLineWidth = _globalToken.SeedToken.LineWidth + increaseThumbWidth;
      var handleLineWidthHover = _globalToken.SeedToken.LineWidth + increaseThumbWidth * 1.5;

      SliderTrackSize = controlSizeHover;
      RailSize = 4;
      MarkSize = 8;
      ThumbCircleSize = controlSize;
      ThumbCircleSizeHover = controlSizeHover;
      ThumbCircleBorderThickness = new Thickness(handleLineWidth);
      ThumbCircleBorderThicknessHover = new Thickness(handleLineWidthHover);
      
      var colorNeutralToken = _globalToken.ColorToken.ColorNeutralToken;
      var colorPrimaryToken = _globalToken.ColorToken.ColorPrimaryToken;

      RailBg = colorNeutralToken.ColorFillTertiary;
      RailHoverBg = colorNeutralToken.ColorFillSecondary;
      TrackBg = colorPrimaryToken.ColorPrimaryBorder;
      TrackHoverBg = colorPrimaryToken.ColorPrimaryBorderHover;

      MarkBorderColor = colorNeutralToken.ColorBorderSecondary;
      MarkBorderColorHover = _globalToken.ColorFillContentHover;
      MarkBorderColorActive = colorPrimaryToken.ColorPrimaryBorder;
      
      ThumbCircleBorderColor = colorPrimaryToken.ColorPrimaryBorder;
      ThumbCircleBorderHoverColor = colorPrimaryToken.ColorPrimaryBorderHover;
      ThumbCircleBorderActiveColor = colorPrimaryToken.ColorPrimary;
      ThumbCircleBorderColorDisabled = ColorUtils.OnBackground(_globalToken.ColorTextDisabled, colorNeutralToken.ColorBgContainer);
      TrackBgDisabled = _globalToken.ColorBgContainerDisabled;
      
      SliderPaddingHorizontal = new Thickness(SliderTrackSize / 2, (_globalToken.SeedToken.ControlHeight - SliderTrackSize) / 2);
      SliderPaddingVertical = new Thickness((_globalToken.SeedToken.ControlHeight - SliderTrackSize) / 2, SliderTrackSize / 2);
      MarginPartWithMark = new Thickness(0, 0, 0, _globalToken.HeightToken.ControlHeightLG - SliderTrackSize);

      ThumbOutlineColor = ColorUtils.FromRgbF(0.2, ThumbCircleBorderActiveColor.GetRedF(), 
                                              ThumbCircleBorderActiveColor.GetGreenF(), 
                                              ThumbCircleBorderActiveColor.GetBlueF());
      ThumbOutlineThickness = new Thickness(_globalToken.WaveAnimationRange);
      ThumbSize = ThumbCircleSizeHover + ThumbCircleBorderThicknessHover.Left * 2 + ThumbOutlineThickness.Left * 2;
   }
}