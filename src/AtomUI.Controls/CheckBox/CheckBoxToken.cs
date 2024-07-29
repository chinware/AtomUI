using AtomUI.Theme.TokenSystem;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class CheckBoxToken : AbstractControlDesignToken
{
   public const string ID = "CheckBox";

   public CheckBoxToken()
      : base(ID)
   {
   }
   
   /// <summary>
   /// 复选框标志的大小
   /// </summary>
   public double CheckIndicatorSize { get; set; }
   
   public double IndicatorTristateMarkSize { get; set; }
   
   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      CheckIndicatorSize = _globalToken.ControlInteractiveSize;
      IndicatorTristateMarkSize = _globalToken.FontToken.FontSizeLG / 2;
   }
}