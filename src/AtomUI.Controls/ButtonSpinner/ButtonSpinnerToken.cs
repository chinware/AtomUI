using AtomUI.Media;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class ButtonSpinnerToken : LineEditToken
{
   new public const string ID = "ButtonSpinner";
   
   public ButtonSpinnerToken()
      : this(ID)
   {
   }
   
   protected ButtonSpinnerToken(string id)
      : base(id)
   {}
   
   /// <summary>
   /// 输入框宽度
   /// </summary>
   public double ControlWidth { get; set; }
   
   /// <summary>
   /// 操作按钮宽度
   /// </summary>
   public double HandleWidth { get; set; }
   
   /// <summary>
   /// 操作按钮图标大小
   /// </summary>
   public double HandleFontSize { get; set; }
   
   /// <summary>
   /// 操作按钮背景色
   /// </summary>
   public Color HandleBg { get; set; }
   
   /// <summary>
   /// 操作按钮激活背景色
   /// </summary>
   public Color HandleActiveBg { get; set; }
   
   /// <summary>
   /// 操作按钮悬浮颜色
   /// </summary>
   public Color HandleHoverColor { get; set; }
   
   /// <summary>
   /// 操作按钮边框颜色
   /// </summary>
   public Color HandleBorderColor { get; set; }
   
   /// <summary>
   /// 面性变体操作按钮背景色
   /// </summary>
   public Color FilledHandleBg { get; set; }

   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      var colorNeutral = _globalToken.ColorToken.ColorNeutralToken;
      ControlWidth = 90;
      HandleWidth = _globalToken.HeightToken.ControlHeightSM;
      HandleFontSize = _globalToken.FontToken.FontSize / 2;
      HandleActiveBg = _globalToken.ColorFillAlter;
      HandleBg = colorNeutral.ColorBgContainer;
      FilledHandleBg = ColorUtils.OnBackground(colorNeutral.ColorFillSecondary,
                                               HandleBg);
      HandleHoverColor = _globalToken.ColorToken.ColorPrimaryToken.ColorPrimary;
      HandleBorderColor = colorNeutral.ColorBorder;
   }
}