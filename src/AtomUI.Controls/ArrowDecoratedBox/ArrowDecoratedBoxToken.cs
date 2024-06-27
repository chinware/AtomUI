using AtomUI.TokenSystem;

namespace AtomUI.Controls;

[ControlDesignToken]
public class ArrowDecoratedBoxToken : AbstractControlDesignToken
{
   public const string ID = "ArrowDecoratedBox";
   
   /// <summary>
   /// 箭头三角形大小
   /// </summary>
   public double ArrowSize { get; set; }
   
   public ArrowDecoratedBoxToken()
      : base(ID)
   {
   }

   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      ArrowSize = _globalToken.SeedToken.SizePopupArrow / 1.3;
   }
}