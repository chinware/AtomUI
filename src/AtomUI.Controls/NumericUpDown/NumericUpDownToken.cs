using AtomUI.Theme.TokenSystem;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class NumericUpDownToken : ButtonSpinnerToken
{
   new public const string ID = "NumericUpDown";
   
   public NumericUpDownToken()
      : base(ID)
   {
   }
}